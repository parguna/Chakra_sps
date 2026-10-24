using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPGRCRole: DomainModelBase<DomainModel.SAPGRCRole, EntityModel.SAPGRCRole>, IXLData, IApprover
    {
        public string Remarks { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string BusinessRoleName { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public bool? IsModified { get; set; }
        public bool? IsDeleted { get; set; }
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
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.SAP_GRC);
            }
        }
        public string BusinessRole
        {
            get { return this.BusinessRoleName; }
        }
        public static List<SAPGRCRole> GetAllSAPGRCRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_GRCRole");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPGRCRole GetSAPGRCRolebyObject(SAPGRCRole obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                        .Get(cnfg => cnfg.BusinessRoleName == obj.BusinessRoleName).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                        .Any(sap => sap.Level1Approver == employeeId
                        || sap.Level2Approver == employeeId
                        || sap.Level3Approver == employeeId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Dictionary<long, string> GetBusinessRoles(long endpointId, ApplicationType _applicationType, AdHocActionType actionType, string searchString, string subModule, string Module)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPGRCRoleRepository(dataAccessTransaction).GetBusinessRoles(endpointId, _applicationType , actionType, searchString, subModule, Module);
            }
        }
        public static Dictionary<long, string> GetBusinessRoles(long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPGRCRoleRepository(dataAccessTransaction).GetBusinessRoles(endpointUserId);
            }
        }
        public static List<string> BindTechnicalFromSAPGRCRole(string BusinessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                       .Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now && item.BusinessRoleName == BusinessRole)
                       .Select(item => item.TechnicalRole).Distinct().ToList();
            }
        }
        public static List<string> BindModuleFromGRCRole()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                       .Get(item => item.ValidFrom <= DateTime.Now &&
                            item.ValidTo >= DateTime.Now).
                            Select(item => item.Module).Distinct().ToList();
            }
        }
        public static Dictionary<long, string> GetBusinessRoleFromGRCRole(string searchString, string subModule, string Module)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                       .Get(item => (string.IsNullOrEmpty(searchString) ? 1 == 1 : item.BusinessRoleName.Contains(searchString)) &&
                                    item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now &&
                                     (string.IsNullOrEmpty(Module) ? 1 == 1 : item.Module == Module) &&
                                     (string.IsNullOrEmpty(subModule) ? 1 == 1 : item.SubModule == subModule))
                           .Select(x => new { Key = x.Id, Value = x.BusinessRoleName })
                           .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public static List<string> BindSubModuleFromGRCRole(string Module)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                       .Get(item => item.ValidFrom <= DateTime.Now
                                        && item.ValidTo >= DateTime.Now && item.Module == Module
                                        && !string.IsNullOrEmpty(item.SubModule)).Select(item => item.SubModule).Distinct().ToList();

            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
        public static SAPGRCRole GetGRCMatrixByRequest(string bussinessRole)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPGRCRole>(dataAccessTransaction)
                        .Get(item => item.BusinessRoleName == bussinessRole && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
