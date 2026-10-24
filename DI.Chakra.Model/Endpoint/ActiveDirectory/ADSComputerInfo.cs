using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_ADS_ComputerInfo", "DI_ADS_ComputerInfo")]
    [Table("DI_ADS_ComputerInfo")]
    public class ADSComputerInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string cn { get; set; }
        public string givenName { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public DateTime? whenCreated { get; set; }
        public DateTime? whenChanged { get; set; }
        public string displayName { get; set; }
        public Int64 uSNCreated { get; set; }
        public Int64 uSNChanged { get; set; }
        public string name { get; set; }
        public string objectGUID { get; set; }
        public string userAccountControl { get; set; }
        public string badPwdCount { get; set; }
        public string codePage { get; set; }
        public string countryCode { get; set; }
        public DateTime? badPasswordTime { get; set; }
        public DateTime? lastLogoff { get; set; }
        public DateTime? lastLogon { get; set; }
        public string localPolicyFlags { get; set; }
        public DateTime? pwdLastSet { get; set; }
        public string primaryGroupID { get; set; }
        public string objectSid { get; set; }
        public DateTime? accountExpires { get; set; }
        public int logonCount { get; set; }
        public string sAMAccountName { get; set; }
        public string sAMAccountType { get; set; }
        public string operatingSystem { get; set; }
        public string operatingSystemVersion { get; set; }
        public string operatingSystemServicePack { get; set; }
        public string serverReferenceBL { get; set; }
        public string dNSHostName { get; set; }
        public string rIDSetReferences { get; set; }
        public string servicePrincipalName { get; set; }
        public string objectCategory { get; set; }
        public string isCriticalSystemObject { get; set; }
        public string frsComputerReferenceBL { get; set; }
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
        public static ADSComputerInfo CreateADSGroupInfo()
        {
            ADSComputerInfo adsComputerInfo = new ADSComputerInfo();

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

            if (obj is ADSComputerInfo)
            {
                ADSComputerInfo other = (ADSComputerInfo)obj;
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
