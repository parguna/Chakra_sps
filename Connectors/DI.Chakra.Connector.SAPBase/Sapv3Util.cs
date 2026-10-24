using DI.Chakra.Logger;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace DI.Chakra.Connector.SAPBase
{
    public class Sapv3Util
    {

        public string AppServer { get; set; }
        public string SystemNumber { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Client { get; set; }
        public string DestinationName { get; set; }
        public int RowCount { get; set; }
        public int RowSkips { get; set; }

        private RfcDestination m_destination = null;
        public void ResetPaging()
        {
            RowSkips = 0;
        }
        public void NextPage()
        {
            this.RowSkips = this.RowSkips + this.RowCount;
        }

        public bool SNCMode { get; set; }
        public string SNCLib { get; set; }
        public string SNCQop { get; set; }
        public string SNCPartnerName { get; set; }

        void WriteDebugStep(string message)
        {
            DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.DebugStep, message);
        }
        void WriteInfoStep(string message)
        {
            DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Info, message);
        }
        public RfcDestination RfcDestination
        {
            get
            {

                if (m_destination == null)
                    m_destination = GetSAPDestination();

                return m_destination;
            }
        }

        public RfcDestination GetSAPDestination()
        {


            RfcConfigParameters configParameters = new RfcConfigParameters();
            RfcDestination rfcSAPDestination = null;

            //if (configParameters.Count == 0)
            {

                configParameters.Add(RfcConfigParameters.AppServerHost, AppServer);
                configParameters.Add(RfcConfigParameters.SystemNumber, SystemNumber);
                configParameters.Add(RfcConfigParameters.User, UserId);
                configParameters.Add(RfcConfigParameters.Password, Password);
                configParameters.Add(RfcConfigParameters.Client, Client);
                configParameters.Add(RfcConfigParameters.Language, "EN");
                configParameters.Add(RfcConfigParameters.PoolSize, "5");
                configParameters.Add(RfcConfigParameters.Name, DestinationName);

                if (SNCMode)
                {
                    configParameters.Add(RfcConfigParameters.SncMode, "1");
                    configParameters.Add(RfcConfigParameters.SncQOP, SNCQop);
                    configParameters.Add(RfcConfigParameters.SncLibraryPath, SNCLib);
                    configParameters.Add(RfcConfigParameters.SncPartnerName, SNCPartnerName);
                }

                rfcSAPDestination = RfcDestinationManager.GetDestination(configParameters);

            }

            return rfcSAPDestination;
        }
        public bool Connect()
        {
            RfcDestination rfcDestination;
            try
            {
                rfcDestination = this.RfcDestination;
                WriteInfoStep($"SAPv3Util: Trying to connect to SAP Server: RFCDestination, AppServer:{rfcDestination?.AppServerHost}, " +
                 $"SystemNumber: {rfcDestination?.SystemNumber}, ClientNumber: {rfcDestination?.Client}");
                if (rfcDestination != null)
                {
                    rfcDestination.Ping();
                    return true;
                }
            }
            catch (RfcCommunicationException ex)
            {
                WriteInfoStep($"SAPv3Util: RfcCommunicationException :{ex.Message} ");
                throw ex;
            }
            catch (Exception ex)
            {
                if (ex != null && ex.Message != null && ex.Message.Contains("Function module"))
                {
                    try
                    {
                        WriteDebugStep("Re-initiating the ping");
                        rfcDestination = this.RfcDestination;
                        rfcDestination.Ping();
                        return true;
                    }
                    catch (Exception ex1)
                    {
                        WriteDebugStep($"SAPv3Util: Exception - Ping :{ex.Message} ");
                        throw ex1;
                    }
                }
                WriteDebugStep($"SAPv3Util: Exception :{ex.Message} ");
                throw ex;
            }

            return false;
        }
        public DataTable ExecuteTable(IRfcFunction fn, string tableName)
        {
            IRfcTable returnTable = fn.GetTable(tableName);
            fn.Invoke(this.RfcDestination);

            DataTable dtResult = returnTable.ToDataTable(tableName);

            return dtResult;
        }
        public DataTable ExecuteTable(string functionName, Dictionary<string, object> dicParams, int index)
        {
            IRfcFunction fn = getRfcFunction(functionName, dicParams);
            IRfcTable returnTable = fn[index].GetTable(); ;
            DataTable dtResult = returnTable.ToDataTable("USER_LIST");

            return dtResult;
        }
        public DataTable ExecuteTable(string functionName, Dictionary<string, object> dicParams, string tableName)
        {

            IRfcFunction fn = getRfcFunction(functionName, dicParams);

            return ExecuteTable(fn, tableName);


        }
        public IRfcStructure ExecuteStructure(IRfcFunction fn, string structName)
        {
            fn.Invoke(this.RfcDestination);
            IRfcStructure returnStruct = fn.GetStructure(structName);

            return returnStruct;
        }

        public void DeleteAllRowsInTable(IRfcFunction fn, IRfcTable tbl, string tblName)
        {
            int cnt = tbl.Count;

            for (int i = 0; i < cnt; i++)
            {
                tbl.Delete();
            }
            fn.SetValue(tblName, tbl);

            ExecuteTable(fn, tblName);
        }
        public IRfcFunction getRfcFunction(string functionName, Dictionary<string, object> dicParams)
        {
            IRfcFunction rfcFunction;
            StringBuilder sb = new StringBuilder();

            var rfcDestination = this.RfcDestination;
            RfcRepository bapiRepository = rfcDestination.Repository;
            rfcFunction = bapiRepository.CreateFunction(functionName);

            try
            {
                foreach (var item in dicParams)
                {
                    sb.Append($"Key: {item.Key} | Value {item.Value},");

                    if (item.Value != null && item.Key != null)
                        rfcFunction.SetValue(item.Key, item.Value);
                }
                int reTryRFCCall = 2;
                while(reTryRFCCall > 0)
                {
                    try
                    {
                        rfcFunction.Invoke(rfcDestination);
                        reTryRFCCall = 0;
                    }
                    catch (Exception ex)
                    {
                        if(reTryRFCCall == 1)
                        {
                            DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Exception occured during getRfcFunction {functionName}, params {sb.ToString().TrimEnd(',')}", ex);
                            throw ex;
                        }
                    }
                    finally
                    {
                        reTryRFCCall = reTryRFCCall - 1;
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, $"Exception occured during getRfcFunction {functionName}, params {sb.ToString().TrimEnd(',')}", ex);
                throw ex;
            }
            finally
            {
                sb.Length = 0;
            }
            
            return rfcFunction;
        }
        public IRfcStructure getRfcStructure(string functionName, Dictionary<string, object> dicParams, string structureName)
        {

            var rfcFunction = getRfcFunction(functionName, dicParams);

            if (rfcFunction == null)
                return null;

            var rfcStructure = rfcFunction.GetStructure(structureName);


            return rfcStructure;
        }
        public IRfcTable getRfcTable(string functionName, Dictionary<string, object> dicParams, string tableName)
        {

            var rfcFunction = getRfcFunction(functionName, dicParams);

            if (rfcFunction == null)
                return null;

            var rfcTable = rfcFunction.GetTable(tableName);


            return rfcTable;
        }

        public DataTable getTableFromRfcFunction(string functionName, Dictionary<string, object> dicParams, string tableName)
        {


            var rfcTable = getRfcTable(functionName, dicParams, tableName);

            var dtTable = rfcTable.ToDataTable(tableName);

            return dtTable;
        }
        public DataTable ReadRFCTblAsFunction(string tableName, string delimiter = null, List<string> fields = null, string options = null, bool enablePaging = true)
        {

            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            DataTable dtReturn = new DataTable();
            DataTable dtData = new DataTable();
            DataTable dtFields = new DataTable();
            int rowCount = RowCount;
            int rowSkips = RowSkips;

            var rfcDestination = this.RfcDestination;

            RfcRepository bapiRepository = rfcDestination.Repository;

            if (delimiter == null)
                delimiter = "|";

            //do
            //{

            readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
            readTableFunction.SetValue("QUERY_TABLE", tableName);
            readTableFunction.SetValue("DELIMITER", delimiter);
            fieldTable = readTableFunction.GetTable("FIELDS");

            if (enablePaging == true)
            {
                readTableFunction.SetValue("ROWSKIPS", rowSkips);
                readTableFunction.SetValue("ROWCOUNT", rowCount);
            }


            if (fields != null)
            {
                foreach (var field in fields)
                {
                    fieldTable.Append();
                    fieldTable.SetValue("FIELDNAME", field);
                }
            }

            if (options != null)
            {
                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", options);

            }

            readTableFunction.Invoke(rfcDestination);

            readTable = readTableFunction.GetTable("DATA");



            dtData = readTable.ToDataTable(tableName);
            dtFields = fieldTable.ToDataTable("FIELDS");
            dtData = convertPipesToColumnsUsers(dtFields, dtData);

            //dtReturn.Merge(dtData);

            //rowSkips = rowSkips + rowCount;

            //} while (dtData.Rows.Count > 0);

            return dtData;
        }
        //public DataTable ReadRFCTblAsFunction1(string tableName, string delimiter = null, List<string> fields = null, string options = null)
        //{

        //    IRfcFunction readTableFunction;
        //    IRfcTable fieldTable;
        //    IRfcTable optionsTable;
        //    IRfcTable readTable;
        //    DataTable dtReturn = new DataTable();
        //    DataTable dtData = new DataTable();
        //    DataTable dtFields = new DataTable();
        //    int rowCount = RowCount;
        //    int rowSkips = 0;

        //    var rfcDestination = this.RfcDestination;

        //    RfcRepository bapiRepository = rfcDestination.Repository;

        //    if (delimiter == null)
        //        delimiter = "|";

        //    do
        //    {

        //        readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
        //        readTableFunction.SetValue("QUERY_TABLE", tableName);
        //        readTableFunction.SetValue("DELIMITER", delimiter);
        //        readTableFunction.SetValue("ROWSKIPS", rowSkips);
        //        readTableFunction.SetValue("ROWCOUNT", rowCount);
        //        fieldTable = readTableFunction.GetTable("FIELDS");


        //        if (fields != null)
        //        {
        //            fieldTable.Append();
        //            foreach (var field in fields)
        //            {
        //                fieldTable.SetValue("FIELDNAME", field);
        //            }
        //        }

        //        if (options != null)
        //        {
        //            optionsTable = readTableFunction.GetTable("OPTIONS");
        //            optionsTable.Append();
        //            optionsTable.SetValue("TEXT", options);

        //        }

        //        readTableFunction.Invoke(rfcDestination);

        //        readTable = readTableFunction.GetTable("DATA");



        //        dtData = readTable.ToDataTable(tableName);
        //        dtFields = fieldTable.ToDataTable("FIELDS");
        //        dtData = convertPipesToColumnsUsers(dtFields, dtData);

        //        dtReturn.Merge(dtData);

        //        rowSkips = rowSkips + rowCount;

        //    } while (dtData.Rows.Count > 0);

        //    return dtReturn;
        //}

        public DataTable convertPipesToColumnsUsers(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtReturn = new DataTable();
            string fullRow;
            string[] SplitRows;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return null;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dtReturn.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullRow = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullRow.IndexOf("|J3G|") < 0)
                    {
                        SplitRows = fullRow.Split('|');
                        if (SplitRows.Length <= dtReturn.Columns.Count)
                            dtReturn.Rows.Add(SplitRows);
                    }
                }
                return dtReturn;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DateTime getLastLogin(IRfcStructure sLogonData, IRfcFunction fnLastLogin)
        {
            string strLastLogin, hh, mm, ss;
            DateTime? validto = null;
            try
            {
                validto = DateTime.ParseExact(sLogonData.GetValue("GLTGB").ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            { }

            DateTime? fromdate = null;
            try
            {
                fromdate = DateTime.ParseExact(sLogonData.GetValue("Gltgv").ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            { }
            var lastLogin = fnLastLogin.GetValue("LAST_LOGON_DATE").ToString();

            hh = sLogonData.GetString("LTime").Substring(0, 2);
            mm = sLogonData.GetString("LTime").Substring(3, 2);
            ss = sLogonData.GetString("LTime").Substring(6, 2);
            string year = lastLogin.Substring(0, 4);
            string month = lastLogin.Substring(5, 2);
            string day = lastLogin.Substring(8, 2);
            if (year == "0000")
            {
                strLastLogin = "";
            }
            else
            {
                strLastLogin = year + "/" + month + "/" + day + " " + hh.ToString() + ":" + mm.ToString() + ":" + ss.ToString();
            }

            try
            {
                return Convert.ToDateTime(strLastLogin);
            }
            catch { }

            return DateTime.Now;
        }

        public DateTime? getDateTime(string strDate)
        {

            try
            {
                return DateTime.ParseExact(strDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }

        }
        public DateTime? getDateTimeWithoutHyphen(string strDate)
        {
            try
            {
                return DateTime.ParseExact(strDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
