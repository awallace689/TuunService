using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tuuncs.Services;
using tuuncs.Models;
using Newtonsoft.Json;
using SpotifyAPI.Web.Models;


namespace tuuncs.Controllers
{
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly RoomService _roomService;
        private readonly MongoService _mongo;
        private readonly AlgoService _algo;

        public RoomController(RoomService roomService, MongoService mongo, AlgoService algo) {
            _roomService = roomService;
            _mongo = mongo;
            _algo = algo;
        }

        // Gets host username from request url, creates options object from
        // json with identical fields in request body.
        [HttpPost]
        [Route("create/{id}/{host}")]
        public IActionResult CreateRoom(int id, string host, [FromBody] Options options)
        {
            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("host", host)
            };

            try
            {
                if (options == null)
                {
                    return StatusCode(400, "Invalid JSON provided in request body.");
                }

                Room room = _roomService.CreateRoom(id, options, host);
                _roomService.AddRoom(room);

                logDoc.Add(new KeyValuePair<string, string>("success", "true"));
                // _mongo.Log(logDoc, "CreateRoom", "RoomsLog");
            }

            catch (Exception ex)
            {
                logDoc.Add(new KeyValuePair<string, string>("success", "false"));
                _mongo.Log(logDoc, "CreateRoom", "RoomsLog");
                return StatusCode(500, ex);
            }
            return Ok();
        }

        // For testing result of '/create'
        [HttpGet]
        [Route("getAll")]
        public IActionResult GetRooms()
        {
            return Ok(JsonConvert.SerializeObject(_roomService.GetAll().ToList()));
        }

        [HttpGet]
        [Route("get/{roomId}")]
        public IActionResult GetRoom(int roomId)
        {
            var logDoc = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("id", roomId.ToString()),
            };

            try
            {
                var result = JsonConvert.SerializeObject(_roomService.GetOne(roomId));
                logDoc.Add(new KeyValuePair<string, string>("success", "true"));
                _mongo.Log(logDoc, "GetRoom", "RoomsLog");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logDoc.Add(new KeyValuePair<string, string>("success", "false"));
                _mongo.Log(logDoc, "GetRoom", "RoomsLog");
                return StatusCode(400, ex);
            }
        }

        // [HttpPost]
        // [Route("user/add/{roomId}")]
        // public IActionResult AddUser(int roomId, [FromBody] User user)
        // {
        //     if (user == null)
        //     {
        //         return StatusCode(400, "Invalid JSON provided in request body.");
        //     }

        //     var logDoc = new List<KeyValuePair<string, string>>()
        //     {
        //         new KeyValuePair<string, string>("id", roomId.ToString()),
        //         new KeyValuePair<string, string>("user", user.Username)
        //     };

        //     try
        //     {
        //         _roomService.AddUser(roomId, user);
        //     }
        //     catch (Exception)
        //     {
        //         logDoc.Add(new KeyValuePair<string, string>("success", "false"));
        //         _mongo.Log(logDoc, "AddUser", "RoomsLog");
        //         return StatusCode(400, "");
        //     }

        //     logDoc.Add(new KeyValuePair<string, string>("success", "true"));
        //     _mongo.Log(logDoc, "AddUser", "RoomsLog");
        //     return Ok();
        // }

        [HttpGet]
        [Route("genCode")]
        public IActionResult GenerateCode()
        {
            return Ok(_roomService.GenerateRoomCode());
        }
    }
}
