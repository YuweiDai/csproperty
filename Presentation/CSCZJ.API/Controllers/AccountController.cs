using CSCZJ.Core;
using CSCZJ.Services.Authentication;
using CSCZJ.Services.Common;
using CSCZJ.Services.AccountUsers;
using CSCZJ.Services.Messages;
using CSCZJ.Web.Framework.Controllers;
using System.Web.Http;
using CSCZJ.Core.Domain.AccountUsers;
using CSCZJ.Services.Logging;
using System;
using CSCZJ.Web.Framework.Response;
using CSCZJ.API.Models.AccountUsers;
using CSCZJ.Web.Api.Extensions;
using System.Linq;
using CSCZJ.Services.Property;
using CSCZJ.Services.Security;
using CSCZJ.Web.Framework;
using System.Web;
using CSCZJ.Api.Models.AccountUser;

namespace CSCZJ.API.Controllers
{
    [RoutePrefix("Systemmanage/Accounts")]
    public class AccountController : BaseAdminApiController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountUserService _accountService;
        private readonly IWechatLoginEventService _wechatLoginEventService;
        private readonly IAccountUserRegistrationService _accountUserRegistrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IAccountUserActivityService _accountUserActivityService;
        private readonly IGovernmentService _governmentService;
        private readonly IEncryptionService _encryptionService;

        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly AccountUserSettings _accountUserSettings;

        public AccountController(
            IAuthenticationService authenticationService,
            IWechatLoginEventService wechatLoginEventService,
            IAccountUserService customerService,
                 IAccountUserRegistrationService customerRegistrationService,
             IGenericAttributeService genericAttributeService,
            IWorkflowMessageService workflowMessageService,
            IAccountUserActivityService accountUserActivityService,
            IGovernmentService governmentService,
             IWebHelper webHelper,
            IWorkContext workContext, IEncryptionService encryptionService,
            AccountUserSettings customerSettings)
        {
            _authenticationService = authenticationService;
            _wechatLoginEventService = wechatLoginEventService;
            _accountService = customerService;
            _accountUserRegistrationService = customerRegistrationService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _accountUserActivityService = accountUserActivityService;
            _governmentService = governmentService;
            _encryptionService = encryptionService;
            _webHelper = webHelper;
            _workContext = workContext;
            _accountUserSettings = customerSettings;
        }

        #region 用户API

        [HttpGet]
        [Route("Unique/{name}")]
        public IHttpActionResult UniqueCheck(string name)
        {
            var result = !_accountService.NameUniqueCheck(name);

            return Ok(result);
        }

