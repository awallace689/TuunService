using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using Secrets;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("spotify")]
    public class SpotifyController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;
        private static SpotifyWebAPI _spotify;

        public SpotifyController(ILogger<Tuun> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok("Route to /spotify/track for proof of concept!");
        }

        [HttpGet]
        [Route("track")]
        [Route("track/{id}")]
        public async Task<IActionResult> getTrackWithID(string id="2374M0fQpWi3dLnB54qaLX")
        {
            return Ok(await getTracks(id));
        }


        public async Task<List<FullTrack>> getTracks(string id)
        {
            List<FullTrack> trackList = new List<FullTrack>();
            if (_spotify == null)
            {
                await InitializeSpotifyService();
            }

            for (int i = 0; i < 10; i++)
            {
                FullTrack track = await _spotify.GetTrackAsync(id);
                if (track.HasError())
                {
                    if (track.Error.Status == 401)
                    {
                        // Refresh token
                        await InitializeSpotifyService();
                        Console.WriteLine("Refreshing token!");
                    }
                    else
                    {
                        throw new Exception(JsonConvert.SerializeObject(track));
                    }
                }

                // If refreshing token did not fix error.
                if (track.HasError())
                {
                    throw new Exception(JsonConvert.SerializeObject(track));
                }

                trackList.Add(track);
            }

            return trackList;
        }

        public async Task InitializeSpotifyService()
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
