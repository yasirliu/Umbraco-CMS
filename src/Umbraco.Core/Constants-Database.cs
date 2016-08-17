namespace Umbraco.Core
{
    public static partial class Constants
    {
        public static class Database
        {
            public const string SqlCe = "System.Data.SqlServerCe.4.0";
            public const string SqlServer = "System.Data.SqlClient";
            public const string MySql = "MySql.Data.MySqlClient";

            public const string UmbracoConnectionName = "umbracoDbDSN";
            public const string UmbracoMigrationName = "Umbraco";
        }
    }
}