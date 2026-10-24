using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPParameterInfo:DomainModelBase<SAPParameterInfo,EntityModel.SAPParameterInfo>
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

        public IList<SAPUserinfo> SAPUserinfos { get; set; }

        public int? Version { get; set; }


        public static SAPParameterInfo CreateSAPParameterInfo()
        {
            SAPParameterInfo SAPParameterInfo = new SAPParameterInfo();

            return SAPParameterInfo;
        }

        public static IList<SAPParameterInfo> GetByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }

        public static IList<SAPParameterInfo> GetByEndpointSystemId( long endPointSystemId,string paramCodeSearchText)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.PARAM_CODE.Contains(paramCodeSearchText)).ToList());
            }
        }

        public static SAPParameterInfo GetByParamCode( string paramCode,long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.PARAM_CODE == paramCode).FirstOrDefault());
            }
        }

        public static IList<SAPParameterInfo> GetByModifiedDate(long endPointSystemId, DateTime? modifiedDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointSystemId && paramInfo.LastModifiedDate <= modifiedDate).ToList());
            }
        }


        public static IList<SAPParameterInfo> GetDeletedSAPParameters(long endPointSystemId)
          {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPParameterInfo> listDeletedSAPParameters = new List<SAPParameterInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointSystemId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);
                    if (process != null)
                    {
                        listDeletedSAPParameters = SAPParameterInfo.GetByModifiedDate(endPointSystemId, process.ProcessStartAt);
                    }
                }
                return listDeletedSAPParameters;

            }
        }

        public static void DeleteByEndpointSystemId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var ItemstobeDeleted = ORMapList(new RepositoryBase<EntityModel.SAPParameterInfo>(dataAccessTransaction).Get(sapparameter => sapparameter.EndPointSystemId == endPointSystemId).ToList());
                DeleteList(ItemstobeDeleted);
            }
        }
        public static void SaveParameterInfos(IList<SAPParameterInfo> list, long EndpointSystemId, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPParameterInfo> listExistingSAPParameterInfos = new List<SAPParameterInfo>();
                SAPParameterInfo sapexistingParameter = null;
                listExistingSAPParameterInfos = SAPParameterInfo.GetAll();
                foreach (SAPParameterInfo para in list)
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
