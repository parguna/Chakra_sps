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

    public class SSIDSRolelnfo : DomainModelBase<SSIDSRolelnfo, EntityModel.SSIDSRolelnfo>, IEndpointEntitlementInfo
    {
        public SSIDSRolelnfo()
        {
            this.SSIDSUserInfos = new List<SSIDSUserInfo>();
        }
        public long EndPointSystemId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public IList<SSIDSUserInfo> SSIDSUserInfos { get; set; }
      
        public EndPoint EndPoint { get; set; }

        public string EntitlementName
        {
            get { return Convert.ToString(this.RoleName); }
        }

        public string EntitlementDesc
        {
            get { return this.Description; }
        }
        public EntitlementType EntitlementType
        {
            get { return EntitlementType.Role; }
        }
        
        //public static IList<SSIDSRolelnfo> GetDeletedByEndPointSystemId(long endPointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        DiscoveryProcess objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemIdDefault(endPointSystemId);

        //        IList<SSIDSRolelnfo> listDeletedGroups = new List<SSIDSRolelnfo>();

        //        if (objDiscoveryProcess != null)
        //        {
        //            Process process = Process.GetByProcessId(objDiscoveryProcess.Id);
        //            if (process != null)
        //            {
        //                listDeletedGroups = ORMapList(new RepositoryBase<EntityModel.SSIDSRolelnfo>(dataAccessTransaction).Get(grp => grp.EndPointSystemId == endPointSystemId && grp.LastModifiedDate < process.ProcessStartAt).ToList());
        //            }
        //        }

        //        return listDeletedGroups;

        //    }
        //}

        public static SSIDSRolelnfo GetByRoleName(string roleName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SSIDSRolelnfo>(dataAccessTransaction)
                      .Get(grp => grp.RoleName == roleName).FirstOrDefault());
            }
        }


        public static List<SSIDSRolelnfo> GetAllByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SSIDSRolelnfo>(dataAccessTransaction)
                      .Get(grp => grp.EndPointSystemId == endpointSystemId).ToList());
            }
        }
         

        public static Dictionary<string,string> GetAllRoles()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
               // var endpoint = EndPoint.GetByApplicationType(ApplicationType.SSIDS_GUI);

               // long endpointId = 0;

                //if (endpoint != null)
                //    endpointId = endpoint.Id;

                return new RepositoryBase<EntityModel.SSIDSRolelnfo>(dataAccessTransaction)
                      .GetAll().Select(item => new { Key = item.RoleName, Value = item.Description }).Distinct().ToDictionary(item => item.Key, item => item.Value); ;
            }
        }

    }
}
