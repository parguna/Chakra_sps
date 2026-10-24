using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.DomainModel
{
    public class SAPRoleMandatoryField : DomainModelBase<DomainModel.SAPRoleMandatoryField, EntityModel.SAPRoleMandatoryField>, IXLData
    {

        public string BusinessRole { get; set; }
        public string MandatoryField1 { get; set; }
        public string MandatoryField2 { get; set; }
        public Nullable<bool> IsModified { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public Nullable<bool> IsModifiedbyExcelUpload { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static Dictionary<string, string> MandatoryFieldIdentification(string businsessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction).Get(item => item.BusinessRole == businsessRole && (item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now))
                        .Select(x => new { Key = x.MandatoryField1, Value = x.MandatoryField2 })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
            }
        }

        public static bool checkBusinessRoleCompanyOnlyMandatory(string BusinessRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction).Get(c => c.BusinessRole == BusinessRole).All(c => string.IsNullOrEmpty(c.MandatoryField2));
            }
        }

      
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_RoleMandatoryFields");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPRoleMandatoryField GetSAPRoleMandatoryFieldbyObject(SAPRoleMandatoryField obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction)
                        .Get(sap => sap.BusinessRole == obj.BusinessRole).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SAPRoleMandatoryField> GetAllSAPRoleMandatoryFields(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static bool CheckBusinessRoleExists(string bussinessRole)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                        .Any(item => item.BusinessRole == bussinessRole);
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
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
