using DI.Chakra.DBConfigHelper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Master;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web
{
    public class UIBaseUC : UserControl
    {
        public string oldData = string.Empty;
        AppTextSource appTextSource = null;
        public UIBaseUC()
        {
            appTextSource = AppTextSource.Instance;
        }
        private string _invalidApprovalTypeMatrixMessage;
        
        /// <summary>
        /// Used to get the existing objects and write to the log before deletion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetSerializeString<T>(long id)
        {
            var serializeString = string.Empty;
            try
            {
                var data = DomainApiClient<T>.GetById(id);
                if(data != null)
                {
                    serializeString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                }
            }
            catch (Exception ex)
            {
            }
            return serializeString;
        }
        public void LogDeletedRecords<T>(long id,string matrixName)
        {
            UpdatedUILog<T>(id, matrixName, isDelete:true);
        }
        public void UpdatedUILog<T>(long id,string matrixName ,bool isDelete = false, string data=null)
        {
            if (id > 0)
            {
                try
                {
                    if(data == null)
                        data = GetSerializeString<T>(id);

                    var sess = new Application.SessionStatusImpl();
                    DILog.UILogger.WriteLog(sess.User.Id, LogLevelType.Info, string.Format("{3} : {4}:{0} {2} {1}", data, sess.User.UserName, isDelete? "Deleted by" : "Updated by", matrixName, isDelete?"Deleted Record":"Old Record"));
                }
                catch
                {
                }
            }
        }

        public void LoginsertRecords<T>(string endpointsystemname, string technicalrole, string conflictingtechnicalrole, string matrixName)
        {

            insertUILog<T>(0, endpointsystemname, technicalrole, conflictingtechnicalrole, matrixName, isinsert: true);
        }
        public void insertUILog<T>(long id, string endpointsystemname, string technicalrole, string conflictingtechnicalrole, string matrixName, bool isinsert = false, string data = null)
        {
            if (id == 0)
            {
                try
                {
                    if (data == null)
                        data = "Endpointsystemname: " + endpointsystemname + " TechnicalRole: " + technicalrole + " ConflictingTechnicalRole:" + conflictingtechnicalrole;
                    var sess = new Application.SessionStatusImpl();
                    DILog.UILogger.WriteLog(sess.User.Id, LogLevelType.Info, string.Format("{3} : {4}:{0} {2} {1}", data, sess.User.UserName, "created by", matrixName, "insert New Record"));
                }
                catch
                {
                }
            }
        }
        protected string InvalidApprovalTypeMatrixMessage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_invalidApprovalTypeMatrixMessage))
                    _invalidApprovalTypeMatrixMessage = MasterConfigHelper.InvalidApprovalTypeMatrixMessage;
                return _invalidApprovalTypeMatrixMessage;
            }
        }

        public ProtoMast4 MasterPage
        {
            get
            {
                return (ProtoMast4)Page.Master;
            }
        }
        public Unit GetUnit(int percent)
        {
            return Unit.Pixel(MasterPage.ScreenWidth * percent / 100);
        }

        public string T(string key)
        {
            return appTextSource.GetString(key);
        }
        protected ApprovalUserType GetApproverType(string approverType)
        {
            if (string.IsNullOrWhiteSpace(approverType))
                return ApprovalUserType.None;
            return (ApprovalUserType)(Convert.ToByte(approverType));
        }
        protected bool IsValidApproverType(string approverType)
        {
            if (string.IsNullOrWhiteSpace(approverType))
                return true;
            return approverType == "1" || approverType == "3" || approverType == "0";
        }

        public void ShowAlertMessage(DI.Chakra.Infrastructure.AlertType AlertType, string ErrorMessage, bool showWarning = false)
        {
            MasterPage.ShowAlert(AlertType, ErrorMessage,showWarning);
        }
        protected string GetSortExpression(GridSortExpressionCollection sortExpressions)
        {
            var resultSortExpress = string.Empty;
            if (sortExpressions.Count == 0)
                return resultSortExpress;
            var sortingFieldName = sortExpressions[0].FieldName;
            var sortOrder = sortExpressions[0].SortOrderAsString();
            foreach (GridSortExpression item in sortExpressions)
            {
                resultSortExpress += string.Format("{0} {1},", item.FieldName, item.SortOrderAsString());
            }
            return resultSortExpress.Trim(',').Trim();
        }
        protected string GetFilterExpression(string filterExpression)
        {
            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }
                filterExpression = string.Join(" AND ", finalOutput);
                return filterExpression;
            }
            return filterExpression;
        }

        public bool ValidateAccountName(string accname, EndpointType EndpointType, long? endpointSystemId, string EmailAddress = null)
        {
            // TextBox control = (TextBox)Control;
            // string accname = control.Text;
            bool isExists = false;
            bool isExistsFullName = false;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointsystemId", endpointSystemId.ToString());
            parameter.Add("lastName", accname.Trim());


            if (EndpointType == EndpointType.ActiveDirectory)
            {
                parameter.Add("sAMAccountName", accname.Trim());
                isExists = DomainApiClient<Dto.ADSUserInfo>.GetByAction<bool>
                    (DomainApiAction.Endpoint.ActiveDirectory.ADSUserInfo.DoesExists, parameter);
            }

            else if (EndpointType == EndpointType.SAP)
            {
                parameter.Add("userName", accname.Trim());
                isExists = DomainApiClient<Dto.SAPUserinfo>.GetByAction<bool>(DomainApiAction.Endpoint.SAP.SAPUserinfo.DoesExists, parameter);
            }
            else if (EndpointType == EndpointType.SAPEnterprisePortal)
            {
                parameter.Add("userName", accname.Trim());
                isExists = DomainApiClient<Dto.SAPEPUserInfo>.GetByAction<bool>(DomainApiAction.Endpoint.SAPEP.SAPEPUserinfo.DoesExists, parameter);
            }
            string spc = "!@#$%^&*()+=-[]\\\';,./{}|\":<>?";
            if (isExistsFullName)
            {
                ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Full Name already exists. Please try with another Name.");
                return false;
            }
            else if (isExists)
            {
                //if (EndpointType == EndpointType.LotusNotes)
                //{
                //    ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Short Name already exists. Please try with another Name.");
                //    return false;
                //}
                //else
                //{
                ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Account Name already exists. Please try with another Name.");
                return false;
                //}
            }
            else
            {

                if (spc.Any(c => accname.Contains(c)))
                {
                    ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Special characters not allowed in Account Name.");
                    return false;
                }

            }
            //if (EndpointType != EndpointType.LotusNotes)
            //{
            if (accname.Any(x => Char.IsWhiteSpace(x)))
            {
                ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Space is not allowed in Account Name.");
                return false;
            }
            //}
            Int32 length = 0;
            if(EndpointType == EndpointType.Office365)
            {
                length = MasterConfigHelper.Office365AccountNameMinimumLength;
            }
            else
            {
                length = MasterConfigHelper.AccountNameMinimumLength;
            }
            if (accname.Length < length)
            {
                //if (EndpointType != EndpointType.LotusNotes)
                //{
                ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Account Name is too short.");
                //}
                //else
                //{
                //    ShowAlertMessage(DI.Chakra.Infrastructure.AlertType.Error, "Full Name is too short.");
                //}
                return false;
            }
            return true;
        }
        RadGrid radGridNew;
        public void ExportNew(RadGrid gridControlNew, ACLAction actionType, string fileName=null, List<string> excludeColumnList = null)
        {
            try
            {
                radGridNew = gridControlNew;
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (gridControlNew.Items.Count > 0)
                {
                    if (actionType == ACLAction.ExcelAll || actionType == ACLAction.CSVAll)
                    {
                        foreach (GridColumn column in gridControlNew.MasterTableView.Columns)
                        {
                            if (excludeColumnList != null && excludeColumnList.Count > 0 && excludeColumnList.Contains(column.UniqueName))
                                column.Display = false;
                            else
                                column.Display = true;
                        }
                    }

                    foreach (GridColumn column in gridControlNew.MasterTableView.Columns)
                    {
                        if (column is GridBoundColumn && column.UniqueName.ToLower().Contains("dateshort"))
                        {
                            ((GridBoundColumn)column).DataType = Type.GetType("System.String");

                        }
                        else if (column is GridBoundColumn && column.DataType == Type.GetType("System.String"))
                        {
                            ((GridBoundColumn)column).DataFormatString = null;
                            ((GridBoundColumn)column).DataType = Type.GetType("System.String");
                        }
                        else if (column is GridBoundColumn && column.UniqueName.ToLower().Contains("date"))
                        {
                            ((GridBoundColumn)column).DataType = Type.GetType("System.DateTime");
                            ((GridBoundColumn)column).DataFormatString = "{0:dd-MM-yyyy hh:mm tt}";
                        }
                    }

                    //gridControlNew.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControlNew_ExportCellFormatting);
                    //gridControlNew.ExportSettings.SuppressColumnDataFormatStrings = true;
                    gridControlNew.ExportSettings.HideStructureColumns = true;
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        gridControlNew.ExportSettings.FileName = fileName;
                    }
                    gridControlNew.ExportSettings.OpenInNewWindow = true;
                    gridControlNew.ExportSettings.Excel.Format = GridExcelExportFormat.ExcelML;
                    gridControlNew.ExportSettings.IgnorePaging = true;
                    gridControlNew.ExportSettings.ExportOnlyData = true;
                    gridControlNew.GridLines = GridLines.Both;
                    gridControlNew.AllowFilteringByColumn = true;
                    //gridControlNew.InfrastructureExporting += GridControlNew_InfrastructureExporting;
                    if (actionType == ACLAction.CSV || actionType == ACLAction.CSVAll)
                    {
                        gridControlNew.ExportSettings.ExportOnlyData = false;
                        gridControlNew.MasterTableView.ExportToCSV();
                    }
                    else if (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll)
                    {
                        gridControlNew.ExportSettings.ExportOnlyData = true;
                        gridControlNew.MasterTableView.ExportToExcel();
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No data to export");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        
        private void GridControlNew_InfrastructureExporting(object sender, GridInfrastructureExportingEventArgs e)
        {
            if (radGridNew == null)
                return;

            var table = e.ExportStructure.Tables[0];
            for (int col = 0; col < radGridNew.Columns.Count; col++)
            {
                for (int row = 1; row <= table.Rows.Count; row++)
                {
                    if (radGridNew.Columns[col].HeaderText.Contains("Date"))
                    {
                        table.Cells[col + 1, row].Format = "dd/MM/yyyy";
                    }
                    else
                    {
                        table.Cells[col + 1, row].Format = "@";
                    }
                }
            }
        }

        void gridControlNew_ExportCellFormatting(object sender, ExportCellFormattingEventArgs e)
        {
            if ((e.FormattedColumn.UniqueName).ToLower().Contains("date"))
            {
                e.Cell.Style["mso-number-format"] = "dd/MM/yyyy";
            }
            else
            {
                e.Cell.Style["mso-number-format"] = "@";
            }
            GridDataItem item = e.Cell.Parent as GridDataItem;
            e.Cell.HorizontalAlign = HorizontalAlign.Center;
        }


        // private bool GetIsExistsAccessRequestForServiceAndShared(AdHocAccessRequest AdhocrequestObj)
        // {
        //  bool isExists = false;
        //int requesterUserId = 0;

        //var inStoreRequest = GetFromInterim().Where(item =>
        //item.RequesteeUserId == AdhocrequestObj.RequesteeUserId &&
        //item.AdhocActionType == AdhocrequestObj.AdhocActionType &&
        //item.EndpointSystemType == AdhocrequestObj.EndpointSystemType &&
        //item.EntitlementDetailType == AdhocrequestObj.EntitlementDetailType &&
        //item.AccountType == AdhocrequestObj.AccountType &&
        //item.EndpointSystemId == AdhocrequestObj.EndpointSystemId);

        //if (inStoreRequest != null)
        //{
        //    foreach (var accessRequest in inStoreRequest)
        //    {
        //        requesterUserId = accessRequest.RequesterUserId;
        //        // AD check
        //        if (accessRequest.EntitlementDetailType == ApplicationType.ADSDCLocal || accessRequest.EntitlementDetailType == ApplicationType.ADSingaporePowerLocal)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocADRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocADRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocADSharedAccount != null && adHocExistsEntitlementDetail.AdHocADSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocADSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocADSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }
        //        // Lotus Notes
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.LotusNotes)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocLNSRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocLNSRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocLNSSharedAccount != null && adHocExistsEntitlementDetail.AdHocLNSSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocLNSSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocLNSSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // SAP - SAP-EP, CE and InternetPortal
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.CE || accessRequest.EntitlementDetailType == ApplicationType.SAP_EP ||
        //            accessRequest.EntitlementDetailType == ApplicationType.InternetPortal)
        //        {
        //            var adHocExistsEntitlementDetail = (AdhocSAPEPRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdhocSAPEPRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPEPSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPEPSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPEPSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPEPSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // SAP - MSSL
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.MSSL_ISU || accessRequest.EntitlementDetailType == ApplicationType.MSSL_CRM ||
        //            accessRequest.EntitlementDetailType == ApplicationType.MSSL_BI)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocMSSLRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocMSSLRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPMSSLSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPMSSLSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPMSSLSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPMSSLSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }
        //        // SAP - EBS
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.EBS_ISU || accessRequest.EntitlementDetailType == ApplicationType.EBS_CRM ||
        //            accessRequest.EntitlementDetailType == ApplicationType.EBS_BI)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocEBSRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocEBSRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPEBSSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPEBSSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPEBSSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPEBSSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // SAP - eLearning
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.eLearning)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocPLRELearningRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocPLRELearningRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPELearningSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPELearningSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPELearningSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPELearningSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }
        //        // SAP - eLearning
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.eRecruitment)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocPERERecruitmentRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocPERERecruitmentRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPERecruitmentSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPERecruitmentSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPERecruitmentSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPERecruitmentSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //         // SAP - SAP_PI
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.SAP_PI)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocSAPPIRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocSAPPIRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPPISharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPPISharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPPISharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPPISharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // SAP - SAP_GRC
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.SAP_GRC)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocSAPGRCRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocSAPGRCRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if ((adHocCurrentEntitlementDetail.AdHocSAPGRCSharedAccount != null && adHocExistsEntitlementDetail.AdHocSAPGRCSharedAccount != null) &&
        //                    adHocCurrentEntitlementDetail.AdHocSAPGRCSharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocSAPGRCSharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }
        //        // SAP - FLINS_BI
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.FLINS_BI)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocSAPFLINSBIRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocSAPFLINSBIRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if (adHocCurrentEntitlementDetail.AdHocFLINSBISharedAccount.AccountName.ToUpper().Equals(adHocExistsEntitlementDetail.AdHocFLINSBISharedAccount.AccountName.ToUpper()))
        //                {
        //                    isExists = true;
        //                    break;
        //                }
        //            }
        //        }

        //        // SAP - HRMS
        //        else if (accessRequest.EntitlementDetailType == ApplicationType.SAP)
        //        {
        //            var adHocExistsEntitlementDetail = (AdHocSAPHRFLINSRequest)accessRequest.GetEntitlementDetail();
        //            var adHocCurrentEntitlementDetail = (AdHocSAPHRFLINSRequest)AdhocrequestObj.GetEntitlementDetail();
        //            if (adHocCurrentEntitlementDetail != null && adHocExistsEntitlementDetail != null)
        //            {
        //                if (adHocExistsEntitlementDetail.AdHocSAPHRFLINSRequestDetails != null && adHocExistsEntitlementDetail.AdHocSAPHRFLINSRequestDetails.Count != 0 &&
        //                   adHocCurrentEntitlementDetail.AdHocSAPHRFLINSRequestDetails != null && adHocCurrentEntitlementDetail.AdHocSAPHRFLINSRequestDetails.Count != 0)
        //                {
        //                    foreach (AdHocSAPHRFLINSRequestDetail requestDetails in adHocExistsEntitlementDetail.AdHocSAPHRFLINSRequestDetails)
        //                    {
        //                        if ((requestDetails.AdHocSAPHRFLINSSharedAccount != null && adHocCurrentEntitlementDetail.AdHocSAPHRFLINSRequestDetails[0].AdHocSAPHRFLINSSharedAccount != null) &&
        //                            requestDetails.AdHocSAPHRFLINSSharedAccount.AccountName.ToUpper().Equals(adHocCurrentEntitlementDetail.AdHocSAPHRFLINSRequestDetails[0].AdHocSAPHRFLINSSharedAccount.AccountName.ToUpper()))
        //                        {
        //                            isExists = true;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //    if (isExists)
        //    {
        //        string requesterName = new UserFacade().GetEndPointUserDetailByEndpointUserID(requesterUserId, "DIUser");

        //        ShowAlertMessage(AlertType.Error, "Same access already requested by " + requesterName + "!");
        //        return true;
        //    }
        //}
        // return isExists;
        //}

        public long? ContextObjectId
        {
            get
            {
                return ((UIBasePage)this.Page).ContextObjectId;
            }
            set
            {
                ((UIBasePage)this.Page).ContextObjectId = value;
            }
        }

        public string TypeofRequest
        {
            get
            {
                return ((UIBasePage)this.Page).TypeofRequest;
            }
            set
            {
                ((UIBasePage)this.Page).TypeofRequest = value;
            }
        }

        public EndpointType EndpointType
        {
            get
            {
                return ((UIBasePage)this.Page).EndpointType;
            }
            set
            {
                ((UIBasePage)this.Page).EndpointType = value;
            }
        }

        public string EntitlementTypeValue
        {
            get
            {
                return ((UIBasePage)this.Page).EntitlementTypeValue;
            }
            set
            {
                ((UIBasePage)this.Page).EntitlementTypeValue = value;
            }
        }

        public void Export(RadGrid gridControl, ACLAction actionType)
        {
            try
            {
                if (gridControl.Items.Count > 0)
                {
                    gridControl.ExportSettings.IgnorePaging = true;
                    gridControl.GridLines = GridLines.Both;
                    gridControl.AllowFilteringByColumn = false;
                    string filename = gridControl.ExportSettings.FileName;
                    gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);
                    if (actionType == ACLAction.CSV)
                    {
                        if (filename == "RiskScore" || filename == "RiskLevel" || filename == "RiskCode" || filename == "Mail-Server")
                        {
                            gridControl.ExportSettings.ExportOnlyData = false;
                        }
                        else
                        {
                            gridControl.ExportSettings.ExportOnlyData = true;
                        }
                        gridControl.MasterTableView.ExportToCSV();
                    }
                    else if (actionType == ACLAction.Excel)
                    {
                        gridControl.ExportSettings.ExportOnlyData = true;
                        gridControl.MasterTableView.ExportToExcel();
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No data to export");
                }
            }
            catch
            {
                throw;
            }
        }

        public void DownloadTemplate(RadGrid gridControl, ACLAction actionType)
        {
            try
            {
                foreach (GridColumn item in gridControl.MasterTableView.Columns)
                {
                    if (item.HeaderText == string.Empty)
                    {
                        item.Visible = false;
                    }
                }
                gridControl.ExportSettings.IgnorePaging = true;
                gridControl.GridLines = GridLines.Both;
                gridControl.AllowFilteringByColumn = true;
                gridControl.ExportSettings.ExportOnlyData = true;
                gridControl.ExportSettings.HideStructureColumns = true;
                gridControl.ExportCellFormatting += new EventHandler<ExportCellFormattingEventArgs>(gridControl_ExportCellFormatting);
                gridControl.MasterTableView.ExportToCSV();
            }
            catch
            {
                throw;
            }
        }

        void gridControl_ExportCellFormatting(object sender, ExportCellFormattingEventArgs e)
        {
            if ((e.FormattedColumn.UniqueName).ToLower().Contains("date"))
            {
                e.Cell.Style["mso-number-format"] = "Medium Date";
            }
            GridDataItem item = e.Cell.Parent as GridDataItem;
            e.Cell.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void SaveGridSettings(UserGridSettings gridSettings)
        {

        }
        protected void DeleteGridFilter(UserGridFilterSettings filterSettings)
        {

        }
        protected void SaveGridFilter(UserGridFilterSettings filterSettings)
        {

        }
        protected virtual void SetMandatory(HtmlGenericControl control, bool isMandatory)
        {
            var CssClass = control.Attributes["class"].Replace("required", "");
            control.Attributes.Add("class", isMandatory ? CssClass + " required" : CssClass);
        }
        protected virtual void SetMandatory(WebControl control, bool isMandatory)
        {
            var CssClass = control.CssClass.Replace("required", "");
            control.CssClass = isMandatory ? CssClass + " required" : CssClass;
        }
        public string PDFFileLeftCell
        {

            get
            {

                //return string.Format("<img src=\"~/Images/Clients/Logo/{0}\"  style=\"width:{1}px; height: {2}px;\" />", ResourceStore.Instance.Logo, ResourceStore.Instance.Logo_Width, ResourceStore.Instance.Logo_Height);
                return string.Format("<img src=\"~/Images/Clients/{0}\"  style=\"width:{1}px; height: {2}px;\" />", T("App.Branding.Logo"), 30, 30);
            }

        }

        protected virtual IDMSReportParam GetReportHeaderColumnMapping()
        {
            return new IDMSReportParam();
        }

        #region Export data - working properly

        protected void ExportData<T>(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<T> userList, bool showAllColumn, bool isExcel, string workBookName)
        {
            if (isExcel)
                ExportDataToExcel(grid, userList, showAllColumn, workBookName);
            else
                ExportDataToCSV(grid, userList, showAllColumn, workBookName);
        }
        void ExportDataToExcel<T>(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<T> userList, bool showAllColumn, string workBookName)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                byte[] report = null;
                //collect grid columns
                Dictionary<string, string> columns = new Dictionary<string, string>();

                foreach (GridHeaderItem itm in grid.MasterTableView.GetItems(GridItemType.Header))
                {
                    foreach (GridColumn col in grid.MasterTableView.Columns)
                    {
                        if (!string.IsNullOrWhiteSpace(col.UniqueName))
                        {
                            if (showAllColumn)
                                columns.Add(col.UniqueName, col.HeaderText);
                            else if (!showAllColumn && col.Display)
                                columns.Add(col.UniqueName, col.HeaderText);
                        }
                    }
                }

                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();

                var workSheet = excel.Workbook.Worksheets.Add(workBookName);

                //Header
                // loop top level users
                // add workcell entry for top level
                // TOP User header
                int currentRow = 1;
                int colStartPosition = 0;
                int colNum = colStartPosition;

                foreach (var column in columns)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = column.Value;
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                //move the row index next to header
                currentRow++;
                foreach (var usr in userList)
                {
                    //usr.GetType().GetProperty(propertyName).GetValue(usr, null);
                    colStartPosition = 0;
                    colNum = colStartPosition;
                    foreach (var column in columns)
                    {
                        workSheet.Cells[currentRow, ++colNum].Value = Convert.ToString(usr.GetType().GetProperty(column.Key).GetValue(usr, null));
                    }
                    currentRow++;
                }

                report = excel.GetAsByteArray();

                // Fit the columns according to its content
                for (int i = 1; i < colNum; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                if (report != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("Employee.xlsx"));
                    response.BinaryWrite(report);
                    response.End();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        void ExportDataToCSV<T>(DI.Chakra.Web.CustomControl.IACMRadGridControl grid, List<T> userList, bool showAllColumn, string workBookName)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                StringBuilder objWriter = new StringBuilder();
                byte[] report = null;
                //collect grid columns
                List<string> columns = new List<string>();
                foreach (GridHeaderItem itm in grid.MasterTableView.GetItems(GridItemType.Header))
                {
                    foreach (GridColumn col in grid.MasterTableView.Columns)
                    {
                        if (!string.IsNullOrWhiteSpace(col.UniqueName))
                            columns.Add(col.UniqueName);
                    }
                }

                //Header
                string header = string.Join(",", columns);
                objWriter.AppendLine(header);

                foreach (var usr in userList)
                {
                    List<string> towrite = new List<string>();

                    foreach (var column in columns)
                    {
                        towrite.Add(Convert.ToString(usr.GetType().GetProperty(column).GetValue(usr, null)));
                    }

                    objWriter.AppendLine(string.Join(",", towrite));
                }
                report = Encoding.ASCII.GetBytes(objWriter.ToString());

                if (report != null)
                {
                    HttpResponse response = HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("Employee.csv"));
                    response.BinaryWrite(report);
                    response.End();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }
        #endregion
        public void GenericExport(dynamic reportData, ACLAction actionType, string reportName="Report")
        {
            if (string.IsNullOrWhiteSpace(reportName)) reportName = "Report";
            Session["ExportStatus"] = "false";
            var exportFileType = (actionType == ACLAction.Excel || actionType == ACLAction.ExcelAll) ? "xlsx" : "csv";
            byte[] streamData = null;
            IDMSReportParam param = GetReportHeaderColumnMapping();
            if (param != null && param.ReportDataColumns != null && param.ReportDataColumns.Count > 0)
            {
                var reportSheetName = reportName;
                var temp = HttpContext.Current.Request.PhysicalApplicationPath + "\\temp\\";
                string filePath = string.Empty;
                try
                {
                    if (!System.IO.Directory.Exists(temp)) System.IO.Directory.CreateDirectory(temp);
                    filePath = string.Format("{0}\\{1}_{2}.{3}", temp, reportSheetName, DateTime.Now.ToString(Constants.Report_Date_Format), exportFileType);
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
                                foreach (var item in param.ReportDataColumns)
                                {
                                    workSheet.Cells[currentRow, colNum].Value = item.Header;
                                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                    colNum++;
                                }
                                currentRow++;

                                foreach (var data in reportData)
                                {

                                    colStartPosition = 1;
                                    colNum = colStartPosition;
                                    foreach (var item in param.ReportDataColumns)
                                    {
                                        workSheet.Cells[currentRow, colNum].Value = data.GetReportPropertyValue(item.DataFieldName, item.Format);
                                        try
                                        {
                                            if(!string.IsNullOrWhiteSpace(item.Format))
                                                workSheet.Cells[currentRow, colNum].Style.Numberformat.Format = item.Format;
                                        }   
                                        catch
                                        {
                                        }
                                        colNum++;
                                    }
                                    currentRow++;
                                }
                                workSheet.Cells.AutoFitColumns();
                                objExcelPackage.Workbook.Properties.Title = reportName;
                                streamData = objExcelPackage.GetAsByteArray();
                            }
                        }
                    }
                    else if (exportFileType.ToUpper() == "CSV")
                    {
                        using (StreamWriter stream = new StreamWriter(filePath))
                        {
                            stream.WriteLine(string.Join(",",param.ReportDataColumns.Select(i=> string.Format("\"{0}\"", i.Header))));
                            foreach (var data in reportData)
                            {
                                stream.WriteLine(string.Join(",",param.ReportDataColumns.Select(item => string.Format("\"\t{0}\"", data.GetReportPropertyValue(item.DataFieldName, item.Format)??""))));
                            }
                        }
                        streamData = File.ReadAllBytes(filePath);
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
            if (streamData != null)
            {
                Session["ExportStatus"] = "true";
                HttpResponse response = Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("Content-Length", streamData.Length.ToString());
                response.AddHeader("Content-Disposition", "attachment;filename=" + string.Format("{0}{2}.{1}", reportName,exportFileType,DateTime.Now.ToString(Constants.Report_Date_Format)));
                response.BinaryWrite(streamData);
                response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

    }
    
}