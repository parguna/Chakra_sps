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
    public class SkypeForBusinessProvisioningMatrix : DomainModelBase<DomainModel.SkypeForBusinessProvisioningMatrix, EntityModel.SkypeForBusinessProvisioningMatrix>, IXLData,IApprover
    {
        public long EndPointSystemId { get; set; }

        public EntitlementType RequestType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ADID { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public string BusinessRole
        {
            get { return this.Name; }
        }

        public string SystemName
        {
            get { return ApplicationType.MicrosoftTeams.GetDescription(); }
        }

        public static List<SkypeForBusinessProvisioningMatrix> GetAllSkypeForBusinessMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static SkypeForBusinessProvisioningMatrix GetByName(string name, EntitlementType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).Get(Item => Item.Name == name && Item.RequestType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static List<SkypeForBusinessProvisioningMatrix> GetSkypeforbusinessMatrixDataByType(long endpointId, EntitlementType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).Get(Item => Item.EndPointSystemId == endpointId && Item.RequestType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).ToList());
            }
        }
        public static SkypeForBusinessProvisioningMatrix GetSkypeforbusinessMatrixDataByEntitlementType(long endpointId, EntitlementType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SkypeForBusinessProvisioningMatrix>(dataAccessTransaction).Get(Item => Item.EndPointSystemId == endpointId && Item.RequestType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static bool TruncateTable()
        {
            return true;
        }
    }
}
