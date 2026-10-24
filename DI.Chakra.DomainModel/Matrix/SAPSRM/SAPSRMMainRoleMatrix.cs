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
    public class SAPSRMMainRoleMatrix : DomainModelBase<DomainModel.SAPSRMMainRoleMatrix, EntityModel.SAPSRMMainRoleMatrix>, IXLData, IApprover
    {
        public string RoleType { get; set; }
        public string BusinessRole { get; set; }
        public string SRMRoles { get; set; }
        public string SUSRoles { get; set; }
        public string SRMEnablerRule { get; set; }
        public string BIRoles { get; set; }
        public string BIEnablerRule { get; set; }
        public string InternalPortalRoles { get; set; }
        public string InternalPortalGroups { get; set; }
        public string ExternalPortalRoles { get; set; }
        public string ExternalPortalGroups { get; set; }
        public string ADGroups { get; set; }
        public string DepartmentCode { get; set; }
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
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SRM);
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_SRMMainRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPSRMMainRoleMatrix GetSAPSRMMainRoleMatrixbyObject(SAPSRMMainRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.BusinessRole == obj.BusinessRole).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SAPSRMMainRoleMatrix GetSRMMatrixByRequest(string role)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.BusinessRole == role && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SAPSRMMainRoleMatrix> GetAllSAPSRMEMainRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }

        public static bool IsAdGroupsExist(string RoleName,AdHocActionType actionType, AccountType accountType, long DIUserID, string AccountName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).IsAdGroupsExist(RoleName,   actionType,   accountType,   DIUserID,   AccountName);
            }
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static Dictionary<string, string> GetSAPSRMRoles(AdHocActionType adhocActiontype, long userId, string searchString, AccountType accountType, string srmUserName, long endpointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repo = new SAPSRMMainRoleMatrixRepository(dataAccessTransaction);
                return repo.GetSAPSRMRoles(adhocActiontype, userId, searchString, accountType, srmUserName, endpointId);
            }
        }

        public static Dictionary<string, string> GetAllSAPSRMRoles(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPSRMMainRoleMatrixRepository(dataAccessTransaction).GetSRMAllRoles(searchString);
            }
        }

        public static SAPSRMMainRoleMatrix GetSRMMatrixByRequest(AdHocSAPSRMRequestDetail requestDetail)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).Get(item => item.BusinessRole == requestDetail.Entitlement &&
                    item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
            }

        }
        public static List<string> GetAllSRMADGroups()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).Get(item => !string.IsNullOrEmpty(item.ADGroups)).Select(item => item.ADGroups).Distinct().ToList();
            }
        }
        public static List<string> GetAllSRMFLINSBIRoles()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction).Get(item => !string.IsNullOrEmpty(item.BIRoles)).Select(item => item.BIRoles).Distinct().ToList();
            }
        }

        public static List<string> GetAllSRMFLINSBIRolesIncludePOrgPGrp()
        {
            try
            {
                var biRoles = SAPSRMMainRoleMatrix.GetAllSRMFLINSBIRoles();
                var biPOrgRoles = SAPSRMEnablerRoleMatrix.GetAllSRMFLINSBIPOrgRoles();
                var biPGrpRoles = SAPSRMEnablerRoleMatrix.GetAllSRMFLINSBIPGrpRoles();

                var roles = biRoles.Union(biPOrgRoles);
                var flinsBIRoles = roles.Union(biPGrpRoles).ToList();

                return flinsBIRoles;
            }
            catch
            {
                throw;
            }
        }
        public static List<SAPSRMMainRoleMatrix> GetByBusinessRoles(List<string> businessRoleName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction)
                    .Get(cnfg => businessRoleName.Contains(cnfg.BusinessRole)).ToList());
            }
        }
    }
}
