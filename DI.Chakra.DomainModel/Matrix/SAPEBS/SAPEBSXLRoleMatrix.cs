using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPEBSXLRoleMatrix: DomainModelBase<DomainModel.SAPEBSXLRoleMatrix, EntityModel.SAPEBSXLRoleMatrix>, IXLData, IApprover
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
        public string SystemName
        {
            get
            {
                ApplicationType applicationType;
                if (this.Application == Constants.EBSISP)
                {
                    applicationType = ApplicationType.EBS_ISU;
                }
                else
                {
                    applicationType = ApplicationType.EBS_CRM;
                }
                return UtilityMethods.GetEnumAttribute<Description>(applicationType);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_EBS_XL_RolesMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPEBSXLRoleMatrix GetSAPEBSXLRoleMatrixbyObject(SAPEBSXLRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.TechnicalRole == obj.TechnicalRole && cnfg.Application == obj.Application).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SAPEBSXLRoleMatrix GetEBSRoleMatrix(string role, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.RoleName == role && cnfg.Application == applicationName).FirstOrDefault());
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
                    return new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction)
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
        public static List<SAPEBSXLRoleMatrix> GetAllSAPEBSXLRoleMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static IList<SAPEBSXLRoleMatrix> BindRolesEBS(ApplicationType ApplicationName)
        {
            try
            {
                string appliction = ApplicationName == ApplicationType.EBS_ISU ? Constants.EBSISP : ApplicationName == ApplicationType.EBS_CRM ? Constants.EBSCRP : Constants.EBSBI;
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var result = new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(source => source.Application == appliction &&
                                                                                                                            !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName); 
                    List<SAPEBSXLRoleMatrix> data = new List<SAPEBSXLRoleMatrix>();
                    foreach (var items in result)
                    {
                        var check = data.Count(v => v.RoleName == items.RoleName);
                        if (check == 0)
                        {
                            data.Add(ORMap(items));
                        }
                    }
                    return data;
                }
            }
            catch
            {
                throw;
            }
        }
        public static IList<SAPEBSXLRoleMatrix> BindViewDetailsEBS(string Role)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result =  new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(source=> source.RoleName == Role && !string.IsNullOrEmpty(source.RoleName)).OrderBy(source => source.RoleName);
                List<SAPEBSXLRoleMatrix> data = new List<SAPEBSXLRoleMatrix>();
                foreach (var items in result)
                {
                    var check = data.Count(v => v.TechnicalRole == items.TechnicalRole);
                    if (check == 0)
                    {
                        data.Add(ORMap(items));
                    }
                }
                return data;
            }
        }

        public static List<string> GetTechnicalRolesForEBS(string roleName, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && item.RoleName == roleName && item.Application == applicationName).Select(role => role.TechnicalRole).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> GetTechnicalRolesForEBS(List<string> roleNames, string applicationName)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPEBSXLRoleMatrix>(dataAccessTransaction).Get(item => item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now
                            && roleNames.Contains(item.RoleName) && item.Application == applicationName).Select(role => role.TechnicalRole).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
