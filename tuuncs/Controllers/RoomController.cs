using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tuuncs.Services;
using tuuncs.Models;
using Newtonsoft.Json;


namespace tuuncs.Controllers
{
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        // Gets host username from request url, creates options object from
        // json with identical fields in request body.
        [HttpPost]
        [Route("create/{host}")]
        public IActionResult CreateRoom(string host, [FromBody] Options options)
        {
            try
            {
                if (options == null)
                {
                    return StatusCode(400, "Invalid JSON provided in request body.");
                }

                Room room = _roomService.CreateRoom(options, host);
                _roomService.AddRoom(room);
                _roomService.AddUser(room.Id, new User(host));
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // For testing result of '/create'
        [HttpGet]
        [Route("get")]
        public IActionResult GetRooms()
        {
            return Ok(JsonConvert.SerializeObject(_roomService.RoomsTable));
        }

        [HttpPost]
        [Route("user/add/{roomId}")]
        public IActionResult AddUser(int roomId, [FromBody] User user)
        {
            if (user == null)
            {
                return StatusCode(400, "Invalid JSON provided in request body.");
            }

            try
            {
                _roomService.AddUser(roomId, user);
            }
            catch (Exception)
            {
                return StatusCode(400, "");
            }

            return Ok();
        }

        [HttpDelete]
        public void DeleteRoom(int id)
        {
            try
            {
                RoomsTable.Remove(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
