using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANARoleProfileMapping : DomainModelBase<SAPHANARoleProfileMapping, EntityModel.SAPHANARoleProfileMapping>
    {
        public string RoleName { get; set; }

        public string ProfileName { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public static List<SAPHANARoleProfileMapping> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleProfileMapping>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPHANARoleProfileMapping> GetByRoleCodeAndEndpointId(string roleCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleProfileMapping>(dataAccessTransaction).Get(obj => obj.RoleName == roleCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPHANARoleProfileMapping> GetByProfileAndEndpointId(string profile, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleProfileMapping>(dataAccessTransaction).Get(obj => obj.ProfileName == profile && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static void SaveSapRoleProfileMapping(IList<SAPHANARoleProfileMapping> list, long endpointSystemId, string currentUser)
        {
            IList<SAPHANARoleProfileMapping> listExistingSapRoleProfileMappings = new List<SAPHANARoleProfileMapping>();
            SAPHANARoleProfileMapping sapexistingSapRoleProfileMapping = null;
            listExistingSapRoleProfileMappings = SAPHANARoleProfileMapping.GetByEndpointId(endpointSystemId);
            foreach (SAPHANARoleProfileMapping saproleprofiles in list)
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

        public static void SaveSapRoleProfileMapping(IList<SAPHANARoleProfileMapping> list, string currentUser)
        {
            foreach (SAPHANARoleProfileMapping saproleprofiles in list)
            {
                saproleprofiles.Save(currentUser);
            }
        }

        public static void DeleteSapRoleProfileMappingForRole(string roleCode, long endpointSystemId)
        {
            var deleteList = SAPHANARoleProfileMapping.GetByRoleCodeAndEndpointId(roleCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleProfileMappingForProfile(string profile, long endpointSystemId)
        {
            var deleteList = SAPHANARoleProfileMapping.GetByProfileAndEndpointId(profile, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleProfileMappingForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAPHANA_ROLEPROFILE] WHERE EndPointSystemId = {0}";
            ExecuteQuery(String.Format(query, endpointSystemId));
        }
    }
}
