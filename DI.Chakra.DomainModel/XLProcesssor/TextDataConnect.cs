using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DI.Chakra.DomainModel
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

            return xlColumnList;
        }
        #endregion

        #region Read CSV file with Full text from all rows
        public DataTable ReadCSV(string filename)
        {
            DataTable csvData = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                parser.HasFieldsEnclosedInQuotes = true;
                string[] colFields = parser.ReadFields();

                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }

                while (!parser.EndOfData)
                {
                    string[] fieldData = parser.ReadFields();

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

            return csvData;
        }
        public DataTable ReadCSV(string filename, int colLength)
        {
            DataTable csvData = new DataTable();

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                parser.HasFieldsEnclosedInQuotes = true;

                for (int i = 0; i < colLength; i++)
                {
                    DataColumn datecolumn = new DataColumn(i.ToString());
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }

                while (!parser.EndOfData)
                {
                    string[] fieldData = parser.ReadFields();
                    string[] rowdData = new string[colLength];

                    for (int i = 0; i < colLength; i++)
                    {
                        rowdData[i] = !string.IsNullOrEmpty(fieldData[i]) ? fieldData[i] : null;
                    }
                    csvData.Rows.Add(rowdData);
                }
            }

            return csvData;
        }
        #endregion
    }
}
