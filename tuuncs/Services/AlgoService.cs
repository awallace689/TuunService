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

        public async Task<List<AudioFeatures>> GenerateTrackList(List<User> users, Options options)
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

            IEnumerable<AudioFeatures> songCollection = sharedTracksFeatures.Union(trackPoolFeatures);
            return songCollection.ToList();
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
    }
}
