using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SAPParameterHistoryInfoRepository : RepositoryBase<SAPParameterHistoryInfo>
    {
        public SAPParameterHistoryInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<SAPParameterHistoryInfo> GetSAPParameters(long endPointSystemId)
        {
            //string query = "DI_spGetAllSAPParametersByEndPointID";
            SAPParameterHistoryInfo _SAPParameter = new SAPParameterHistoryInfo();
            IList<SAPParameterHistoryInfo> _list = new List<SAPParameterHistoryInfo>();
            var _list1 = (from saph in CurrentContext.SAPParameterInfo // SAPParameterHistoryInfos
                          where saph.EndPointSystemId == endPointSystemId
                          select saph).Distinct();
            foreach (var tLst in _list1)
            {
                _SAPParameter.PARAM_CODE = CheckNull.AgainstString(tLst.PARAM_CODE);
                _SAPParameter.PARAM_DESC = CheckNull.AgainstString(tLst.PARAM_DESC);
                _SAPParameter.PARAM_VALUE = CheckNull.AgainstString(tLst.PARAM_VALUE);
                _SAPParameter.Id = CheckNull.AgainstInt(tLst.Id); ;
                _list.Add(_SAPParameter);
            }
            return _list;
        }
    }
}
