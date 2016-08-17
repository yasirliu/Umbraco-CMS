using System.Collections.Generic;
using System.IO;
using System.Configuration;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;

namespace Umbraco.Tests.TestHelpers
{
    public static class SettingsForTests
    {
        
        private static readonly TestGlobalSettings GlobalSettings = new TestGlobalSettings();

        /// <summary>
        /// Sets the umbraco settings singleton to the object specified
        /// </summary>
        /// <param name="settings"></param>
        public static void ConfigureSettings(IUmbracoSettingsSection settings)
        {
            UmbracoConfig.For.SetUmbracoSettings(settings);
        }

        /// <summary>
        /// Returns generated settings which can be stubbed to return whatever values necessary
        /// </summary>
        /// <returns></returns>
        public static IUmbracoSettingsSection GenerateMockSettings()
        {
            var settings = new Mock<IUmbracoSettingsSection>();

            var content = new Mock<IContentSection>();
            var security = new Mock<ISecuritySection>();
            var requestHandler = new Mock<IRequestHandlerSection>();
            var templates = new Mock<ITemplatesSection>();
            var dev = new Mock<IDeveloperSection>();
            var viewStateMover = new Mock<IViewStateMoverModuleSection>();
            var logging = new Mock<ILoggingSection>();
            var tasks = new Mock<IScheduledTasksSection>();
            var distCall = new Mock<IDistributedCallSection>();
            var repos = new Mock<IRepositoriesSection>();
            var providers = new Mock<IProvidersSection>();
            var help = new Mock<IHelpSection>();
            var routing = new Mock<IWebRoutingSection>();
            var scripting = new Mock<IScriptingSection>();            

            settings.Setup(x => x.Content).Returns(content.Object);
            settings.Setup(x => x.Security).Returns(security.Object);
            settings.Setup(x => x.RequestHandler).Returns(requestHandler.Object);
            settings.Setup(x => x.Templates).Returns(templates.Object);
            settings.Setup(x => x.Developer).Returns(dev.Object);
            settings.Setup(x => x.ViewStateMoverModule).Returns(viewStateMover.Object);
            settings.Setup(x => x.Logging).Returns(logging.Object);
            settings.Setup(x => x.ScheduledTasks).Returns(tasks.Object);
            settings.Setup(x => x.DistributedCall).Returns(distCall.Object);
            settings.Setup(x => x.PackageRepositories).Returns(repos.Object);
            settings.Setup(x => x.Providers).Returns(providers.Object);
            settings.Setup(x => x.Help).Returns(help.Object);
            settings.Setup(x => x.WebRouting).Returns(routing.Object);
            settings.Setup(x => x.Scripting).Returns(scripting.Object);

            //Now configure some defaults - the defaults in the config section classes do NOT pertain to the mocked data!!
            settings.Setup(x => x.Content.UseLegacyXmlSchema).Returns(false);
            settings.Setup(x => x.Content.ForceSafeAliases).Returns(true);
            settings.Setup(x => x.Content.ImageAutoFillProperties).Returns(ContentImagingElement.GetDefaultImageAutoFillProperties());
            settings.Setup(x => x.Content.ImageFileTypes).Returns(ContentImagingElement.GetDefaultImageFileTypes());
            settings.Setup(x => x.RequestHandler.AddTrailingSlash).Returns(true);
            settings.Setup(x => x.RequestHandler.UseDomainPrefixes).Returns(false);
            settings.Setup(x => x.RequestHandler.CharCollection).Returns(RequestHandlerElement.GetDefaultCharReplacements());
            settings.Setup(x => x.Content.UmbracoLibraryCacheDuration).Returns(1800);
            settings.Setup(x => x.WebRouting.UrlProviderMode).Returns("AutoLegacy");
            settings.Setup(x => x.Templates.DefaultRenderingEngine).Returns(RenderingEngine.Mvc);
            
            return settings.Object;
        }      

        public static bool HideTopLevelNodeFromPath
        {
            get { return GlobalSettings.HideTopLevelNodeFromPath; }
            set { GlobalSettings.HideTopLevelNodeFromPath = value; }
        }

        public static bool UseDirectoryUrls
        {
            get { return GlobalSettings.UseDirectoryUrls; }
            set { GlobalSettings.UseDirectoryUrls = value; }
        }

        public static string UmbracoPath
        {
            get { return GlobalSettings.Path; }
            set { GlobalSettings.Path = value; }
        }

