using System;
using System.Collections;
using System.Collections.Generic;
using tuuncs.Models;

namespace tuuncs.Services
{
    public class RoomService
    {
        private Random _random { get; set; }
        public Dictionary<int, Room> RoomsTable {get; set;}
        private HashSet<int> TakenIds { get; set; }

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

        public int GenerateRoomCode()
        {
            int code;
            while (true)
            {
                code = _random.Next(0, 9999);
                if (TakenIds.Contains(code))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        code += 1;
                        if (!TakenIds.Contains(code))
                        {
                            TakenIds.Add(code);
                            return code;
                        }
                    }
                }
            }
        }
    }
}