        [HttpGet]
        [Route("{accountId:int}")]
        public IHttpActionResult Get(int accountId)
        {
            var account = _accountService.GetAccountUserById(accountId);
            if (account == null || account.Deleted)
                return NotFound();


            var model = account.ToModel();
            model.IsAdministrator = account.IsAdmin();

            //activity log
            _accountUserActivityService.InsertActivity("GetAccountInfo", "获取 名为 {0} 的用户信息", account.UserName);

            return Ok(model);         
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(string query = "", string sort = "", int pageSize = Int32.MaxValue, int pageIndex = 0, bool showHidden = false)
        {
            //初始化排序条件
            var sortConditions = PropertySortCondition.Instance(sort);

            //特殊字段排序调整
            if (sort.ToLower().StartsWith("governmentname")) sortConditions[0].PropertyName = "Government";


            var accounts = _accountService.GetAllAccountUsers(query, pageIndex, pageSize, showHidden, sortConditions);

            var response = new ListResponse<AccountUserModel>
            {
                Paging = new Paging
                {
                    PageIndex = pageIndex,
                    PageSize = pageIndex,
                    Total = accounts.TotalCount,
                    FilterCount = string.IsNullOrEmpty(query) ? accounts.TotalCount : accounts.Count,
                },
                Data = accounts.Select(s =>
                {
                    var accountModel = s.ToModel();
                    accountModel.GovernmentName = s.Government.Name;
                    return accountModel;
                })
            };

            //activity log
            _accountUserActivityService.InsertActivity("GetAccountList", "获取用户列表信息");

            return Ok(response);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(AccountUserModel accountModel)
        {
            var account = accountModel.ToEntity();

            //问题： An entity object cannot be referenced by multiple instances of IEntityChangeTracker 
            //状态：Fixed
            //原因：不明，但是应该和缓存机制有关
            var government = _governmentService.GetGovernmentUnitById(accountModel.GovernmentId);
            if (government == null) return BadRequest("用户所属单位不存在");
            account.Government = government;

            var registerRole = _accountService.GetAccountUserRoleBySystemName(SystemAccountUserRoleNames.Registered);
            var adminRole = _accountService.GetAccountUserRoleBySystemName(SystemAccountUserRoleNames.Administrators);

            account.Password = "123456";  //设置初始密码

            var role = _accountService.GetAccountUserRoleBySystemName(accountModel.RoleName);
            if (role != null && accountModel.RoleName != SystemAccountUserRoleNames.Registered) account.AccountUserRoles.Add(role);
       
            var registrationRequest = new AccountUserRegistrationRequest(account, account.UserName,
    account.Password, _accountUserSettings.DefaultPasswordFormat, accountModel.Active);

            var registrationResult = _accountUserRegistrationService.RegisterAccountUser(registrationRequest);
            if (registrationResult.Success)
            {
                //保存用户
                _accountService.InsertAccountUser(account);

                //activity log
                _accountUserActivityService.InsertActivity("AddNewAccount", "增加 名为 {0} 的用户", account.UserName);

                return Ok(account.ToModel());
            }
            else return BadRequest("添加用户失败");
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{accountId:int}")]
        public IHttpActionResult UpdateAccount(int accountId, AccountUserModel accountModel)
        {
            var account = _accountService.GetAccountUserById(accountId);
            if (account == null || account.Deleted) return NotFound();
            account = accountModel.ToEntity(account);

            var registerRole = _accountService.GetAccountUserRoleBySystemName(SystemAccountUserRoleNames.Registered);

            account.AccountUserRoles.Clear();

            var role = _accountService.GetAccountUserRoleBySystemName(accountModel.RoleName);
            if (role != null && accountModel.RoleName != SystemAccountUserRoleNames.Registered) account.AccountUserRoles.Add(role);
            account.AccountUserRoles.Add(registerRole);

            if (accountModel.InitPassword) account.Password = "123456";  //设置初始密码

            //单位更新
            if (accountModel.GovernmentId != account.Government.Id)
            {
                var governament = _governmentService.GetGovernmentUnitById(accountModel.GovernmentId);
                if (governament == null) return BadRequest("用户所属单位不存在");
                account.Government = governament;
            }

            //保存用户
            _accountService.UpdateAccountUser(account);

            //activity log
            _accountUserActivityService.InsertActivity("UpdateAccount", "更新 名为 {0} 的用户的基本信息", account.UserName);

            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));

            return Ok(account.ToModel());
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("resetpwd")]
        public IHttpActionResult ResetPwd(AccountResetPwdModel accountResetPwdModel)
        {

            var account = _accountService.GetAccountUserByUsername(accountResetPwdModel.UserName);

            if (account.Password != _encryptionService.EncryptText(accountResetPwdModel.Password)) return BadRequest("原密码输入不正确！");

            if (accountResetPwdModel.Newpassword != accountResetPwdModel.Againpassword) return BadRequest("两次输入的密码不一致！");

            account.Password = _encryptionService.EncryptText(accountResetPwdModel.Newpassword); 

            //保存用户
            _accountService.UpdateAccountUser(account);

            //activity log
            _accountUserActivityService.InsertActivity("UpdateAccount", "更新 名为 {0} 的用户的基本信息", account.UserName);

            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Added"));

            return Ok();
        }

        [HttpDelete]
        [Route("{accountId:int}")]
        public IHttpActionResult DeleteAccount(int accountId)
        {

            var account = _accountService.GetAccountUserById(accountId);
            if (account == null || account.Deleted) return NotFound();

            _accountService.DeleteAccountUser(account);

            //activity log
            _accountUserActivityService.InsertActivity("DeleteAccount", "删除 名为 {0} 的用户", account.UserName);

            //通知
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));

