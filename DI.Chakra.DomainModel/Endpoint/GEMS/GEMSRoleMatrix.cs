using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace DI.Chakra.DomainModel
{

    public class GEMSRoleMatrix : DomainModelBase<GEMSRoleMatrix, EntityModel.GEMSRoleMatrix>, IEndpointEntitlementInfo, IXLData, IApprover
    {
        public long EndPointSystemId { get; set; }

        public string BusinessRoleName { get; set; }

        public string TechnicalRole { get; set; }
        public string ADGroups { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public string EntitlementName
        {
            get { return this.BusinessRoleName; }
        }
        public string EntitlementDesc
        {
            get { return this.BusinessRoleName; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.GEMS);
            }
        }
        public string BusinessRole
        {
            get { return this.BusinessRoleName; }
        }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public static List<string> GetGEMSBusinessRolesByTechnicalRoles(string roleName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSRoleMatrixRepository(transaction).GetGEMSTechnicalRolesFromBusinessRole(roleName));
            }
        }

        public static bool CheckBusinessRolesUsedInOtherBusinessRoles(string businessroles, ref List<string> tobeRemovedtechRoles, ref List<string> tobeRemoveadGroups)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSRoleMatrixRepository(transaction).CheckBusinessRolesUsedInOtherBusinessRoles(businessroles, ref tobeRemovedtechRoles, ref tobeRemoveadGroups));
            }
        }

        public static List<string> GetGEMSTechnicalRolesFromBusinessRole(string roleName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSRoleMatrixRepository(transaction).GetGEMSTechnicalRolesFromBusinessRole(roleName));
            }
        }

        public static List<string> GetGEMSADGroupsFromBusinessRole(string roleName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSRoleMatrixRepository(transaction).GetGEMSADGroupsFromBusinessRole(roleName));
            }
        }

        public static List<string> GetGEMSGroupsFromADGroups(ADSUserInfo adsUserValue)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return (new GEMSRoleMatrixRepository(transaction).GetGEMSGroupsFromADGroups(adsUserValue.Id));
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_GEMS_RoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static GEMSRoleMatrix GetGEMSRoleMatrixbyObject(GEMSRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction)
                        .Get(sap => sap.BusinessRoleName == obj.BusinessRoleName).FirstOrDefault());
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
                    return new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction)
                        .Any(gems => gems.Level1Approver == employeeId
                        || gems.Level2Approver == employeeId
                        || gems.Level3Approver == employeeId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<GEMSRoleMatrix> GetGEMSRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static bool IsGEMSRoleValid(string value)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.GEMSRoleInfo>(dataAccessTransaction)
                        .Any(cmp => cmp.Display_Name == value);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static List<GEMSRoleMatrix> BindRolesGEMS()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetAll().Select(matrix => matrix).OrderBy(matrix => matrix.BusinessRoleName).Distinct().ToList());
            }
        }

        public static GEMSRoleMatrix GetByBusinessRole(string businessRole)
        {  
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).Get(b => b.BusinessRoleName == businessRole).FirstOrDefault());
            }
        }

        public static List<string> GetAllTables(string searchString)
        {
            var allTables = new List<string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                string query = "select TABLE_NAME from INFORMATION_SCHEMA.TABLES order by TABLE_NAME asc";
                if (!string.IsNullOrWhiteSpace(searchString) && searchString != "*")
                {
                    query = $"select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME like '%{searchString}%' order by TABLE_NAME asc";
                }
                var dtTables = new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetDataTableByQuery(query);
                if (dtTables != null && dtTables.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dtTables.Rows)
                    {
                        allTables.Add(Convert.ToString(dataRow["TABLE_NAME"]));
                    }
                }
                return allTables;
            }
        }

        public static List<string> GetAllColumnsByTableName(string tableName)
        {
            var allColumns = new List<string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var query = $"select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{tableName}'";
                var dtColumns = new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction).GetDataTableByQuery(query);
                if (dtColumns != null && dtColumns.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in dtColumns.Rows)
                    {
                        allColumns.Add(Convert.ToString(dataRow["COLUMN_NAME"]));
                    }
                }
                return allColumns;
            }
        }

        public static GEMSRoleMatrix GetGEMSMatrixByRequest(string bussinessRole)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.GEMSRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.BusinessRoleName == bussinessRole && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
                }
            }
            catch
            {
                throw;
            }
        }

        public static Dictionary<long, string> GetBusinessRoles(long endpointId, ApplicationType _applicationType, AdHocActionType actionType, string searchstring)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ep = EndPoint.GetByApplicationType(_applicationType, false);
                return new GEMSRoleInfoRepository(dataAccessTransaction).GetBusinessRoles(endpointId, ep.Id, actionType, searchstring);
            }
        }
        public static List<GEMSRoleMatrix> GetAllBusinessRole(long endpointId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMapList(new GEMSRoleMatrixRepository(transaction).Get(item => item.EndPointSystemId == endpointId && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).ToList());
            }
        }

        public static List<string> GetADGroupsFromTechnicalRole(string technicalRole)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return new GEMSRoleMatrixRepository(transaction).GetRoleMatrixFromRoleInfo(technicalRole);
            }
        }
        public static GEMSRoleMatrix GetMatrixFromTechnicalRole(string technicalRole, long endpointId)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new GEMSRoleMatrixRepository(transaction).Get(item => item.EndPointSystemId == endpointId && item.TechnicalRole == technicalRole && item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
            }
        }
    }
}
