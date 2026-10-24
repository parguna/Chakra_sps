using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANACompositeRoleInfo : DomainModelBase<SAPHANACompositeRoleInfo, EntityModel.SAPHANACompositeRoleInfo>
    {
        public string CompositeRole_Code { get; set; }

        public string CompositeRole_Desc { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPHANASingleRoleInfo> SAPSingleRoleInfos { get; set; }


        public static SAPHANACompositeRoleInfo GetByRoleCode(string roleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANACompositeRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.CompositeRole_Code == roleCode).FirstOrDefault());
            }
        }
        public static SAPHANACompositeRoleInfo GetByRoleCodeIncludingSAPSingleRoleInfos(string compositeRole_Code, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANACompositeRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.CompositeRole_Code == compositeRole_Code && roleInfo.EndPointSystemId == endPointSystemId, roleInfo => roleInfo.SAPSingleRoleInfos).FirstOrDefault());
            }
        }
        public static SAPHANACompositeRoleInfo GetByRoleCodeAndEndpoint(string roleCode, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANACompositeRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.CompositeRole_Code == roleCode && roleInfo.EndPointSystemId == endpointSystemId).FirstOrDefault());
            }
        }
        public static List<SAPHANACompositeRoleInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANACompositeRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId, true).ToList());
            }
        }

        public static IList<SAPHANACompositeRoleInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANACompositeRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.ModifiedDate < modifiedDate, true).ToList());
            }
        }

        public static List<string> GetChildRolesByCompositeRoleCode(List<string> compositeRolesList)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new SAPHANACompositeRoleInfoRepository(dataAccessTransaction).GetChildRolesByCompositeRoleCode(compositeRolesList)); 
            }

        }

        public static IList<SAPHANACompositeRoleInfo> GetDeletedSAPCompositeRoleInfo(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANACompositeRoleInfo> listDeletedSAPCompositeRoles = new List<SAPHANACompositeRoleInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    
                    if (process != null)
                    {
                        listDeletedSAPCompositeRoles = SAPHANACompositeRoleInfo.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedSAPCompositeRoles;

            }
        }

        public static IList<SAPHANACompositeRoleInfo> GetDeletedSAPCompositeRoleInfo(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANACompositeRoleInfo> listDeletedSAPCompositeRoles = new List<SAPHANACompositeRoleInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);

                    if (process != null)
                    {
                        listDeletedSAPCompositeRoles = SAPHANACompositeRoleInfo.GetByModifiedDate(endPointSystemId, modifiedDate);
                    }
                }

                return listDeletedSAPCompositeRoles;

            }
        }


        public static void SaveSAPCompositeRoles(IList<SAPHANACompositeRoleInfo> list, long endpointSystemId, string currentUser)
        {

            SAPHANACompositeRoleInfo sapexistingCompositeRole = null;
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                foreach (SAPHANACompositeRoleInfo compositerole in list)
                {
                    sapexistingCompositeRole = GetByRoleCodeAndEndpoint(compositerole.CompositeRole_Code, endpointSystemId);
                    //sapexistingCompositeRole = GetByRoleCodeIncludingSAPSingleRoleInfos(compositerole.CompositeRole_Code, endpointSystemId);
                    if (sapexistingCompositeRole == null)
                    {
                        compositerole.Save(currentUser);
                    }
                    else
                    {
                        //compositerole.CompositeRole_Code = sapexistingCompositeRole.CompositeRole_Code;
                        compositerole.Id = sapexistingCompositeRole.Id;
                        compositerole.CreatedBy = sapexistingCompositeRole.CreatedBy;
                        compositerole.CreatedDate = sapexistingCompositeRole.CreatedDate;
                        compositerole.Save(currentUser);

                    }
                }
                //dataAccessTransaction.Commit();
            }           
        }

        public static void SaveSAPCompositeRoles(IList<SAPHANACompositeRoleInfo> list, string currentUser)
        {
            if (list != null && list.Count > 0)
            {
                try
                {
                    SQLBulkUploader<SAPHANACompositeRoleInfo> sqlSAPCompositeRole = null;
                    try
                    {

                        list = list.Distinct().ToList();
                        string tableName = "DI_SAPHANA_COMPOSITEROLEINFO";

                        sqlSAPCompositeRole = new SQLBulkUploader<SAPHANACompositeRoleInfo>(tableName);

                        sqlSAPCompositeRole.Flush(list);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlSAPCompositeRole?.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //foreach (SAPHANACompositeRoleInfo compositerole in list)
                //{
                //    compositerole.Save(currentUser);
                //}
            }
        }
        //3081
        public static void DeleteSAPCompositeRoleInfoByEndpoint(long endpointSystemId, string currentUser)
        {
            string deleteChildQuery = "DELETE from [dbo].[SAPHANASingleRoleInfoSAPHANACompositeRoleInfo] where EndpointSystemId = {0}";
            string query = "DELETE [DI_SAPHANA_COMPOSITEROLEINFO] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQueryWithCommandTimeout(String.Format(deleteChildQuery, endpointSystemId), 300);
                ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
            }
        }
    }
}
