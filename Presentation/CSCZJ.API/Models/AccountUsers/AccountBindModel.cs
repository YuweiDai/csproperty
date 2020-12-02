using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.AccountUsers
{
    public class AccountBindModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Nickname { get; set; }

        public string AvatarUrl { get; set; }
    }
}