using System;
using System.Collections;
using System.Collections.Generic;
using tuuncs.Models;

namespace tuuncs.Services
{
    public class RoomService
    {
        private Random _random { get; set; }
        private Dictionary<int, Room> RoomsTable { get; set;  }

        public RoomService()
        {
            _random = new Random();
            RoomsTable = new Dictionary<int, Room>();
        }

        public Room CreateRoom(Options options, string host)
        {
            int id = GenerateRoomCode();
            return new Room(id, options, host);
        }

        public void AddRoom(Room room)
        {
            RoomsTable.Add(room.Id, room);
        }

        public void AddUser(int roomId, User user)
        {
            if (!RoomsTable[roomId].Users.ContainsKey(user.Username))
            {
                RoomsTable[roomId].Users.Add(user.Username, user);
            }
            else
            {
                throw new Exception("User already exists in room.");
            }
        }

        public void RemoveUser(int roomId, User user)
        {
            if (RoomsTable[roomId].Users.ContainsKey(user.Username))
            {
                RoomsTable[roomId].Users.Remove(user.Username);
            }
            else
            {
                throw new Exception("User does not exist in room.");
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
