using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.AccountUsers
{
    public class WechatTokenModel
    {
        public string Token { get; set; }

        public int Days { get; set; }
    }
}