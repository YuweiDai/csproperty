using Microsoft.AspNet.Identity;
using CSCZJ.Core.Domain.Common;
using CSCZJ.Core.Domain.Properties;
using System;
using System.Collections.Generic;

namespace CSCZJ.Core.Domain.AccountUsers
{

    public class AccountUser : BaseEntity, IUser<int>
    {
        private ICollection<AccountUserRole> _customerRoles;

        public AccountUser()
        {
            this.AccountUserGuid = Guid.NewGuid();          
        }
        
        public string UserName { get; set; }

        public string NickName { get; set; }

        public bool Active { get; set; }

        public Guid AccountUserGuid { get; set; }
           
        public string LastIpAddress { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastLoginDate { get; set; }        

        public string Password { get; set; }

        public int PasswordFormatId { get;  set; }

        public PasswordFormat PasswordFormat
        {
            get { return (PasswordFormat)PasswordFormatId; }
            set { this.PasswordFormatId = (int)value; }
        }

        public string PasswordSalt { get; set; }

        public bool IsSystemAccount { get; set; }

        public string SystemName { get; set; }

        public int DisplayOrder { get; set; }

        public string Remark { get; set; }

        #region 扩展微信账号信息

        /// <summary>
        /// 微信用户唯一ID，用于绑定微信账号
        /// </summary>
        public string WechatOpenId { get; set; }

        /// <summary>
        /// 用户昵称，来自微信
        /// </summary>
        public string WechatNickName { get; set; }

        /// <summary>
        /// 用户头像，来自微信
        /// </summary>
        public string AvatarUrl { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<AccountUserRole> AccountUserRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<AccountUserRole>()); }
            protected set { _customerRoles = value; }
        }

        /// <summary>
        /// 所属单位
        /// </summary>
        public virtual GovernmentUnit Government { get; set; }
    }
}
