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
        private readonly AlgoService _algoService;
        public RoomsHub(RoomService roomService, AlgoService algoService) : base()
        {
            _roomService = roomService;
            _algoService = algoService;
        }

        //
        // Summary:
        //     Called when a new connection is established with the hub.
        public async Task AddUser(int roomId, string username, string token)
        {
            var user = new User(username, token);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            _roomService.AddUser(roomId, user, Tuple.Create(Context.UserIdentifier, Context.ConnectionId));

            Room room = _roomService.GetOne(roomId);
            await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
        }

        public async Task LeaveRoom(int roomId, string username) 
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
            _roomService.RemoveUser(username, roomId);
            
            if (_roomService.RoomsTable.ContainsKey(roomId))
            {
                Room room = _roomService.GetOne(roomId);
                await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
            }
        }

        public async Task Promote(int roomId, string username) 
        {
            _roomService.SetHost(roomId, username);

            Room room = _roomService.GetOne(roomId);
            await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
        }

        public async Task Kick(int roomId, string username)
        {
            var user = new User(username);
            await Clients.User(_roomService.UsersTable[user].Item1).SendAsync("GetKicked");
            await Groups.RemoveFromGroupAsync(_roomService.UsersTable[user].Item2, roomId.ToString());
            _roomService.RemoveUser(username, roomId);
            
            if (_roomService.RoomsTable.ContainsKey(roomId))
            {
                Room room = _roomService.GetOne(roomId);
                await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
            }
        }

        public async Task Generate(int roomId) 
        {
            var room = _roomService.GetOne(roomId);
            var algoTuple = await _algoService.GenerateTrackList(room.Users.ToList(), room.Options);
            room.Playlist = algoTuple.Item1;
            room.Profile = algoTuple.Item2;

            await Clients.Group(roomId.ToString()).SendAsync("SetState", JsonConvert.SerializeObject(room));
        }
    }
}
