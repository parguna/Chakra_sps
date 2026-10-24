using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class ExcelConnect
    {
        #region Read the CSV file 
        public DataTable ReadExcel(string fileName)
        {
            DataTable dtImport = new DataTable();
                string connectionString = GetConnectionString(fileName);

            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(connectionString))
                {
                    myConnection.Open();
                    using (OleDbDataAdapter myImportCommand = new OleDbDataAdapter
                    ("select * from [" + Path.GetFileName(fileName) + "]", myConnection))
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


        public   DataSet ConvertExcelToDataSet(string fileName)
        {
            DataSet ds = new DataSet();
            DataTable dtResult = null;int totalSheet;
            string connString = GetConnectionString(fileName);

            using (OleDbConnection objConn = new OleDbConnection(connString))
            {
                objConn.Open();
                OleDbCommand cmd = new OleDbCommand();
                OleDbDataAdapter oleda = new OleDbDataAdapter();

                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                List<string> sheetNames = new List<string>();
                if (dt != null)
                {
                    var tempDataTable = (from dataRow in dt.AsEnumerable()
                                         where !dataRow["TABLE_NAME"].ToString().Contains("FilterDatabase")
                                         select dataRow).CopyToDataTable();
                    dt = tempDataTable;
                    totalSheet = dt.Rows.Count;
                    sheetNames = dt.AsEnumerable().Select(dr => dr.Field<string>("TABLE_NAME")).ToList(); 
                }
                cmd.Connection = objConn;
                foreach (var s in sheetNames)
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM [" + s + "]";
                    oleda = new OleDbDataAdapter(cmd);
                    oleda.Fill(ds, s);
                    //dtResult = ds.Tables["s"];
                }

                objConn.Close();
                return ds; //Returning Dattable  
            }
        }

        #region Read header columns name from CSV file
        public List<string> GetHeaderColumnListFromExcel(string fileName)
        {
            List<string> xlColumnList = new List<string>();
            DataTable dt = new DataTable();
            
                DataTable dtImport = new DataTable();
                string connectionString = GetConnectionString(fileName);

            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(connectionString))
                {
                    myConnection.Open();
                    using (OleDbDataAdapter myImportCommand = new OleDbDataAdapter
                    ("select * from [" + Path.GetFileName(fileName) + "]", myConnection))
                        myImportCommand.Fill(dtImport);
                    myConnection.Close();
                }
            }
            catch (Exception ex)
            {
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

        #region CSV file Oldedb connection string
        private static readonly string[] AllowedExtensions = { ".csv", ".txt", ".xls", ".xlsx" };

        /// <summary>
        /// The only caller of this class (the XL bulk-upload feature - see XLUploadUC.ascx.cs)
        /// always saves the uploaded file under an "XLUploadFiles" folder beneath the web
        /// application root, and passes that full path straight through as fileName. This
        /// validates the path resolves (after canonicalization, so ".." segments can't be used
        /// to escape it) to somewhere inside a directory named "XLUploadFiles", which rules out
        /// UNC paths, absolute paths to unrelated locations, and traversal outside the upload
        /// area - not just a "contains .." string check.
        /// </summary>
        private const string RequiredContainingFolderName = "XLUploadFiles";

        private static void ValidateFileResource(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required.", nameof(fileName));

            if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || fileName.IndexOfAny(new[] { ';', '"' }) >= 0)
                throw new ArgumentException("File name contains invalid characters.", nameof(fileName));

            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension.ToLowerInvariant()))
                throw new ArgumentException("File type is not allowed.", nameof(fileName));

            if (fileName.StartsWith(@"\\") || fileName.StartsWith("//"))
                throw new ArgumentException("UNC paths are not allowed.", nameof(fileName));

            // Canonicalizes the path (resolves any "..") so the containment check below can't
            // be bypassed by traversal segments, and throws for malformed paths.
            string fullPath = Path.GetFullPath(fileName);

            bool isWithinAllowedFolder = fullPath
                .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .Any(segment => string.Equals(segment, RequiredContainingFolderName, StringComparison.OrdinalIgnoreCase));
            if (!isWithinAllowedFolder)
                throw new ArgumentException("File path must be located under the XLUploadFiles folder.", nameof(fileName));
        }

        private string GetConnectionString(string fileName)
        {
            ValidateFileResource(fileName);

            var directoryFullPath = Path.GetDirectoryName(fileName);
            var builder = new OleDbConnectionStringBuilder
            {
                Provider = "Microsoft.Jet.OLEDB.4.0"
            };
            builder["Data Source"] = directoryFullPath;
            builder["Extended Properties"] = "Text;HDR=YES;Text;FMT=Delimited(,)";

            return builder.ConnectionString;
        }
        #endregion
    }
}