            return Ok();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="accountIdString"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{accountIdString}")]
        public IHttpActionResult DeleteAccount(string accountIdString)
        {
            var idStringArr = accountIdString.Split('_');
            foreach (var idStr in idStringArr)
            {
                int id = 0;
                if (!int.TryParse(idStr, out id)) continue;

                var account = _accountService.GetAccountUserById(id);
                if (account == null) continue;

                _accountService.DeleteAccountUser(account);
            }


            //活动日志
            _accountUserActivityService.InsertActivity("DeleteAccounts", "批量删除 Id为 {0} 的用户", accountIdString);

            //通知
            //SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));

            return Ok();
        }
        #endregion

        #region 微信小程序 API

        /// <summary>
        /// 获取当前微信账号的状态
        /// 0代表未绑定；
        /// 1代表已绑定
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("GetWechatStatus")]
        public IHttpActionResult GetWechatStatus()
        {
            var response = new ResponseObject<WechatAccountModel>();

            var message = "当前用户未绑定";
            AccountUser account = null;

            try
            {
                var request = HttpContext.Current.Request;
                var authToken = request.Headers["Authorization"].Replace("Bear ", "");

                var openId = _wechatLoginEventService.GetOpenIdWithToken(authToken);
                if (string.IsNullOrEmpty(openId)) throw new Exception("当前token无效");
                account = _accountService.GetAccountByOpenId(openId);

                if (account != null)
                {
                    response.Data = new WechatAccountModel()
                    {
                        UserName = account.UserName,
                        NickName = account.WechatNickName,
                        AvatarUrl = account.AvatarUrl,
                        Government = account.Government.Name
                    };
                    message = "用户已绑定。";
                }

                response.Message = message;
                response.Code = "200";
                //_logger.Information("获取微信状态成功！", account);
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Code = "401";

                //_logger.Error(string.Format("获取微信状态失败，错误原因：{0}", ex.GetOriginalException().Message), finder);
            }

            return Ok(response);
        }

        /// <summary>
        /// 绑定学生账号
        /// </summary>
        /// <param name="accountBindModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Binding")]
        public IHttpActionResult BindingFinder(AccountBindModel accountBindModel)
        {
            var response = new SimpleResponse();
            AccountUser account = null;
            try
            {
                if (string.IsNullOrWhiteSpace(accountBindModel.Username)) throw new Exception("账号不能为空！");
                if (string.IsNullOrWhiteSpace(accountBindModel.Password)) throw new Exception("密码不能为空！");

                //删除请求中的空字符串
                accountBindModel.Username = accountBindModel.Username.Replace(" ", "");
                accountBindModel.Password = accountBindModel.Password.Replace(" ", "");


                var result = _accountUserRegistrationService.ValidateAccountUser(accountBindModel.Username, accountBindModel.Password);
                if (result != AccountUserLoginResults.Successful) throw new Exception("账号和密码不匹配！");

                account = _accountService.GetAccountUserByUsername(accountBindModel.Username);
                account.WechatNickName = accountBindModel.NickName;
                account.AvatarUrl = accountBindModel.AvatarUrl;

                var request = HttpContext.Current.Request;
                var authToken = request.Headers["Authorization"].Replace("Bear ", "");
                var openId = _wechatLoginEventService.GetOpenIdWithToken(authToken);

                account.WechatOpenId = openId;
                _accountService.UpdateAccountUser(account);
                response.Message = "绑定成功！";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                //var finderInfo = string.Format("{0}{1}{2}{3}", accountBindModel.Name, accountBindModel.SchoolNumber);
                //_logger.Error(string.Format("{0}，绑定失败，错误原因：{1}", finderInfo, ex.GetOriginalException().Message), account);

                response.Message = ex.Message;
                response.Code = "400";
            }

            return Ok(response);
        }

        /// <summary>
        /// 解绑账号
        /// </summary>
        /// <param name="finderBindingModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("UnBinding")]
        public IHttpActionResult UnBindingFinder()
        {
            var response = new SimpleResponse();
            AccountUser account = null;
            try
            {
                var request = HttpContext.Current.Request;
                var authToken = request.Headers["Authorization"].Replace("Bear ", "");
                var openId = _wechatLoginEventService.GetOpenIdWithToken(authToken);
                account = _accountService.GetAccountByOpenId(openId);
                if (account == null) throw new Exception("当前微信未绑定学生信息！");

                if (account == null)
                {
                    throw new Exception("所输入的学生的学校、班级、姓名和学号不匹配！");
                }
                else
                {
                    account.WechatOpenId = string.Empty;
                    _accountService.UpdateAccountUser(account);
                    response.Message = "绑定成功！";

                    //_logger.Information("学生与微信解绑成功！", account);
                }
                response.Code = "200";
            }
            catch (Exception ex)
            {

                //_logger.Error(string.Format("{0}，解绑失败，错误原因：{1}", account.Name, ex.GetOriginalException().Message), account);

                response.Message = ex.Message;
                response.Code = "400";
            }

            return Ok(response);
        }


        #endregion

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("Register")]
        //public IHttpActionResult Register(RegisterModel model)
        //{
        //    //判断当前是否为认证用户
        //    if (ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
        //        return BadRequest("当前用户已经注册");

        //    //检查是否允许注册用户
        //    if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
        //    {
        //        return BadRequest("用户注册已关闭");
        //    }

        //    if (_workContext.CurrentCustomer.IsRegistered()) return BadRequest("当前用户已注册");

        //    var accountUser = _workContext.CurrentCustomer;
        //    if (accountUser.IsRegistered()) return BadRequest("当前用户已经注册");

        //    //TODO：自定义属性

        //    //TODO：验证码

        //    if (_customerSettings.UsernamesEnabled && model.Username != "")
        //    {
        //        model.Username = model.Username.Trim();
        //    }

        //    bool isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
        //    var registrationRequest = new CustomerRegistrationRequest(accountUser, model.Email, model.Mobile, model.Username,
        //        model.Password, _customerSettings.DefaultPasswordFormat, isApproved);

        //    var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
        //    if (registrationResult.Success)
        //    {
        //        //associated with external account (if possible)

        //        //insert default address (if possible)

        //        //notifications
        //        //_workflowMessageService

        //        switch (_customerSettings.UserRegistrationType)
        //        {
        //            case UserRegistrationType.EmailValidation:
        //                {
        //                    //email validation message
        //                    _genericAttributeService.SaveAttribute(accountUser, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
        //                    _workflowMessageService.SendCustomerEmailValidationMessage(accountUser);

        //                    //result
        //                    return Ok(new ApiResponseResult { Code = 1, Message = "邮件认证" });
        //                }
        //            case UserRegistrationType.AdminApproval:
        //                {
        //                    return Ok(new ApiResponseResult { Code = 1, Message = "管理员认证" });
        //                }
        //            default:
        //                {
        //                    //send accountUser welcome message
        //                    _workflowMessageService.SendCustomerWelcomeMessage(accountUser);

        //                    return Ok(new ApiResponseResult { Code = 1, Message = "注册成功" });
        //                }
        //        }
        //    }

        //    //errors
        //    foreach (var error in registrationResult.Errors)
        //        ModelState.AddModelError("", error);
        //    return BadRequest(ModelState);
        //}


        #region 登陆实现
        //[HttpPost]
        //[Route("Login")]
        //public IHttpActionResult Login(LoginModel model)
        //{

        //    var loginResult = _customerRegistrationService.ValidateCustomer(model.Account, model.Password);

        //    if (loginResult == CustomerLoginResults.Successful)
        //    {
        //        var accountUser = _customerService.GetCustomerByAccount(model.Account);

        //        //migrate shopping cart
        //        //_shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, accountUser, true);

        //        //sign in new accountUser
        //        _authenticationService.SignIn(accountUser, model.RememberMe);

        //        //activity log
        //        //_accountUserActivityService.InsertActivity("PublicAccount.Login", _localizationService.GetResource("ActivityLog.PublicAccount.Login"), accountUser);

        //        return Ok();
        //    }
        //    else
        //    {
        //        switch (loginResult)
        //        {
        //            case CustomerLoginResults.NotActive:
        //                return BadRequest("用户未激活");
        //            default:
        //                return BadRequest("用户名不存在或密码不正确");
        //        }
        //    }
        //} 
        #endregion

        // GET: api/accountUser/5
        //[HttpGet]
        //[Route("{id:int}")]
        //[Authorize]
        //public string Get(int id)
        //{
        //    var user = System.Web.HttpContext.Current.User;
        //    return user.Identity.Name;
        //}

        // POST: api/accountUser
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/accountUser/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/accountUser/5
        public void Delete(int id)
        {
        }
    }
}