using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;
using tuuncs.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace tuuncs.Services
{
    public class AlgoService {

        private readonly SpotifyService _spotify;
        public AlgoService(SpotifyService spotify)
        {
            _spotify = spotify;
        }

        public List<FullTrack> GenerateTrackList(List<User> users, Options options)
        {
            HashSet<FullTrack> songPool = new HashSet<FullTrack>(new FullTrackComparer());

            AddPlayed(users, songPool);
            AddPlaylists(users, songPool);

            Dictionary<string, HashSet<FullTrack>> artistDict = new Dictionary<string, HashSet<FullTrack>>();
            foreach (FullTrack track in songPool)
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

            foreach (string id in artistDict.Keys)
            {
                bool sharesGenre = false;
                FullArtist artist =_spotify.client.GetArtist(id);
                foreach (string genre in artist.Genres)
                {
                    if (options.Genres.Contains(genre))
                    {
                        sharesGenre = true;
                    }
                }

                if (!sharesGenre)
                {
                    artistDict.Remove(id);
                }
            }

            songPool = new HashSet<FullTrack>(new FullTrackComparer());
            foreach (var pair in artistDict)
            {
                foreach (FullTrack track in pair.Value)
                {
                    songPool.Add(track);
                }
            }

            return songPool.ToList();
        }

        public void AddPlayed(List<User> users, HashSet<FullTrack> songPool)
        {
            foreach (User user in users)
            {
                if (user.Token != null)
                {
                    foreach (FullTrack track in GetRecentlyPlayed(user.Token))
                    {
                        songPool.Add(track);
                    }
                }
            }
        }

        public void AddPlaylists(List<User> users, HashSet<FullTrack> songPool)
        {
            foreach (User user in users)
            {
                var playlists = _spotify.GetUserPlaylists(user.Username);
                foreach (SimplePlaylist playlist in playlists)
                {
                    FullPlaylist fullPlaylist = _spotify.client.GetPlaylist(playlist.Id);
                    foreach (PlaylistTrack pTrack in fullPlaylist.Tracks.Items)
                    {
                        songPool.Add(pTrack.Track);
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
