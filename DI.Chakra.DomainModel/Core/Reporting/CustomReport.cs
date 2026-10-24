using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace DI.Chakra.DomainModel
{
    public class CustomReport : DomainModelBase<CustomReport, EntityModel.CustomReport>
    {
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReportQuery { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> LastRunOn { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public byte[] Attachment { get; set; }
        public bool IsLinkedToMenu { get; set; }
        public long? LinkedToMenu { get; set; }
        public bool IsGroupByEnabled { get; set; }
        public string GroupByColumn { get; set; }
        public long? PageId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public List<CustomReportParameter> CustomReportParameters { get; set; }
        public static List<string> GetAllColumnsFromQuery(string query)
        {
            if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(query))
            {
                DILog.ReportingLogger.WriteLog(LogLevelType.Exception, "Entered query is not valid for report, please enter SELECT query only.");
                return new List<string>();
            }

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.CustomReport>(dataAccessTransaction).GetAllColumnsFromQuery(query);
            }
        }
        public static List<CustomReport> GetAllRunReports()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var result = ORMapList(new RepositoryBase<EntityModel.CustomReport>(dataAccessTransaction).GetAll().Where(x => x.Attachment != null && x.Attachment.Count() > 0 && x.LastRunOn != null
                && !x.IsLinkedToMenu).ToList());
                foreach (var item in result)
                {
                    item.Attachment = null;
                }
                return result;
            }
        }

        public static byte[] GetReportViewerDataForExport(long reportId, bool isExcelReport, string updateQuery, string requestedUser, string filterExpression)
        {
            var customQuery = CustomReport.GetById(reportId);
            if (customQuery != null)
            {
                var reportData = GetReportViewerData(reportId, 0, 0, filterExpression, "", true, updateQuery);
                DataTable dt = reportData.ReportData;
                string pattern = "[\\~#%&*{}/:<>?|\"-]";
                string replacement = "";

                Regex regEx = new Regex(pattern);
                string fileName = Regex.Replace(regEx.Replace(customQuery.Name, replacement), @"\s+", "");
                byte[] content;
                if (isExcelReport)
                    content = DataTableToExcel(dt, fileName, requestedUser);
                else
                    content = DataTableToCSV(dt, fileName);
                customQuery.FileName = fileName;
                customQuery.ContentType = "Excel";
                customQuery.Save("System");
                return content;
            }
            return null;
        }

        public static byte[] GetReportViewerDataForExport(string customQuery, bool isExcelReport, string updateQuery, string requestedUser)
        {
            if (customQuery != null)
            {
                var reportData = GetReportViewerData(customQuery, 0, 0, "", "", true, updateQuery);
                DataTable dt = reportData.ReportData;
                string pattern = "[\\~#%&*{}/:<>?|\"-]";
                string replacement = "";

                Regex regEx = new Regex(pattern);
                string fileName = Regex.Replace(regEx.Replace("Discovery Audit Report", replacement), @"\s+", "");
                byte[] content;
                if (isExcelReport)
                    content = DataTableToExcel(dt, fileName, requestedUser);
                else
                    content = DataTableToCSV(dt, fileName);
                return content;
            }
            return null;
        }

        private static byte[] DataTableToExcel(DataTable dataTable, string fileName, string requestedUser)
        {
            byte[] result;
            //using (ExcelPackage pck = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(fileName.Length > 20 ? "Report" : fileName);
            //    ws.Cells["A1"].Style.Numberformat.Format = "@";
            //    ws.Cells["A1"].LoadFromDataTable(dataTable, true);
            //    ws.Cells["A1"].Style.Numberformat.Format = "@";

            //    for (int i = 0; i <= ws.Cells["A1"].Rows; i++)
            //    {
            //        if (dataTable.Columns[i].DataType == typeof(DateTime))
            //        {
            //            ws.Cells["A1"].Style.Numberformat.Format = "dd-MM-yyyy hh:mm:ss";
            //        }
            //    }

            //    result = pck.GetAsByteArray();
            //}
            result = new ExcelExport(dataTable, fileName, requestedUser).ExportToExcel();
            return result;
        }

        private static byte[] DataTableToCSV(DataTable dataTable, string fileName)
        {
            byte[] result;
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            result = Encoding.ASCII.GetBytes(sb.ToString());

            return result;
        }

        public static PresentationCustomReportData GetReportViewerData(long reportId, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, bool forExport = false, string updatedQuery = null)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                PresentationCustomReportData customReportData = new PresentationCustomReportData();
                var customQuery = CustomReport.GetById(reportId, true);
                RowComparer comp = new RowComparer();
                if (customQuery != null && customQuery.IsLinkedToMenu)
                {
                    DataTable finalDataTable = new DataTable();
                    var strQuery = string.IsNullOrEmpty(updatedQuery) ? customQuery.ReportQuery.Trim() : updatedQuery.Trim();
                    var reportQueryParameters = new List<SqlParameter>();

                    if (string.IsNullOrEmpty(updatedQuery) && customQuery.CustomReportParameters != null && customQuery.CustomReportParameters.Count > 0
                        && customQuery.CustomReportParameters.Any(a => !a.IsDeleted))
                    {
                        customQuery.CustomReportParameters.ForEach(reportParameter =>
                        {
                            if (!reportParameter.IsDeleted)
                            {
                                if (reportParameter.ParameterVariant == CustomReportParameterVariant.Value)
                                {
                                    // Bind the report parameter's value as a real SQL parameter instead of
                                    // splicing it into the query text, since @ParameterName is already valid
                                    // T-SQL parameter syntax.
                                    reportQueryParameters.Add(new SqlParameter($"@{reportParameter.ParameterName}", (object)reportParameter.DefaultValue ?? DBNull.Value));
                                }
                                else
                                {
                                    strQuery = strQuery.Replace($":{reportParameter.ParameterName}", " 1=1 ");
                                }
                            }
                        });
                    }

                    if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(strQuery))
                    {
                        customQuery.Status = "Failed";
                        customQuery.LastRunOn = DateTime.Now;
                        customQuery.Save("System");
                        DILog.ReportingLogger.WriteLog(reportId, LogLevelType.Error, "Entered query is not valid for report, please enter SELECT query only.");
                        return customReportData;
                    }

                    DataTable dt = new RepositoryBase<EntityModel.CustomReport>(dataAccessTransaction).GetDataTableByQuery(strQuery, reportQueryParameters.ToArray());
                    if (dt == null || dt.Rows.Count == 0) return customReportData;

                    List<DataColumn> reportColumns = new List<DataColumn>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        reportColumns.Add(column);
                    }
                    customReportData.ReportColumns = reportColumns;
                    maximumRows = maximumRows > 0 ? maximumRows : dt.Rows.Count;
                    try
                    {
                        if (!string.IsNullOrEmpty(FilterExpression) && !string.IsNullOrEmpty(sortExpression))
                        {
                            var tmpWhere = dt.Select(FilterExpression, sortExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else if (!string.IsNullOrEmpty(FilterExpression))
                        {
                            var tmpWhere = dt.Select(FilterExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else if (!string.IsNullOrEmpty(sortExpression))
                        {
                            var tmpWhere = dt.Select("", sortExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else
                        {
                            Dictionary<int, SortOrder> sortColumns = new Dictionary<int, SortOrder>();
                            sortColumns.Add(0, SortOrder.Ascending);
                            comp.SortColumns = sortColumns;
                            finalDataTable = dt.AsEnumerable().AsQueryable().OrderBy(a => a, comp).CopyToDataTable();
                        }
                        if (finalDataTable != null && finalDataTable.Rows != null)
                        {
                            customReportData.Count = finalDataTable.Rows.Count;
                            customReportData.ReportData = finalDataTable.AsEnumerable().Skip(startRowIndex).Take(maximumRows).CopyToDataTable();
                        }
                        return customReportData;
                    }
                    catch (Exception ex)
                    {
                        return customReportData;
                    }
                }
                else
                    return customReportData;
            }
        }


        public static PresentationCustomReportData GetReportViewerData(string query, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, bool forExport = false, string updatedQuery = null)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                PresentationCustomReportData customReportData = new PresentationCustomReportData();
                var customQuery = query;
                DataTable finalDataTable = new DataTable();
                RowComparer comp = new RowComparer();
                if (customQuery != null)
                {
                    if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(customQuery))
                    {
                        DILog.ReportingLogger.WriteLog(LogLevelType.Exception, "Entered query is not valid for report, please enter SELECT query only.");
                        return customReportData;
                    }

                    DataTable dt = new RepositoryBase<EntityModel.CustomReport>(dataAccessTransaction).GetDataTableByQuery(customQuery);
                    if (dt == null || dt.Rows.Count == 0) return customReportData;
                    if (forExport) { customReportData.ReportData = dt; return customReportData; }
                    maximumRows = maximumRows > 0 ? maximumRows : dt.Rows.Count;
                    try
                    {
                        if (!string.IsNullOrEmpty(FilterExpression) && !string.IsNullOrEmpty(sortExpression))
                        {
                            var tmpWhere = dt.Select(FilterExpression, sortExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else if (!string.IsNullOrEmpty(FilterExpression))
                        {
                            var tmpWhere = dt.Select(FilterExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else if (!string.IsNullOrEmpty(sortExpression))
                        {
                            var tmpWhere = dt.Select("", sortExpression);
                            DataTable tmpDT = new DataTable();
                            if (tmpWhere == null || tmpWhere.Count() == 0) return customReportData;
                            finalDataTable = tmpWhere.CopyToDataTable();
                        }
                        else
                        {
                            Dictionary<int, SortOrder> sortColumns = new Dictionary<int, SortOrder>();
                            sortColumns.Add(0, SortOrder.Ascending);
                            comp.SortColumns = sortColumns;
                            finalDataTable = dt;
                            //finalDataTable = dt.AsEnumerable().AsQueryable().OrderBy(a => a, comp).CopyToDataTable();
                        }

                        // Remove Id and Endpoint System Id columns, its not required while displaying reports. Hence is removed.
                        // Instead of Endpoint System Id system will display Endpoint System Name.
                        //if (finalDataTable.Columns.Count > 2)
                        //{
                        //    finalDataTable.Columns.RemoveAt(0); //Id Column
                        //    finalDataTable.Columns.RemoveAt(0); //Endpoint System Id column
                        //}

                        //if (finalDataTable.Columns.Contains("Id"))
                        //    finalDataTable.Columns.Remove(finalDataTable.Columns["Id"]);

                        //if (finalDataTable.Columns.Contains("EndpointSystemTypeId"))                        
                        //    finalDataTable.Columns.Remove(finalDataTable.Columns["EndpointSystemTypeId"]);

                        //if (finalDataTable.Columns.Contains("EndPointSystemId"))                        
                        //    finalDataTable.Columns.Remove(finalDataTable.Columns["EndPointSystemId"]);

                        //if (finalDataTable.Columns.Contains("DisplayName"))
                        //    finalDataTable.Columns.Remove(finalDataTable.Columns["DisplayName"]);

                        if (finalDataTable != null && finalDataTable.Rows != null)
                        {
                            customReportData.Count = finalDataTable.Rows.Count;
                            customReportData.ReportData = finalDataTable.AsEnumerable().Skip(startRowIndex).Take(maximumRows).CopyToDataTable();
                        }

                        return customReportData;
                    }
                    catch (Exception ex)
                    {
                        return customReportData;
                    }
                }
                else
                    return customReportData;
            }
        }


        public static DataTable GetQueryResults(string customQuery)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (customQuery != null)
                    {
                        if (!DI.Chakra.Repository.SqlSafetyGuard.IsSelectOnly(customQuery))
                        {
                            DILog.GeneralLogger.WriteLog(LogLevelType.Exception, "Entered query is not valid for report, please enter SELECT query only.");
                            return new DataTable();
                        }

                        DataTable customReportData = new RepositoryBase<EntityModel.CustomReport>(dataAccessTransaction).GetDataTableByQuery(customQuery);
                        if (customReportData == null || customReportData.Rows.Count == 0) return new DataTable();

                        return customReportData;
                    }
                    else
                        return new DataTable();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
                return new DataTable();
            }
        }

        //public static long GetReportViewerDataCount(long reportId, string FilterExpression, string updatedQuery = null)
        //{
        //    long getReportViewerDataCount = 0;
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        getReportViewerDataCount = CustomReport.GetReportDataCount(reportId, FilterExpression, updatedQuery);
        //    }
        //    return getReportViewerDataCount;
        //}

        //public static long GetReportDataCount(long reportId, string FilterExpression, string updatedQuery = null)
        //{
        //    long count = 0;
        //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        var reportData = GetReportViewerData(reportId, 0, 0, FilterExpression, "",false,updatedQuery);
        //        DataTable dt = reportData.ReportData;
        //        if (dt != null && dt.Rows != null)
        //        {
        //            count = dt.Rows.Count;
        //        }
        //    }
        //    return count;
        //}

        public static int DeleteCustomReportByRuleId(long reportId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                CustomReport report = CustomReport.GetById(reportId);

                if (report != null && report.IsLinkedToMenu && report.PageId.HasValue)
                {
                    try
                    {
                        var pageId = report.PageId.Value;
                        AccessRights.ExecuteQuery(string.Format("DELETE FROM DI_M_AccessRights WHERE PagePermissionId in (SELECT Id FROM DI_M_PagePermission WHERE PageId = {0})", pageId));
                        PagePermission.ExecuteQuery(string.Format("DELETE FROM DI_M_PagePermission WHERE PageId in (SELECT Id FROM DI_M_Page WHERE Id = {0})", pageId));
                        TabMenu.ExecuteQuery(string.Format("DELETE FROM DI_M_TabMenu WHERE PageId in (SELECT Id FROM DI_M_Page WHERE Id = {0})", pageId));
                        WebPage.ExecuteQuery("DELETE FROM DI_M_Page WHERE Id = " + pageId);
                        var urlToLoadReport = string.Format("/Report/CustomReport/CustomReportViewer.aspx?reportId={0}", reportId);
                        PageUrl.ExecuteQuery(string.Format("DELETE FROM DI_M_PageUrl WHERE Url = '{0}'", urlToLoadReport));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                report.Delete(tranaction);

                tranaction.Commit();
            }

            return 1;
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;

            bool createLinkedMenu = false;
            if (!isNew)
            {
                using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
                {
                    var reportToEdit = ORMap(new RepositoryBase<EntityModel.CustomReport>(transaction)
                        .Get(g => g.Id == this.Id).FirstOrDefault());

                    if (reportToEdit != null && reportToEdit.IsLinkedToMenu && reportToEdit.LinkedToMenu.HasValue)
                    {
                        createLinkedMenu = false;
                    }
                    else if (this.IsLinkedToMenu && this.LinkedToMenu.HasValue)
                    {
                        createLinkedMenu = true;
                    }
                }
            }
            else if (this.IsLinkedToMenu && this.LinkedToMenu.HasValue)
            {
                createLinkedMenu = true;
            }

            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                this.Save(transaction);

                if (!isNew && this.CustomReportParameters != null)
                {
                    this.CustomReportParameters.ForEach(parameter =>
                    {
                        parameter.CustomReportId = this.Id;
                        parameter.Save(transaction);
                    });
                }
                transaction.Commit();
            }

            if (createLinkedMenu)
            {
                try
                {
                    var urlToLoadReport = string.Format("/Report/CustomReport/CustomReportViewer.aspx?reportId={0}", this.Id);
                    var url = PageUrl.GetByWebPageURL(urlToLoadReport);
                    if (url == null)
                    {
                        // {0}/{1}/... are EF's own positional-parameter tokens (bound as real
                        // SqlParameters by Database.ExecuteSqlCommand), not string.Format
                        // substitutions - this.Name is free text from the report-creation form
                        // and must never be concatenated into the SQL text directly.
                        PageUrl.ExecuteQuery("INSERT INTO [dbo].[DI_M_PageUrl] ( [Url],[CreatedDate] ) VALUES({0}, SYSDATETIME() )", urlToLoadReport);
                        url = PageUrl.GetByWebPageURL(urlToLoadReport);
                    }
                    if (url != null)
                    {
                        var uniqueName = string.Format("REPORT_{0}", this.Name.Trim().Replace(" ", "_").ToUpper());
                        WebPage.ExecuteQuery("INSERT INTO [dbo].[DI_M_Page] ( [UniqueName] ,  [Name] , [PageUrlId],[CreatedDate]) VALUES ({0}, {1}, {2}, SYSDATETIME() )", uniqueName, this.Name, url.Id);

                        var page = WebPage.GetByUniqueName(uniqueName);

                        if (page != null)
                        {
                            TabMenu.ExecuteQuery("INSERT INTO [dbo].[DI_M_TabMenu] ([ParentTabMenuId],[UniqueName], [MenuName] , [PageId],[CreatedDate]) VALUES ({0}, {1}, {2}, {3}, SYSDATETIME() )",
                                this.LinkedToMenu, uniqueName, this.Name, page.Id);

                            PagePermission.ExecuteQuery("INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( {0} , 1, NULL,SYSDATETIME() );INSERT INTO [dbo].[DI_M_PagePermission](PageId , PermissionId, UrlId,CreatedDate) VALUES( {0} , 12, {1},SYSDATETIME() );",
                                page.Id, url.Id);

                            CustomReport.ExecuteQuery("UPDATE DI_M_CustomReport SET PageId = {0}, UniqueName = {2} WHERE Id = {1} AND islinkedToMenu = 1", page.Id, this.Id, uniqueName);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            //return this.Id;
            return 0;
        }


        private class RowComparer : IComparer<DataRow>
        {
            public Dictionary<int, SortOrder> SortColumns { get; set; }

            public int Compare(System.Data.DataRow x, System.Data.DataRow y)
            {
                int returnValue = 0;
                foreach (int key in SortColumns.Keys)
                {
                    int compareResult;

                    // Check for nulls 
                    if (x.ItemArray[key] == DBNull.Value
                               && y.ItemArray[key] == DBNull.Value)
                        compareResult = 0;
                    else if (x.ItemArray[key] == DBNull.Value)
                        compareResult = -1;
                    else if (y.ItemArray[key] == DBNull.Value)
                        compareResult = 1;
                    else
                    {
                        // Execute the compare based on the column type 
                        if (x.Table.Columns[key].DataType.Name ==
                                                    typeof(Decimal).Name)
                            compareResult =
                                 Decimal.Compare((decimal)x.ItemArray[key],
                                                 (decimal)y.ItemArray[key]);

                        else if (x.Table.Columns[key].DataType.Name ==
                                                    typeof(DateTime).Name)
                            compareResult =
                                 DateTime.Compare((DateTime)x.ItemArray[key],
                                                  (DateTime)y.ItemArray[key]);
                        else
                            // Compare anything else as a string 
                            compareResult =
                                 String.Compare(x.ItemArray[key].ToString(),
                                                y.ItemArray[key].ToString());
                    }

                    if (compareResult != 0)
                    {
                        returnValue =
                           SortColumns[key] == SortOrder.Ascending ?
                                          compareResult : -compareResult;
                        break;
                    }
                }
                return returnValue;
            }
        }

    }
}