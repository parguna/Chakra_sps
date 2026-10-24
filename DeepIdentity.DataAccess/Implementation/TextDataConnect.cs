using DeepIdentity.Logger;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess
{
    public class TextDataConnect
    {
        #region CSV file Oldedb connection string
        private string GetConnectionString(string fileName)
        {

            var directoryFullPath = Path.GetDirectoryName(fileName);
            string connStr = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + directoryFullPath + ";Extended Properties=\"Text;HDR=YES;Text;FMT=Delimited(,)\"";
            return connStr;

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

        #region Read CSV file with Full text from all rows
        public DataTable ReadCSV(string filename) 
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(filename))
                {
                    csvReader.SetDelimiters(",");
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "GetCSVWithFullText", ex.Message, string.Empty, 0, 0, "Ssyte");
            }
            return csvData;
        }
        #endregion
    }
}
