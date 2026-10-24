using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ProvisioningConfigMatrixSapEEGroup : DomainModelBase<DomainModel.ProvisioningConfigMatrixSapEEGroup, EntityModel.ProvisioningConfigMatrixSapEEGroup>, IXLData
    {
        public string EmployeeGroupNo { get; set; }
        public string EmployeeGroup { get; set; }
        public string PArea { get; set; }
        public string DefaultRole { get; set; }
        public string Remark { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static string GetSAPRolesBasedOnEEGroup(User user)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var matrices = ORMapList(new ProvisioningConfigMatrixEEGrpRepository(dataAccessTransaction).GetSAPRolesBasedOnEEGroup
                        (user.EEGroup, user.DepartmentCode));

                    return string.Join(",", matrices.Select(item => item.DefaultRole));
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).ExecuteQuery("Truncate table DI_M_ProvisioningConfigMatrixSAPEEGroup");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static ProvisioningConfigMatrixSapEEGroup GetADSXLProvisioningMatrixbyObject(ProvisioningConfigMatrixSapEEGroup obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction)
                        .Get(sap => sap.EmployeeGroupNo == obj.EmployeeGroupNo && sap.PArea == obj.PArea).FirstOrDefault());
            }
        }
        public static IList<ProvisioningConfigMatrixSapEEGroup> GetAllSAPRoleBasedOnEEGroups(bool Isvalid, bool IsAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (IsAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (Isvalid)
                            return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.ProvisioningConfigMatrixSapEEGroup>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
    }
}
