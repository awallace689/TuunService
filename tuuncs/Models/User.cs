﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuuncs.Models
{
    public class User
    {
        public string Username { get; set; }
        public string? Token { get; set; }
        public User(string username, string token=null, string connectId=null)
        {
            Username = username;
            Token = token;
            ConnectionId = connectId;
        }
        public string? ConnectionId { get; set; }
    }
}
