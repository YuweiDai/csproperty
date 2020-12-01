using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCZJ.API.Models.AccountUsers
{
    public class WechatLoginEventModel
    {
        /// <summary>
        /// 微信用户唯一标识
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信会话密钥
        /// </summary>
        public string Session_key { get; set; }

        /// <summary>
        /// 微信用户在开放平台的唯一标识符
        /// </summary>
        public string Unionid { get; set; }

        /// <summary>
        /// 错误码
        /// -1	系统繁忙，此时请开发者稍候再试
        ///  0	请求成功	
        /// 40029	code 无效	
        /// 45011	频率限制，每个用户每分钟100次
        /// </summary>
        public int Errcode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Errmsg { get; set; }
    }
}