namespace Umbraco.Core.Configuration.UmbracoSettings
{
    public interface ISecuritySection : IUmbracoConfigurationSection
    {
        bool KeepUserLoggedIn { get; }

        bool HideDisabledUsersInBackoffice { get; }

        bool AllowPasswordReset { get; }

        string AuthCookieName { get; }

        string AuthCookieDomain { get; }

        /// <summary>
        /// The amount of seconds to wait in between checking if the user's security stamp is still valid
        /// </summary>
        /// <remarks>
        /// The security stamp is used for a "log out of all devices" procedure. A user's security stamp is 
        /// updated whenenver a user's password is changed or an external authentication handler is removed from
        /// their profile.
        /// </remarks>
        int SecurityStampValidationInterval { get; }
    }
}