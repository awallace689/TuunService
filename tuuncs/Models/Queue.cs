using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;

namespace tuuncs.Models
{
    public class Queue
    {
        List<FullTrack> Tracks { get; set; }

        public Queue(List<FullTrack> tracks)
        {
            Tracks = tracks;
        }

        public List<FullTrack> Take(int count)
        {
            List<FullTrack> res = new List<FullTrack>();
            while (Tracks.Count > 0 && count > 0)
            {
                res.Add(Tracks[0]);
                Tracks.RemoveAt(0);
            }

            return res;
        }
    }
}
