﻿using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using CSCZJ.Core.Infrastructure;
using CSCZJ.Services.AccountUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSCZJ.Web.Framework.Security.Authorization
{
    public class QMAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var customerRegistrationService = EngineContext.Current.Resolve<IAccountUserRegistrationService>();
            var user = await customerRegistrationService.ValidateAccountUserAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("无效授权", "用户名或密码错误");
                return;
            }
            else
            {
                if(!user.IsRegistered()|| !user.IsAdmin())
                {
                    context.SetError("无效授权", "该用户没有登陆权限！");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.UserName) ? "" : user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.AccountUserGuid.ToString()));
            foreach (var role in user.AccountUserRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }
            identity.AddClaim(new Claim(ClaimTypes.Uri, context.Request.Uri.ToString()));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "as:client_id", context.ClientId ?? string.Empty
                },
                {
                    "userName", user.UserName
                },
                {
                    "nickName", user.NickName??string.Empty
                },
                {
                    "userRoles",string.Join(",", user.AccountUserRoles.Select(cr=>cr.Name).ToArray())
                }
            });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);            
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}
