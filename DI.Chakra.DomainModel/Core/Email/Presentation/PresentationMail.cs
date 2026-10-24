using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.EntityModel.Presentation;
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

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationMail : EmailInfo
    {
        public static IList<PresentationMail> GetAllEmailInfos()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new MailRepository(dataAccessTransaction).GetAllEmailInfos());
            }
        }
        public static IList<PresentationMail> GetByActivityType(string activityType, string status) 
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new MailRepository(dataAccessTransaction).GetByActivityType(activityType, status)).ToList();
            }
        }
       
        public static IList<PresentationMail> GetAllMailHistory()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new MailRepository(dataAccessTransaction).GetAllMailHistory());
            }
        }

        public static PresentationMail ORMap(EntityModel.Presentation.PresentationMail presentationMailEntityModel)
        {
            if (presentationMailEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationMail, PresentationMail>(presentationMailEntityModel);
        }
        public static PresentationGridPagedData<PresentationMail> GetByActivityType(string activityType, string status, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, DateTime? fromDate,DateTime? ToDate)
        {
            var response = new PresentationGridPagedData<PresentationMail>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new MailRepository(dataAccessTransaction).GetPagedDataByActivityType(activityType, status, startRowIndex, maximumRows, filterExpression, sortExpression, fromDate, ToDate);
                response.TotalCount = dbResult.TotalCount;
                response.Data = ORMapList(dbResult.Data);
                return response;
            }
        }
        public static byte[] DownloadEmail(string activityType, string status,int startRowIndex, int maximumRows, string filterExpression, string sortExpression, string exportFileType,DateTime? fromDate, DateTime? ToDate)
        {
            byte[] reportData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new MailRepository(dataAccessTransaction).GetPagedDataByActivityType(activityType, status, startRowIndex, maximumRows, filterExpression, sortExpression,fromDate,ToDate);
                if(dbResult != null && dbResult.TotalCount > 0)
                {
                    reportData = GetReportAsStream(exportFileType, dbResult.Data);
                }
            }
            return reportData;
        }

        private static byte[] GetReportAsStream(string exportFileType, List<EntityModel.Presentation.PresentationMail> result)
        {
            var reportSheetName = "Mail";
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
                            workSheet.Cells[currentRow, colNum].Value = "Subject";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "To";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Cc";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Bcc";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Activity Type";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Status";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created Date & Time";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Sent Date & Time";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                            currentRow++;
                            foreach (var data in result)
                            {
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                workSheet.Cells[currentRow, colNum].Value = data.Subject??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.To??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.Cc??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.Bcc??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.ActivityType??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.Status??"";
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedDate.ToString("dd-MM-yyyy hh:mm:ss tt");
                                workSheet.Cells[currentRow, ++colNum].Value = data.SentDateTime.HasValue ? data.SentDateTime.Value.ToString("dd-MM-yyyy hh:mm:ss tt") : "";
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
                        stream.WriteLine("Subject,To,Cc,Bcc,Activity Type,Status,Created Date & Time,Sent Date & Time");
                        foreach (var data in result)
                        {
                            stream.WriteLine(
                                string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                    data.Subject ?? "",
                                    data.To ?? "",
                                    data.Cc ?? "",
                                    data.Bcc ?? "",
                                    data.ActivityType ?? "",
                                    data.Status ?? "",
                                    data.CreatedDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                                    data.SentDateTime.HasValue? data.SentDateTime.Value.ToString("dd-MM-yyyy hh:mm:ss tt") : ""
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

        public static List<PresentationMail> ORMapList(IList<EntityModel.Presentation.PresentationMail> presentationMailEntityModel)
        {
            return presentationMailEntityModel.Select(ORMap).ToList();
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
