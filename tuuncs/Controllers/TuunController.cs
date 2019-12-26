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

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("spotify")]
    public class TuunController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;
        private static SpotifyWebAPI _spotify;

        public TuunController(ILogger<Tuun> logger)
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
        public IActionResult getTrackWithID(string id="2374M0fQpWi3dLnB54qaLX")
        {
            return Ok(getTracks(id));
        }


        public IActionResult getTracks(string id)
        {
            List<FullTrack> trackList = new List<FullTrack>();
            if (_spotify == null)
            {
                InitializeSpotifyService();
            }

            for (int i = 0; i < 10; i++)
            {
                FullTrack track = _spotify.GetTrack(id);
                if (track.HasError())
                {
                    if (track.Error.Status == 401)
                    {
                        // Refresh token
                        InitializeSpotifyService();
                        Console.WriteLine("Refreshing token!");
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, track.Error);
                    }
                }

                // If refreshing token did not fix error.
                if (track.HasError())
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, track.Error);
                }

                trackList.Add(track);
            }

            return Ok(trackList);
        }

        public async void InitializeSpotifyService()
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
