using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPSingleRoleInfo : DomainModelBase<SAPSingleRoleInfo, EntityModel.SAPSingleRoleInfo>
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

        public static IList<SAPSingleRoleInfo> GetBySingleRoleCode(long endPointSystemId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.SingleRole_Code == singleRoleCode).ToList());
            }
        }

        public static IList<SAPSingleRoleInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
        public static SAPSingleRoleInfo GetByRoleCode(long endPointSystemId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId && roleInfo.SingleRole_Code == singleRoleCode).FirstOrDefault());
            }
        }
        public static SAPSingleRoleInfo GetBySingleRoleCode(string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.SingleRole_Code == singleRoleCode).FirstOrDefault());
            }
        }

        public static IList<SAPSingleRoleInfo> GetbyModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointSystemId
                && ((roleInfo.LastModifiedDate == null && roleInfo.CreatedDate < modifiedDate) || roleInfo.LastModifiedDate < modifiedDate)).ToList());
            }
        }

        public static IList<SAPSingleRoleInfo> GetByEnpointSystemIdIncludingSAPTCodeInfo(long endPointId, string singleRoleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPSingleRoleInfo>(dataAccessTransaction).Get(roleInfo => roleInfo.EndPointSystemId == endPointId && roleInfo.SingleRole_Code == singleRoleCode, roleInfo => roleInfo.SAPTCodes).ToList());
            }
        }

        public static void SaveSingleRoles(IList<SAPSingleRoleInfo> list, long endPointSystemId, string currentUser)
        {
            SAPSingleRoleInfo sapexistingSingleRole = null;


            try
            {
                //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
                {
                    IList<SAPSingleRoleInfo> lstSapSingleRoles = SAPSingleRoleInfo.GetByEndPointSystemId(endPointSystemId);

                    foreach (SAPSingleRoleInfo singlerole in list)
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
                            //_context.SAPSingleRoleInfos.Attach(singlerole);
                            //_context.Entry(singlerole).State = EntityState.Modified;
                            //_context.SaveChanges();
                            //SAPDA.UpdateSAPSingleRole(singlerole);
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

        public static void SaveSingleRoles(IList<SAPSingleRoleInfo> list, string currentUser)
        {
            foreach (SAPSingleRoleInfo singleRole in list)
            {
                singleRole.Save(currentUser);
            }
        }

        public static IList<SAPSingleRoleInfo> GetDeletedSAPSingleRole(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPSingleRoleInfo> listDeletedSingleRole = new List<SAPSingleRoleInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);

                    if (process != null)
                    {
                        listDeletedSingleRole = SAPSingleRoleInfo.GetbyModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }

                return listDeletedSingleRole;

            }
        }

        public static void DeleteSAPSingleRoleInfoByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAP_SINGLEROLEINFO] where [EndPointSystemId] = {0}";
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQueryWithCommandTimeout(String.Format(query, endpointSystemId),300);
            }
        }

    }
}
