using FluentValidation;
using CSCZJ.API.Models.Properties;
using CSCZJ.Core.Domain.Common;
using CSCZJ.Core.Domain.Media;
using CSCZJ.Services.Property;
using CSCZJ.Web.Framework.Validators;

namespace CSCZJ.API.Validators.Properties
{
    public class GovernmentUnitValidator: BaseQMValidator<GovernmentUnitModel>
    {
        private readonly IGovernmentService _governmentService;
        private readonly CommonSettings _commonSettings;
        private readonly MediaSettings _mediaSettings;
        public GovernmentUnitValidator(IGovernmentService governmentService, CommonSettings commonSettings, MediaSettings mediaSettings)
        {
            _governmentService = governmentService;
            _commonSettings = commonSettings;
            _mediaSettings = mediaSettings;

            RuleFor(s => s.Name).NotEmpty().WithMessage("单位名称不能为空").Must(BeUniqueName).WithMessage("名称 {0} 已存在", s => s.Name);
            RuleFor(s => s.Person).NotEmpty().WithMessage("联系人不能为空");
            RuleFor(s => s.Tel).NotEmpty().WithMessage("联系方式不能为空").Matches(commonSettings.TelAndMobliePartten).WithMessage("联系方式不是正确的电话号码格式");
        }

        private bool BeUniqueName(GovernmentUnitModel governmentUserModel, string accountUserName)
        {
            return _governmentService.NameUniqueCheck(accountUserName, governmentUserModel.Id);
        }
    }
}