using FluentValidation;
using CSCZJ.API.Models.AccountUsers;
using CSCZJ.Services.AccountUsers;
using CSCZJ.Services.Property;
using CSCZJ.Web.Framework.Validators;

namespace CSCZJ.API.Validators.AccountUsers
{
    public class AccountUserValidator : BaseQMValidator<AccountUserModel>
    {
        private readonly IAccountUserService _accountUserService = null;
        private readonly IGovernmentService _governmentService = null;

        public AccountUserValidator(IAccountUserService accountUserService, IGovernmentService governmentService)
        {
            _accountUserService = accountUserService;
            _governmentService = governmentService;

            RuleFor(s => s.UserName).NotEmpty().WithMessage("用户名不能为空").Must(BeUniqueName).WithMessage("名称 {0} 已存在", s => s.UserName);
            
            RuleFor(s => s.GovernmentId).Must(governmentId =>
            {
                var accountUserGovernment = governmentService.GetGovernmentUnitById(governmentId);
                return accountUserGovernment != null && !accountUserGovernment.Deleted;
            }).WithMessage(string.Format("单位不存在"));
        }

        private bool BeUniqueName(AccountUserModel accountUserModel, string accountUserName)
        {
            return _accountUserService.NameUniqueCheck(accountUserName, accountUserModel.Id);
        }
    }
}