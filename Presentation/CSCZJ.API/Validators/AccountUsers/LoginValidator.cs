using CSCZJ.Web.Framework.Validators;
using FluentValidation;
using CSCZJ.Core.Domain.AccountUsers;
using CSCZJ.Api.Models.AccountUser;

namespace CSCZJ.Api.Validators.AccountUsers
{
    public class LoginValidator:BaseQMValidator<LoginModel>
    {
        public LoginValidator(AccountUserSettings customerSettings)
        {
            if(!customerSettings.UsernamesEnabled)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage("登录账户不能为空");
            }
        }
    }
}