        public static string ReservedPaths
        {
            get { return GlobalSettings.ReservedPaths; }
            set { GlobalSettings.ReservedPaths = value; }
        }

        public static string ReservedUrls
        {
            get { return GlobalSettings.ReservedUrls; }
            set { GlobalSettings.ReservedUrls = value; }
        }

        public static string ConfigurationStatus
        {
            get { return GlobalSettings.ConfigurationStatus; }
            set { GlobalSettings.ConfigurationStatus = value; }
        }

        // reset & defaults

        static SettingsForTests()
        {
            UmbracoConfig.For.SetGlobalConfig(GlobalSettings);
        }

        public static void Reset()
        {
            ResetUmbracoSettings();
            Core.Configuration.GlobalSettings.Reset();
            
            // set some defaults that are wrong in the config file?!
            // this is annoying, really
            HideTopLevelNodeFromPath = false;
        }

        /// <summary>
        /// This sets all settings back to default settings
        /// </summary>
        private static void ResetUmbracoSettings()
        {
            ConfigureSettings(GetDefault());
        }

        private static IUmbracoSettingsSection _defaultSettings;

        internal static IUmbracoSettingsSection GetDefault()
        {
            if (_defaultSettings == null)
            {
                var config = new FileInfo(TestHelper.MapPathForTest("~/Configurations/UmbracoSettings/web.config"));

                var fileMap = new ExeConfigurationFileMap() { ExeConfigFilename = config.FullName };
                var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                _defaultSettings = configuration.GetSection("umbracoConfiguration/defaultSettings") as UmbracoSettingsSection;
            }

            return _defaultSettings;
        }

        private class TestGlobalSettings : IGlobalSettings
        {
            public TestGlobalSettings()
            {
                ReservedUrls = string.Empty;
                ReservedPaths = string.Empty;
                ContentXmlFile = "~/App_Data/umbraco.config";
                StorageDirectory = "~/App_Data";
                Path = string.Empty;
                ConfigurationStatus = string.Empty;
                TimeOutInMinutes = 20;
                UseDirectoryUrls = false;
                DefaultUILanguage = string.Empty;
                HideTopLevelNodeFromPath = false;
                UseSSL = false;
            }

            /// <summary>
            /// Gets the reserved urls from web.config.
            /// </summary>
            /// <value>The reserved urls.</value>
            public string ReservedUrls { get; set; }

            /// <summary>
            /// Gets the reserved paths from web.config
            /// </summary>
            /// <value>The reserved paths.</value>
            public string ReservedPaths { get; set; }

            /// <summary>
            /// Gets the name of the content XML file.
            /// </summary>
            /// <value>The content XML.</value>
            /// <remarks>
            /// Defaults to ~/App_Data/umbraco.config
            /// </remarks>
            public string ContentXmlFile { get; set; }

            /// <summary>
            /// Gets the path to the storage directory
            /// </summary>
            /// <value>The storage directory.</value>
            public string StorageDirectory { get; set; }

            /// <summary>
            /// Gets the path to umbraco's root directory (/umbraco by default).
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

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
            public string UmbracoMvcArea { get; set; }

            /// <summary>
            /// Gets or sets the configuration status. This will return the version number of the currently installed umbraco instance.
            /// </summary>
            /// <value>The configuration status.</value>
            public string ConfigurationStatus { get; set; }

            /// <summary>
            /// Gets the time out in minutes.
            /// </summary>
            /// <value>The time out in minutes.</value>
            public int TimeOutInMinutes { get; set; }

            /// <summary>
            /// Gets a value indicating whether umbraco uses directory urls.
            /// </summary>
            /// <value><c>true</c> if umbraco uses directory urls; otherwise, <c>false</c>.</value>
            public bool UseDirectoryUrls { get; set; }

            /// <summary>
            /// Gets the default UI language.
            /// </summary>
            /// <value>The default UI language.</value>
            public string DefaultUILanguage { get; set; }

            /// <summary>
            /// Gets a value indicating whether umbraco should hide top level nodes from generated urls.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if umbraco hides top level nodes from urls; otherwise, <c>false</c>.
            /// </value>
            public bool HideTopLevelNodeFromPath { get; set; }

            /// <summary>
            /// Gets a value indicating whether umbraco should force a secure (https) connection to the backoffice.
            /// </summary>
            /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
            public bool UseSSL { get; set; }
        }
    }
}
