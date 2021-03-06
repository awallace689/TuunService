﻿using System;
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
using tuuncs.Models;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("spotify")]
    public class SpotifyController : ControllerBase
    { 
        private readonly ILogger<SpotifyController> _logger;
        private readonly SpotifyService _spotify;
        private readonly MongoService _mongo;
        private readonly AlgoService _algo;

        public SpotifyController(ILogger<SpotifyController> logger, SpotifyService spotify, MongoService mongo, AlgoService algo)
        {
            _logger = logger;
            _spotify = spotify;
            _mongo = mongo;
            _algo = algo;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok("It's alive!");
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

                var result = await _spotify.GetTrack(id);

                logDoc.Add(new KeyValuePair<string, string>("success", "true"));
                _mongo.Log(logDoc, "getTrackWithID", "SpotifyLog");
                return Ok(result);
            }

            catch (Exception ex)
            {
                logDoc.Add(new KeyValuePair<string, string>("success", "false"));
                _mongo.Log(logDoc, "getTrackWithID", "SpotifyLog");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("playlists/{uid}")]
        public IActionResult GetPlaylistsWithUserId(string uid)
        {
            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user", uid),
            };

            try
            {
                IList<FullPlaylist> fullPlaylists = new List<FullPlaylist>();

                IEnumerable<SimplePlaylist> simplePlaylists = _spotify.GetUserPlaylists(uid).Result;
                foreach (SimplePlaylist playlist in simplePlaylists)
                {
                    fullPlaylists.Add(_spotify.client.GetPlaylist(playlist.Id));
                }

                logDoc.Add(new KeyValuePair<string, string>("count", fullPlaylists.Count.ToString()));
                logDoc.Add(new KeyValuePair<string, string>("success", "true"));
                _mongo.Log(logDoc, "GetPlaylistsWithUserId", "SpotifyLog");
                return Ok(fullPlaylists);
            }
            catch (Exception ex)
            {
                logDoc.Add(new KeyValuePair<string, string>("success", "false"));
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> test()
        {
            var user1 = new User("asdff01", "BQAFwehPEc1l_3HL8PIP3-TuPcwMgMJZZCHhHpfWySDKD5S9wZU2WanAQuWN4Wk6DKAog--0yK21Q4wS2IoOLdLfoNT2uGi9R3-hoc9S8wEYiRVixnHaqTSlbYkCSqun-vU2W6pcp1XDs5zwYOxmK-9qeyeWpGT_PuM-1P636GZo784jOYl30anM1bDgsSGTugXIH9aoYDRIJiJoIBYp0ChUuUGD0ZcYbvQbBF8Xtkk0GwJUnvAq5VZLGL0AlOK8oQ_9_CHb-A");
            var users = new List<User>() { user1 };
            var options = new Options();
            options.Genres = new List<string>() { "hip-hop" };
            
            var trackList = await _algo.GenerateTrackList(users, options);

            List<string> res = trackList.Item1["shared"].Concat(trackList.Item1["rest"]).ToList();

            // foreach (string track in res) {
            //     // res.Add(track.Name + ", " + track.Artists[0].Name);
            // }
            
            return Ok(res);
        }
    }
}
