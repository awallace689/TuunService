using System;
namespace tuuncs.Services
{
    public class RoomService
    {
        public RoomService()
        {
            _random = new Random();
        }

        private Random _random { get; set; }

        public string GenerateRoomCode()
        {
            return _random.Next(0, 9999).ToString("D4");
        }
    }
}
