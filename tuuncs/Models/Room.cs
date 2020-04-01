using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;

namespace tuuncs.Models
{
    public class Room
    {
        public int Id { get; set; }
        public HashSet<User> Users { get; set; }
        public Options Options { get; set; }
        public TuneableTrack Profile { get; set; }
        // Username of room host
        public string Host { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, List<string>> Playlist { get; set; }
        public Room(int id, Options options, string host)
        {
            Id = id;
            Users = new HashSet<User>(new UserComparer());
            Options = options;
            Profile = null;
            Host = host;
            Timestamp = DateTime.Now;
            Playlist = new Dictionary<string, List<string>>();
            Playlist.Add("shared", new List<string>());
            Playlist.Add("rest", new List<string>());
        }
    }

    /*
     * Host-selected preferences chosen during room creation.
     */
    public class Options
    {
        public List<string> Genres { get; set; }
    }
}
