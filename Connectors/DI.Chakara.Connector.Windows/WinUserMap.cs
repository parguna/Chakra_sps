using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using DI.Chakra.Repository;
using System.Text;
using DI.Chakra.Transaction;


namespace DI.Chakra.Connector
{
    public class WinUserMap
    {

        public bool MapAll { get; set; }


        public WinUserMap()
        {

        }
       

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

        //Newly Added

        public string GetJoinQry()
        {
            string clauseType = "AND";

            if (!MapAll)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();


            IList<AttributeMappingColumns> listAttributes = new List<AttributeMappingColumns>();
            listAttributes = AttributeMappingColumns.GetWinAttributeMappingColumns();

            foreach (AttributeMappingColumns attDto in listAttributes)
            {

                bldr.AppendFormat(string.Format("DI_WIN_UserInfo.{0}=DI_M_User.{1} {2} ", attDto.Tgcolumn, attDto.DiColumn, clauseType));

            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));

            return string.Empty;
        }

        public DataTable MapWithMasterUser(IList<WindowsUserInfo> listWinUser, IList<User> listMaster, EndPoint EndPoint)
        {

            string strQry = string.Format("Update DI_WIN_UserInfo Set DIUserId=NULL, MappingType=NULL where EndpointSystemId={0} AND MappingType='System'", EndPoint.Id.ToString());

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(strQry));
                dataAccessTransaction.Commit();
            }

            StringBuilder bldR = new StringBuilder();

            string strJoin = GetJoinQry();

            if (strJoin == string.Empty)
                return null;

            bldR.Append("Update DI_WIN_UserInfo set DI_WIN_UserInfo.DIUserId = DI_M_User.UserId, DI_WIN_UserInfo.MappingType='System' FROM DI_WIN_UserInfo JOIN DI_M_User on");
            bldR.Append(" ");
            bldR.Append(strJoin);
            bldR.Append(" ");
            bldR.AppendFormat("WHERE DI_WIN_UserInfo.EndpointSystemId = {0}  AND DI_M_User.UserId <> 0", EndPoint.Id.ToString());

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                new ADSUserInfoRepository(dataAccessTransaction).ExecuteQuery(Convert.ToString(bldR));
                dataAccessTransaction.Commit();
            }

            return null;

        }
    }
}
