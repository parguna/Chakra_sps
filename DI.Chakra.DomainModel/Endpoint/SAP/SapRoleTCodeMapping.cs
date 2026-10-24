using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SapRoleTCodeMapping : DomainModelBase<SapRoleTCodeMapping, EntityModel.SapRoleTCodeMapping>
    {
        public string RoleCode { get; set; }

        public string TCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }


        public static List<SapRoleTCodeMapping> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SapRoleTCodeMapping> GetByRoleCodeAndEndpointId(string roleCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.RoleCode == roleCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SapRoleTCodeMapping> GetByTCodeAndEndpointId(string tCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.TCode == tCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static List<SapRoleTCodeMapping> GetTCodeByRCodeAndEndpointId(string rCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SapRoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.RoleCode == rCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static void SaveSapRoleTCodeMapping(IList<SapRoleTCodeMapping> list, long endpointSystemId,string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SapRoleTCodeMapping> listExistingSapRoleTCodeMappings = new List<SapRoleTCodeMapping>();
                SapRoleTCodeMapping sapexistingSapRoleTCodeMapping = null;
                listExistingSapRoleTCodeMappings = SapRoleTCodeMapping.GetAll();
                foreach (SapRoleTCodeMapping saproletcodes in list)
                {
                    sapexistingSapRoleTCodeMapping = listExistingSapRoleTCodeMappings.Where(t => t.RoleCode == saproletcodes.RoleCode && t.TCode == saproletcodes.TCode && t.EndPointSystemId == endpointSystemId).FirstOrDefault();
                    if (sapexistingSapRoleTCodeMapping == null)
                    {
                        saproletcodes.Save(currentUser);
                    }
                    else
                    {
                        saproletcodes.Id = sapexistingSapRoleTCodeMapping.Id;
                        saproletcodes.Save(currentUser);
                    }
                }

                dataAccessTransaction.Commit();
            }
        }

        public static void SaveSapRoleTCodeMapping(IList<SapRoleTCodeMapping> list, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (SapRoleTCodeMapping saproletcodes in list)
                {
                    saproletcodes.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteSapRoleTCodeMappingForRole(string roleCode, long endpointSystemId)
        {
            var deleteList = SapRoleTCodeMapping.GetByRoleCodeAndEndpointId(roleCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleTCodeMappingForTCode(string tCode, long endpointSystemId)
        {
            var deleteList = SapRoleTCodeMapping.GetByTCodeAndEndpointId(tCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleTCodeMappingForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAP_ROLETCODEMAPPING] WHERE EndPointSystemId = {0}";
            ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
        }
    }

}
