using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Web.Comp.Bulk
{
    public static class Common
    {       
        #region Public Utility Functions
        /// <summary>
        /// Function to parse csv files 
        /// </summary>
        /// <param name="fileName">Name of the CSV File</param>
        /// <param name="readFirstLine">Flag to read the first line</param>
        /// <returns>A list of string arrays</returns>
        public static List<string[]> ReadTheCSVFile(string fileName, bool readFirstLine)
        {
            List<string[]> stringRows = new List<string[]>();
            bool firstLine = true;
            try
            {
                Microsoft.VisualBasic.FileIO.TextFieldParser myParser = new TextFieldParser(fileName);
                myParser.TextFieldType = FieldType.Delimited;
                myParser.SetDelimiters(",");
                string[] stringRow;
                while (!myParser.EndOfData)
                {
                    stringRow = myParser.ReadFields();
                    if (firstLine && readFirstLine)
                    {
                        stringRows.Add(stringRow);
                        firstLine = false;
                        break;
                    }
                    else if(!firstLine && !readFirstLine)
                    {
                        stringRows.Add(stringRow);
                    }
                    firstLine = false;
                }
                myParser.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return stringRows;
        }

        /// <summary>
        /// Method to check if the uploaded file is in the given template
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool CompareFileHeaders(string file1, string file2)
        {
            bool status = false;
            List<string[]> headerfile1 = ReadTheCSVFile( file1, true);
            List<string[]> headerfile2 = ReadTheCSVFile(file2, true);
            if (headerfile1[0].Count() == headerfile2[0].Count())
            {
                for (int i = 0; i <=headerfile2[0].Count() - 1; i++)
                {
                    if (string.Compare(headerfile2[0][i], headerfile1[0][i], true) != 0)
                    {
                        status = false;
                        break;
                    }
                    else status = true;
                }
            }
            return status;
        }

        /// <summary>
        /// Method to format the error strings
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static string FormatErrorMessage(string ErrorMessage)
        {
            if(ErrorMessage!=null && ErrorMessage.Contains(","))
            {
                if (ErrorMessage.EndsWith(","))
                    ErrorMessage = ErrorMessage.Remove((ErrorMessage.Length - 1), 1);
                if (ErrorMessage.Contains(","))
                {
                    int lastcommaposition = ErrorMessage.LastIndexOf(",");
                    ErrorMessage = ErrorMessage.Remove(lastcommaposition, 1);
                    ErrorMessage = ErrorMessage.Insert(lastcommaposition, " and ");
                }
            }
            return ErrorMessage;
        }

        /// <summary>
        /// Method to build the error messages
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="rownos"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string BuildErrorMessage(string codes, string rownos,int num)
        {
            string error = null;
            codes = FormatErrorMessage(codes);
            rownos =FormatErrorMessage(rownos);
            switch (num)
            {
                case 0:
                    error = string.Format("Duplicate entry/ies found in line no {0}.", rownos);
                    break;
                case 1:
                    error = string.Format("The Function/s {0} correspondingly found in line no/s {1} already exists", codes, rownos);
                    break;
                case 2:
                    error = string.Format("The Function/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 3:
                    error = string.Format("The Endpoint/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 4:
                    error = string.Format("The Function Item/s found in line no/s {0} already exists in the  corresponding function/s {1}", rownos, codes);
                    break;
                case 5:
                    error = string.Format("The Function Value/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 6:
                    error = string.Format("The Object Code/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 7:
                    error = string.Format("The Field Code/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 8:
                    error = string.Format("The Exception/s {0} correspondingly found in line no/s {1} already exists", codes, rownos);
                    break;
                case 9:
                    error = string.Format("The Risk Owner/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 10:
                    error = string.Format("The Exception/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 11:
                    error = string.Format("The User/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 12:
                    error = string.Format("The Function Item/s {0} correspondingly found in line no/s {1} doesn't exist", codes, rownos);
                    break;
                case 13:
                    error = string.Format("The Exception Item/s found in line no/s {0} already exists in the corresponding Exception/s {1}", rownos, codes);
                    break;
                case 14:
                    error = string.Format("The Function Code/s {0} correspondingly found in line no/s {1} has special characters other than _ and ;",codes,rownos);
                    break;
                case 15:
                    error = string.Format("The Exception Code/s {0} correspondingly found in line no/s {1} has special characters other than _ and ;", codes, rownos);
                    break;
            }
            return error;
        }

        /// <summary>
        /// Method to get the errors 
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="rowNos"></param>
        /// <returns></returns>
        public static List<string> GetErrorsToUpload( string[] codes, string[] rowNos)
        {
            string error = null;
            List<string> errorsWhileUploading = new List<string>(6);
            for (int i = 0; i <= (rowNos.Count() - 1); i++)
            {
                if(rowNos[i] !=null)
                {
                    error = BuildErrorMessage(codes[i], rowNos[i], i);
                    if (error !=null) errorsWhileUploading.Add(error);
                }
            }
            return errorsWhileUploading;
        }

        /// <summary>
        /// Method to validate the code field for special characters
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckCode(string code)
        {
            var regexItem = new Regex("^[a-zA-Z0-9_;]+$");
            if (regexItem.IsMatch(code))    
                return true;
            else
                return false;
        }
        #endregion
    }
}