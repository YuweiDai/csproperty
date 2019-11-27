using FluentValidation.Attributes;
using CSCZJ.Api.Validators.AccountUsers;
using CSCZJ.Web.Framework.Mvc;

namespace CSCZJ.Api.Models.AccountUser
{
    public class LoginModel:BaseQMModel
    {
        /// <summary>
        /// 用户名
        /// </summary>

        public string Username { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}