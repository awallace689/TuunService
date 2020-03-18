using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tuuncs.Models;
using tuuncs.Services;
using Newtonsoft.Json;

namespace tuuncs.Hubs
{
    public class RoomsHub : Hub
    {
        private readonly RoomService _roomService;
        public RoomsHub(RoomService roomService) : base()
        {
          _roomService = roomService;
        }

        //
        // Summary:
        //     Called when a new connection is established with the hub.
        public async Task AddUser(int roomId, string username, string token)
        {
          var user = new User(username, token);
          await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
          _roomService.AddUser(roomId, user, Context.ConnectionId);

          Room room = _roomService.GetOne(roomId);
          await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
        }
    }
}
