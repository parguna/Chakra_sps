using DI.Chakra.DBClient;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ADSXLProvisioningMatrix : DomainModelBase<DomainModel.ADSXLProvisioningMatrix, EntityModel.ADSXLProvisioningMatrix>, IXLData
    {
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DivisionCode { get; set; }
        public string OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostDistributionCode { get; set; }
        public string OU { get; set; }
        public string DefaultGroups { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static ADSXLProvisioningMatrix GetByUser(User user)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return ORMap(new ADSXLProvisioningMatrixRepository(dataAccessTransaction).GetByUser(user.DepartmentCode, user.BranchCode,
                        user.SectionCode, user.CompanyCode, user.CostCenter, user.OrganisationCode));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ADSXLProvisioningMatrix GetADSXLProvisioningMatrixbyObject(ADSXLProvisioningMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction)
                    .Get(cnfg => cnfg.CompanyCode == obj.CompanyCode
                    && cnfg.DepartmentCode == obj.DepartmentCode && cnfg.DivisionCode == obj.DivisionCode && cnfg.OrganisationId == obj.OrganisationId
                    && cnfg.CostCenterCode == obj.CostCenterCode && cnfg.CostDistributionCode == obj.CostDistributionCode).FirstOrDefault());
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_ADS_XLProvMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static bool IsCompanyCodeValid(string value)
        {
            using(DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.Company>(dataAccessTransaction).Any(item => item.Code == value);
            }
        }

         
        public static bool IsPersonnelAreaValid(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Any(item => item.Code == value && item.ScopeId == (long)OrgScopeLevel.Department );
            }
        }
        public static bool IsPersonnelSubAreaValid(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Any(item => item.Code == value && item.ScopeId == (long)OrgScopeLevel.Branch);
            }
        }
        public static bool IsCostCenterValid(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.NCostCenter>(dataAccessTransaction).Any(item => item.Code == value);
            }
        }
        public static bool IsOrgUnitValid(string value)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.NOrganisation>(dataAccessTransaction).Any(item => item.Code == value);
            }
        }
        public static bool IsUserValid(string Value, ApprovalUserType Type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (Type == ApprovalUserType.User)
                {
                    if (Value.Length < 8)
                    {
                        char pad = '0';
                        Value = Value.PadLeft(8, pad);
                    }
                    return new RepositoryBase<EntityModel.User>(dataAccessTransaction).Any(cmp => cmp.EmployeeId == Value);
                }
                else
                {
                    return new RepositoryBase<EntityModel.Group>(dataAccessTransaction).Any(cmp => cmp.GroupName == Value);
                }
            }
        }
        public static IList<ADSXLProvisioningMatrix> GetAllADSXLProvisioningMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        /// <summary>
        /// Shadows the generic ad-hoc query executor for this endpoint: the value here can
        /// originate directly from an HTTP request parameter, so only read-only SELECT
        /// statements are permitted.
        /// </summary>
        public new static bool ExecuteQuery(string Query)
        {
            if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(Query))
                return false;

            return DomainModelBase<ADSXLProvisioningMatrix, EntityModel.ADSXLProvisioningMatrix>.ExecuteQuery(Query);
        }

        /// <summary>
        /// Shadows the generic ad-hoc query executor for this endpoint: the value here can
        /// originate directly from an HTTP request parameter, so only read-only SELECT
        /// statements are permitted.
        /// </summary>
        public new static DataTable GetDataTableByQuery(string Query)
        {
            if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(Query))
                return new DataTable();

            return DomainModelBase<ADSXLProvisioningMatrix, EntityModel.ADSXLProvisioningMatrix>.GetDataTableByQuery(Query);
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.ADSXLProvisioningMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }
    }
}
