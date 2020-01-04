using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Secrets;
using Newtonsoft.Json;

namespace tuuncs.Services
{
    public class SpotifyService : ISpotifyService
    {
        private SpotifyWebAPI _spotify;
        
        public SpotifyService() {
            Initialize();
        }
        public async Task<FullTrack> GetTrack(string id)
        {
            FullTrack track = await _spotify.GetTrackAsync(id);
            if (track.HasError())
            {
                if (track.Error.Status == 401)
                {
                    // Refresh token
                    Initialize();
                    Console.WriteLine("Refreshing token!");
                }
                else
                {
                    throw new Exception(JsonConvert.SerializeObject(track.Error));
                }
            }

            // If refreshing token did not fix error.
            if (track.HasError())
            {
                throw new Exception(JsonConvert.SerializeObject(track.Error));
            }

            return track;
        }

        public async Task<IList<FullTrack>> GetTracks(IList<string> tracks)
        {
            List<FullTrack> trackList = new List<FullTrack>();
            if (_spotify == null)
            {
                Initialize();
            }

            foreach (string id in tracks)
            {
                FullTrack track = await _spotify.GetTrackAsync(id);
                if (track.HasError())
                {
                    if (track.Error.Status == 401)
                    {
                        // Refresh token
                        Initialize();
                        Console.WriteLine("Refreshing token!");
                    }
                    else
                    {
                        throw new Exception(JsonConvert.SerializeObject(track.Error));
                    }
                }

                // If refreshing token did not fix error.
                if (track.HasError())
                {
                    throw new Exception(JsonConvert.SerializeObject(track.Error));
                }

                trackList.Add(track);
            }

            return trackList;
        }

        public async void Initialize()
        {
            CredentialsAuth auth = new CredentialsAuth(Secret._clientID, Secret._clientSecret);
            Token token = await auth.GetToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
        }
    }
}
