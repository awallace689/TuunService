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
        public Task<FullTrack> GetTrack(string id);
        public Task<IList<FullTrack>> GetTracks(IList<string> tracks);
        public void Initialize();
    }
}
