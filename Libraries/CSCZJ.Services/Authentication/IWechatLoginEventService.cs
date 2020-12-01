using CSCZJ.Core.Domain.Authentication;

namespace CSCZJ.Services.Authentication
{
    public interface IWechatLoginEventService
    {
        void DeleteWechatLoginEvent(WechatLoginEvent wechatLoginEvent);

        void InsertWechatLoginEvent(WechatLoginEvent wechatLoginEvent);

        void UpdateWechatLoginEvent(WechatLoginEvent wechatLoginEvent);

        WechatLoginEvent GetWechatLoginEventById(int id);

        WechatLoginEvent GetWechatLoginEventByOpenId(string openId);

        bool ValidateUserToken(string token);

        string GetOpenIdWithToken(string token);
    }
}
