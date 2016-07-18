using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Constants = Umbraco.Core.Constants;
using GlobalSettings = Umbraco.Core.Configuration.GlobalSettings;

namespace Umbraco.Web.Security.Identity
{
    ///// <summary>
    /////     Static helper class used to configure a CookieAuthenticationProvider to validate a cookie against a user's security
    /////     stamp
    ///// </summary>
    //public static class CustomSecurityStampValidator
    //{
    //    /// <summary>
    //    ///     Can be used as the ValidateIdentity method for a CookieAuthenticationProvider which will check a user's security
    //    ///     stamp after validateInterval
    //    ///     Rejects the identity if the stamp changes, and otherwise will call regenerateIdentity to sign in a new
    //    ///     ClaimsIdentity
    //    /// </summary>
    //    /// <typeparam name="TManager"></typeparam>
    //    /// <typeparam name="TUser"></typeparam>
    //    /// <param name="validateInterval"></param>
    //    /// <param name="regenerateIdentity"></param>
    //    /// <returns></returns>
    //    public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager, TUser>(
    //        TimeSpan validateInterval, Func<TManager, TUser, Task<ClaimsIdentity>> regenerateIdentity)
    //        where TManager : UserManager<TUser, string>
    //        where TUser : class, IUser<string>
    //    {
    //        return OnValidateIdentity(validateInterval, regenerateIdentity, id => id.GetUserId());
    //    }

    //    /// <summary>
    //    ///     Can be used as the ValidateIdentity method for a CookieAuthenticationProvider which will check a user's security
    //    ///     stamp after validateInterval
    //    ///     Rejects the identity if the stamp changes, and otherwise will call regenerateIdentity to sign in a new
    //    ///     ClaimsIdentity
    //    /// </summary>
    //    /// <typeparam name="TManager"></typeparam>
    //    /// <typeparam name="TUser"></typeparam>
    //    /// <typeparam name="TKey"></typeparam>
    //    /// <param name="validateInterval"></param>
    //    /// <param name="regenerateIdentityCallback"></param>
    //    /// <param name="getUserIdCallback"></param>
    //    /// <returns></returns>
    //    public static Func<CookieValidateIdentityContext, Task> OnValidateIdentity<TManager, TUser, TKey>(
    //        TimeSpan validateInterval, Func<TManager, TUser, Task<ClaimsIdentity>> regenerateIdentityCallback,
    //        Func<ClaimsIdentity, TKey> getUserIdCallback)
    //        where TManager : UserManager<TUser, TKey>
    //        where TUser : class, IUser<TKey>
    //        where TKey : IEquatable<TKey>
    //    {
    //        if (getUserIdCallback == null)
    //        {
    //            throw new ArgumentNullException("getUserIdCallback");
    //        }
    //        return async context =>
    //        {
    //            var currentUtc = DateTimeOffset.UtcNow;
    //            if (context.Options != null && context.Options.SystemClock != null)
    //            {
    //                currentUtc = context.Options.SystemClock.UtcNow;
    //            }
    //            var issuedUtc = context.Properties.IssuedUtc;

    //            // Only validate if enough time has elapsed
    //            var validate = (issuedUtc == null);
    //            if (issuedUtc != null)
    //            {
    //                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
    //                validate = timeElapsed > validateInterval;
    //            }
    //            if (validate)
    //            {
    //                var manager = context.OwinContext.GetUserManager<TManager>();
    //                var userId = getUserIdCallback(context.Identity);
    //                if (manager != null && userId != null)
    //                {
    //                    var user = await manager.FindByIdAsync(userId);
    //                    var reject = true;
    //                    // Refresh the identity if the stamp matches, otherwise reject
    //                    if (user != null && manager.SupportsUserSecurityStamp)
    //                    {
    //                        var securityStamp =
    //                            context.Identity.FindFirstValue(Microsoft.AspNet.Identity.Constants.DefaultSecurityStampClaimType);
    //                        if (securityStamp == await manager.GetSecurityStampAsync(userId))
    //                        {
    //                            reject = false;
    //                            // Regenerate fresh claims if possible and resign in
    //                            if (regenerateIdentityCallback != null)
    //                            {
    //                                var identity = await regenerateIdentityCallback.Invoke(manager, user);
    //                                if (identity != null)
    //                                {
    //                                    // Fix for regression where this value is not updated
    //                                    // Setting it to null so that it is refreshed by the cookie middleware
    //                                    context.Properties.IssuedUtc = null;
    //                                    context.Properties.ExpiresUtc = null;
    //                                    context.OwinContext.Authentication.SignIn(context.Properties, identity);
    //                                }
    //                            }
    //                        }
    //                    }
    //                    if (reject)
    //                    {
    //                        context.RejectIdentity();
    //                        context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
    //                    }
    //                }
    //            }
    //        };
    //    }
    //}


    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Called at the end of configuring middleware
        /// </summary>
        /// <param name="app"></param>
        /// <remarks>
        /// This could be used for something else in the future - maybe to inform Umbraco that middleware is done/ready, but for
        /// now this is used to raise the custom event
        /// 
        /// This is an extension method in case developer entirely replace the UmbracoDefaultOwinStartup class, in which case they will
        /// need to ensure they call this extension method in their startup class.
        /// 
        /// TODO: Move this method in v8, it doesn't belong in this namespace/extension class
        /// </remarks>
        public static void FinalizeMiddlewareConfiguration(this IAppBuilder app)
        {
            UmbracoDefaultOwinStartup.OnMiddlewareConfigured(new OwinMiddlewareConfiguredEventArgs(app));
        }

