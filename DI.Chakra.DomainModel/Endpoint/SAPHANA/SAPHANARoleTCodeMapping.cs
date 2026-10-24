using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANARoleTCodeMapping : DomainModelBase<SAPHANARoleTCodeMapping, EntityModel.SAPHANARoleTCodeMapping>
    {
        public string RoleCode { get; set; }

        public string TCode { get; set; }

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }



        public int? Version { get; set; }


        public static List<SAPHANARoleTCodeMapping> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPHANARoleTCodeMapping> GetByRoleCodeAndEndpointId(string roleCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.RoleCode == roleCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static List<SAPHANARoleTCodeMapping> GetByTCodeAndEndpointId(string tCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.TCode == tCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static List<SAPHANARoleTCodeMapping> GetTCodeByRCodeAndEndpointId(string rCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANARoleTCodeMapping>(dataAccessTransaction).Get(obj => obj.RoleCode == rCode && obj.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static void SaveSapRoleTCodeMapping(IList<SAPHANARoleTCodeMapping> list, long endpointSystemId,string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPHANARoleTCodeMapping> listExistingSapRoleTCodeMappings = new List<SAPHANARoleTCodeMapping>();
                SAPHANARoleTCodeMapping sapexistingSapRoleTCodeMapping = null;
                listExistingSapRoleTCodeMappings = SAPHANARoleTCodeMapping.GetAll();
                foreach (SAPHANARoleTCodeMapping saproletcodes in list)
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

        public static void SaveSapRoleTCodeMapping(IList<SAPHANARoleTCodeMapping> list, string currentUser)
        {
            if (list != null && list.Count > 0)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    SQLBulkUploader<SAPHANARoleTCodeMapping> sqlSapRoleTCodeMap = null;
                    try
                    {

                        list = list.Distinct().ToList();
                        string tableName = "DI_SAPHANA_ROLETCODEMAPPING";

                        sqlSapRoleTCodeMap = new SQLBulkUploader<SAPHANARoleTCodeMapping>(tableName);

                        sqlSapRoleTCodeMap.Flush(list);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlSapRoleTCodeMap?.Close();
                    }
                    //foreach (SAPHANARoleTCodeMapping saproletcodes in list)
                    //{
                    //    saproletcodes.Save(dataAccessTransaction);
                    //}
                    dataAccessTransaction.Commit();
                }
            }
        }

        public static void DeleteSapRoleTCodeMappingForRole(string roleCode, long endpointSystemId)
        {
            var deleteList = SAPHANARoleTCodeMapping.GetByRoleCodeAndEndpointId(roleCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleTCodeMappingForTCode(string tCode, long endpointSystemId)
        {
            var deleteList = SAPHANARoleTCodeMapping.GetByTCodeAndEndpointId(tCode, endpointSystemId);
            DeleteList(deleteList);
        }

        public static void DeleteSapRoleTCodeMappingForEndpoint(long endpointSystemId)
        {
            string query = "DELETE [DI_SAPHANA_ROLETCODEMAPPING] WHERE EndPointSystemId = {0}";
            ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
        }
    }

}
