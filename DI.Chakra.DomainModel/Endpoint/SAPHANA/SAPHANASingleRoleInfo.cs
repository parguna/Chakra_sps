using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANASingleRoleInfo : DomainModelBase<SAPHANASingleRoleInfo, EntityModel.SAPHANASingleRoleInfo>
    {
        public string SingleRole_Code { get; set; }

        public string SingleRole_Desc { get; set; }



        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public int? Version { get; set; }

        public IList<SAPTCodeInfo> SAPTCodes { get; set; }

        public IList<SAPCompositeRoleInfo> SAPCompositeRoleInfos { get; set; }

        public static IList<SAPHANASingleRoleInfo> GetBySingleRoleCode(long endPointSystemId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.SingleRole_Code == singleRoleCode).ToList());
            }
        }

        public static IList<SAPHANASingleRoleInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static SAPHANASingleRoleInfo GetByRoleCode(long endPointSystemId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.SingleRole_Code == singleRoleCode).FirstOrDefault());
            }
        }
        public static SAPHANASingleRoleInfo GetBySingleRoleCode(string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.SingleRole_Code == singleRoleCode).FirstOrDefault());
            }
        }

        public static IList<SAPHANASingleRoleInfo> GetbyModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId
                && ((roleInfo.LastModifiedDate == null && roleInfo.CreatedDate < modifiedDate) || roleInfo.LastModifiedDate < modifiedDate)).ToList());
            }
        }

        public static IList<SAPHANASingleRoleInfo> GetByEnpointSystemIdIncludingSAPTCodeInfo(long endPointId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANASingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointId && roleInfo.SingleRole_Code == singleRoleCode, roleInfo => roleInfo.SAPTCodes).ToList());
            }
        }

        public static void SaveSingleRoles(IList<SAPHANASingleRoleInfo> list, long endPointSystemId, string currentUser)
        {
            SAPHANASingleRoleInfo sapexistingSingleRole = null;


            try
            {
                //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                {
                    IList<SAPHANASingleRoleInfo> lstSapSingleRoles = SAPHANASingleRoleInfo.GetByEndPointSystemId(endPointSystemId);

                    foreach (SAPHANASingleRoleInfo singlerole in list)
                    {
                        // check for role code and desc, especially since the desc contains the single role code
                        sapexistingSingleRole = lstSapSingleRoles.FirstOrDefault(sRole => sRole.SingleRole_Code == singlerole.SingleRole_Code
                        && sRole.SingleRole_Desc == singlerole.SingleRole_Desc
                        && sRole.EndPointSystemId == singlerole.EndPointSystemId);

                        if (sapexistingSingleRole == null)
                        {
                            singlerole.Save(currentUser);
                            //SAPDA.AddSAPSingleRole(singlerole);
                        }
                        else
                        {
                            singlerole.Id = sapexistingSingleRole.Id;
                            //singlerole.SingleRole_Code = sapexistingSingleRole.SingleRole_Code;
                            singlerole.CreatedBy = sapexistingSingleRole.CreatedBy;
                            singlerole.CreatedDate = sapexistingSingleRole.CreatedDate;
                            singlerole.Save(currentUser);
                        }
                    }

                    //dataAccessTransaction.Commit();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SaveSingleRoles(IList<SAPHANASingleRoleInfo> list, string currentUser)
        {
            if (list != null && list.Count > 0)
            {
                try
                {
                    SQLBulkUploader<SAPHANASingleRoleInfo> sqlSingleRoles = null;
                    try
                    {

                        list = list.Distinct().ToList();
                        string tableName = "DI_SAPHANA_SINGLEROLEINFO";

                        sqlSingleRoles = new SQLBulkUploader<SAPHANASingleRoleInfo>(tableName);

                        sqlSingleRoles.Flush(list);

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlSingleRoles?.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //foreach (SAPHANASingleRoleInfo singleRole in list)
                //{
                //    singleRole.Save(currentUser);
                //}
            }
        }

        public static IList<SAPHANASingleRoleInfo> GetDeletedSAPSingleRole(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANASingleRoleInfo> listDeletedSingleRole = new List<SAPHANASingleRoleInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);

                    if (process != null)
                    {
                        listDeletedSingleRole = SAPHANASingleRoleInfo.GetbyModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedSingleRole;

            }
        }

        public static void DeleteSAPSingleRoleInfoByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAPHANA_SINGLEROLEINFO] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
            }
        }

    }
}