        /// <summary>
        /// Sets the OWIN logger to use Umbraco's logging system
        /// </summary>
        /// <param name="app"></param>
        /// <remarks>
        /// TODO: Move this method in v8, it doesn't belong in this namespace/extension class
        /// </remarks>
        public static void SetUmbracoLoggerFactory(this IAppBuilder app)
        {
            app.SetLoggerFactory(new OwinLoggerFactory());
        }
        
        /// <summary>
        /// Configure Default Identity User Manager for Umbraco
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="userMembershipProvider"></param>
        public static void ConfigureUserManagerForUmbracoBackOffice(this IAppBuilder app,
            ApplicationContext appContext,
            MembershipProviderBase userMembershipProvider)
        {
            if (appContext == null) throw new ArgumentNullException("appContext");
            if (userMembershipProvider == null) throw new ArgumentNullException("userMembershipProvider");

            //Configure Umbraco user manager to be created per request
            app.CreatePerOwinContext<BackOfficeUserManager>(
                (options, owinContext) => BackOfficeUserManager.Create(
                    options,
                    appContext.Services.UserService,
                    appContext.Services.ExternalLoginService,
                    userMembershipProvider));

            //Create a sign in manager per request
            app.CreatePerOwinContext<BackOfficeSignInManager>((options, context) => BackOfficeSignInManager.Create(options, context, app.CreateLogger<BackOfficeSignInManager>()));
        }

        /// <summary>
        /// Configure a custom UserStore with the Identity User Manager for Umbraco
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="userMembershipProvider"></param>
        /// <param name="customUserStore"></param>
        public static void ConfigureUserManagerForUmbracoBackOffice(this IAppBuilder app,
            ApplicationContext appContext,
            MembershipProviderBase userMembershipProvider,
            BackOfficeUserStore customUserStore)
        {
            if (appContext == null) throw new ArgumentNullException("appContext");
            if (userMembershipProvider == null) throw new ArgumentNullException("userMembershipProvider");
            if (customUserStore == null) throw new ArgumentNullException("customUserStore");

            //Configure Umbraco user manager to be created per request
            app.CreatePerOwinContext<BackOfficeUserManager>(
                (options, owinContext) => BackOfficeUserManager.Create(
                    options,
                    customUserStore,
                    userMembershipProvider));

            //Create a sign in manager per request
            app.CreatePerOwinContext<BackOfficeSignInManager>((options, context) => BackOfficeSignInManager.Create(options, context, app.CreateLogger(typeof(BackOfficeSignInManager).FullName)));
        }

        /// <summary>
        /// Configure a custom BackOfficeUserManager for Umbraco
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="userManager"></param>
        public static void ConfigureUserManagerForUmbracoBackOffice<TManager, TUser>(this IAppBuilder app,
            ApplicationContext appContext,
            Func<IdentityFactoryOptions<TManager>, IOwinContext, TManager> userManager)
            where TManager : BackOfficeUserManager<TUser>
            where TUser : BackOfficeIdentityUser
        {
            if (appContext == null) throw new ArgumentNullException("appContext");
            if (userManager == null) throw new ArgumentNullException("userManager");

            //Configure Umbraco user manager to be created per request
            app.CreatePerOwinContext<TManager>(userManager);

            //Create a sign in manager per request
            app.CreatePerOwinContext<BackOfficeSignInManager>((options, context) => BackOfficeSignInManager.Create(options, context, app.CreateLogger(typeof(BackOfficeSignInManager).FullName)));
        }

        /// <summary>
        /// Ensures that the UmbracoBackOfficeAuthenticationMiddleware is assigned to the pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <returns></returns>
        /// <remarks>
        /// By default this will be configured to execute on PipelineStage.Authenticate
        /// </remarks>
        public static IAppBuilder UseUmbracoBackOfficeCookieAuthentication(this IAppBuilder app, ApplicationContext appContext)
        {
            return app.UseUmbracoBackOfficeCookieAuthentication(appContext, PipelineStage.Authenticate);
        }

