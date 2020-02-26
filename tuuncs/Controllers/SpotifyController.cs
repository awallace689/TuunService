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
        private readonly SpotifyService _spotify;
        private readonly MongoService _mongo;

        public SpotifyController(ILogger<SpotifyController> logger, SpotifyService spotify, MongoService mongo)
        {
            _logger = logger;
            _spotify = spotify;
            _mongo = mongo;
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
            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Count", trackList.Count.ToString())
            };
            
            _mongo.Log(logDoc, "GetTracks", "SpotifyLog");
            return await _spotify.GetTracks(trackList);
        }

        [HttpGet]
        [Route("track")]
        [Route("track/{id}")]
        public async Task<IActionResult> getTrackWithID(string id="2374M0fQpWi3dLnB54qaLX")
        {
            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("trackId", id)
            };

            try
            {
                _mongo.Log(logDoc, "getTrackWithID", "SpotifyLog");
                return Ok(await _spotify.GetTrack(id));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("playlists/{uid}")]
        public IActionResult GetPlaylistsWithUserId(string uid)
        {
            IList<FullPlaylist> fullPlaylists = new List<FullPlaylist>();

            IEnumerable<SimplePlaylist> simplePlaylists = _spotify.GetUserPlaylists(uid);
            foreach (SimplePlaylist playlist in simplePlaylists)
            {
                fullPlaylists.Add(_spotify.client.GetPlaylist(playlist.Id));
            }

            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user", uid),
                new KeyValuePair<string, string>("count", fullPlaylists.Count.ToString())
            };

            _mongo.Log(logDoc, "GetPlaylistsWithUserId", "SpotifyLog");
            return Ok(fullPlaylists);
        }
    }
}
