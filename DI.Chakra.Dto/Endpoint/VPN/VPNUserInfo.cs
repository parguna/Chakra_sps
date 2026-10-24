using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class VPNUserInfo : DtoBase, IEndpointUserInfo
    {
        public string OneKeyTokenId { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<IEndpointEntitlementInfo> Entitlements { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public long EndPointSystemId { get; set; }

    }
    public class SnapshotUserDetails<T>
    {
        public T UserInfo { get; set; }
        public List<SnapshotEntitlement> Entitlements { get; set; } = new List<SnapshotEntitlement>();
    }
    public class SnapshotEntitlement
    {
        public string Entitlement { get; set; }
        public string Function { get; set; }
        public EntitlementType EntitlementType { get; set; }
    }
    public class SnapshotUser
    {
        public string UserName { get; set; }
        public string Details { get; set; }
        public string UserType { get; set; }
        public bool HasIDD { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

    }
    public class SRMSnapShot
    {
        public List<SRMUserRoleMappingData> SRMUserRoles { get; set; } = new List<SRMUserRoleMappingData>();

    }
    public class SRMRoleData
    {
        public string AccountName { get; set; }

        public string SRPRoles { get; set; }
        public string SRPEnablerRoles { get; set; }

        public string SUSRoles { get; set; }

        public string FLINSBIRoles { get; set; }
        public string FLINSBIPOrgRoles { get; set; }
        public string FLINSBIPGrpRoles { get; set; }

        public string ADGroups { get; set; }
    }

    public class SRMUserRoleMappingData
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
    }

    public class SRMRolePOrgData
    {
        public string EnablerRule { get; set; }
        public string BusinessRole { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
    }
}
