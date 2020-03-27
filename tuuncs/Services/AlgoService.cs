using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;
using tuuncs.Models;
using System.Collections;
using System.Linq;

namespace tuuncs.Services
{
    public class AlgoService
    {

        private readonly SpotifyService _spotify;
        public AlgoService(SpotifyService spotify)
        {
            _spotify = spotify;
        }

        public async Task<(List<SimpleTrack>, TuneableTrack)> GenerateTrackList(List<User> users, Options options)
        {

            HashSet<FullTrack> trackPool = new HashSet<FullTrack>(new FullTrackComparer());
            HashSet<FullTrack> sharedTracks = new HashSet<FullTrack>(new FullTrackComparer());
            Dictionary<FullTrack, HashSet<string>> contributors = new Dictionary<FullTrack, HashSet<string>>(new FullTrackComparer());

            AddPlayed(users, trackPool, contributors);
            AddPlaylists(users, trackPool, contributors);
            FilterByGenre(ref trackPool, options);
            PopulateSharedTracks(trackPool, sharedTracks, contributors);

            HashSet<AudioFeatures> trackPoolFeatures = await GetAudioFeatures(trackPool);
            HashSet<AudioFeatures> sharedTracksFeatures = await GetAudioFeatures(sharedTracks);

            TuneableTrack trackPoolAvg = GetAverageTrack(trackPoolFeatures);
            TuneableTrack sharedTracksAvg = GetAverageTrack(sharedTracksFeatures);

            TuneableTrack avgFeatures = getAverageFeatureValues(trackPoolAvg, sharedTracksAvg);

            //IEnumerable<AudioFeatures> songCollection = sharedTracksFeatures.Union(trackPoolFeatures);

            //TuneableTrack averageTrack = GetAverageTrack(songCollection);

            List<string> artistSeed = GetArtistSeed(sharedTracks);
            List<string> trackSeed = GetTrackSeed(sharedTracks);

            List<SimpleTrack> recommendedTracks = await GetRecommendedSongs(artistSeed, options.Genres, trackSeed, avgFeatures);

            HashSet<FullTrack> fullSet = new HashSet<FullTrack>(trackPool.Union(sharedTracks).ToList(), new FullTrackComparer());
            HashSet<FullTrack> subset = new HashSet<FullTrack>(new FullTrackComparer());
            List<SimpleTrack> subsetSimple = new List<SimpleTrack>();
            int i = 0;
            if (fullSet.Count() > 9)
            {
                foreach (FullTrack track in fullSet)
                {
                    if (i == 7)
                    { break; }
                    else
                    {
                        Random random = new Random();
                        int rand = random.Next(0, 20);
                        if (rand > 11)
                        {
                            subset.Add(track);
                        }
                        i++;
                    }
                }
                subsetSimple = convertHashSetToList(subset);
            }
            else
            {
                subsetSimple = convertHashSetToList(fullSet);
            }

            List<SimpleTrack> tracks = new List<SimpleTrack>();
            tracks.AddRange(recommendedTracks.GetRange(0, 3));
            List<SimpleTrack> mixTracks = tracks.Union(subsetSimple).ToList();
            return (mixTracks, avgFeatures);
        }

        public TuneableTrack getAverageFeatureValues(TuneableTrack individual, TuneableTrack shared)
        {
            TuneableTrack averageTrackFeatrues = new TuneableTrack
            {
                Acousticness = (individual.Acousticness + shared.Acousticness) / 2,
                Danceability = (individual.Danceability + shared.Danceability) / 2,
                Energy = (individual.Energy + shared.Energy) / 2,
                Instrumentalness = (individual.Instrumentalness + shared.Instrumentalness) / 2,
                Tempo = (individual.Tempo + shared.Tempo) / 2,
                Valence = (individual.Valence + shared.Valence) / 2
            };


            return averageTrackFeatrues;
        }

        public List<SimpleTrack> convertHashSetToList(HashSet<FullTrack> fullTracks)
        {
            List<SimpleTrack> tracks = new List<SimpleTrack>();
            foreach (FullTrack fullTrack in fullTracks)
            {
                tracks.Add(new SimpleTrack
                {
                    Artists = fullTrack.Artists,
                    AvailableMarkets = fullTrack.AvailableMarkets,
                    DiscNumber = fullTrack.DiscNumber,
                    DurationMs = fullTrack.DurationMs,
                    Explicit = fullTrack.Explicit,
                    ExternUrls = fullTrack.ExternUrls,
                    Href = fullTrack.Href,
                    Id = fullTrack.Id,
                    Name = fullTrack.Name,
                    PreviewUrl = fullTrack.PreviewUrl,
                    Restrictions = fullTrack.Restrictions,
                    TrackNumber = fullTrack.TrackNumber,
                    Type = fullTrack.Type,
                    Uri = fullTrack.Uri
                });
            }
            return tracks;
        }

        public async Task<HashSet<AudioFeatures>> GetAudioFeatures(IEnumerable<FullTrack> tracks)
        {
            var trackIds = new List<string>();
            var audioFeatures = new List<AudioFeatures>();
            var result = new List<AudioFeatures>();

            foreach (FullTrack track in tracks)
            {
                if (trackIds.Count <= 100)
                {
                    trackIds.Add(track.Id);
                }
                if (trackIds.Count == 100 || track.Id == tracks.Last().Id)
                {
                    audioFeatures = await _spotify.GetSeveralAudioFeatures(trackIds);
                    result.AddRange(audioFeatures);

                    audioFeatures = new List<AudioFeatures>();
                }
            }

            return new HashSet<AudioFeatures>(result);
        }

