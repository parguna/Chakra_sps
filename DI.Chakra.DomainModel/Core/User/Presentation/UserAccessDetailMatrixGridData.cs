using DI.Chakra.DomainModel.Core;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.DomainModel.Presentation
{
    public class UserAccessDetailMatrixGridData: DomainModelBase<UserAccessDetailMatrixGridData, EntityModel.Presentation.UserAccessDetailMatrixGridData>
    {
        public string UserName { get; set; }
        public string EndpointSystemName { get; set; }
        public string EndpointHostName { get; set; }
        public long EndpointSystemID { get; set; }
        public long? EndpointSystemTypeId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string AccountType { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; } // User, Group, Role, TCode
        public int ParentId { get; set; }
        public DateTime? LastLogonDate { get; set; }
        public string Institution { get; set; }
        public string Description { get; set; }
        public string ApplicationType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementType { get; set; }
        public long AccountReconcilationId { get; set; }
        public string Company { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public string Branch { get; set; }
        public string Section { get; set; }
        public string Unit { get; set; }
        public string ReportingOfficer { get; set; }
        public static List<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixGridData(List<long> userIds)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var excludedUsers = MasterConfigHelper.ExcludedUsers;
                return ORMapList(new UserRepository(transaction).GetUserAccessDetailMatrixGridData(userIds, excludedUsers));
            }
        }
        public static PresentationGridPagedData<UserAccessDetailMatrixGridData> GetUserAccessDetailMatrixPagedData(List<long> userIds, int startIndex, int pageSize, string filterExpression, string sortExpression)
        {
            var response = new PresentationGridPagedData<UserAccessDetailMatrixGridData>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var excludedUsers = MasterConfigHelper.ExcludedUsers;
                var dbResult = new UserRepository(dataAccessTransaction).GetUserAccessDetailMatrixPagedData(userIds, excludedUsers, sortExpression, filterExpression, pageSize, startIndex);
                response.TotalCount = dbResult.TotalCount;
                response.Data = ConverToDomain(dbResult.Data);
                return response;
            }
        }
        public static byte[] DownloadUserAccessDetail(List<long> userIds, int startIndex, int pageSize, string filterExpression, string sortExpression, string exportFileType)
        {
            byte[] reportData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var excludedUsers = MasterConfigHelper.ExcludedUsers;
                var dbResult = new UserRepository(dataAccessTransaction).GetUserAccessDetailMatrixPagedData(userIds, excludedUsers, sortExpression, filterExpression, pageSize, startIndex);
                if (dbResult != null && dbResult.TotalCount > 0)
                {
                    dbResult.Data.ForEach(i=> { i.Description = i.Description?.Replace("<br/>", Environment.NewLine); });
                    reportData = GetReportAsStream(exportFileType, ConverToDomain(dbResult.Data));
                }
            }
            return reportData;
        }
        private static byte[] GetReportAsStream(string exportFileType, List<DomainModel.Presentation.UserAccessDetailMatrixGridData> result)
        {
            if (result != null && result.Count > 0)
            {

                var reportSheetName = "UserAccessDetail";
                var temp = HttpContext.Current.Request.PhysicalApplicationPath + "\\temp\\";
                string filePath = string.Empty;
                try
                {
                    if (!System.IO.Directory.Exists(temp)) System.IO.Directory.CreateDirectory(temp);
                    filePath = GetSafePath(temp, string.Format("{0}_{1}.{2}", reportSheetName, DateTime.Now.ToString(Constants.Report_Date_Format), exportFileType));
                    if (exportFileType.ToUpper() == "XLSX" || exportFileType.ToUpper() == "XLS")
                    {

                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            using (var objExcelPackage = new ExcelPackage(stream))
                            {
                                int currentRow = 1;
                                int colStartPosition = 1;
                                int colNum = colStartPosition;
                                ExcelWorksheet workSheet = objExcelPackage.Workbook.Worksheets.Add(reportSheetName);
                                workSheet.Row(1).Height = 20;
                                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                workSheet.Row(1).Style.Font.Bold = true;
                                workSheet.Cells[currentRow, colNum].Value = "Application Type";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Application Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Full Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "User Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Account Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "TCode";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Description";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "EntitlementType";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Reporting Officer";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Last Logon";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "AccountType";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "IsActive";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Company";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                //workSheet.Cells[currentRow, ++colNum].Value = "Division";
                                //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Department";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Branch";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Section";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Unit";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                                currentRow++;
                                foreach (var data in result)
                                {
                                    colStartPosition = 1;
                                    colNum = colStartPosition;
                                    workSheet.Cells[currentRow, colNum].Value = data.ApplicationType ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.EndpointSystemName ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.FullName ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.UserName ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Name ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Entitlement ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Institution ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Description ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.EntitlementType ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.ReportingOfficer ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.LastLogonDate.HasValue ? data.LastLogonDate.Value.ToString("dd-MM-yyyy hh:mm:ss") : "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.AccountType ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.IsActive.ToString();
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Company ?? "";
                                    //workSheet.Cells[currentRow, ++colNum].Value = data.Division?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Department ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Branch ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Section ?? "";
                                    workSheet.Cells[currentRow, ++colNum].Value = data.Unit ?? "";

                                    currentRow++;
                                }
                                workSheet.Cells.AutoFitColumns();
                                objExcelPackage.Workbook.Properties.Title = "Mail Report";
                                return objExcelPackage.GetAsByteArray();
                            }
                        }
                    }
                    else if (exportFileType.ToUpper() == "CSV")
                    {
                        using (StreamWriter stream = new StreamWriter(filePath))
                        {

                            StringBuilder objWriter = new StringBuilder();
                            stream.WriteLine("Application Type,Application Name,Full Name,User Name,Account Name,Entitlement,TCode,Description,EntitlementType,Reporting Officer,Last Logon,AccountType,IsActive,Company,Department,Branch,Section,Unit");
                            foreach (var data in result)
                            {
                                stream.WriteLine(
                                    string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\"",
                                        data.ApplicationType ?? "",
                                        data.EndpointSystemName ?? "",
                                        data.FullName ?? "",
                                        data.UserName ?? "",
                                        data.Name ?? "",
                                        data.Entitlement ?? "",
                                        data.Institution ?? "",
                                        data.Description ?? "",
                                        data.EntitlementType ?? "",
                                        data.ReportingOfficer ?? "",
                                        data.LastLogonDate.HasValue ? data.LastLogonDate.Value.ToString("dd-MM-yyyy hh:mm:ss") : "",
                                        data.AccountType ?? "",
                                        data.IsActive.ToString(),
                                        data.Company ?? "",
                                        //data.Division ?? "",
                                        data.Department ?? "",
                                        data.Branch ?? "",
                                        data.Section ?? "",
                                        data.Unit ?? ""
                                ));
                            }

                        }
                        return File.ReadAllBytes(filePath);
                    }

                }
                catch (Exception ex)
                {
                }
                finally
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return null;
        }
        public static List<UserAccessDetailMatrixGridData> ConverToDomain(List<EntityModel.Presentation.UserAccessDetailMatrixGridData> data)
        {
            List<UserAccessDetailMatrixGridData> lst = new List<UserAccessDetailMatrixGridData>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    lst.Add(new UserAccessDetailMatrixGridData()
                    {
                        AccountReconcilationId = item.AccountReconcilationId,
                        AccountType = item.AccountType,
                        ApplicationType = item.ApplicationType,
                        Branch = item.Branch,
                        Company = item.Company,
                        Department = item.Department,
                        Description = item.Description,
                        Division = item.Division,
                        EndpointHostName = item.EndpointHostName,
                        EndpointSystemID = item.EndpointSystemID,
                        EndpointSystemName = item.EndpointSystemName,
                        EndpointSystemTypeId = item.EndpointSystemTypeId,
                        Entitlement = item.Entitlement,
                        EntitlementType = item.EntitlementType,
                        FullName = item.FullName,
                        Institution = item.Institution,
                        IsActive = item.IsActive,
                        LastLogonDate = item.LastLogonDate,
                        Name = item.Name,
                        ParentId = item.ParentId,
                        ReportingOfficer = item.ReportingOfficer,
                        Section = item.Section,
                        Type = item.Type,
                        Unit = item.Unit,
                        UserName = item.UserName
                    });
                }
            }
            return lst;
        }

        private static string GetSafePath(string baseDirectory, string fileName)
        {
            string fullBase = Path.GetFullPath(baseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            if (string.IsNullOrWhiteSpace(fileName) || Path.IsPathRooted(fileName) || fileName.Contains("..") || fileName.Contains("~"))
                throw new InvalidOperationException("Invalid file path.");
            string combined = Path.GetFullPath(Path.Combine(fullBase, fileName));
            if (!combined.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Invalid file path.");
            return combined;
        }

    }
}
