using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class NOVAManagementRoleMatrix: DomainModelBase<NOVAManagementRoleMatrix, EntityModel.NOVAManagementRoleMatrix>, IXLData, IApprover
    {
        public string JobDescription { get; set; }
        public long EndPointSystemId { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string FLINSRoles { get; set; }
        public string BWAccess { get; set; }
        public string ADGroups { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }

        

        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string BusinessRole
        {
            get
            {
                return this.Position;
            }
        }
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.EAM);
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    else
                        return string.Empty;
                }
            }
        }

        public static NOVAManagementRoleMatrix GetNOVAManagementRoleMatrixbyObject(NOVAManagementRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction)
                        .Get(nova => nova.Department == obj.Department && nova.Position == obj.Position).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static NOVAManagementRoleMatrix GetNovaMatrix(AdHocNovaRequestDetails detail)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction)
                        .Get(m => m.Department== detail.DepartmentCode && (m.ValidTo==null || m.ValidTo>=DateTime.Now) && m.Position == detail.RoleName).FirstOrDefault());
            }
        }

        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_NOVA_ManagementRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static List<NOVAManagementRoleMatrix> GetNOVAManagementRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static List<string> GetManagementDepartment(long endpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NOVAManagementRoleMatrixRepository(dataAccessTransaction).GetDapartment(endpointsystemId);
            }
         }
        public static List<NOVAManagementRoleMatrix> GetManagementMatrixByDepartemnt(long endpointsystemId, string departmentCode)
        {
           using(var tran = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.NOVAManagementRoleMatrix>(tran).Get(i=>i.EndPointSystemId == endpointsystemId && i.Department == departmentCode && i.ValidTo.Value >=DateTime.Now).ToList());
            }
        }

    }
}
