using CSCZJ.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.AccountUsers
{
    public class AccountResetPwdModel: BaseQMEntityModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }
        public string Newpassword { get; set; }
        public string Againpassword { get; set; }


    }
}