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
        //       If we decide not to use this API library, we won't have to change controller code as long
        //       as the replacement extends the interface.
        public Task<FullTrack> GetTrack(string id);
        public Task<IList<FullTrack>> GetTracks(IList<string> tracks);
    }
}
