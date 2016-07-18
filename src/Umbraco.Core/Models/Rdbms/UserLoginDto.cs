using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Umbraco.Core.Models.Rdbms
{
    [TableName("umbracoUserLogin")]
    [PrimaryKey("id", autoIncrement = true)]
    [ExplicitColumns]
    internal class UserLoginDto
    {
        [Column("id")]
        [PrimaryKeyColumn(Name = "PK_userLogin")]
        public int Id { get; set; }

        [Column("loginDateUtc")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public DateTime CreateDateUtc { get; set; }

        [Column("updateDateUtc")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public DateTime UpdateDateUtc { get; set; }

        [Column("token")]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        public Guid LoginToken { get; set; }

        /// <summary>
        /// The user id for the login
        /// </summary>
        /// <remarks>
        /// The index is on 3 columns in the order in which we query for them
        /// </remarks>
        [Column("userId")]
        [ForeignKey(typeof(UserDto))]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        [Index(IndexTypes.NonClustered, Name = "IX_userLogin_userId", ForColumns = "userId,token,isValid")]
        public int UserId { get; set; }

        [Column("isValid")]
        [NullSetting(NullSetting = NullSettings.NotNull)]        
        public bool IsValid { get; set; }

        /// <summary>
        /// This will be a json structure with some details of the login being made such as headers, etc...
        /// </summary>
        [Column("details")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [Length(2000)]
        public string Details { get; set; }
    }
}