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
    public class SAPHRFLINSSuppRoleMatrix : DomainModelBase<DomainModel.SAPHRFLINSSuppRoleMatrix, EntityModel.SAPHRFLINSSuppRoleMatrix>, IXLData
    {

        public string Rule { get; set; }

        public string Company { get; set; }

        public string CostCenter { get; set; }

        public string PurchasingOrg { get; set; }

        public string PurchasingGrp { get; set; }

        public string Plant { get; set; }

        public string MRPControl { get; set; }

        public string StorageLocation { get; set; }

        public string Role { get; set; }

        public string RoleName { get; set; }

        public Nullable<bool> IsModified { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }

        public Nullable<bool> IsModifiedbyExcelUpdate { get; set; }
        public Nullable<int> ReferenceId { get; set; }

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static List<SAPHRFLINSSuppRoleMatrix> GetByTechnicalRuleAndCompany(string techRule, string company)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.Rule == techRule && item.Company == company).ToList());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<SAPHRFLINSSuppRoleMatrix> GetByTechnicalRule(string techRule)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction)
                        .Get(item => item.Rule == techRule).ToList());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SAPHRFLINSSuppRoleMatrix GetSAPHRFLINSSuppRoleMatrixbyObject(SAPHRFLINSSuppRoleMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction)
                        .Get(sap => sap.Rule == obj.Rule
                                                                            && sap.Company == obj.Company
                                                                            && sap.CostCenter == obj.CostCenter
                                                                            && sap.PurchasingOrg == obj.PurchasingOrg
                                                                            && sap.PurchasingGrp == obj.PurchasingGrp
                                                                            && sap.MRPControl == obj.MRPControl
                                                                            && sap.StorageLocation == obj.StorageLocation).FirstOrDefault());
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
                    new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_HRFLINSSuppRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static List<SAPHRFLINSSuppRoleMatrix> GetAllSAPHRFLINSSuppRoleMatrices(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPHRFLINSSuppRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