        /// <summary>
        /// Ensures that the UmbracoBackOfficeAuthenticationMiddleware is assigned to the pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="stage">
        /// Configurable pipeline stage
        /// </param>
        /// <returns></returns>
        public static IAppBuilder UseUmbracoBackOfficeCookieAuthentication(this IAppBuilder app, ApplicationContext appContext, PipelineStage stage)
        {
            //Create the default options and provider
            var authOptions = app.CreateUmbracoCookieAuthOptions();
            
            authOptions.Provider = new BackOfficeCookieAuthenticationProvider
            {
                // Enables the application to validate the security stamp when the user 
                // logs in. This is a security feature which is used when you 
                // change a password or modify an external login to your account.  
                OnValidateIdentity = SecurityStampValidator
                        .OnValidateIdentity<BackOfficeUserManager, BackOfficeIdentityUser, int>(
                            //The amount of seconds to wait in between validating a user's security stamp
                            TimeSpan.FromSeconds(UmbracoConfig.For.UmbracoSettings().Security.SecurityStampValidationInterval),
                            (manager, user) => user.GenerateUserIdentityAsync(manager),
                            identity => identity.GetUserId<int>()),
                // On sign out, this will change the user's security stamp, this is a temporary meansure
                // until we can get another table to track a user's login sessions instead of a user's 
                // global session.
                OnResponseSignOut = context =>
                {
                    //var backofficeIdentity = context.OwinContext.Authentication.User.Identity as UmbracoBackOfficeIdentity;
                    //if (backofficeIdentity == null) return;
                    //var userMgr = context.OwinContext.GetUserManager<BackOfficeUserManager>();
                    //try
                    //{
                    //    var id = Convert.ToInt32(backofficeIdentity.Id);
                    //    var result = userMgr.UpdateSecurityStamp(id);
                    //}
                    //catch (Exception ex)
                    //{
                    //    //wasn't an int
                    //}
                }
            };

            return app.UseUmbracoBackOfficeCookieAuthentication(appContext, authOptions, stage);
        }

        /// <summary>
        /// Ensures that the UmbracoBackOfficeAuthenticationMiddleware is assigned to the pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="cookieOptions">Custom auth cookie options can be specified to have more control over the cookie authentication logic</param>
        /// <param name="stage">
        /// Configurable pipeline stage
        /// </param>
        /// <returns></returns>
        public static IAppBuilder UseUmbracoBackOfficeCookieAuthentication(this IAppBuilder app, ApplicationContext appContext, CookieAuthenticationOptions cookieOptions, PipelineStage stage)
        {
            if (app == null) throw new ArgumentNullException("app");
            if (appContext == null) throw new ArgumentNullException("appContext");
            if (cookieOptions == null) throw new ArgumentNullException("cookieOptions");
            if (cookieOptions.Provider == null) throw new ArgumentNullException("cookieOptions.Provider");
            if ((cookieOptions.Provider is BackOfficeCookieAuthenticationProvider) == false) throw new ArgumentException("The cookieOptions.Provider must be of type " + typeof(BackOfficeCookieAuthenticationProvider));
            
            app.UseUmbracoBackOfficeCookieAuthenticationInternal(cookieOptions, appContext, stage);

            //don't apply if app is not ready
            if (appContext.IsUpgrading || appContext.IsConfigured)
            {
                var getSecondsOptions = app.CreateUmbracoCookieAuthOptions(
                    //This defines the explicit path read cookies from for this middleware
                    new[] {string.Format("{0}/backoffice/UmbracoApi/Authentication/GetRemainingTimeoutSeconds", GlobalSettings.Path)});
                getSecondsOptions.Provider = cookieOptions.Provider;

                //This is a custom middleware, we need to return the user's remaining logged in seconds
                app.Use<GetUserSecondsMiddleWare>(
                    getSecondsOptions,
                    UmbracoConfig.For.UmbracoSettings().Security,
                    app.CreateLogger<GetUserSecondsMiddleWare>());
            }

            return app;
        }

        private static void UseUmbracoBackOfficeCookieAuthenticationInternal(this IAppBuilder app, CookieAuthenticationOptions options, ApplicationContext appContext, PipelineStage stage)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            //First the normal cookie middleware
            app.Use(typeof(CookieAuthenticationMiddleware), app, options);
            //don't apply if app isnot ready
            if (appContext.IsUpgrading || appContext.IsConfigured)
            {
                //Then our custom middlewares
                app.Use(typeof(ForceRenewalCookieAuthenticationMiddleware), app, options, new SingletonUmbracoContextAccessor());
                app.Use(typeof(FixWindowsAuthMiddlware));                
            }

