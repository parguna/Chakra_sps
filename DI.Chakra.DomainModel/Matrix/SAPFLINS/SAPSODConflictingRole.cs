using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
   public class SAPSODConflictingRole : DomainModelBase<DomainModel.SAPSODConflictingRole, EntityModel.SAPSODConflictingRole>, IXLData
    {
        public string BusinessRole { get; set; }
        public string ConflictingBusinessRole { get; set; }
        public string TechnicalRole { get; set; }
        public string ConflictingTechnicalRole { get; set; }
        public string ConflictingRemarks { get; set; }
        public string SODOwner { get; set; }
        public string EndpointSystemName { get; set; }
        public Nullable<bool> IsModified { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<DateTime> ValidFrom { get; set; }
        public Nullable<DateTime> ValidTo { get; set; }
        public string FileName { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public static List<SAPSODConflictingRole> GetAllSAPSODConflictRoles(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year <= DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static List<SAPSODConflictingRole> GetAllSAPSODendpointtechConflictRoles(string EndpointSystemName, string TechnicalRole, string ConflictingTechnicalRole, long id)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    
                    if(id > 0)
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).Get(item => item.Id != id && item.EndpointSystemName == EndpointSystemName && item.TechnicalRole == TechnicalRole && item.ConflictingTechnicalRole == ConflictingTechnicalRole).ToList());
                    }
                    else
                    {
                        return ORMapList(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).Get(item => item.EndpointSystemName == EndpointSystemName && item.TechnicalRole == TechnicalRole && item.ConflictingTechnicalRole == ConflictingTechnicalRole).ToList());
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
                    new RepositoryBase<EntityModel.SAPRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_SODConfilictingRole");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPSODConflictingRole GetSAPSODConflictingRolebyObject(SAPSODConflictingRole obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction)
                        .Get(sap => sap.TechnicalRole == obj.TechnicalRole
                                    && sap.ConflictingTechnicalRole == obj.ConflictingTechnicalRole
                                    && sap.EndpointSystemName == obj.EndpointSystemName).FirstOrDefault());
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
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPSODConflictingRole>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
