using CSCZJ.API.Models.AccountUsers;
using CSCZJ.Services.Authentication;
using CSCZJ.Web.Api.Extensions;
using CSCZJ.Web.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Http;

namespace CSCZJ.API.Controllers
{
    /// <summary>
    /// 认证服务器
    /// </summary>
    [RoutePrefix("Auth")]
    public class AuthenticationController : ApiController
    {
        private string _wechatAppId = ConfigurationManager.AppSettings["AppId"];
        private string _wechatSecret = ConfigurationManager.AppSettings["AppSecret"];
        private readonly IWechatLoginEventService _wechatLoginEventService;

        public AuthenticationController(IWechatLoginEventService wechatLoginEventService)
        {
            _wechatLoginEventService = wechatLoginEventService;
        }

        [HttpGet]
        [Route("Wechat")]
        public IHttpActionResult Wechat(string code)
        {
            //定义返回对象
            var response = new ResponseObject<WechatTokenModel>();

            try
            {
                #region 微信认证
                string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", _wechatAppId, _wechatSecret, code);

                var httpReq = WebRequest.Create(url);
                httpReq.Method = "Get";

                var httpResponse = httpReq.GetResponse() as HttpWebResponse;
                var responseTxt = string.Empty;
                using (StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    responseTxt = sr.ReadToEnd();
                }
                #endregion

                if (string.IsNullOrEmpty(responseTxt)) response = new ResponseObject<WechatTokenModel> { Code = "500", Message = "微信登录失败" };
                var wechatLoginViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WechatLoginEventModel>(responseTxt);

                if (wechatLoginViewModel.Errcode == 0)
                {
                    #region 记录登录状态
                    var wechatLoginEvent = _wechatLoginEventService.GetWechatLoginEventByOpenId(wechatLoginViewModel.OpenId);
                    if (wechatLoginEvent == null)
                    {
                        wechatLoginEvent = wechatLoginViewModel.ToEntity();
                        wechatLoginEvent.Token = Guid.NewGuid().ToString();
                        _wechatLoginEventService.InsertWechatLoginEvent(wechatLoginEvent);
                    }
                    else
                    {
                        _wechatLoginEventService.UpdateWechatLoginEvent(wechatLoginEvent);
                    }

                    #endregion

                    response = new ResponseObject<WechatTokenModel> { Code = "200", Data = new WechatTokenModel { Token = wechatLoginEvent.Token, Days = 1 } };
                }
                else
                {
                    response = new ResponseObject<WechatTokenModel> { Code = wechatLoginViewModel.Errcode.ToString(), Message = wechatLoginViewModel.Errmsg };
                }

            }
            catch (Exception ex)
            {
                response = new ResponseObject<WechatTokenModel> { Code = "500", Message = ex.Message };
                //_logger.Error(string.Format("微信登录失败，错误原因：{0}", ex.GetOriginalException().Message));
            }

            return Ok(response);
        }
    }
}
