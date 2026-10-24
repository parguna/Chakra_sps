using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAParameterInfo : DomainModelBase<SAPHANAParameterInfo, EntityModel.SAPHANAParameterInfo>
    {
        public string PARAM_CODE { get; set; }

        public Nullable<int> EndPointSystemId { get; set; }

        public string PARAM_DESC { get; set; }

        public string PARAM_VALUE { get; set; }

        public string TargetSysId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public IList<SAPHANAUserinfo> SAPUserinfos { get; set; }

        public int? Version { get; set; }


        public static SAPHANAParameterInfo CreateSAPParameterInfo()
        {
            SAPHANAParameterInfo SAPParameterInfo = new SAPHANAParameterInfo();

            return SAPParameterInfo;
        }

        public static IList<SAPHANAParameterInfo> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static IList<SAPHANAParameterInfo> GetByEndpointSystemId( long endPointSystemId,string paramCodeSearchText)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.PARAM_CODE.Contains(paramCodeSearchText)).ToList());
            }
        }

        public static SAPHANAParameterInfo GetByParamCode( string paramCode,long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.PARAM_CODE == paramCode).FirstOrDefault());
            }
        }

        public static IList<SAPHANAParameterInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.LastModifiedDate <= modifiedDate).ToList());
            }
        }


        public static IList<SAPHANAParameterInfo> GetDeletedSAPParameters(long endPointSystemId)
          {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPHANAParameterInfo> listDeletedSAPParameters = new List<SAPHANAParameterInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedSAPParameters = SAPHANAParameterInfo.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }
                return listDeletedSAPParameters;

            }
        }

        public static void DeleteByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterInfo>(dataAccessTransaction).Get(sapparameter => sapparameter.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }
        public static void SaveParameterInfos(IList<SAPHANAParameterInfo> list, long EndpointSystemId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPHANAParameterInfo> listExistingSAPParameterInfos = new List<SAPHANAParameterInfo>();
                SAPHANAParameterInfo sapexistingParameter = null;
                listExistingSAPParameterInfos = SAPHANAParameterInfo.GetAll();
                foreach (SAPHANAParameterInfo para in list)
                {
                    sapexistingParameter = listExistingSAPParameterInfos.Where(t => t.PARAM_CODE == para.PARAM_CODE && t.EndPointSystemId == EndpointSystemId).FirstOrDefault();
                    if (sapexistingParameter == null)
                    {
                        para.Save(currentUser);
                    }
                    else
                    {
                        para.Id = sapexistingParameter.Id;
                        para.PARAM_CODE = sapexistingParameter.PARAM_CODE;
                        para.CreatedDate = sapexistingParameter.CreatedDate;
                        para.CreatedBy = sapexistingParameter.CreatedBy;
                        para.Save(currentUser);
                    }
                }

                dataAccessTransaction.Commit();
            }
        }

          //public IList<SAPParameterHistoryInfo> GetSAPParametersFromRepository(long endPointSystemId)
          //{
          //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
          //    {
          //        return ORMapList(new SAPParameterHistoryInfoRepository(dataAccessTransaction).GetSAPParameters(endPointSystemId));
          //    }
          //}


       
  
    }
}
