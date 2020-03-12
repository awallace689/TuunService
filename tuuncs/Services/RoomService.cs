using System;
using System.Collections;
using System.Collections.Generic;
using tuuncs.Models;
using System.Linq;

namespace tuuncs.Services
{
    public class RoomService
    {
        private Random _random { get; set; }
        private readonly AlgoService _algo;
        private Dictionary<int, Room> RoomsTable { get; set; }
        private Dictionary<string, User> UsersTable { get; set; }

        public RoomService(AlgoService algo)
        {
            _random = new Random();
            RoomsTable = new Dictionary<int, Room>();
            UsersTable = new Dictionary<string, User>();
            _algo = algo;
        }

        public void CreatePlaylist(List<User> users)
        {
            
        }

        public Room CreateRoom(int id, Options options, string host)
        {
            return new Room(id, options, host);
        }

        public void AddRoom(Room room)
        {
            RoomsTable.Add(room.Id, room);
        }

        public void SetHost(int roomId, string user)
        {
            if (!RoomsTable.ContainsKey(roomId))
            {
                throw new Exception("Room does not exist.");
            }
            if (!RoomsTable[roomId].Users.ContainsKey(user))
            {
                throw new Exception("User does not exist in room.");
            }

            RoomsTable[roomId].Host = user;
        }

        public void DeleteRoom(int roomId)
        {
            if (RoomsTable.ContainsKey(roomId))
            {
                RoomsTable.Remove(roomId);
            }

            else
            {
                throw new Exception("Room does not exist.");
            }
        }

        public Dictionary<int, Room> GetAll()
        {
            return RoomsTable;
        }

        public Room GetOne(int roomId)
        {
            if (RoomsTable.ContainsKey(roomId))
            {
                return RoomsTable[roomId];
            }
            else
            {
                throw new Exception("Invalid room id.");
            }
        }

        public int GenerateRoomCode()
        {
            int code;
            while (true)
            {
                code = _random.Next(0, 9999);
                if (RoomsTable.ContainsKey(code))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        code += 1;
                        if (!RoomsTable.ContainsKey(code))
                        {
                            return code;
                        }
                    }
                }

                else
                {
                    return code;
                }
            }
        }
    }
}
