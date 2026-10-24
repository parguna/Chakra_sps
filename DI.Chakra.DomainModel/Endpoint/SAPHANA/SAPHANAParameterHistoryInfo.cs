using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAParameterHistoryInfo : DomainModelBase<SAPHANAParameterHistoryInfo, EntityModel.SAPHANAParameterHistoryInfo>
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

        public int? Version { get; set; }

        public static SAPHANAParameterHistoryInfo CreateSAPParameterHistoryInfo()
        {
            SAPHANAParameterHistoryInfo SAPParameterHistoryInfo = new SAPHANAParameterHistoryInfo();

            return SAPParameterHistoryInfo;
        }

        public static List<SAPHANAParameterHistoryInfo> GetAllByEndpointSystemId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId).ToList());
            }

        }
        public static List<SAPHANAParameterHistoryInfo> GetAllByEndpointSystemId(long endPointId,string paramCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId && paramInfo.PARAM_CODE.Contains(paramCode)).ToList());
            }

        }


        public static SAPHANAParameterHistoryInfo GetByEndpointSystemId(long endPointId, string paramCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId && paramInfo.PARAM_CODE.Contains(paramCode)).FirstOrDefault());
            }

        }

        

        public static IList<SAPHANAParameterHistoryInfo> GetAllByModifiedDate(long endPointId,DateTime? processStartAt)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPHANAParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId &&paramInfo.LastModifiedDate< processStartAt).ToList());
            }
        }


        public static IList<SAPHANAParameterHistoryInfo> GetSAPParameters(EndPoint endPoint)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPHANAParameterHistoryInfo _SAPParameter = null;
            IList<SAPHANAParameterHistoryInfo> _list = new List<SAPHANAParameterHistoryInfo>();

           
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    IList<SAPHANAParameterInfo> parameterInfo = SAPHANAParameterInfo.GetByEndpointSystemId(endPoint.Id).Distinct().ToList();


                    foreach (var tLst in parameterInfo)
                    {
                        _SAPParameter = SAPHANAParameterHistoryInfo.CreateSAPParameterHistoryInfo();

                        _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                        _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                        _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                        _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                        _list.Add(_SAPParameter);
                    }
                }
                return _list;
          
           
        }

    }
}
