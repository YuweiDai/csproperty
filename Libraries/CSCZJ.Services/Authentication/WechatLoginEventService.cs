using CSCZJ.Core.Data;
using CSCZJ.Core.Domain.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCZJ.Services.Authentication
{
    public class WechatLoginEventService : IWechatLoginEventService
    {
        private readonly IRepository<WechatLoginEvent> _repository;

        public WechatLoginEventService(IRepository<WechatLoginEvent> repository)
        {
            _repository = repository;
        }

        public void DeleteWechatLoginEvent(WechatLoginEvent wechatLoginEvent)
        {
            throw new NotImplementedException();
        }


        public WechatLoginEvent GetWechatLoginEventById(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据微信用户OpenId获取
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public WechatLoginEvent GetWechatLoginEventByOpenId(string openId)
        {
            if (string.IsNullOrEmpty(openId))
                throw new ArgumentNullException("openId is null");

            var wechatLoginEvent = _repository.Table.Where(wle => wle.OpenId == openId).SingleOrDefault();

            return wechatLoginEvent;
        }

        public void InsertWechatLoginEvent(WechatLoginEvent wechatLoginEvent)
        {
            if (wechatLoginEvent == null)
                throw new ArgumentNullException("wechatLoginEvent is null");

            _repository.Insert(wechatLoginEvent);
        }

        public void UpdateWechatLoginEvent(WechatLoginEvent wechatLoginEvent)
        {
            if (wechatLoginEvent == null)
                throw new ArgumentNullException("wechatLoginEvent is null");

            wechatLoginEvent.Token = Guid.NewGuid().ToString();
            _repository.Update(wechatLoginEvent);
        }

        /// <summary>
        /// 验证token 是否有效
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateUserToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            else
            {
                var enties = _repository.TableNoTracking.Where(wle => wle.Token == token);

                return enties.Count() > 0;
            }
        }

        /// <summary>
        /// 根据Token获取微信OpenId
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetOpenIdWithToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException("token is null");

            var wechatLoginEvent = _repository.TableNoTracking.Where(wle => wle.Token == token).SingleOrDefault();
            if (wechatLoginEvent != null) return wechatLoginEvent.OpenId;
            else return string.Empty;
        }
    }
}
