using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.AccountUsers
{
    public class WechatAccountModel
    {
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string AvatarUrl { get; set; }
        public string Government { get; set; }
    }
}