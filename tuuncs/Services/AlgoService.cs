using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;
using tuuncs.Models;

namespace tuuncs.Services
{
    public class AlgoService {

        private readonly SpotifyService _spotify;
        public AlgoService(SpotifyService spotify)
        {
            _spotify = spotify;
        }

        /*public List<FullTrack> GenerateTrackList(List<User> users, Options options)
        {
            HashSet<FullTrack> songPool = new HashSet<FullTrack>();

            foreach (User user in users)
            {
                if (user.Token != null)
                {
                    foreach (FullTrack track in GetRecentlyPlayed(user.Token))
                    {
                        if (!songPool.Contains(track))
                        {
                            songPool.Add(track);
                        }
                    }
                }
            }
        }*/

        public List<FullTrack> GetRecentlyPlayed(string token)
        {
            return _spotify.GetRecentlyPlayed(token).Result;
        }
    }
}
