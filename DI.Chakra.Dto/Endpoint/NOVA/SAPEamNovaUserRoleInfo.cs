using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SAPEamNovaUserRoleInfo:DtoBase, IEndpointEntitlementInfo
    {
        
        public long SAPEamNovaUserInfoId { get; set; }
        public string RoleName { get; set; }
        public string FLINS_TCodes { get; set; }
        public string FLINS_BI_TCodes { get; set; }
        public string AD_Groups { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }

        public bool IsExclusiveForUserAccount { get; set; }
        public string Department { get; set; }
        public long EndPointSystemId { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SAPEamNovaUserRoleInfo)
            {
                SAPEamNovaUserRoleInfo other = (SAPEamNovaUserRoleInfo)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.RoleName.GetHashCode();
        }

        public string EntitlementName
        {
            get { return RoleName; }
        }

        public string EntitlementDesc
        {
            get { return RoleName; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
    }
}
