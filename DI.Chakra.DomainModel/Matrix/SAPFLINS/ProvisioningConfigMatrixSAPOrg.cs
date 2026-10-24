using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{

    public class ProvisioningConfigMatrixSAPOrg : DomainModelBase<DomainModel.ProvisioningConfigMatrixSAPOrg, EntityModel.ProvisioningConfigMatrixSAPOrg>, IXLData
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string DefaultHRMSGeneralRoles { get; set; }
        public string DefaultEPGroup { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }

        public string personnelArea { get; set; }
        public string PersonnelSubArea { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static string GetSAPRolesBasedOnUserOrg(User user)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var matrices = ORMapList(new ProvisioningConfigMatrixSAPOrgRepository(dataAccessTransaction).GetSAPRolesBasedOnUserOrg
                        (user.CompanyCode, user.DepartmentCode, user.BranchCode, user.CostCenter));

                    return string.Join(",", matrices.Select(item => item.DefaultHRMSGeneralRoles));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static IList<ProvisioningConfigMatrixSAPOrg> GetProvisioningConfigMatrixSAPOrgs(bool Isvalid, bool IsAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (IsAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (Isvalid)
                            return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
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
                    new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).ExecuteQuery("Truncate table DI_M_ProvisioningConfigMatrixSAPORG");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static ProvisioningConfigMatrixSAPOrg GetSAPRoleMatrixGroupbyObject(ProvisioningConfigMatrixSAPOrg obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction)
                        .Get(sap => sap.CompanyCode == obj.CompanyCode
                                    && sap.CompanyName == obj.CompanyName
                                    && sap.DepartmentCode == obj.DepartmentCode
                                    && sap.DivisionCode == obj.DivisionCode
                                    && sap.CostCenterCode == obj.CostCenterCode).FirstOrDefault());
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
                var lastModifiedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSAPOrg>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
        public static List<ProvisioningConfigMatrixSAPOrg> GetSAPRolesMatrixesBasedOnUserOrg(User user)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new ProvisioningConfigMatrixSAPOrgRepository(dataAccessTransaction).GetSAPRolesBasedOnUserOrg
                          (user.CompanyCode, user.DepartmentCode, user.BranchCode, user.CostCenter));

                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string GetSAPEPGroupBasedOnUserOrg(User user)
        {
            try
            {
                string group = string.Empty;

                var matrices = ProvisioningConfigMatrixSAPOrg.GetSAPRolesMatrixesBasedOnUserOrg(user);

                foreach (ProvisioningConfigMatrixSAPOrg item in matrices)
                {
                    group += item.DefaultEPGroup + ",";
                }

                return string.IsNullOrEmpty(group) ? group : group.Remove(group.Length - 1);
            }
            catch
            {
                throw;
            }
        }

    }
}
