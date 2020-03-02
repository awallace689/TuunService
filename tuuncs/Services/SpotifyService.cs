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
using tuuncs.Services;
using System.Net.Http;

namespace tuuncs.Services
{
    public class SpotifyService
    {
        public SpotifyWebAPI client;

        public SpotifyService() { }

        public async Task Initialize()
        {
            CredentialsAuth auth = new CredentialsAuth(Secret._clientID, Secret._clientSecret);
            Token token = await auth.GetToken();
            client = new SpotifyWebAPI()
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };
        }

        public async Task<List<FullTrack>> GetRecentlyPlayed(string token)
        {
            using var httpClient = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/top/tracks?limit=50");
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            requestMessage.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage resp = await httpClient.SendAsync(requestMessage);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string jsonContent = await resp.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(jsonContent);
                List<FullTrack> tracks = JsonConvert.DeserializeObject<List<FullTrack>>(JsonConvert.SerializeObject(json.items));
                return tracks;
            }
            else
            {
                return new List<FullTrack>();
            }
        }

        public async Task<List<AudioFeatures>> GetSeveralAudioFeatures(List<string> ids) {
            if (ids.Count > 100) {
                throw new Exception("Id limit 100 exceeded.");
            }

            string idsString = "";
            foreach (string id in ids) {
                string str = id == ids.Last() ? id : id + ",";
                idsString += str;
            }
            using var httpClient = new HttpClient();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/audio-features/?ids=" + idsString);
            requestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", client.AccessToken);
            requestMessage.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage resp = await httpClient.SendAsync(requestMessage);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string jsonContent = await resp.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject(jsonContent);
                List<AudioFeatures> analysisObjects = JsonConvert.DeserializeObject<List<AudioFeatures>>(JsonConvert.SerializeObject(json.audio_features));
                return analysisObjects;
            }
            else
            {
                throw new Exception("Token error.");
            }
        }

        public async Task<FullTrack> GetTrack(string id)
        {
            FullTrack track = await client.GetTrackAsync(id);
            if (track.HasError())
            {
                if (track.Error.Status == 401)
                {
                    // Refresh token
                    await Initialize();
                    track = await client.GetTrackAsync(id);

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
            foreach (string id in tracks)
            {
                FullTrack track = await client.GetTrackAsync(id);
                if (track.HasError())
                {
                    if (track.Error.Status == 401)
                    {
                        // Refresh token
                        await Initialize();
                        track = await client.GetTrackAsync(id);
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

        public IEnumerable<SimplePlaylist> GetUserPlaylists(string uid)
        {
            Paging<SimplePlaylist> playlists = client.GetUserPlaylists(uid);

            return playlists.Items;
        }
    }
}
