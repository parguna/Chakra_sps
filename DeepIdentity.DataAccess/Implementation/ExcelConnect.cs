using DeepIdentity.Logger;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    
    public class ExcelConnect
    {
        #region Read the CSV file 
        public DataTable ReadExcel(string fileName)
        {
            DataTable dtImport = new DataTable();
            try
            {
                string connectionString = GetConnectionString(fileName);

                using (OleDbConnection myConnection = new OleDbConnection(connectionString))
                {
                    myConnection.Open();
                    using (OleDbDataAdapter myImportCommand = new OleDbDataAdapter("select * from [" + Path.GetFileName(fileName) + "]", myConnection))
                        myImportCommand.Fill(dtImport);
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return dtImport;
        }
        #endregion

        #region Read header columns name from CSV file
        public List<string> GetHeaderColumnListFromExcel(string fileName)
        {
            List<string> xlColumnList = new List<string>();
            DataTable dt = new DataTable();
            try
            {
                DataTable dtImport = new DataTable();
                string connectionString = GetConnectionString(fileName);

                using (OleDbConnection myConnection = new OleDbConnection(connectionString))
                {
                    myConnection.Open();
                    using (OleDbDataAdapter myImportCommand = new OleDbDataAdapter("select * from [" + Path.GetFileName(fileName) + "]", myConnection))
                        myImportCommand.Fill(dtImport);
                    myConnection.Close();
                }

                foreach (DataColumn column in dtImport.Columns)
                {
                    string ColumnName = column.Caption;
                    ColumnName = string.Join("", ColumnName.Where(c => !char.IsWhiteSpace(c)));
                    xlColumnList.Add(ColumnName.Trim());
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error", ex.Message, string.Empty, 0, 0, "Ssyte");
            }
            return xlColumnList;
        }
        #endregion

        #region CSV file Oldedb connection string
        private string GetConnectionString(string fileName)
        {
       
            var directoryFullPath = Path.GetDirectoryName(fileName);
            string connStr = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + directoryFullPath + ";Extended Properties=\"Text;HDR=YES;Text;FMT=Delimited(,)\"";
            return connStr;

        }
        #endregion

       
    }
}