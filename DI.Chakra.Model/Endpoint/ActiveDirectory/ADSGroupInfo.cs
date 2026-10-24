using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [TableInfo("DI_ADS_GroupInfo", "DI_ADS_GroupInfo")]
    [Table("DI_ADS_GroupInfo")]
    public class ADSGroupInfo : IAggregateRoot, IEndpointEntitlementInfo
    {

        public ADSGroupInfo()
        {
            this.ADSUsers = new List<ADSUserInfo>();
        }

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string cn { get; set; }
        public string description { get; set; }
        public string member { get; set; }
        public string distinguishedName { get; set; }
        public string instanceType { get; set; }
        public Nullable<System.DateTime> whenCreated { get; set; }
        public Nullable<System.DateTime> whenChanged { get; set; }
        public Nullable<long> uSNCreated { get; set; }
        public Nullable<long> uSNChanged { get; set; }
        public string name { get; set; }
        public string objectGUID { get; set; }
        public string objectSid { get; set; }
        public string sAMAccountName { get; set; }
        public string sAMAccountType { get; set; }
        public string groupType { get; set; }
        public string groupTypeDesc { get; set; }
        public string objectCategory { get; set; }
        public string nTSecurityDescriptor { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }
        public bool? CanRequest { get; set; }

        public virtual List<ADSUserInfo> ADSUsers { get; set; }

        public string FormattedOU
        {
            get
            {
                return this.distinguishedName.Remove(0, this.distinguishedName.IndexOf(",") + 1);
            }
        }
        public override string ToString()
        {
            return this.name.ToString();
        }
        public static ADSGroupInfo CreateADSGroupInfo()
        {
            ADSGroupInfo adsUserInfo = new ADSGroupInfo();

            return adsUserInfo;
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

            if (obj is ADSGroupInfo)
            {
                ADSGroupInfo other = (ADSGroupInfo)obj;
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





        public string EntitlementName
        {
            get { return cn; }
        }

        public string EntitlementDesc
        {
            get { return description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Group; }
        }
    }
}
