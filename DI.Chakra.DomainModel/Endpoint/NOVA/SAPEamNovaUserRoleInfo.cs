using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPEamNovaUserRoleInfo : DomainModelBase<SAPEamNovaUserRoleInfo, EntityModel.SAPEamNovaUserRoleInfo>, IEndpointEntitlementInfo
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
        public string Company { get; set; }
        public string Section { get; set; }
        public string DepartmentBranch { get; set; }
        public NovaMatrixType NovaPositionMatrixType { get; set; }

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

        public static List<SAPEamNovaUserRoleInfo> GetAllByEndpointUserId(long EndpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPEamNovaUserRoleInfo>(dataAccessTransaction).GetAll(detail => detail.SAPEamNovaUserInfoId == EndpointUserId).ToList());
            }
        }

        public override int GetHashCode()
        {
            if (this.Id == 0) return base.GetHashCode();
            return this.Id.GetHashCode() ^ this.RoleName.GetHashCode();
        }

        public string EntitlementName
        {
            get
            {

                if (NovaPositionMatrixType == NovaMatrixType.User)
                    return $"Role:{RoleName};Company:{Company};Section:{Section}";
                else if (NovaPositionMatrixType == NovaMatrixType.Management)
                    return $"Role:{RoleName};DepartmentBranch:{DepartmentBranch}";
                else
                    return RoleName;
            }
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
