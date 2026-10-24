using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SMSCompany : DomainModelBase<SMSCompany, EntityModel.SMSCompany>, IEndpointEntitlementInfo
    {
        public long EndPointSystemId { get; set; }
        public int CompanyKey { get; set; }
        public string Name { get; set; }
        public int SegmentID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return Convert.ToString(this.Name); }
        }

        public string EntitlementDesc
        {
            get { return Convert.ToString(this.Name); }
        }
        public Infrastructure.Enums.EntitlementType EntitlementType
        {
            get { return EntitlementType.AccessLevel; }
        }
        public static SMSCompany GetSMSCompanyByKey(int companyKey)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSCompany>(dataAccessTransaction)
                      .Get(grp => grp.CompanyKey == companyKey).FirstOrDefault());
            }
        }
        public static SMSCompany GetSMSCompanyByName(string companyName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.SMSCompany>(dataAccessTransaction)
                      .Get(grp => grp.Name == companyName).FirstOrDefault());
            }
        }
        //public static IList<SMSCompany> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<SMSCompany> listDeletedGroups = new List<SMSCompany>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.SMSCompany>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.LastModifiedDate < process.ProcessStartAt).ToList());
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}
        public static List<SMSCompany> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.SMSCompany>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }
        public static void SaveCompanies(IList<SMSCompany> list, long endpointId, string currentUser)
        {
            try
            {
                var allexistingCompanies = SMSCompany.GetAllByEndpointSystemId(endpointId);
                if (allexistingCompanies == null)
                    allexistingCompanies = new List<SMSCompany>();

                SMSCompany existingCompany = null;
                foreach (SMSCompany comp in list)
                {
                    existingCompany = allexistingCompanies.Where(c => c.CompanyKey == comp.CompanyKey).FirstOrDefault();
                    if (existingCompany == null)
                    {
                        comp.CreatedBy = "System";
                        comp.CreatedDate = DateTime.Now;
                        comp.Save("System");
                        allexistingCompanies.Add(comp);
                    }
                    else
                    {
                        comp.Id = existingCompany.Id;
                        comp.CreatedBy = existingCompany.CreatedBy;
                        comp.CreatedDate = existingCompany.CreatedDate;

                        comp.Save("System");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
