using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuuncs.Models
{
    public class Room
    {
        public int Id { get; set; }
        public Dictionary<string, User> Users { get; set; }
        public Options Options { get; set; }
        public Profile Profile { get; set; }
        // Username of room host
        public string Host { get; set; }
        public Room(int id, Options options, string host)
        {
            Id = id;
            Users = new Dictionary<string, User>();
            Options = options;
            Profile = new Profile();
            Host = host;
        }
    }

    /*
     * Host-selected preferences chosen during room creation.
     */
    public class Options
    {
        public string Genre { get; set; }
        public double Popularity { get; set; }
        // Add additional customizations (for use w/ model)
    }

    /*
     * Object stores ideal song analysis result from algorithm
     */
    public class Profile
    {
        // Add Song analysis fields here, corresponding to SongAnalysis object fields from Spotify library
    }
}
