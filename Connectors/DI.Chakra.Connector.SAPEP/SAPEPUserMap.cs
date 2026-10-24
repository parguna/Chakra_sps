using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DI.Chakra.DomainModel;
using System.Reflection;

namespace DI.Chakra.Connector.SAPEP
{
    public class SAPEPUserMap
    {
        SAPEPUserMap usermap = null;
        public bool MapAll { get; set; }
        private Object GetPropValue(String name, Object obj)
        {
            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(name);

            if (info == null)
                return null;
            if (info.ToString().Equals(string.Empty))
                return null;

            obj = info.GetValue(obj, null);
            if (info.PropertyType == typeof(DateTime))
            {
                if (obj == null || obj.ToString() == string.Empty)
                    return DBNull.Value;
            }

            return obj;
        }
  

        public string GetJoinQry(long endpointSystemId)
        {
            string clauseType = "AND";
            if (!MapAll)
            {
                clauseType = "OR";
            }
            StringBuilder bldr = new StringBuilder();
            IList<AttributeMappingColumns> listAttributes = new SAPEPUserMap().GetAttributeMappingColumns(endpointSystemId);
            foreach (AttributeMappingColumns attDto in listAttributes)
            {
                bldr.AppendFormat(string.Format("(DI_SAPEP_UserInfo.{0}=DI_M_User.{1}  and DI_SAPEP_UserInfo.{2} != '' and DI_SAPEP_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));
            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));
            return string.Empty;
        }

        public DataTable MapWithMasterUser(EndPoint EndPoint)
        {            
            StringBuilder bldR = new StringBuilder();
            string strJoin = GetJoinQry(EndPoint.Id);

            if (strJoin == string.Empty)
                return null;

            bldR.Append("Update DI_SAPEP_UserInfo set DI_SAPEP_UserInfo.DIUserId = DI_M_User.Id, DI_SAPEP_UserInfo.MappingType='System',DI_SAPEP_UserInfo.AccountType=1 FROM DI_SAPEP_UserInfo JOIN DI_M_User on");
            bldR.Append(" ");
            bldR.Append(strJoin);
            bldR.Append(" ");
            bldR.AppendFormat("WHERE DI_SAPEP_UserInfo.EndpointSystemId = {0} AND DI_M_User.EmployeeId <> '' AND DI_M_User.Id <> 1 AND ISNULL(MappingType,'') <> 'Manual'", EndPoint.Id.ToString());
            SAPEPFacade.ExecuteQry(bldR.ToString());
            return null;
        }
    }
}

