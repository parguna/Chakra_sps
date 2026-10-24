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
    public class PresentationTaskdelegation : TaskDelegation
    {
        public long DelegationId { get; set; }

        public string ActionUserName { get; set; }

        public long DelegatedToUserId { get; set; }

        public string DelegatedToUserName { get; set; }

        public bool IsAdvance { get; set; }

        public static List<PresentationTaskdelegation> GetDelegationsByUserId(int userId, bool isAdmin)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (isAdmin)
                {
                    return ORMapList(new TaskDelegationRepository(dataAccessTransaction).GetDelegationsByAdmin());
                }
                else
                {
                    return ORMapList(new TaskDelegationRepository(dataAccessTransaction).GetDelegationsByUserId(userId));
                }
            }
        }

        public static PresentationTaskdelegation ORMap(EntityModel.Presentation.PresentationTaskdelegation presentationTaskDelegationEntityModel)
        {
            if (presentationTaskDelegationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationTaskdelegation, PresentationTaskdelegation>(presentationTaskDelegationEntityModel);
        }

        public static List<PresentationTaskdelegation> ORMapList(IList<EntityModel.Presentation.PresentationTaskdelegation> presentationTaskDelegationEntityModel)
        {
            return presentationTaskDelegationEntityModel.Select(ORMap).ToList();
        }
        public static PresentationGridPagedData<PresentationTaskdelegation> GetAllDelegationByActionUser(long actionUser, bool isAdmin,bool includeInActive,string sortExpression, string filterExpression,int startIndex, int maximumRow)
        {
            var response = new PresentationGridPagedData<PresentationTaskdelegation>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var result = new TaskDelegationRepository(dataAccessTransaction).GetPagedAllDelegationByActionUser(actionUser, isAdmin,includeInActive,sortExpression, filterExpression,startIndex,maximumRow);
                response.TotalCount = result.TotalCount;
                response.Data = ORMapList(result.Data);
                return response;
                //if (isAdmin == null || isAdmin == false)
                //{
                //    return ORMapList(new TaskDelegationRepository(dataAccessTransaction).GetDelegationsByUserId(actionUser).ToList());
                //}
                //else
                //{
                //    return ORMapList(new TaskDelegationRepository(dataAccessTransaction).GetDelegationsByAdmin().ToList());
                //}
            }
        }

        public static byte[] DownloadDelegation(long actionUser, bool isAdmin,bool includeInActive, string sortExpression, string filterExpression, int startIndex, int maximumRow,string exportFileType)
        {
            byte[] downloadData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var result = new TaskDelegationRepository(dataAccessTransaction).GetPagedAllDelegationByActionUser(actionUser, isAdmin,includeInActive, sortExpression, filterExpression, startIndex, maximumRow);
                if(result.Data.Count > 0)
                {
                    downloadData = GetReportAsStream(exportFileType, result.Data);
                }
            }
            return downloadData;
        }
        private static byte[] GetReportAsStream(string exportFileType, List<EntityModel.Presentation.PresentationTaskdelegation> result)
        {
            var reportSheetName = "AdvanceDelegation";
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
                            workSheet.Cells[currentRow, colNum].Value = "Delegation ID";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Delegated From";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Delegated To";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Provisioning";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Attestation";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Comments";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Start Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "End Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Is Active";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created By";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Changed By";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Changed Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                            currentRow++;
                            foreach (var data in result)
                            {
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                workSheet.Cells[currentRow, colNum].Value = data.Id;
                                workSheet.Cells[currentRow, ++colNum].Value = data.ActionUserName ?? "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.DelegatedToUserName ?? "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.DelegateProvisioningTask ? "Yes":"No";
                                workSheet.Cells[currentRow, ++colNum].Value = data.DelegateAttestationTask ? "Yes" : "No";
                                workSheet.Cells[currentRow, ++colNum].Value = data.DelegationComments ?? "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.StartDate.HasValue ? data.StartDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.EndDate.HasValue ? data.EndDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.IsActive ? "Yes" : "No";
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedBy ?? "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt");
                                workSheet.Cells[currentRow, ++colNum].Value = data.LastModifiedBy ?? "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.LastModifiedDate.HasValue ? data.LastModifiedDate.Value.ToString("dd-MM-yyyy hh:mm tt") : "";
                                currentRow++;
                            }
                            workSheet.Cells.AutoFitColumns();
                            objExcelPackage.Workbook.Properties.Title = "AdvanceDelegation Report";
                            return objExcelPackage.GetAsByteArray();
                        }
                    }
                }
                else if (exportFileType.ToUpper() == "CSV")
                {
                    using (StreamWriter stream = new StreamWriter(filePath))
                    {
                        StringBuilder objWriter = new StringBuilder();
                        stream.WriteLine("Delegation ID,Delegated From,Delegated To,Provisioning,Attestation,Comments,Start Date,End Date,Is Active,Created By,Created Date,Changed By,Changed Date");
                        foreach (var data in result)
                        {
                            stream.WriteLine(
                                string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\"",
                                    data.Id,
                                    data.ActionUserName,
                                    data.DelegatedToUserName ?? "",
                                    data.DelegateProvisioningTask ? "Yes" : "No",
                                    data.DelegateAttestationTask ? "Yes" : "No",
                                    data.DelegationComments ?? "",
                                    data.StartDate.HasValue ? data.StartDate.Value.ToString("dd-MM-yyyy"):"",
                                    data.EndDate.HasValue ? data.EndDate.Value.ToString("dd-MM-yyyy") : "",
                                    data.IsActive ? "Yes" : "No",
                                    data.CreatedBy,
                                    data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt"),
                                    data.LastModifiedBy ?? "",
                                    data.LastModifiedDate.HasValue ? data.LastModifiedDate.Value.ToString("dd-MM-yyyy hh:mm tt") : ""
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
