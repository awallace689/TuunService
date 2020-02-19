using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tuuncs.Services;
using tuuncs.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tuuncs.Controllers
{
    [Route("room")]
    public class RoomController : Controller
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService) {
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
                return Ok();
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // For testing result of '/create'
        [HttpGet]
        [Route("getRooms")]
        public IActionResult GetRooms()
        {
            return Ok(_roomService.RoomsTable);
        }
    }
}
