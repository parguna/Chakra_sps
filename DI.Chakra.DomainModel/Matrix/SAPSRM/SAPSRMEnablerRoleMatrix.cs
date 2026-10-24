using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPSRMEnablerRoleMatrix : DomainModelBase<DomainModel.SAPSRMEnablerRoleMatrix, EntityModel.SAPSRMEnablerRoleMatrix>, IXLData
    {
        public string SRMEnablerRule { get; set; }
        public string POrg { get; set; }
        public string PGrp { get; set; }
        public string SRMTechRole { get; set; }
        public string BIPOrgRole { get; set; }
        public string BIPGrpRole { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
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
                    new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_SRMEnablerRoleMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPSRMEnablerRoleMatrix GetSAPSRMEnablerRoleMatrixbyObject(SAPSRMEnablerRoleMatrix obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction)
                    .Get(cnfg => cnfg.SRMTechRole == obj.SRMTechRole).FirstOrDefault());
            }
        }
        public static bool IsSAPSRMMainRoleExist(string techRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMMainRoleMatrix>(dataAccessTransaction)
                    .Any(cnfg => cnfg.BusinessRole == techRole);
            }
        }
        public static List<SAPSRMEnablerRoleMatrix> GetAllSAPSRMEnablerMatrix(bool isValidRecords, bool isAllRecords)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (isAllRecords)
                    return ORMapList(new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).GetAll().ToList());
                else
                {
                    if (isValidRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                    else
                        return ORMapList(new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                }
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static SAPSRMEnablerRoleMatrix GetSRMEnablerMatrixByRequest(string enablerRule, string pOrg, string pGrp)
        {            
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).Get(item => item.SRMEnablerRule == enablerRule &&
                        item.POrg == pOrg && item.PGrp == pGrp &&
                        item.ValidFrom <= DateTime.Now && item.ValidTo >= DateTime.Now).FirstOrDefault());
                }            
        }

        public static List<string> GetAllSRMFLINSBIPOrgRoles()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).Get(item => !string.IsNullOrEmpty(item.BIPOrgRole)).Select(item => item.BIPOrgRole).Distinct().ToList();
            }
        }
        public static List<string> GetAllSRMFLINSBIPGrpRoles()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SAPSRMEnablerRoleMatrix>(dataAccessTransaction).Get(item => !string.IsNullOrEmpty(item.BIPGrpRole)).Select(item => item.BIPGrpRole).Distinct().ToList();
            }

        }

    }
}
