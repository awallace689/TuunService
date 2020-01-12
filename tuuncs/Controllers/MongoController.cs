using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using tuuncs.Services;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("db")]
    public class MongoController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;
        private MongoService _mongo;

        public MongoController(ILogger<Tuun> logger, MongoService mongo)
        {
            _logger = logger;
            _mongo = mongo;
        }

        [HttpGet]
        [Route("template")]
        public string Get()
        {
            return _mongo.Get("LiveRooms")?.ToJson();
        }


        /* Projections let you SELECT what is shown from document in database.
         * Use [BsonDocument].ToJson() before returning result, ASP.NET can crash
         * and throw exception when automatically converting some Bson objects to Json. 
         */
        [HttpGet]
        [Route("tracklist")]
        public IActionResult getList()
        {
            string res = _mongo.GetPlaylist();
            if (res != null)
            {
                return Ok(res);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Action returned null.");
            }
        }

        [HttpPost]
        [Route("write")]
        public IActionResult write()
        {
            var document = new BsonDocument
            {
                {"test", "Test" }
            };
            
            try
            {
                _mongo.WriteDocument("WriteTest", document);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
