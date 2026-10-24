using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace DI.Chakra.Infrastructure
{
    public class CollectionHelper
    {
        private CollectionHelper()
        {
        }
        public static DataTable ConvertToUserInfo<T>(IList<T> list, DataTable table)
        {

            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);


            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop1 in properties)
                {
                    if ((string.Compare(prop1.Name, "UserId", true) != 0) && (string.Compare(prop1.Name, "DIUserId", true) != 0))
                    //if (prop1.Name != "DIUserId")
                    {
                        if (prop1.PropertyType.Namespace == "System")
                        {
                            if (null == prop1.GetValue(item))
                                row[prop1.Name] = DBNull.Value;
                            else
                                row[prop1.Name] = prop1.GetValue(item);
                        }
                        else
                        {

                            Type entityType1 = prop1.PropertyType;
                            PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

                            object obj = item.GetType().GetProperty(prop1.Name).GetValue(item, null);

                            foreach (PropertyDescriptor prop11 in properties1)
                            {
                                if (prop11.GetValue(obj) == null)
                                    row[string.Format("{0}.{1}", prop1.Name, prop11.Name)] = DBNull.Value;
                                else
                                    row[string.Format("{0}.{1}", prop1.Name, prop11.Name)] = prop11.GetValue(obj);
                            }

                        }
                    }

                }
                table.Rows.Add(row);
            }




            return table;
        }
        public static DataTable ConvertTo<T>(IList<T> list, DataTable table)
        {

            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            TableInfoAttribute tableInfo = null;
            object[] attrs = entityType.GetCustomAttributes(typeof(TableInfoAttribute), false); // Get collection of custom attributes from the method
            foreach (Attribute attr in attrs)
            {
                if (attr is TableInfoAttribute)
                {
                    tableInfo = (TableInfoAttribute)attr;
                }
            }

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop1 in properties)
                {
                    if (prop1.PropertyType.Namespace == "System")
                    {
                        if (null == prop1.GetValue(item))
                            row[string.Format("{0}.{1}", tableInfo.FriendlyName, prop1.Name)] = DBNull.Value;
                        else
                            row[string.Format("{0}.{1}", tableInfo.FriendlyName, prop1.Name)] = prop1.GetValue(item);
                    }
                    else
                    {

                        Type entityType1 = prop1.PropertyType;
                        PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);


                        TableInfoAttribute tableInfo1 = null;
                        object[] attrs1 = entityType1.GetCustomAttributes(typeof(TableInfoAttribute), false); // Get collection of custom attributes from the method
                        foreach (Attribute attr in attrs1)
                        {
                            if (attr is TableInfoAttribute)
                            {
                                tableInfo1 = (TableInfoAttribute)attr;
                            }
                        }

                        if (tableInfo1 != null)
                        {
                            object obj = null;
                            try
                            {
                                obj = item.GetType().GetProperty(prop1.Name).GetValue(item, null);
                            }
                            catch (Exception) { }

                            foreach (PropertyDescriptor prop11 in properties1)
                            {
                                if (prop11.GetValue(obj) == null)
                                    row[string.Format("{0}.{1}", tableInfo1.FriendlyName, prop11.Name)] = DBNull.Value;
                                else if (obj != null)
                                    row[string.Format("{0}.{1}", tableInfo1.FriendlyName, prop11.Name)] = prop11.GetValue(obj);
                            }
                        }

                    }

                }
                table.Rows.Add(row);
            }




            return table;
        }
        public static DataTable ConvertToNormal<T>(IList<T> list, DataTable table)
        {
            try
            {



                Type entityType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);



                foreach (T item in list)
                {
                    DataRow row = table.NewRow();

                    foreach (PropertyDescriptor prop1 in properties)
                    {
                        if (prop1.PropertyType.Namespace == "System")
                        {
                            if (null == prop1.GetValue(item))
                                row[string.Format("{0}", prop1.Name)] = DBNull.Value;
                            else
                                row[string.Format("{0}", prop1.Name)] = prop1.GetValue(item);
                        }
                        else
                        {

                            Type entityType1 = prop1.PropertyType;
                            PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);
                            if (properties1.Count > 0)
                            {
                                object obj = null;
                                try
                                {
                                    obj = item.GetType().GetProperty(prop1.Name).GetValue(item, null);
                                }
                                catch (Exception ex) { }
                                if (obj != null)
                                {
                                    foreach (PropertyDescriptor prop11 in properties1)
                                    {

                                        try
                                        {
                                            if (prop11.GetValue(obj) == null)
                                                row[string.Format("{0}.{1}", prop1.Name, prop11.Name)] = DBNull.Value;
                                            else
                                                row[string.Format("{0}.{1}", prop1.Name, prop11.Name)] = prop11.GetValue(obj);
                                        }
                                        catch (Exception) { }

                                    }
                                }
                            }
                        }
                    }
                    table.Rows.Add(row);
                }

            }
            catch (Exception ex)
            {

                throw;
            }


            return table;
        }
        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();

            return ConvertTo<T>(list, table);


        }

        public static DataTable ConvertToNormalTable<T>(IList<T> list)
        {
            DataTable table = CreateTableNormal<T>();

            return ConvertToNormal<T>(list, table);


        }


        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static T CreateItem<T>(DataRow row)
        {
            try
            {
                T obj = default(T);
                if (row != null)
                {
                    obj = Activator.CreateInstance<T>();

                    foreach (DataColumn column in row.Table.Columns)
                    {
                        PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);


                        try
                        {
                            if (prop != null)
                            {
                                object value = row[column.ColumnName];
                                //TODO : Check this fix with wasim
                                if (string.Compare(column.ColumnName, "GUID", true) == 0 || string.Compare(column.ColumnName, "OrgUnitGUID", true) == 0 || string.Compare(column.ColumnName, "MSrepl_tran_version", true) == 0)
                                {
                                    if (value == DBNull.Value)
                                        prop.SetValue(obj, null, null);
                                    else
                                        prop.SetValue(obj, value.ToString(), null);
                                }
                                else
                                {
                                    if (value == DBNull.Value)
                                        prop.SetValue(obj, null, null);
                                    else
                                        prop.SetValue(obj, value, null);
                                }
                            }
                        }
                        catch
                        {
                            // You can log something here
                            throw;
                        }
                    }
                }

                return obj;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            TableInfoAttribute tableInfo = null;

            object[] attrs = entityType.GetCustomAttributes(typeof(TableInfoAttribute),false); // Get collection of custom attributes from the method

            string strColumnChecker = string.Empty;

            foreach (Attribute attr in attrs) 

            {
                if (attr is TableInfoAttribute)
                {
                    tableInfo = (TableInfoAttribute)attr;
                }
            }

            DataTable table = null;


            table = new DataTable(entityType.Name);


            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop1 in properties)
            {
                if (prop1.PropertyType.Namespace == "System")
                {
                    if (tableInfo != null)
                    {
                        strColumnChecker = string.Format("{0}.{1}", tableInfo.FriendlyName, prop1.Name);
                        if (table.Columns.Contains(strColumnChecker) == false)
                            table.Columns.Add(strColumnChecker, Nullable.GetUnderlyingType(prop1.PropertyType) ?? prop1.PropertyType);
                    }
                }
                else
                {

                    Type entityType1 = prop1.PropertyType;
                    TableInfoAttribute tableInfo1 = null;
                    object[] attrs1 = entityType1.GetCustomAttributes(typeof(TableInfoAttribute), false); // Get collection of custom attributes from the method
                    foreach (Attribute attr in attrs1)
                    {
                        if (attr is TableInfoAttribute)
                        {
                            tableInfo1 = (TableInfoAttribute)attr;
                        }
                    }
                    if (tableInfo1 != null)
                    {

                        PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

                        foreach (PropertyDescriptor prop11 in properties1)
                        {
                            strColumnChecker = string.Format("{0}.{1}", tableInfo1.FriendlyName, prop11.Name);
                            if (table.Columns.Contains(strColumnChecker) == false)
                                table.Columns.Add(strColumnChecker, Nullable.GetUnderlyingType(prop11.PropertyType) ?? prop11.PropertyType);
                        }
                    }

                }
            }

            return table;
        }


        public static DataTable CreateTableNormal<T>()
        {

            try
            {


                Type entityType = typeof(T);
                //TableInfoAttribute tableInfo = null;
                //object[] attrs = entityType.GetCustomAttributes(typeof(TableInfoAttribute), false); // Get collection of custom attributes from the method
                //foreach (Attribute attr in attrs)
                //{
                //    if (attr is TableInfoAttribute)
                //    {
                //        tableInfo = (TableInfoAttribute)attr;
                //    }
                //}

                DataTable table = null;


                table = new DataTable(entityType.Name);


                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

                foreach (PropertyDescriptor prop1 in properties)
                {
                    if (prop1.PropertyType.Namespace == "System")
                    {
                        table.Columns.Add(string.Format("{0}", prop1.Name), Nullable.GetUnderlyingType(prop1.PropertyType) ?? prop1.PropertyType);
                    }
                    else
                    {

                        Type entityType1 = prop1.PropertyType;
                        PropertyDescriptorCollection properties1 = TypeDescriptor.GetProperties(entityType1);

                        if (properties1 != null && properties1.Count > 0)
                        {
                            foreach (PropertyDescriptor prop11 in properties1)
                            {
                                table.Columns.Add(string.Format("{0}.{1}", prop1.Name, prop11.Name), Nullable.GetUnderlyingType(prop11.PropertyType) ?? prop11.PropertyType);
                            }
                        }
                    }
                }

                return table;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GetConcatenatedStringFromList<T>(IList<T> sourceList, string propertyName)
        {
            return string.Join(", ", from item in sourceList select item.GetType().GetProperty(propertyName).GetValue(item));
        }

        //public static DataSet CreateDataSet<T>()
        //{
        //    DataSet dset = new DataSet();

        //    Type entityType = typeof(T);


        //    DataTable table = new DataTable(entityType.Name);

        //    DataTable dtable = CreateTable<T>();

        //    dset.Tables.Add(dtable);
        //    DataTable dtt;
        //    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

        //    foreach (PropertyDescriptor prop1 in properties)
        //    {
        //        if (!prop1.PropertyType.IsPrimitive)
        //        {
        //            Type ent = prop1.PropertyType;
        //            dtt = CreateTable<T>();
        //        }
        //    }

        //    return dset;
        //}
    }

}
