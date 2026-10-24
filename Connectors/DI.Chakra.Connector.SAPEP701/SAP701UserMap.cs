using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Facade;
using System.Data;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;

using System.Reflection;

namespace DI.Chakra.Connector.SAPIP
{
    public class SAPEPUserMap
    {
        SAPEPFacade SAPEPFacade = null;

        public bool MapAll { get; set; }


        //public System.Data.DataTable MapWithMasterUser(IList<SAPEPUserInfo> listSAPEPUsers, IList<User> listMaster, EndPoint EndPoint)
        //{
        //    SAPEPFacade = new SAPEPFacade();


        //    int count = 0;

        //    DataRow[] dRows = null;
        //    IList<User> users = new List<User>();
        //    DataTable dtAdsUser = null;

        //    string clauseType = "AND";

        //    try
        //    {

        //        listSAPEPUsers = SAPEPFacade.GetAllUnMappedSAPEPUsers(EndPoint);

        //        if (listSAPEPUsers.Count <= 0)
        //            return null;

        //        DataTable dtMaster = CollectionHelper.ConvertToNormalTable<User>(listMaster);

        //        dtAdsUser = CollectionHelper.ConvertToNormalTable<SAPEPUserInfo>(listSAPEPUsers);


        //        IList<AttributeMappingColumns> listAttributes = SAPEPFacade.GetAttributeMappingColumns(Constants.SAPEP);

        //        Type typeMaster = typeof(User);

        //        IList<PropertyInfo> listProperty = typeMaster.GetProperties().ToList();

        //        if (!MapAll)
        //        {
        //            clauseType = "OR";
        //        }

        //        listMaster = listMaster.Where(l => l.IsActive == true).ToList();

        //        foreach (User user in listMaster)
        //        {
        //            count = 0;
        //            object objUserId = null;

        //            StringBuilder bldr = new StringBuilder();

        //            foreach (AttributeMappingColumns attDto in listAttributes)
        //            {

        //                IList<PropertyInfo> listProp = listProperty.Where(prop => prop.Name.ToUpper() == attDto.DiColumn.ToUpper()).ToList();

        //                if (listProp.Count <= 0)
        //                    break;

        //                PropertyInfo property = listProp.First();

        //                if (property == null)
        //                    break;

        //                object objUserValue = GetPropValue(property.Name, user);

        //                objUserId = GetPropValue("UserId", user);

        //                bldr.AppendFormat(string.Format("{0}='{1}'", attDto.Tgcolumn, objUserValue.ToString().Replace("'", "''")));

        //                count++;

        //                if (count < listAttributes.Count)
        //                    bldr.AppendFormat(" {0} ", clauseType);

        //            }

        //            dRows = dtAdsUser.Select(bldr.ToString());

        //            foreach (DataRow dRw in dRows)
        //            {
        //                dRw["DIUserId"] = objUserId;
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //NOTE: Eat the exception.
        //        Log.General.WriteMessage(LogLevel.ERROR, string.Format("MAP IACM error occured"), ex);

        //    }
        //    return dtAdsUser;


        //}

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

        public string GetJoinQry(long endpointSystemId)
        {
            string clauseType = "AND";

            if (!MapAll)
            {
                clauseType = "OR";
            }

            StringBuilder bldr = new StringBuilder();


            IList<AttributeMappingColumns> listAttributes = new EndPointFacade().GetAttributeMappingColumns(endpointSystemId);

            foreach (AttributeMappingColumns attDto in listAttributes)
            {

                bldr.AppendFormat(string.Format("(DI_SAPEP_UserInfo.{0}=DI_M_User.{1} and DI_SAPEP_UserInfo.{2} != '' and DI_SAPEP_UserInfo.{3} is not null and DI_M_User.{4} != '' and DI_M_User.{5} is not null)  {6} ", attDto.Tgcolumn, attDto.DiColumn, attDto.Tgcolumn, attDto.Tgcolumn, attDto.DiColumn, attDto.DiColumn, clauseType));

            }
            if (bldr.ToString() != string.Empty)
                return bldr.ToString().Remove(bldr.ToString().Length - (clauseType.Length + 1));

            return string.Empty;
        }

        public DataTable MapWithMasterUser(EndPoint EndPoint)
        {
            SAPEPFacade = new SAPEPFacade();


            //string strQry = string.Format("Update DI_SAPEP_UserInfo Set DIUserId=NULL, MappingType=NULL where EndpointSystemId={0} AND MappingType='System'", EndPoint.EndpointSystemId.ToString());


            //SAPEPFacade.ExecuteQry(strQry);


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
