using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SapRoleProfileMapping : DomainModelBase<SapRoleProfileMapping, EntityModel.SapRoleProfileMapping>
    {
        public string RoleName { get; set; }

        public string ProfileName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static List<SapRoleProfileMapping> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleProfileMapping>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SapRoleProfileMapping> GetByRoleCodeAndEndpointId(string roleCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleProfileMapping>(dataAccessTransaction).Get(obj => obj.RoleName == roleCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SapRoleProfileMapping> GetByProfileAndEndpointId(string profile, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleProfileMapping>(dataAccessTransaction).Get(obj => obj.ProfileName == profile && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static void SaveSapRoleProfileMapping(IList<SapRoleProfileMapping> list, long endpointSystemId, string currentUser)
        {
            IList<SapRoleProfileMapping> listExistingSapRoleProfileMappings = new List<SapRoleProfileMapping>();
            SapRoleProfileMapping sapexistingSapRoleProfileMapping = null;
            listExistingSapRoleProfileMappings = SapRoleProfileMapping.GetByEndpointId(endpointSystemId);
            foreach (SapRoleProfileMapping saproleprofiles in list)
            {
                sapexistingSapRoleProfileMapping = listExistingSapRoleProfileMappings.Where(t => t.RoleName == saproleprofiles.RoleName && t.ProfileName == saproleprofiles.ProfileName).FirstOrDefault();
                if (sapexistingSapRoleProfileMapping == null)
                {
                    saproleprofiles.Save(currentUser);
                }
                else
                {
                    saproleprofiles.Id = sapexistingSapRoleProfileMapping.Id;
                    saproleprofiles.Save(currentUser);
                }
            }
        }

        public static void SaveSapRoleProfileMapping(IList<SapRoleProfileMapping> list, string currentUser)
        {
            foreach (SapRoleProfileMapping saproleprofiles in list)
            {
                saproleprofiles.Save(currentUser);
            }
        }

        public static void DeleteSapRoleProfileMappingForRole(string roleCode, long endpointSystemId)
        {
            var deleteList = SapRoleProfileMapping.GetByRoleCodeAndEndpointId(roleCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleProfileMappingForProfile(string profile, long endpointSystemId)
        {
            var deleteList = SapRoleProfileMapping.GetByProfileAndEndpointId(profile, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleProfileMappingForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAP_ROLEPROFILE] WHERE EndPointSystemId = {0}";
            ExecuteQuery(String.Format(query, endpointSystemId));
        }
    }
}
