using CSCZJ.Core;
using CSCZJ.Core.Domain.AccountUsers;
using CSCZJ.Core.Infrastructure;
using CSCZJ.Services.Common;
using System;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;

namespace CSCZJ.Web.Framework.Controllers
{
    public class StoreLastVisitedPageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

 
            if (actionContext == null || actionContext.Request == null)
                return;

            //only GET requests
            if (!String.Equals(actionContext.Request.Method.Method, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var customerSettings = EngineContext.Current.Resolve<AccountUserSettings>();
            if (!customerSettings.StoreLastVisitedPage)
                return;

            //暂时不实现
            //var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //var pageUrl = webHelper.GetThisPageUrl(true);
            //if (!String.IsNullOrEmpty(pageUrl))
            //{
            //    var workContext = EngineContext.Current.Resolve<IWorkContext>();
            //    var genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();

            //    var previousPageUrl = workContext.AccountUsers.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage);
            //    if (!pageUrl.Equals(previousPageUrl))
            //    {
            //        genericAttributeService.SaveAttribute(workContext.AccountUsers, SystemCustomerAttributeNames.LastVisitedPage, pageUrl);
            //    }
            //}

            base.OnActionExecuting(actionContext);
        }
    }
}