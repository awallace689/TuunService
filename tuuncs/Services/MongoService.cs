using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Secrets;


namespace tuuncs.Services
{
    public class MongoService
    {
        public MongoClient client; 
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;
        public bool LoggingEnabled = true;

        public MongoService() 
        {
            client = new MongoClient(Secret._MongoClient);
            _database = client.GetDatabase("Tuun");
        }

        public List<BsonDocument> Get(string collection, ProjectionDefinition<BsonDocument> projection=null)
        {
            _collection = _database.GetCollection<BsonDocument>(collection);
            if (projection == null)
            {
                var res = _collection.Find(new BsonDocument()).ToList();
                return res;
            }

            else
            {
                var res = _collection.Find(new BsonDocument()).Project(projection).ToList();
                return res;
            }
        }

        public BsonDocument GetFirst(string collection, ProjectionDefinition<BsonDocument> projection=null)
        {
            _collection = _database.GetCollection<BsonDocument>(collection);
            if (projection == null) 
            {
                var res = _collection.Find(new BsonDocument()).First();
                return res;
            }

            else
            {
                var res = _collection.Find(new BsonDocument()).Project(projection).First();
                return res;
            }
        }

        public string GetPlaylist()
        {
            var projection = Builders<BsonDocument>.Projection.Include("playlists").Exclude("_id");
            var res = GetFirst("Rooms", projection);

            return res["playlists"][0]?.ToJson();
        }

        public void WriteDocument(string collection, BsonDocument doc)
        {
            _collection = _database.GetCollection<BsonDocument>(collection);
            _collection.InsertOne(doc);
        }

        public void Log(List<KeyValuePair<string, string>> logDoc, string actionName, string collection)
        {
            if (LoggingEnabled)
            {
                logDoc.Add(new KeyValuePair<string, string>("timestamp", DateTime.Now.ToString()));
                logDoc.Add(new KeyValuePair<string, string>("action", actionName));
                var logDict = new Dictionary<string, string>(logDoc);

                var bson = new BsonDocument(logDict);
                WriteDocument(collection, bson);
            }
            else
            {
                return;
            }
        }
    }
}
