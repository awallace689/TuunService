using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("Tuun")]
    public class TuunController : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;

        public TuunController(ILogger<Tuun> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            return Ok("Route to /Tuun/proof for proof of concept!");
        }

        [HttpGet]
        [Route("proof")]
        public async Task<IActionResult> proof()
        {
            return Ok(await asyncFunction("This is my async string!"));
        }

        public async Task<string> asyncFunction(string str)
        {
            string asyncStr = await Task.FromResult(str);
            return asyncStr;
        }
    }
}
