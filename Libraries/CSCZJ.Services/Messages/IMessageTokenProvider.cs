using CSCZJ.Core;
using CSCZJ.Core.Domain.AccountUsers;
using CSCZJ.Core.Domain.Messages;
using CSCZJ.Services.Events;
using System.Collections.Generic;

namespace CSCZJ.Services.Messages
{
    public partial interface IMessageTokenProvider
    {


        void AddStoreTokens(IList<Token> tokens, EmailAccount emailAccount);

        void AddAccountUserTokens(IList<Token> tokens, AccountUser property);
    }
}