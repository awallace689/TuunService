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
using tuuncs.Services;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("spotify")]
    public class SpotifyController : ControllerBase
    { 
        private readonly ILogger<SpotifyController> _logger;
        private readonly ISpotifyService _spotify;

        public SpotifyController(ILogger<SpotifyController> logger, ISpotifyService spotify)
        {
            _logger = logger;
            _spotify = spotify;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok("Route to /spotify/track[/{songid}] to get song data from spotify!\n" +
                "Route to /db/template to read document from database!\n" +
                "Route to /db/tracklist to get list of tracks selected from database!\n" +
                "POST to /db/write to write to the 'WriteTest' collection in database!\n");
        }


        public async Task<IList<FullTrack>> GetTracks(List<string> trackList)
        {         
            return await _spotify.GetTracks(trackList);
        }

        [HttpGet]
        [Route("track")]
        [Route("track/{id}")]
        public async Task<IActionResult> getTrackWithID(string id="2374M0fQpWi3dLnB54qaLX")
        {
            List<string> trackList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                trackList.Add(id);
            }

            try
            {
                return Ok(await GetTracks(trackList));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
