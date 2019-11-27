using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Owin;
using CSCZJ.Core.Infrastructure;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using FluentValidation.WebApi;
using System.Web.Http.Validation;
using Microsoft.Owin.Security;
using CSCZJ.API;
using CSCZJ.Web.Framework.Filters;
using CSCZJ.Web.Framework;
using CSCZJ.Web.Framework.Security.Authorization;

[assembly: OwinStartup(typeof(CSCZJ.Api.Startup))]
namespace CSCZJ.Api
{
    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            //增加过滤
            config.Filters.Add(new ValidateModelAttribute());
            config.Filters.Add(new APIExceptionFilterAttribute());

            //solved from http://stackoverflow.com/questions/12905174/how-to-add-custom-modelvalidatorproviders-to-web-api-project
            //自定义配置
            config.Services.Add(typeof(ModelValidatorProvider), new FluentValidationModelValidatorProvider(new CSCZJValidatorFactory()));

           // app.UseCors()
            ConfigureOAuth(app);

            //initialize engine context
            EngineContext.Initialize(false, config);

            // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
            // and finally the standard Web API middleware.
            app.UseAutofacMiddleware(EngineContext.Current.ContainerManager.Container);
            app.UseAutofacWebApi(config);

            app.UseWebApi(config);

            SqlServerTypes.Utilities.LoadNativeAssemblies(System.Web.HttpContext.Current.Server.MapPath("~/bin"));

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new QMAuthorizationServerProvider(),
                //AccessTokenProvider=new AccessTokenAuthorizationServerProvider(),
                //refresh token provider
                RefreshTokenProvider = new QMRefreshTokenProvider(),
                AccessTokenExpireTimeSpan = new System.TimeSpan(0, 60, 0),
                AuthenticationMode = AuthenticationMode.Active,
                //HTTPS is allowed only AllowInsecureHttp = false
#if DEBUG
                AllowInsecureHttp = true,
#endif
                ApplicationCanDisplayErrors = true,
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}