using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web.Models;

namespace tuuncs.Models
{
    public class FullTrackComparer : IEqualityComparer<FullTrack>
    {
        public bool Equals(FullTrack first, FullTrack second)
        {
            return first.Id == second.Id ? true : false;
        }

        public int GetHashCode(FullTrack track)
        {
            return track.Id.GetHashCode();
        }
    }
}
