//#region namespace
//using system;
//using system.collections;
//using system.collections.generic;
//using system.data;
//using system.io;
//using system.data;
//using system.data.sqlclient;
//using system.linq;
//using system.text;
//using system.reflection;
//using system.configuration;
//using di.chakra.facade;
//using di.chakra.infrastructure;
//using di.chakra.model;
//using di.chakra.dataaccess;
//using di.chakra.logger;
//using di.chakra.infrastructure.enums;

//#endregion


//namespace deepidentity.connector
//{
//    class sqlusermap
//    {
//        public mssqlumfacade mssqlumfacade { get; set; }
//        public bool mapall { get; set; }

//        public datatable mapwithmasteruser(ilist<mssqlumlogininfo> listsqlumuser, ilist<user> listmaster, endpoint endpoint)
//        {

//            mssqlumda mssqlumda = new mssqlumda();

//            mssqlumfacade = new mssqlumfacade();

//            int count = 0;
//            bool ismatch = true;
//            datarow[] drows = null;
//            ilist<user> users = new list<user>();


//            string clausetype = "and";

//            object obj = null;

//            listsqlumuser = mssqlumfacade.getallunmappedsqlusers(endpoint);

//            if (listsqlumuser.count <= 0)
//                return null;

//            datatable dtmaster = collectionhelper.converttonormaltable<user>(listmaster);

//            datatable dtadsuser = collectionhelper.converttonormaltable<mssqlumlogininfo>(listsqlumuser);


//            ilist<attributemappingcolumns> listattributes = mssqlumda.getattributemappingcolumns(constants.sqlum);
//            type typemaster = typeof(user);

//            ilist<propertyinfo> listproperty = typemaster.getproperties().tolist();

//            if (!mapall)
//            {
//                clausetype = "or";
//            }


//            foreach (user user in listmaster)
//            {
//                count = 0;
//                object objuserid = null;

//                stringbuilder bldr = new stringbuilder();

//                foreach (attributemappingcolumns attdto in listattributes)
//                {

//                    ilist<propertyinfo> listprop = listproperty.where(prop => prop.name == attdto.dicolumn).tolist();

//                    if (listprop.count <= 0)
//                        break;

//                    propertyinfo property = listprop.first();

//                    if (property == null)
//                        break;

//                    object objuservalue = getpropvalue(property.name, user);

//                    objuserid = getpropvalue("userid", user);

//                    bldr.appendformat(string.format("{0}='{1}'", attdto.tgcolumn, objuservalue.tostring()));

//                    count++;

//                    if (count < listattributes.count)
//                        bldr.appendformat(" {0} ", clausetype);

//                }

//                drows = dtadsuser.select(bldr.tostring());

//                if (drows.count() > 0)
//                {
//                    drows[0]["diuserid"] = objuserid;
//                }


//            }

//            return dtadsuser;

//        }

//        private object getpropvalue(string name, object obj)
//        {

//            type type = obj.gettype();

//            propertyinfo info = type.getproperty(name);

//            if (info == null)
//                return null;


//            if (info.tostring().equals(string.empty))
//                return null;

//            obj = info.getvalue(obj, null);

//            if (info.propertytype == typeof(datetime))
//            {
//                if (obj == null || obj.tostring() == string.empty)
//                    return dbnull.value;
//            }

//            return obj;
//        }
//    }
//}
