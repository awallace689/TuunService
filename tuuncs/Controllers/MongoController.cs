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
    [Route("db")]
    public class MongoController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;

        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;

        public MongoController(ILogger<Tuun> logger)
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
                return Ok(res["playlists"][0].ToJson());
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, res.ToJson());
            }
        }


        // Should be a post, eventually...
        [HttpGet]
        [Route("write")]
        public IActionResult write()
        {
            _collection = _database.GetCollection<BsonDocument>("WriteTest");
            var document = new BsonDocument
            {
                {"test", "Test" }
            };

            _collection.InsertOne(document);

            return Ok();
        }
    }
}