        public void PopulateSharedTracks(HashSet<FullTrack> trackPool, HashSet<FullTrack> sharedTracks, Dictionary<FullTrack, HashSet<string>> contributors)
        {
            foreach (FullTrack track in trackPool)
            {
                if (contributors[track].Count > 1)
                {
                    sharedTracks.Add(track);
                }
            }

            trackPool.ExceptWith(sharedTracks);
        }

        public void AddPlayed(List<User> users, HashSet<FullTrack> trackPool, Dictionary<FullTrack, HashSet<string>> contributors)
        {
            foreach (User user in users)
            {
                if (user.Token != null)
                {
                    foreach (FullTrack track in GetRecentlyPlayed(user.Token))
                    {
                        if (!trackPool.Add(track))
                        {
                            contributors[track].Add(user.Username);
                        }
                        else
                        {
                            contributors.Add(track, new HashSet<string>() { user.Username });
                        }
                    }
                }
            }
        }

        public void FilterByGenre(ref HashSet<FullTrack> trackPool, Options options)
        {
            Dictionary<string, HashSet<FullTrack>> artistDict = new Dictionary<string, HashSet<FullTrack>>();
            foreach (FullTrack track in trackPool)
            {
                if (!artistDict.ContainsKey(track.Artists[0].Id))
                {
                    artistDict.Add(track.Artists[0].Id, new HashSet<FullTrack>(new FullTrackComparer()) { track });
                }
                else
                {
                    artistDict[track.Artists[0].Id].Add(track);
                }
            }

            List<string> artistIds = new List<string>();
            SeveralArtists artists;
            foreach (string id in artistDict.Keys)
            {
                if (artistIds.Count <= 50)
                {
                    artistIds.Add(id);
                }
                if (artistIds.Count == 50 || id == artistDict.Keys.Last())
                {
                    artists = _spotify.client.GetSeveralArtists(artistIds);

                    foreach (FullArtist artist in artists.Artists)
                    {
                        bool sharesGenre = false;
                        foreach (string genre in artist.Genres)
                        {
                            if (options.Genres.Contains(genre))
                            {
                                sharesGenre = true;
                                break;
                            }
                        }

                        if (!sharesGenre)
                        {
                            artistDict.Remove(artist.Id);
                        }
                    }

                    artistIds = new List<string>();
                }
            }

            trackPool = new HashSet<FullTrack>(new FullTrackComparer());
            foreach (var pair in artistDict)
            {
                foreach (FullTrack track in pair.Value)
                {
                    trackPool.Add(track);
                }
            }
        }

        public void AddPlaylists(List<User> users, HashSet<FullTrack> trackPool, Dictionary<FullTrack, HashSet<string>> contributors)
        {
            foreach (User user in users)
            {
                var playlists = _spotify.GetUserPlaylists(user.Username);
                foreach (SimplePlaylist playlist in playlists)
                {
                    FullPlaylist fullPlaylist = _spotify.client.GetPlaylist(playlist.Id);
                    foreach (PlaylistTrack pTrack in fullPlaylist.Tracks.Items)
                    {
                        if (!trackPool.Add(pTrack.Track))
                        {
                            contributors[pTrack.Track].Add(user.Username);
                        }
                        else
                        {
                            contributors.Add(pTrack.Track, new HashSet<string>() { user.Username });
                        }
                    }
                }
            }
        }

        public List<FullTrack> GetRecentlyPlayed(string token)
        {
            if (token != null)
            {
                return _spotify.GetRecentlyPlayed(token).Result;
            }
            else
            {
                return new List<FullTrack>();
            }
        }

        public TuneableTrack GetAverageTrack(IEnumerable<AudioFeatures> songs)
        {
            AudioFeatures sum = new AudioFeatures();
            int count = 0;
            foreach(AudioFeatures song in songs)
            {
                count++;
                sum.Acousticness += song.Acousticness;
                sum.Danceability += song.Danceability;
                sum.Energy += song.Energy;
                sum.Instrumentalness += song.Instrumentalness;
                sum.Liveness += song.Liveness;
                sum.Loudness += song.Loudness;
                sum.Tempo += song.Tempo;
                sum.Valence += song.Valence;
                sum.Speechiness += song.Speechiness;
            }

            TuneableTrack avg = new TuneableTrack();

            avg.Acousticness = sum.Acousticness / count;
            avg.Danceability = sum.Danceability / count;
            avg.Energy = sum.Energy / count;
            avg.Instrumentalness = sum.Instrumentalness / count;
            avg.Liveness = sum.Liveness / count;
            avg.Loudness = sum.Loudness / count;
            avg.Tempo = sum.Tempo / count;
            avg.Valence = sum.Valence / count;
            avg.Speechiness = sum.Speechiness / count;

            return avg;
        }

        public async Task<List<SimpleTrack>> GetRecommendedSongs(List<string> artistSeed, List<string> genreSeed, List<string> trackSeed, TuneableTrack averageSong)
        {
            List<SimpleTrack> songs = await _spotify.GetRecommendedSongs(artistSeed, genreSeed, trackSeed, averageSong);
            return songs;
        }

        public List<string> GetArtistSeed(HashSet<FullTrack> sharedTracks)
        {
            List<string> artistSeed = new List<string>();
            int count = 0;
            foreach(FullTrack track in sharedTracks)
            {
                artistSeed.Add(track.Artists[0].Id);
                count++;
                if(count == 5)
                {
                    break;
                }
            }
            return artistSeed;
        }

        public List<string> GetTrackSeed(HashSet<FullTrack> sharedTracks)
        {
            List<string> trackSeed = new List<string>();
            int count = 0;
            foreach (FullTrack track in sharedTracks)
            {
                trackSeed.Add(track.Id);
                count++;
                if (count == 5)
                {
                    break;
                }
            }
            return trackSeed;
        }
    }
}
