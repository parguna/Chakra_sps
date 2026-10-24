using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_Linux_UserInfo", "LinuxUserInfo")]
    [Table("DI_Linux_UserInfo")]
    public class LinuxUserInfo : IAggregateRoot, IEndpointUserInfo
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string LinuxUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string GroupId { get; set; }
        public string UserIdInfo { get; set; }
        public string HomeDirectory { get; set; }
        public string CommandShell { get; set; }
        public long? DIUserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Version { get; set; }
        public string AccountPassword { get; set; }
        public string AccountName
        {
            get { return UserName; }
            set { value = UserName; }
        }
        public bool IsLocked { get; set; }
        public LockScenario LockScenario { get; set; }
        public AccountType AccountType { get; set; }
        public string UserValidFrom { get; set; }

        public virtual List<string> Groups { get; set; }
        public virtual List<LinuxGroupInfo> LinuxGroup { get; set; }
        public virtual EndPoint EndPointSystem { get; set; }

        public override string ToString()
        {
            return this.UserName;
        }
        public static LinuxUserInfo CreateLinuxUserInfo()
        {
            LinuxUserInfo adsUserInfo = new LinuxUserInfo();

            return adsUserInfo;
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Checks if the given instance is equals to the current one.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if they equals, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is LinuxUserInfo)
            {
                LinuxUserInfo other = (LinuxUserInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>
        /// <returns>The category hash code.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.UserName.GetHashCode();
        }

        public string UserValidTo { get; set; }

        public string Encrypt_Pwd { get; set; }

        public string MappingType { get; set; }

        public string DisplayName
        {
            get
            {
                return this.UserName;
            }
        }
    }
}
