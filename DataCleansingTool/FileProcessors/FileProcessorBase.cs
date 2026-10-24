using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataCleansingTool.FileProcessors
{
    public abstract class FileProcessorBase
    {
        public const string USER_UPLOAD_FILE_COLUMNS = "ADLogin,Pernr,IdentificationNo,KnownAs,ReferenceUser,ReferenceUserPernr,JoinDate,ValidTill,EmployeeType";
        public const string MAP_USER_UPLOAD_FILE_COLUMNS = "EndpointKey,OwnerPernr,Service,Shared,Primary,Secondary";
        public const string MAP_INHOUSE_USER_UPLOAD_FILE_COLUMNS = "EndpointKey,SystemCode,OwnerPernr,Service,Shared,Primary,Secondary";
        public const string MAP_APPROVER_EXCEPTION_UPLOAD_FILE_COLUMNS = "Person,Approver,ApprovalLevel";

        public string UploadLog { get; set; }

        public EndpointType EndpointTypes { get; set; }

        public bool IsApproverException { get; set; }

        public abstract void UploadData(string fileName);

        protected bool IsValidFile(string fileName, ref string _validationMsg)
        {
            List<String> ColumnList = new ExcelConnect().GetHeaderColumnListFromExcel(fileName);
            List<string> actualColumnList = new List<string>();

            if (this.IsApproverException)
            {
                actualColumnList = MAP_APPROVER_EXCEPTION_UPLOAD_FILE_COLUMNS.Split(',').ToList();
            }
            else
            {
                if (this.EndpointTypes == EndpointType.None)
                {
                    actualColumnList = USER_UPLOAD_FILE_COLUMNS.Split(',').ToList();
                }
                else if (this.EndpointTypes == EndpointType.InhouseSystems)
                {
                    actualColumnList = MAP_INHOUSE_USER_UPLOAD_FILE_COLUMNS.Split(',').ToList();
                }
                else
                {
                    actualColumnList = MAP_USER_UPLOAD_FILE_COLUMNS.Split(',').ToList();
                }
            }

            if (!ColumnList.SequenceEqual(actualColumnList))
            {
                _validationMsg += "Invalid excel file. Columns mismatched." + "\n";
                _validationMsg += "Column List - " + string.Join(",", ColumnList.Select(item => item).ToList()) + "\n";
                _validationMsg += "Actual Column List - " + string.Join(",", actualColumnList.Select(item => item).ToList()) + "\n";
                return false;
            }
            return true;
        }

        protected string CheckNullAndGetValueFromCell(string cellText)
        {
            cellText = cellText.Trim();
            if (string.Compare(cellText, "NULL", true) != 0)
                return cellText;
            else
                return string.Empty;
        }

        protected UserXLData GetUserObjectFromData(DataRow row)
        {
            try
            {
                UserXLData user = new UserXLData();

                user.ADLogin = CheckNullAndGetValueFromCell(row.ItemArray[0].ToString());
                user.Pernr = CheckNullAndGetValueFromCell(row.ItemArray[1].ToString());
                user.IdentificationNo = CheckNullAndGetValueFromCell(row.ItemArray[2].ToString());
                user.KnownAs = CheckNullAndGetValueFromCell(row.ItemArray[3].ToString());
                user.ReferenceUser = CheckNullAndGetValueFromCell(row.ItemArray[4].ToString());
                user.ReferenceUserPernr = CheckNullAndGetValueFromCell(row.ItemArray[5].ToString());
                user.JoinDate = CheckNullAndGetValueFromCell(row.ItemArray[6].ToString());
                user.TerminationDate = CheckNullAndGetValueFromCell(row.ItemArray[7].ToString());
                user.EmployeeType = CheckNullAndGetValueFromCell(row.ItemArray[8].ToString());

                return user;
            }
            catch (Exception ex)
            {
                this.UploadLog += string.Format("Error while reading a row in excel. Error detail - {0}", ex.Message) + "\n";
                return null;
            }
        }

        protected ApproverExceptionData GetApproverExceptionObjectFromData(DataRow row)
        {
            try
            {
                ApproverExceptionData exception = new ApproverExceptionData();

                exception.Person = CheckNullAndGetValueFromCell(row.ItemArray[0].ToString());
                exception.Approver = CheckNullAndGetValueFromCell(row.ItemArray[1].ToString());
                exception.ApprovalLevel = Convert.ToInt32(CheckNullAndGetValueFromCell(row.ItemArray[2].ToString()));

                return exception;
            }
            catch (Exception ex)
            {
                this.UploadLog += string.Format("Error while reading a row in excel. Error detail - {0}", ex.Message) + "\n";
                return null;
            }
        }

        protected UserMappingXLData GetMappingObjectFromData(DataRow row)
        {
            try
            {
                UserMappingXLData mappingXlData = new UserMappingXLData();

                mappingXlData.EndpointKey = CheckNullAndGetValueFromCell(row.ItemArray[0].ToString());
                mappingXlData.OwnerPernr = CheckNullAndGetValueFromCell(row.ItemArray[1].ToString());

                mappingXlData.AccountType = (AccountType)Convert.ToByte(CheckNullAndGetValueFromCell(row.ItemArray[2].ToString()));

                return mappingXlData;
            }
            catch (Exception ex)
            {
                this.UploadLog += string.Format("Error while reading a row in excel. Error detail - {0}", ex.Message) + "\n";
                return null;
            }
        }

        protected InhouseUserMappingXLData GetInhouseMappingObjectFromData(DataRow row)
        {
            try
            {
                InhouseUserMappingXLData mappingXlData = new InhouseUserMappingXLData();

                mappingXlData.EndpointKey = CheckNullAndGetValueFromCell(row.ItemArray[0].ToString());
                mappingXlData.SystemCode = CheckNullAndGetValueFromCell(row.ItemArray[1].ToString());
                mappingXlData.OwnerPernr = CheckNullAndGetValueFromCell(row.ItemArray[2].ToString());
                mappingXlData.AccountType = (AccountType)Convert.ToByte(CheckNullAndGetValueFromCell(row.ItemArray[3].ToString()));
                
                return mappingXlData;
            }
            catch (Exception ex)
            {
                this.UploadLog += string.Format("Error while reading a row in excel. Error detail - {0}", ex.Message) + "\n";
                return null;
            }
        }

    }
}
