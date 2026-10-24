using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DI.Chakra.DomainModel
{

    public class Office365LicenseConfig : DomainModelBase<Office365LicenseConfig, EntityModel.Office365LicenseConfig>, IXLData
    {
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public string LicenseDescription { get; set; }
        public string ServicePlan { get; set; }
        public string ServicePlanDescription { get; set; }

        [NotMapped]
        public Nullable<DateTime> ValidFrom { get; set; }
        [NotMapped]
        public Nullable<DateTime> ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction).ExecuteQuery("Truncate table DI_Office365_LicenseConfig");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static Office365LicenseConfig GetOffice365LicenseConfig(Office365LicenseConfig obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction)
                        .Get(item => item.AccountSkuId == obj.AccountSkuId && item.ServicePlan == obj.ServicePlan).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<Office365LicenseConfig> GetAllOffice365LicenseConfig(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMapList(new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction).GetAll().ToList());
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static Dictionary<string, string> GetAdhocOffice365LicenseInfos(string SearchString, List<AdHocLicenseInfo> AdHocLicenseInfo, List<ServicePlanDetail> ExistingServicePlanDetail, AdHocActionType adhocActionType)
        {
            var data = new Dictionary<string, string>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repo = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction);
                if (adhocActionType == AdHocActionType.AmendmentAdd)
                {
                    List<string> existingServicePlans = ExistingServicePlanDetail.Select(item => item.ServiceName).ToList();
                    List<string> Selectedlist = AdHocLicenseInfo.Select(x => x.AccountSkuId).ToList();
                    List<Office365LicenseConfig> nonEligibleList = new List<Office365LicenseConfig>();
                    List<long> noneligibles = new List<long>();
                    foreach (var existingItem in ExistingServicePlanDetail)
                    {
                        var Noneligible = repo.Get(item => item.AccountSkuId == existingItem.AccountSkuId && item.ServicePlan == existingItem.ServiceName).FirstOrDefault();
                        if (Noneligible != null)
                        {
                            noneligibles.Add(Noneligible.Id);
                        }
                    }
                     
                    var result = repo.Get(item => !noneligibles.Contains(item.Id));
                    result = result.Where(item => !Selectedlist.Contains(item.AccountSkuId));
                    if (!string.IsNullOrWhiteSpace(SearchString))
                    {
                        result = result.Where(item => item.LicenseDescription.Contains(SearchString));
                    }
                    
                    data = result.GroupBy(item => item.AccountSkuId)
                            .Select(item => item.FirstOrDefault()).Distinct().ToDictionary(item => item.AccountSkuId, item=> item.LicenseDescription);
                   
                }
                else if (adhocActionType == AdHocActionType.AmendmentRemove)
                {

                    List<string> selectedList = AdHocLicenseInfo.Select(x => x.AccountSkuId).ToList();
                    List<Office365LicenseConfig> result = new List<Office365LicenseConfig>();
                    foreach (var existingItem in ExistingServicePlanDetail)
                    {
                        var eligible = repo.Get(item => item.AccountSkuId == existingItem.AccountSkuId && item.ServicePlan == existingItem.ServiceName).FirstOrDefault();
                        if (eligible != null)
                        {
                            result.Add(ORMap(eligible));
                        }
                    }
                    var filteredData = result.Where(item => !selectedList.Contains(item.AccountSkuId));
                    if (!string.IsNullOrWhiteSpace(SearchString))
                    {
                        result = filteredData.Where(item =>  item.LicenseDescription.Contains(SearchString)).ToList();
                    }

                    data = filteredData.GroupBy(item => item.AccountSkuId)
                        
                        .Select(item => item.FirstOrDefault()).Distinct().ToDictionary(item => item.AccountSkuId, item => item.LicenseDescription);
                   
                }
                else if (adhocActionType == AdHocActionType.NewAccount)
                {
                    List<string> selectedList = AdHocLicenseInfo.Select(x => x.AccountSkuId).ToList();
                    var result = repo.Get(item => !selectedList.Contains(item.AccountSkuId));
                    if (!string.IsNullOrWhiteSpace(SearchString))
                    {
                        result  = result.Where(item=> item.LicenseDescription.Contains(SearchString)).ToList();
                    }
                    data = result.GroupBy(item => item.AccountSkuId).Select(item => item.FirstOrDefault()).Distinct().ToDictionary(item => item.AccountSkuId, item => item.LicenseDescription);
                }
                if (data == null)
                    data = new Dictionary<string, string>();
                return data;
            }
        }

        public static List<Office365LicenseConfig> GetOffice365ServicePlansforBulkReq(string license)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repo = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction);

                var result = repo.Get(item => item.AccountSkuId == license).ToList();
                return ORMapList(result); 
            } 
        }
        public static Dictionary<string, string> GetLicenseInfoForBulkReq(string SearchString, List<AdHocLicenseInfo> SelectedLicenseInfo)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var repo = new RepositoryBase<EntityModel.Office365LicenseConfig>(dataAccessTransaction);
                List<string> selectedList = SelectedLicenseInfo.Select(x => x.AccountSkuId).ToList();
                var result = repo.Get(item => !selectedList.Contains(item.AccountSkuId) && string.IsNullOrEmpty(SearchString) ? 1==1 : item.LicenseDescription.ToLower().Contains(SearchString) ).ToList();
                Dictionary<string, string> returnValues = result.GroupBy(item => item.AccountSkuId).Select(item => item.FirstOrDefault()).Distinct().ToDictionary(item=> item.AccountSkuId, item=> item.LicenseDescription);
                return returnValues;
            }
            
        }

        public static Dictionary<string, string> GetO365Users(string searchString, long endpointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
               return new O365UserInfoRepository(dataAccessTransaction).GetO365Users(searchString, endpointsystemId);
            } 
        }

    }
}
