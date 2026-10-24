using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPXLMSSLRolesMatrix : DomainModelBase<DomainModel.SAPXLMSSLRolesMatrix, EntityModel.SAPXLMSSLRolesMatrix>, IXLData, IApprover
    {
        public string Remarks { get; set; }
        public string Application { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string RoleName { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom
        {
            get;
            set;
        }

        public DateTime? ValidTo
        {
            get;
            set;
        }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public string BusinessRole
        {
            get { return this.RoleName; }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string SystemName
        {
            get
            {
                ApplicationType applicationType;
                if (this.Application == Constants.MSSLMSP)
                {
                    applicationType = ApplicationType.MSSL_ISU;
                }
                else
                {
                    applicationType = ApplicationType.MSSL_CRM;
                }
                return UtilityMethods.GetEnumAttribute<Description>(applicationType);
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_MSSL_XL_RolesMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPXLMSSLRolesMatrix GetSAPXLMSSLRolesMatrixbyObject(SAPXLMSSLRolesMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.TechnicalRole == obj.TechnicalRole && cnfg.Application == obj.Application).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SAPXLMSSLRolesMatrix GetMSSLRoleMatrix(string role, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.RoleName == role && cnfg.Application == applicationName).FirstOrDefault());
                }
            }
            catch
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
                    return new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction)
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
        public static List<SAPXLMSSLRolesMatrix> GetAllSAPMSSLXLRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (isAllRecords)
                    return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).GetAll().ToList());
                else
                {
                    if (isValidRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                    else
                        return ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                }
            }
        }

        public static IList<SAPXLMSSLRolesMatrix> BindRoles(ApplicationType ApplicationName)
        {
            string appliction = ApplicationName == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : ApplicationName == ApplicationType.MSSL_CRM ? Constants.MSSLMCP : Constants.MSSLBI;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result = ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).Get(source =>
                            source.Application == appliction && !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName).ToList());

                List<SAPXLMSSLRolesMatrix> data = new List<SAPXLMSSLRolesMatrix>();
                foreach (var items in result)
                {
                    var check = data.Count(v => v.RoleName == items.RoleName);
                    if (check == 0)
                    {
                        data.Add(items);
                    }
                }

                return data;
            }
        }

        public static IList<SAPXLMSSLRolesMatrix> BindViewDetails(string Role)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                var result = ORMapList(new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).Get(source =>
                source.RoleName == Role && !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName).ToList());
                List<SAPXLMSSLRolesMatrix> data = new List<SAPXLMSSLRolesMatrix>();
                foreach (var items in result)
                {
                    var check = data.Count(v => v.TechnicalRole == items.TechnicalRole);
                    if (check == 0)
                    {
                        data.Add(items);
                    }
                }
                return data;
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static List<string> GetTechnicalRolesForMSSL(string roleName, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction)
                        .Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && item.RoleName == roleName && item.Application == applicationName)
                        .Select(role => role.TechnicalRole)
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<string> GetTechnicalRolesForMSSL(List<string> roleNames, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPXLMSSLRolesMatrix>(dataAccessTransaction)
                        .Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && roleNames.Contains(item.RoleName) && item.Application == applicationName)
                        .Select(role => role.TechnicalRole)
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
