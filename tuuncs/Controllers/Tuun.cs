using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace tuuncs.Controllers
{
    [ApiController]
    [Route("/Tuun/proof")]
    public class Tuun : ControllerBase
    { 
        private readonly ILogger<Tuun> _logger;

        public Tuun(ILogger<Tuun> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            return new string("This is Tuun now!");
        }
    }
}