            //Marks all of the above middlewares to execute on Authenticate
            app.UseStageMarker(stage);            
        }


        /// <summary>
        /// Ensures that the cookie middleware for validating external logins is assigned to the pipeline with the correct
        /// Umbraco back office configuration
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <returns></returns>
        /// <remarks>
        /// By default this will be configured to execute on PipelineStage.Authenticate
        /// </remarks>
        public static IAppBuilder UseUmbracoBackOfficeExternalCookieAuthentication(this IAppBuilder app, ApplicationContext appContext)
        {
            return app.UseUmbracoBackOfficeExternalCookieAuthentication(appContext, PipelineStage.Authenticate);
        }

        /// <summary>
        /// Ensures that the cookie middleware for validating external logins is assigned to the pipeline with the correct
        /// Umbraco back office configuration
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static IAppBuilder UseUmbracoBackOfficeExternalCookieAuthentication(this IAppBuilder app, ApplicationContext appContext, PipelineStage stage)
        {
            if (app == null) throw new ArgumentNullException("app");
            if (appContext == null) throw new ArgumentNullException("appContext");

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Constants.Security.BackOfficeExternalAuthenticationType,
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = Constants.Security.BackOfficeExternalCookieName,
                ExpireTimeSpan = TimeSpan.FromMinutes(5),
                //Custom cookie manager so we can filter requests
                CookieManager = new BackOfficeCookieManager(new SingletonUmbracoContextAccessor()),
                CookiePath = "/",
                CookieSecure = GlobalSettings.UseSSL ? CookieSecureOption.Always : CookieSecureOption.SameAsRequest,
                CookieHttpOnly = true,
                CookieDomain = UmbracoConfig.For.UmbracoSettings().Security.AuthCookieDomain
            }, stage);

            return app;
        }

        /// <summary>
        /// In order for preview to work this needs to be called
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <returns></returns>
        /// <remarks>
        /// This ensures that during a preview request that the back office use is also Authenticated and that the back office Identity
        /// is added as a secondary identity to the current IPrincipal so it can be used to Authorize the previewed document.
        /// </remarks>
        /// <remarks>
        /// By default this will be configured to execute on PipelineStage.PostAuthenticate
        /// </remarks>
        public static IAppBuilder UseUmbracoPreviewAuthentication(this IAppBuilder app, ApplicationContext appContext)
        {
            return app.UseUmbracoPreviewAuthentication(appContext, PipelineStage.PostAuthenticate);
        }

        /// <summary>
        /// In order for preview to work this needs to be called
        /// </summary>
        /// <param name="app"></param>
        /// <param name="appContext"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        /// <remarks>
        /// This ensures that during a preview request that the back office use is also Authenticated and that the back office Identity
        /// is added as a secondary identity to the current IPrincipal so it can be used to Authorize the previewed document.
        /// </remarks>
        public static IAppBuilder UseUmbracoPreviewAuthentication(this IAppBuilder app, ApplicationContext appContext, PipelineStage stage)
        {
            //don't apply if app isnot ready
            if (appContext.IsConfigured)
            {
                var authOptions = app.CreateUmbracoCookieAuthOptions();                
                app.Use(typeof(PreviewAuthenticationMiddleware),  authOptions);

                //This middleware must execute at least on PostAuthentication, by default it is on Authorize
                // The middleware needs to execute after the RoleManagerModule executes which is during PostAuthenticate, 
                // currently I've had 100% success with ensuring this fires after RoleManagerModule even if this is set
                // to PostAuthenticate though not sure if that's always a guarantee so by default it's Authorize.
                if (stage < PipelineStage.PostAuthenticate)
                {
                    throw new InvalidOperationException("The stage specified for UseUmbracoPreviewAuthentication must be greater than or equal to " + PipelineStage.PostAuthenticate);
                }

                
                app.UseStageMarker(stage);
            }

            return app;
        }

        public static void SanitizeThreadCulture(this IAppBuilder app)
        {
            Thread.CurrentThread.SanitizeThreadCulture();
        }

        /// <summary>
        /// Create the default umb cookie auth options
        /// </summary>
        /// <param name="app"></param>
        /// <param name="explicitPaths"></param>
        /// <returns></returns>
        public static UmbracoBackOfficeCookieAuthOptions CreateUmbracoCookieAuthOptions(this IAppBuilder app, string[] explicitPaths = null)
        {
            var authOptions = new UmbracoBackOfficeCookieAuthOptions(
                explicitPaths,
                UmbracoConfig.For.UmbracoSettings().Security,
                GlobalSettings.TimeOutInMinutes,
                GlobalSettings.UseSSL);
            return authOptions;
        }
    }
}