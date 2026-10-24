using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPParameterHistoryInfo : DomainModelBase<SAPParameterHistoryInfo, EntityModel.SAPParameterHistoryInfo>
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

        public static SAPParameterHistoryInfo CreateSAPParameterHistoryInfo()
        {
            SAPParameterHistoryInfo SAPParameterHistoryInfo = new SAPParameterHistoryInfo();

            return SAPParameterHistoryInfo;
        }

        public static List<SAPParameterHistoryInfo> GetAllByEndpointSystemId(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId).ToList());
            }

        }
        public static List<SAPParameterHistoryInfo> GetAllByEndpointSystemId(long endPointId,string paramCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId && paramInfo.PARAM_CODE.Contains(paramCode)).ToList());
            }

        }


        public static SAPParameterHistoryInfo GetByEndpointSystemId(long endPointId, string paramCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId && paramInfo.PARAM_CODE.Contains(paramCode)).FirstOrDefault());
            }

        }

        

        public static IList<SAPParameterHistoryInfo> GetAllByModifiedDate(long endPointId,DateTime? processStartAt)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPParameterHistoryInfo>(dataAccessTransaction).Get(paramInfo => paramInfo.EndPointSystemId == endPointId &&paramInfo.LastModifiedDate< processStartAt).ToList());
            }
        }


        public static IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPParameterHistoryInfo _SAPParameter = null;
            IList<SAPParameterHistoryInfo> _list = new List<SAPParameterHistoryInfo>();

           
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    IList<SAPParameterInfo> parameterInfo = SAPParameterInfo.GetByEndpointSystemId(endPoint.Id).Distinct().ToList();


                    foreach (var tLst in parameterInfo)
                    {
                        _SAPParameter = SAPParameterHistoryInfo.CreateSAPParameterHistoryInfo();

                        _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                        _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                        _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                        _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                        _list.Add(_SAPParameter);
                    }
                }
                return _list;
          
           
        }


        public IList<SAPParameterHistoryInfo> GetSAPParameters(EndPoint endPoint, string searchText)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPParameterHistoryInfo _SAPParameter = null;
            IList<SAPParameterHistoryInfo> _list = new List<SAPParameterHistoryInfo>();


            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPParameterInfo> parameterInfo = SAPParameterInfo.GetByEndpointSystemId(endPoint.Id, searchText);

                foreach (var tLst in parameterInfo)
                    {
                        _SAPParameter = SAPParameterHistoryInfo.CreateSAPParameterHistoryInfo();

                        _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                        _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                        _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                        _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                        _list.Add(_SAPParameter);
                    }
                }
                return _list;
           
        }


        public void AddSAPParameterHistoryInfoBulk(IList<SAPParameterHistoryInfo> lstParameters)
        {
            SQLBulkUploader<SAPParameterHistoryInfo> sqlParameterHistory = null;
            try
            {
                string tableName = "DI_SAP_PARAMETERHISTORYINFO";
                sqlParameterHistory = new SQLBulkUploader<SAPParameterHistoryInfo>(tableName);
                sqlParameterHistory.Flush(lstParameters);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                sqlParameterHistory.Close();
            }
        }

    }
}
