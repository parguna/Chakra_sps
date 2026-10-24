
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Office365License : DomainModelBase<Office365License, EntityModel.Office365License>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public string AccountSkuId { get; set; }
        public int ActiveUnits { get; set; }
        public int WarningUnits { get; set; }
        public int ConsumedUnits { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public List<Office365ServicePlan> ServicePlans;
        public string EntitlementName
        {
            get { return Convert.ToString(this.AccountSkuId); }
        }

        public string EntitlementDesc
        {
            get { return this.AccountSkuId; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.License; }
        }

        public static Office365License GetLicenseByAccountSkuId(string AccountSkuId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365License>(dataAccessTransaction)
                      .Get(t => t.AccountSkuId == AccountSkuId).FirstOrDefault());
            }
        }
        public static List<Office365License> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.Office365License>(dataAccessTransaction)
                      .Get(t => t.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        public static void SaveLicenses(IList<Office365License> list, long endpointSystemId, string currentUser)
        {
            try
            {
                var allExistingLicenses = Office365License.GetAllByEndpointSystemId(endpointSystemId);
                foreach (Office365License lic in list)
                {
                    var existLicense = allExistingLicenses.Where(l => l.AccountSkuId == lic.AccountSkuId).FirstOrDefault();

                    if (existLicense == null)
                    {
                        lic.Save(currentUser);
                        allExistingLicenses.Add(lic);
                    }
                    else
                    {
                        existLicense.AccountSkuId = lic.AccountSkuId;
                        existLicense.ActiveUnits = lic.ActiveUnits;
                        existLicense.ConsumedUnits = lic.ConsumedUnits;
                        existLicense.EndPointSystemId = endpointSystemId;
                        existLicense.WarningUnits = existLicense.WarningUnits;
                        existLicense.LastModifiedBy = currentUser;
                        existLicense.LastModifiedDate = DateTime.Now;
                        existLicense.Save(currentUser);
                    }

                    foreach (Office365ServicePlan svcPlan in lic.ServicePlans)
                    {
                        var existingSvcPlan = Office365ServicePlan.GetServicePlanByServiceName(lic.AccountSkuId, svcPlan.ServiceName);
                        if (existingSvcPlan == null)
                        {
                            svcPlan.Save(currentUser);                           
                        }
                        else
                        {
                            existingSvcPlan.AccountSkuId = svcPlan.AccountSkuId;
                            existingSvcPlan.ServiceName = svcPlan.ServiceName;
                            existingSvcPlan.ServiceType = svcPlan.ServiceType;
                            existingSvcPlan.EndPointSystemId = endpointSystemId;
                            existingSvcPlan.LastModifiedBy = currentUser;
                            existingSvcPlan.LastModifiedDate = DateTime.Now;
                            existingSvcPlan.Save(currentUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, "Exception Occured at Office 365 SaveLicenses", ex);
                throw;
            }
        }
    }

    [Serializable]
    public class ProvLicenseInfo
    {
        public List<ProvLicenseInfoDetail> LicenseDetails { get; set; }
    }

    [Serializable]
    public class ProvLicenseInfoDetail
    {
        public string License { get; set; }
        public string ServiceName { get; set; }
    }
}
