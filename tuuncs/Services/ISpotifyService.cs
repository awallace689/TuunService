using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace tuuncs.Services
{
    public interface ISpotifyService
    {
        // TODO: Create 'Track' interface and implementation to combine FullTrack and SongAnalysis objects.
        public Task<FullTrack> GetTrack(string id);
        public Task<IList<FullTrack>> GetTracks(IList<string> tracks);
    }
}
