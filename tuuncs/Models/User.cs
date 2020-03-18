using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tuuncs.Models
{
    public class User
    {
        public string Username;
        public string Token;
        public User(string username, string token=null)
        {
            Username = username;
            Token = token;
        }
    }
}
