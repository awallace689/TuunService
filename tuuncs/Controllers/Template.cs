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

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("/Tuun/Template")]
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
        public string Get()
        {
            return _collection.Find(new BsonDocument()).First().ToString();
        }
    }
}
