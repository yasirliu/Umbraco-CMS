namespace Umbraco.Core.Configuration
{
    /// <summary>
    /// Contains general settings information for the entire Umbraco instance based on information from  web.config appsettings 
    /// </summary>
    public interface IGlobalSettings
    {
        /// <summary>
        /// Gets the reserved urls from web.config.
        /// </summary>
        /// <value>The reserved urls.</value>
        string ReservedUrls { get; }

        /// <summary>
        /// Gets the reserved paths from web.config
        /// </summary>
        /// <value>The reserved paths.</value>
        string ReservedPaths { get; }

        /// <summary>
        /// Gets the name of the content XML file.
        /// </summary>
        /// <value>The content XML.</value>
        /// <remarks>
        /// Defaults to ~/App_Data/umbraco.config
        /// </remarks>
        string ContentXmlFile { get; }

        /// <summary>
        /// Gets the path to the storage directory
        /// </summary>
        /// <value>The storage directory.</value>
        string StorageDirectory { get; }

        /// <summary>
        /// Gets the path to umbraco's root directory (/umbraco by default).
        /// </summary>
        /// <value>The path.</value>
        string Path { get; }

        /// <summary>
        /// This returns the string of the MVC Area route.
        /// </summary>
        /// <remarks>
        /// This will return the MVC area that we will route all custom routes through like surface controllers, etc...
        /// We will use the 'Path' (default ~/umbraco) to create it but since it cannot contain '/' and people may specify a path of ~/asdf/asdf/admin
        /// we will convert the '/' to '-' and use that as the path. 
        /// 
        /// We also make sure that the virtual directory (SystemDirectories.Root) is stripped off first, otherwise we'd end up with something
        /// like "MyVirtualDirectory-Umbraco" instead of just "Umbraco".
        /// </remarks>
        string UmbracoMvcArea { get; }

        /// <summary>
        /// Gets or sets the configuration status. This will return the version number of the currently installed umbraco instance.
        /// </summary>
        /// <value>The configuration status.</value>
        string ConfigurationStatus { get; }

        /// <summary>
        /// Gets the time out in minutes.
        /// </summary>
        /// <value>The time out in minutes.</value>
        int TimeOutInMinutes { get; }

        /// <summary>
        /// Gets a value indicating whether umbraco uses directory urls.
        /// </summary>
        /// <value><c>true</c> if umbraco uses directory urls; otherwise, <c>false</c>.</value>
        bool UseDirectoryUrls { get; }

        /// <summary>
        /// Gets the default UI language.
        /// </summary>
        /// <value>The default UI language.</value>
        string DefaultUILanguage { get; }

        /// <summary>
        /// Gets a value indicating whether umbraco should hide top level nodes from generated urls.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if umbraco hides top level nodes from urls; otherwise, <c>false</c>.
        /// </value>
        bool HideTopLevelNodeFromPath { get; }

        /// <summary>
        /// Gets a value indicating whether umbraco should force a secure (https) connection to the backoffice.
        /// </summary>
        /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
        bool UseSSL { get; }
    }
}