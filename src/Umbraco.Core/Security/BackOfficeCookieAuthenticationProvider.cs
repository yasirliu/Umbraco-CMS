using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Umbraco.Core.Configuration;

namespace Umbraco.Core.Security
{
    /// <summary>
    /// The Umbraco back office cookie authentication provider
    /// </summary>
    public class BackOfficeCookieAuthenticationProvider : CookieAuthenticationProvider
    {
        /// <summary>
        /// On sign in will create a new token in the database to be tracked
        /// </summary>
        /// <param name="context"></param>
        public override void ResponseSignIn(CookieResponseSignInContext context)
        {
            
            base.ResponseSignIn(context);
        }

        public override void ResponseSignOut(CookieResponseSignOutContext context)
        {
            base.ResponseSignOut(context);

            //TODO: Here we should evict an active session so the cookie cannot be replayed:
            // http://issues.umbraco.org/issue/U4-5896#comment=67-30139
            var authTicket = context.OwinContext.GetUmbracoAuthTicket();

            //Make sure the definitely all of these cookies are cleared when signing out with cookies
            context.Response.Cookies.Append(UmbracoConfig.For.UmbracoSettings().Security.AuthCookieName, "", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(-1),
                Path = "/"
            });
            context.Response.Cookies.Append(Constants.Web.PreviewCookieName, "", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(-1),
                Path = "/"
            });
            context.Response.Cookies.Append(Constants.Security.BackOfficeExternalCookieName, "", new CookieOptions
            {
                Expires = DateTime.Now.AddYears(-1),
                Path = "/"
            });
        }

        /// <summary>
        /// Ensures that the culture is set correctly for the current back office user
        /// </summary>
        /// <param name="context"/>
        /// <returns/>
        public override Task ValidateIdentity(CookieValidateIdentityContext context)
        {
            var umbIdentity = context.Identity as UmbracoBackOfficeIdentity;
            if (umbIdentity != null && umbIdentity.IsAuthenticated)
            {
                Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture =
                        new CultureInfo(umbIdentity.Culture);
            }

            return base.ValidateIdentity(context);
        }
    }
}