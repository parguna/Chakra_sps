using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.DomainModel
{

    public class SAPRoleMatrix : DomainModelBase<DomainModel.SAPRoleMatrix, EntityModel.SAPRoleMatrix>, IXLData
    {
        public string Application { get; set; }

        public string EligibilityRule { get; set; }

        public string Company { get; set; }

        public string PersonnelArea { get; set; }

        public string PersonnelSubArea { get; set; }

        public string Position { get; set; }

        public string OrgUnit { get; set; }

        public string CostCenter { get; set; }

        public string Remarks { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsUpdatedbyExcel { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static List<SAPRoleMatrix> GetAllSAPRoleEligibilityMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
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
                    new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_RoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPRoleMatrix GetSAPRoleMatrixbyObject(SAPRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction)
                        .Get(sap => sap.Application == obj.Application && sap.EligibilityRule == obj.EligibilityRule).FirstOrDefault());
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
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
