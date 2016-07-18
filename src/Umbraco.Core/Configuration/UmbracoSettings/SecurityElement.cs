using System.Configuration;

namespace Umbraco.Core.Configuration.UmbracoSettings
{
    internal class SecurityElement : ConfigurationElement, ISecuritySection
    {
        [ConfigurationProperty("keepUserLoggedIn")]
        internal InnerTextConfigurationElement<bool> KeepUserLoggedIn
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<bool>(
                    (InnerTextConfigurationElement<bool>)this["keepUserLoggedIn"],
                    //set the default
                    true);   
            }
        }

        [ConfigurationProperty("hideDisabledUsersInBackoffice")]
        internal InnerTextConfigurationElement<bool> HideDisabledUsersInBackoffice
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<bool>(
                    (InnerTextConfigurationElement<bool>)this["hideDisabledUsersInBackoffice"],
                    //set the default
                    false);                          
            }
        }

        [ConfigurationProperty("allowPasswordReset")]
        internal InnerTextConfigurationElement<bool> AllowPasswordReset
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<bool>(
                    (InnerTextConfigurationElement<bool>)this["allowPasswordReset"],
                    //set the default
                    true);
            }
        }

        [ConfigurationProperty("authCookieName")]
        internal InnerTextConfigurationElement<string> AuthCookieName
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<string>(
                    (InnerTextConfigurationElement<string>)this["authCookieName"],
                    //set the default
                    Constants.Web.AuthCookieName);                
            }
        }

        [ConfigurationProperty("authCookieDomain")]
        internal InnerTextConfigurationElement<string> AuthCookieDomain
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<string>(
                    (InnerTextConfigurationElement<string>)this["authCookieDomain"],
                    //set the default
                    null);                    
            }
        }

        /// <summary>
        /// The amount of seconds to wait in between checking if the user's security stamp is still valid
        /// </summary>
        /// <remarks>
        /// The security stamp is used for a "log out of all devices" procedure. A user's security stamp is 
        /// updated whenenver a user's password is changed or an external authentication handler is removed from
        /// their profile.
        /// </remarks>
        [ConfigurationProperty("securityStampValidationInterval")]
        public InnerTextConfigurationElement<int> SecurityStampValidationInterval
        {
            get
            {
                return new OptionalInnerTextConfigurationElement<int>(
                    (InnerTextConfigurationElement<int>) this["securityStampValidationInterval"],
                    //set the default - 5 minutes
                    300);
            }
        }

        bool ISecuritySection.KeepUserLoggedIn
        {
            get { return KeepUserLoggedIn; }
        }

        bool ISecuritySection.HideDisabledUsersInBackoffice
        {
            get { return HideDisabledUsersInBackoffice; }
        }

        bool ISecuritySection.AllowPasswordReset
        {
            get { return AllowPasswordReset; }
        }

        string ISecuritySection.AuthCookieName
        {
            get { return AuthCookieName; }
        }

        string ISecuritySection.AuthCookieDomain
        {
            get { return AuthCookieDomain; }
        }

        int ISecuritySection.SecurityStampValidationInterval
        {
            get { return SecurityStampValidationInterval; }
        }
    }
}