using AutoMapper;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel.Core;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationUser : DomainModelBase<PresentationUser, EntityModel.Presentation.PresentationUser>
    {
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string EmailId { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public EmployeeType? EmployeeType { get; set; }
        public bool? isActiveUser { get; set; }
        public string OrganisationName { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public string BusinessRoleName { get; set; }
        public string ManagerName { get; set; }

        public string Grade { get; set; }

        public string CSEL { get; set; }

        public bool GoldBar { get; set; }

        public string Company { get; set; }
        public string NCompany { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public bool? IsActive { get; set; }
        public string GroupName { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Department { get; set; }
        public string CostCenter { get; set; }
        public string Division { get; set; }
        public string OrganisationUnit { get; set; }
        public string ReportingOfficer { get; set; }
        public string ProjectDescription { get; set; }
        public string CreatedBy { get; set; }
        public string ReferenceUser { get; set; }
        public string Status { get; set; }
        public string SectionName { get; set; }
        public string Branch { get; set; }
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string BranchCode { get; set; }
        public string SectionCode { get; set; }
        public string UnitCode { get; set; }
        public string OrganisationCode { get; set; }
        public string CounterSigningOfficer { get; set; }
        public string EmployeeTypeString
        {
            get; set;
        }
        public static List<PresentationUser> GetEmployees(DateTime? FromDate, DateTime? ToDate, bool? IsActive)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetEmployees(FromDate, ToDate, IsActive));
            }
        }

        public static List<PresentationUser> GetEmployees()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetEmployees());
            }
        }

        public static List<PresentationUser> GetMembers(long userId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetMembers(userId));
            }
        }

        public static PresentationGridPagedData<PresentationUser> GetLazyLoadFullTimeEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, string departmentCode, bool showAllNonEmployees)
        {
            PresentationGridPagedData<PresentationUser> result = new PresentationGridPagedData<PresentationUser>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new PresentationUserRepository(dataAccessTransaction).GetLazyLoadFullTimeEmployees(fullTimeEmp, startRowIndex, maximumRows, FilterExpression, sortExpression, departmentCode, showAllNonEmployees);
                result.Data = ORMapList(dbResult.Data);
                result.TotalCount = dbResult.TotalCount;
                return result;
            }
            
        }
        public static byte[] DownloadFullTimeEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, string exportFileType)
        {
            byte[] reportData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result = new PresentationUserRepository(dataAccessTransaction).GetLazyLoadFullTimeEmployees(fullTimeEmp, startRowIndex, maximumRows, filterExpression, sortExpression);
                if(result.Data.Count > 0)
                    reportData = GetReportAsStream(exportFileType, result.Data);
            }
            return reportData;
        }

        private static byte[] GetReportAsStream(string exportFileType, IList<EntityModel.Presentation.PresentationUser> result)
        {
            var reportSheetName = "Employee";
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
                            workSheet.Cells[currentRow, colNum].Value = "User Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Employee Id";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Display Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Company";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Department";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Branch";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Section Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Unit";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Is Active";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Join Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Last Service Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Reporting Officer";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "CounterSigning Officer";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Position";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "EmailId";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Cost Center";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Company Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Department Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Branch Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "SectionCode";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Unit Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Organisation Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            
                           
                        

                            currentRow++;
                            foreach (var data in result)
                            {
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                workSheet.Cells[currentRow, colNum].Value = data.UserName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.EmployeeId;
                                workSheet.Cells[currentRow, ++colNum].Value = data.DisplayName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Company;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Department;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Branch;
                                workSheet.Cells[currentRow, ++colNum].Value = data.SectionName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.OrganisationUnit;
                                workSheet.Cells[currentRow, ++colNum].Value = data.IsActive;
                                workSheet.Cells[currentRow, ++colNum].Value = data.JoinDate.HasValue ? data.JoinDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.TerminationDate.HasValue ? data.TerminationDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.ReportingOfficer;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CounterSigningOfficer;
                                workSheet.Cells[currentRow, ++colNum].Value = data.BusinessRoleName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.EmailId;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CostCenter;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CompanyCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.DepartmentCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.BranchCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.SectionCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.UnitCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.OrganisationCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt");
                               
                                
                                currentRow++;
                            }
                            workSheet.Cells.AutoFitColumns();
                            objExcelPackage.Workbook.Properties.Title = "Employee Report";
                            return objExcelPackage.GetAsByteArray();
                        }
                    }
                }
                else if (exportFileType.ToUpper() == "CSV")
                {
                    using (StreamWriter stream = new StreamWriter(filePath))
                    {
                        StringBuilder objWriter = new StringBuilder();
                        stream.WriteLine("User Name,Employee Id,Display Name,Company,Department,Branch,Section Name,Unit,IsActive,Join Date,Last Service Date,Reporting Officer,CounterSigning Officer,Position,EmailId,Cost Center,Company Code,Department Code,Branch Code,SectionCode,Unit Code,Organisation Code,Created Date");
                        foreach (var data in result)
                        {
                            stream.WriteLine(
                                string.Format("\"\t{0}\",\"\t{1}\",\"\t{2}\",\"\t{3}\",\"\t{4}\",\"\t{5}\",\"\t{6}\",\"\t{7}\",\"\t{8}\",\"\t{9}\",\"\t{10}\",\"\t{11}\",\"\t{12}\",\"\t{13}\",\"\t{14}\",\"\t{15}\",\"\t{16}\",\"\t{17}\",\"\t{18}\",\"\t{19}\",\"\t{20}\",\"\t{21}\",\"\t{22}\"",
                                data.UserName ?? "",
                                data.EmployeeId ?? "",
                                data.DisplayName ?? "",
                                data.Company ?? "",
                                data.Department ?? "",
                                data.Branch ?? "",
                                data.SectionName ?? "",
                                data.OrganisationUnit ?? "",        
                                data.IsActive,
                                data.JoinDate != null ? data.JoinDate.Value.ToString("dd-MM-yyyy") : "",
                                data.TerminationDate != null ? data.TerminationDate.Value.ToString("dd-MM-yyyy") : "",
                                data.ReportingOfficer ?? "",
                                data.CounterSigningOfficer ?? "",
                                data.BusinessRoleName ?? "",
                                data.EmailId ?? "",
                                data.CostCenter ?? "",
                                data.CompanyCode ?? "",
                                data.DepartmentCode ?? "",
                                data.BranchCode ?? "",
                                data.SectionCode ?? "",
                                data.UnitCode ?? "",
                                data.OrganisationCode ?? "",
                                data.CreatedDate != null ? data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt") : ""
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

        public static byte[] DownloadNonEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, string exportFileType,string departmentCode, bool showAllNonEmployees)
        {
            byte[] reportData = null;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result = new PresentationUserRepository(dataAccessTransaction).GetLazyLoadFullTimeEmployees(fullTimeEmp, startRowIndex, maximumRows, filterExpression, sortExpression, departmentCode, showAllNonEmployees);
                if(result.Data.Count > 0)
                    reportData = GetNonEmployeeReport(exportFileType, result.Data);
            }
            return reportData;
        }

        private static byte[] GetNonEmployeeReport(string exportFileType, IList<EntityModel.Presentation.PresentationUser> result)
        {
            var reportSheetName = "NonEmployee";
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
                            workSheet.Cells[currentRow, colNum].Value = "User Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Employee Id";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Display Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Company";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Department";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Branch";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Section Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Unit";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Is Active";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Join Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Last Service Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Reporting Officer";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "CounterSigning Officer";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Reference User";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "EmailId";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "EmployeeType";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "NonEmployee Company";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Cost Center";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Project/Description";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Mobile Number";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Company Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Department Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Branch Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Section Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Unit Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;                        
                            workSheet.Cells[currentRow, ++colNum].Value = "Organisation Code";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Created By";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Status";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                            currentRow++;
                            foreach (var data in result)
                            {
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                workSheet.Cells[currentRow, colNum].Value = data.UserName;
                                workSheet.Cells[currentRow, ++colNum].Value   = data.EmployeeId;
                                workSheet.Cells[currentRow, ++colNum].Value = data.DisplayName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Company;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Department;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Branch;
                                workSheet.Cells[currentRow, ++colNum].Value = data.SectionName;
                                workSheet.Cells[currentRow, ++colNum].Value = data.OrganisationUnit;
                                workSheet.Cells[currentRow, ++colNum].Value = data.IsActive;
                                workSheet.Cells[currentRow, ++colNum].Value = data.JoinDate.HasValue ? data.JoinDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.TerminationDate.HasValue ? data.TerminationDate.Value.ToString("dd-MM-yyyy") : "";
                                workSheet.Cells[currentRow, ++colNum].Value = data.ReportingOfficer;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CounterSigningOfficer;
                                workSheet.Cells[currentRow, ++colNum].Value = data.ReferenceUser;
                                workSheet.Cells[currentRow, ++colNum].Value = data.EmailId;
                                workSheet.Cells[currentRow, ++colNum].Value = data.EmployeeTypeString;
                                workSheet.Cells[currentRow, ++colNum].Value = data.NCompany;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CostCenter;
                                workSheet.Cells[currentRow, ++colNum].Value = data.ProjectDescription;
                                workSheet.Cells[currentRow, ++colNum].Value = data.MobileNumber;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CompanyCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.DepartmentCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.BranchCode;                                
                                workSheet.Cells[currentRow, ++colNum].Value = data.SectionCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.UnitCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.OrganisationCode;
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt");
                                workSheet.Cells[currentRow, ++colNum].Value = data.CreatedBy;
                                workSheet.Cells[currentRow, ++colNum].Value = data.Status;
                                currentRow++;
                            }
                            workSheet.Cells.AutoFitColumns();
                            objExcelPackage.Workbook.Properties.Title = "NonEmployee Report";
                            return objExcelPackage.GetAsByteArray();
                        }
                    }
                }
                else if (exportFileType.ToUpper() == "CSV")
                {
                    using (StreamWriter stream = new StreamWriter(filePath))
                    {
                        StringBuilder objWriter = new StringBuilder();
                        stream.WriteLine("User Name,Employee Id,Display Name,Company,Department,Branch,Section Name,Unit,Is Active,Join Date,Last Service Date,Reporting Officer,CounterSigning Officer,Reference User,EmailID,EmployeeType,NonEmployee Company,Cost Center,Project/Description,Mobile Number,Company Code,Department Code,Branch Code,Section Code,Unit Code,Organisation Code,Created Date,Created By,Status");
                        foreach (var data in result)
                        {
                            stream.WriteLine(
                                string.Format("\"\t{0}\",\"\t{1}\",\"\t{2}\",\"\t{3}\",\"\t{4}\",\"\t{5}\",\"\t{6}\",\"\t{7}\",\"\t{8}\",\"\t{9}\",\"\t{10}\",\"\t{11}\",\"\t{12}\",\"\t{13}\",\"\t{14}\",\"\t{15}\",\"\t{16}\",\"\t{17}\",\"\t{18}\",\"\t{19}\",\"\t{20}\",\"\t{21}\",\"\t{22}\",\"\t{23}\",\"\t{24}\",\"\t{25}\",\"\t{26}\",\"\t{27}\",\"\t{28}\"",
                                 data.UserName ?? "",
                                 data.EmployeeId ?? "",
                                 data.DisplayName ?? "",
                                 data.Company ?? "",
                                 data.Department ?? "",
                                 data.Branch ?? "",
                                 data.SectionName ?? "",
                                 data.OrganisationUnit ?? "",
                                 data.IsActive,
                                 data.JoinDate != null ? data.JoinDate.Value.ToString("dd-MM-yyyy") : "",
                                 data.TerminationDate != null ? data.TerminationDate.Value.ToString("dd-MM-yyyy") : "",
                                 data.ReportingOfficer ?? "",
                                 data.CounterSigningOfficer ?? "",
                                 data.ReferenceUser ?? "",
                                 data.EmailId ?? "",
                                 data.EmployeeTypeString ?? "",
                                 data.NCompany ?? "",
                                 data.CostCenter ?? "",
                                 data.ProjectDescription ?? "",
                                 data.MobileNumber ?? "",
                                 data.CompanyCode ?? "",
                                 data.DepartmentCode ?? "",
                                 data.BranchCode ?? "",
                                 data.SectionCode ?? "",
                                 data.UnitCode ?? "",
                                 data.OrganisationCode ?? "",
                                 data.CreatedDate != null ? data.CreatedDate.ToString("dd-MM-yyyy hh:mm tt") : "",
                                 data.CreatedBy,
                                 data.Status ?? ""
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

        public static List<PresentationUser> GetActiveEmployees(string employeeType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (employeeType == "All")
                    return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetAllActiveEmployees());
                else if (employeeType == "Employee")
                    return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetActiveEmployees());
                else
                    return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetActiveNonEmployees());
            }
        }
        public static List<PresentationUser> GetNonEmployees()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetNonEmployees());
            }
        }
        public static List<PresentationUser> GetUserGroup()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetUserGroup());
            }
        }

        public static List<PresentationUser> GetUserRole()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationUserRepository(dataAccessTransaction).GetUserRole());
            }
        }
        public static PresentationUser ORMap(EntityModel.Presentation.PresentationUser presentationUser)
        {
            if (presentationUser == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationUser, PresentationUser>(presentationUser);
        }

        public static List<PresentationUser> ORMapList(IList<EntityModel.Presentation.PresentationUser> presentationUser)
        {
            return presentationUser.Select(ORMap).ToList();
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
