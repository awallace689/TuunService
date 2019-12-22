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

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("Tuun")]
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
            return Ok("Route to /Tuun/proof for proof of concept!");
        }

        [HttpGet]
        [Route("proof")]
        public async Task<IActionResult> proof()
        {
            await initializeSpotifyService();
            return Ok(await BlessTheRain());
        }


        public async Task<string> BlessTheRain()
        {
            FullTrack track = await _spotify.GetTrackAsync("2374M0fQpWi3dLnB54qaLX");
            return track.Name;
        }
        public async Task initializeSpotifyService()
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
