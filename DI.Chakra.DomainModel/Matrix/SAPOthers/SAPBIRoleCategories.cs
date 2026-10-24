using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{

    public class SAPBIRoleCategory  : DomainModelBase<DomainModel.SAPBIRoleCategory,EntityModel.SAPBIRoleCategory> , IXLData, IApprover
    {               
        public string Remarks { get; set; }
        
        public string Application { get; set; }
        
        public string Module { get; set; }
        
        public string SubModule { get; set; }
        
        public string BusinessRole { get; set; }
        
        public string TechnicalRole { get; set; }
        
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
      
        public Nullable<DateTime> ValidTo { get; set; }

        //public Nullable<bool> IsModifiedbyExcelUpload{get;set;}

        //public Nullable<int> ReferenceId { get; set; }

        
        public bool IsValid { get; set; }
        
        public string ValidationMessage { get; set; }

        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.FLINS_BI);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static List<string> BindTechnicalFromFLINSBI(string BusinessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction)
                    .Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now &&
                                                item.BusinessRole == BusinessRole)
                           .Select(item => item.TechnicalRole).Distinct().ToList();
            }
        }

        public static List<string> BindSubModuleFromFLINSBI(string Module) 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now && item.Module == Module && item.SubModule != null).Select(item => item.SubModule).Distinct().ToList();
            }
        }

        public static List<string> BindModuleFromFLINSBI()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now && item.Module != null, false, 0, 0).Select(item => item.Module).Distinct().ToList();
            }
        }

        public static Dictionary<long, string> GetBusinessRoleFromFlinsBIRole(string searchString, string subModule, string Module)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            { 
                    return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction)
                                    .Get(item => ( string.IsNullOrEmpty(searchString)? 1==1 : (item.BusinessRole.StartsWith(searchString) || item.BusinessRole.Contains(searchString))) &&
                                    item.ValidFrom <= DateTime.Now &&
                                    item.ValidTo >= DateTime.Now &&
                                    (string.IsNullOrEmpty(Module) ? 1 == 1 : item.Module == Module) &&
                                    (string.IsNullOrEmpty(subModule) ? 1 == 1 : item.SubModule == subModule)) 
                                    .Select(x => new { Key = x.Id, Value = x.BusinessRole })
                                    .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
        }


        public static Dictionary<long, string> GetBusinessRoles(string searchString, long endpointUserId , AdHocActionType actionType,string subModule, string   Module)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetBusinessRoles(searchString, endpointUserId, ApplicationType.FLINS_BI, actionType, subModule, Module);
                                 
            }
        }
        public static Dictionary<long, string> GetBusinessRoles(long endpointUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPRoleInfoRepository(dataAccessTransaction).GetBusinessRoles(endpointUserId);
            }
        }
        public static IList<SAPBIRoleCategory> GetAllSAPBIRoleCategories(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
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
                    new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_BIRoleCategories");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static SAPBIRoleCategory GetSAPBIRoleCategoriesbyObject(SAPBIRoleCategory obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(sap => sap.BusinessRole == obj.BusinessRole).FirstOrDefault());
            }
        }

        public static bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Any(sap => sap.Level1Approver == employeeId
                    || sap.Level2Approver == employeeId
                    || sap.Level3Approver == employeeId);
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
        public static SAPBIRoleCategory GetFLINSBIMatrixByRequest(string bussinessRole)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.BusinessRole == bussinessRole &&
                        item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
                }
            }
            catch
            {
                throw;
            }
        }
        public static List<string> GetFLINSBITechnicalRoleByBusinessRole(string bussinessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.SAPBIRoleCategory>(dataAccessTransaction).Get(item => item.BusinessRole == bussinessRole &&
                    item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).Select(i => i.TechnicalRole).ToList();
            }
        }
    }
}
