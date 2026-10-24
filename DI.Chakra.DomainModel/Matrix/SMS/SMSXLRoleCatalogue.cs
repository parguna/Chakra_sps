using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SMSXLRoleCatalogue : DomainModelBase<DomainModel.SMSXLRoleCatalogue, EntityModel.SMSXLRoleCatalogue>, IXLData, IApprover
    {
        public string Building { get; set; }
        public string Section { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string BusinessRole
        {
            get
            {
                return this.TechnicalRole;
            }
        }
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.OneCard);
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
                    new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SMS_XLRoleCatalogue");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static List<SMSXLRoleCatalogue> GetAllSMSRoleCatalogues(bool isValidRecords, bool isAllRecords)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll().ToList());
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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
                        // string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }

        public static Dictionary<string, string> GetAllBuildings()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                //return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll()
                //     .Select(x => new { Key = x.Building, Value = x.Building })
                //                                 .Distinct(StringComparer.CurrentCultureIgnoreCase).ToDictionary(o => o.Key, o => o.Value);
                return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll()
                     .Select(x => x.Building).ToList().Distinct(StringComparer.CurrentCultureIgnoreCase).ToDictionary(x=>x,x=>x);
            }
        }

        public static Dictionary<string, string> GetSectionByBuilding(string Building)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                //return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(item => item.Building == Building)
                //         .Select(x => new { Key = x.Section, Value = x.Section }).Distinct()
                //                                     .ToDictionary(o => o.Key, o => o.Value);
                Building = Building.ToLower();
                return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(item => item.Building.ToLower() == Building)
                         .Select(x => x.Section).ToList().Distinct(StringComparer.CurrentCultureIgnoreCase)
                                                     .ToDictionary(o => o, o => o);
            }
        }


        public static Dictionary<string, string> GetTechRolesByBuildingandSection(string Building, string section)
        {
            Building = Building.ToLower();
            section = section.ToLower();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(item => item.Building.ToLower() == Building && item.Section.ToLower() == section)
                         .Select(x =>x.TechnicalRole).ToList().Distinct()
                                                      .ToDictionary(o => o, o => o);

            }
        }

        public static string GetDescription(string techRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var roleCatlouge = new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(item => item.TechnicalRole == techRole).FirstOrDefault();
                if (roleCatlouge != null)
                {
                    return roleCatlouge.Description;
                }
                return "";
            } 
        }

        public static SMSXLRoleCatalogue GetBuildingandSectionByTechRoles(string techRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return ORMap(new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(item => item.TechnicalRole == techRole).FirstOrDefault());

            }
        }

        public static Dictionary<string, string> GetDistinctTechRoles()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {

                return new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).GetAll().Select(item=>item.TechnicalRole).Distinct().ToDictionary(key => key , value => value); 
            }
           
        }
        public static SMSXLRoleCatalogue GetOneCardRole(string building,string section, string roleName, string tecRole)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SMSXLRoleCatalogue>(dataAccessTransaction).Get(Item => Item.Building == building && Item.Section == section &&
                (Item.RoleName == roleName || Item.TechnicalRole == tecRole)).FirstOrDefault());
            }
        }
    }
}
