using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure;
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

namespace DI.Chakra.DomainModel
{
    public class PresentationAccountEntitlementAuditTrial : DomainModelBase<PresentationAccountEntitlementAuditTrial, EntityModel.PresentationAccountEntitlementAuditTrial>
    {
        public string EndpointName { get; set; }
        public string DisplayName { get; set; }
        public string AccountType { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

        public static PresentationGridPagedData<PresentationAccountEntitlementAuditTrial> GetLazyLoadOtherSystemAuditTrial(int startRowIndex, int maximumRows, string filterExpression, string sortExpression, List<long> ids,DateTime? FromDate, DateTime? ToDate)
        {
            PresentationGridPagedData<PresentationAccountEntitlementAuditTrial> result = new PresentationGridPagedData<PresentationAccountEntitlementAuditTrial>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new InhouseUserInfoRepository(dataAccessTransaction).GetLazyLoadOtherSystemAuditTrial( startRowIndex, maximumRows, filterExpression, sortExpression, ids, FromDate, ToDate);
                result.Data = ORMapList(dbResult.Data);
                result.TotalCount = dbResult.TotalCount;
                return result;
            }
        }

        public static byte[] DownloadOtherSystemAuditTrial(int startRowIndex, int maximumRows, string filterExpression, string sortExpression, string exportFileType, List<long> ids, DateTime? FromDate, DateTime? ToDate)
        {
            byte[] reportData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result = new InhouseUserInfoRepository(dataAccessTransaction).GetLazyLoadOtherSystemAuditTrial(startRowIndex, maximumRows, filterExpression, sortExpression, ids,FromDate, ToDate);
                if (result.Data.Count > 0)
                    reportData = GetOtherSystemAuditTrialReport(exportFileType, result.Data);
            }
            return reportData;
        }
        private static byte[] GetOtherSystemAuditTrialReport(string exportFileType, IList<EntityModel.PresentationAccountEntitlementAuditTrial> result)
        {
            var reportSheetName = "OtherSystemAuditTrial";
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
                            workSheet.Cells[currentRow, colNum].Value = "EndpointName";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "UserName";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "AccountType";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Permission";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Remarks";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "CreatedDate";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            currentRow++;
                            foreach (var data in result)
                            {
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                workSheet.Cells[currentRow, colNum].Value = data.EndpointName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.DisplayName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.UserName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.AccountType;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Permission;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Remarks;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedDate.ToString("dd-MM-yyyy hh:mm:ss");
                                currentRow++;
                            }
                            workSheet.Cells.AutoFitColumns();
                            objExcelPackage.Workbook.Properties.Title = "OtherSystem Audit Trial Report";
                            return objExcelPackage.GetAsByteArray();
                        }
                    }
                }
                else if (exportFileType.ToUpper() == "CSV")
                {
                    using (StreamWriter stream = new StreamWriter(filePath))
                    {
                        StringBuilder objWriter = new StringBuilder();
                        stream.WriteLine("EndpointName,Name,UserName,AccountType,Permission,Remarks,CreatedDate");
                        foreach (var data in result)
                        {
                            stream.WriteLine(
                                string.Format("\"\t{0}\",\"\t{1}\",\"\t{2}\",\"\t{3}\",\"\t{4}\",\"\t{5}\",\"\t{6}\"",
                                 data.EndpointName ?? "",
                                 data.DisplayName ?? "",
                                 data.UserName ?? "",
                                 data.AccountType ?? "",
                                 data.Permission ?? "",
                                 data.Remarks ?? "",
                                 data.CreatedDate != null ? data.CreatedDate.ToString("dd-MM-yyyy hh:mm:ss") : ""
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
            return null;
        }

        public static Dictionary<string, List<string>> GetAuditTrailForEmail(DateTime fromDate, DateTime toDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new InhouseUserInfoRepository(dataAccessTransaction).GetAuditTrailForEmail(fromDate, toDate);

            }
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
