using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using Secrets;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("Tuun")]
    public class TemplateController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;

        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;

        public TemplateController(ILogger<Tuun> logger)
        {
            _logger = logger;
            _client = new MongoClient(Secret._MongoClient);
            _database = _client.GetDatabase("Tuun");
            _collection = _database.GetCollection<BsonDocument>("LiveRooms");
        }

        [HttpGet]
        [Route("template")]
        public string Get()
        {
            return _collection.Find(new BsonDocument()).First().ToString();
        }

        [HttpGet]
        [Route("dbTrackList")]
        public IActionResult getList()
        {
            _collection = _database.GetCollection<BsonDocument>("Rooms");
            var projection = Builders<BsonDocument>.Projection.Include("playlists").Exclude("_id");
            var res = _collection.Find(new BsonDocument()).Project(projection).First();
            if (res != null)
            {
                return Ok(JsonConvert.SerializeObject((res["playlists"].AsBsonArray)[0], Formatting.Indented));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, res.ToString());
            }
        }

      /*  class Room
        {
            public ObjectId id { get; set; }

            [BsonElement("owner")]
            public string owner { get; set; }

            [BsonElement("active")]
            public bool active { get; set; }

            //Array<Array<string>>
            [BsonElement("playlists")]
            public Array playlists { get; set; }

            //Array<string>
            [BsonElement("members")]
            public Array members { get; set; }
        }*/
    }
}
