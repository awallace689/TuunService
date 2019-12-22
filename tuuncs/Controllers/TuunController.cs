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
using Newtonsoft.Json;

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
            return Ok("Route to /Tuun/track for proof of concept!");
        }

        [HttpGet]
        [Route("track")]
        [Route("track/{id}")]
        public async Task<IActionResult> getTrackWithID(string id="2374M0fQpWi3dLnB54qaLX")
        {
            await initializeSpotifyService();
            return Ok(await getTrack(id));
        }


        public async Task<string> getTrack(string id)
        {
            FullTrack track = await _spotify.GetTrackAsync(id);
            if (track.HasError())
            {
                string errStr = "Error Status: " + track.Error.Status + '\n' + "Error Msg: " + track.Error.Message;
                return errStr;
            }

            return JsonConvert.SerializeObject(track, Formatting.Indented);
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
