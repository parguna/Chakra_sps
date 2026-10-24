using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DI.Chakra.Model
{
    [TableInfo("DI_ADS_SharedFolderInfo", "DI_ADS_SharedFolderInfo")]
    [Table("DI_ADS_SharedFolderInfo")]
    public class ADSSharedFolderInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string cn { get; set; }
        public string description { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public DateTime? whenCreated { get; set; }
        public DateTime? whenChanged { get; set; }
        public Int64 uSNCreated { get; set; }
        public Int64 uSNChanged { get; set; }
        public string name { get; set; }
        public string objectGUID { get; set; }
        public string uNCName { get; set; }
        public string managedBy { get; set; }
        public string objectCategory { get; set; }
        public string nTSecurityDescriptor { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int? Version { get; set; }


        public override string ToString()
        {
            return this.name.ToString();
        }
        public static ADSSharedFolderInfo CreateADSGroupInfo()
        {
            ADSSharedFolderInfo adsComputerInfo = new ADSSharedFolderInfo();

            return adsComputerInfo;
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

            if (obj is ADSSharedFolderInfo)
            {
                ADSSharedFolderInfo other = (ADSSharedFolderInfo)obj;
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
            return this.Id.GetHashCode() ^ this.name.GetHashCode();
        }

    }
}
