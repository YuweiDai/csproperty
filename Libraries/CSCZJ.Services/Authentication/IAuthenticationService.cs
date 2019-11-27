using CSCZJ.Core.Domain.AccountUsers;

namespace CSCZJ.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService
    {
        //void SignIn(AccountUser property, bool createPersistentCookie);

        void SignOut();

        AccountUser GetAuthenticatedAccountUser();
    }
}
