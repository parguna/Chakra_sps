using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_GEMS_UserInfo")]
    public class GEMSUserInfo : IEndpointUserInfo
    {

        public GEMSUserInfo()
        {
        }

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public Nullable<DateTime> DateModified { get; set; }
        public string LastUser { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public AccountType AccountType { get; set; }
        
        public string IfUnmappedMapTo { get; set; }
        public int Version { get; set; }
        public long? DIUserId { get; set; }
        public string MappingType { get; set; }        
        
        public LockScenario LockScenario { get; set; }
        public bool IsLocked { get; set; }
      
        public virtual EndPoint EndPointSystem { get; set; }

        public string AccountName
        {
            get { return this.UserName; }
        }

        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                return null;
            }
        }

        public string AccountPassword
        {
            get { return string.Empty; }
        }
    }
}
