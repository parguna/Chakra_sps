using System;
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using DI.Chakra.Dto.Core;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Logger;
using OfficeOpenXml;
using System.Drawing;

namespace DI.Chakra.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AttestationDataService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AttestationDataService.svc or AttestationDataService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AttestationDataService : IAttestationDataService
    {
        [WebInvoke(Method = "POST", UriTemplate = "GetAttestationUserComparisionData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        public UserEntitlementComparisionResult GetAttestationUserComparisionData(long taskId, int draw, int start, int length, DataTableSearch search = null)
        {
            UserEntitlementComparisionResult response = new UserEntitlementComparisionResult();
            //length = 500;
            try
            {
                var param = new Dictionary<string, string>();
                param.Add("taskId", taskId.ToString());
                param.Add("draw", draw.ToString());
                param.Add("start", start.ToString());
                param.Add("length", length.ToString());
                APIAttestationSearch aPIAttestation = new APIAttestationSearch();
                if (search != null)
                {
                    aPIAttestation.EndpointIds = search != null ? search.EndpointIds : null;
                    aPIAttestation.Value = search.Value;
                    aPIAttestation.Regex = search.Regex;
                    aPIAttestation.AccountType = search.AccountType;
                }
                aPIAttestation.TaskId = taskId;
                aPIAttestation.Draw = draw;
                aPIAttestation.Start = start;
                aPIAttestation.Length = length;
                //param.Add("search", search != null && !string.IsNullOrWhiteSpace(search.Value) ? search.Value.Trim() : "");
                var previousApprovers = DomainApiClient<Dto.FilterData>.
                    GetByAction<PresentationGridPagedData<ViewUserEntitlementData>>
                    ("GetAttestationUserComparisionData", null, aPIAttestation);
                response = new UserEntitlementComparisionResult()
                {
                    draw = draw,
                    recordsTotal = previousApprovers.TotalCount,
                    recordsFiltered = previousApprovers.TotalCount, // In this example, no filtering is applied
                    data = previousApprovers.Data
                };
            }
            catch (Exception ex)
            {
                DILog.UILogger.WriteLog(null,LogLevelType.Error,$"GetAttestationUserComparisionData:{ex}");
                response.data = new List<ViewUserEntitlementData>();
            }
            return response;    
        }
        [WebInvoke(Method = "POST", UriTemplate = "GetTaskFilterData", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        public List<FilterData> GetTaskFilterData(TaskFilter taskFilter)
        {
            var param = new Dictionary<string, string>();
            //param.Add("taskId", taskFilter.TaskId.ToString());
            return DomainApiClient<Dto.FilterData>.
                GetByAction<List<FilterData>>
                ("GetFilterDataByType", null, taskFilter);
        }
        [WebInvoke(Method = "POST", UriTemplate = "UpdateAttestationTaskActionByUser", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        public bool UpdateAttestationTaskActionByUser(DTORequestParameter dTORequest)
        {
            var param = new Dictionary<string, string>();
            //param.Add("taskId", dTORequest.TaskId.ToString());
            //param.Add("endpointId", dTORequest.EndpointId.ToString());
            //param.Add("userId", dTORequest.UserId.ToString());
            //param.Add("entitlement", dTORequest.Entitlement);
            //param.Add("userName", dTORequest.UserName);
            //param.Add("accountType",((int)dTORequest.AccountType).ToString());
            //param.Add("accessType", dTORequest.AccessType);
            //param.Add("level", dTORequest.Level.ToString());
            //param.Add("actionUserId", dTORequest.ActionUserId.ToString());
            //EntitlementType entType = (EntitlementType)Enum.Parse(typeof(EntitlementType), dTORequest.EntitlementType);
            //param.Add("entitlementType", ((int)entType).ToString());
            //DTORequestParameter parameter = new DTORequestParameter();
            return DomainApiClient<Dto.FilterData>.
                GetByAction<bool>
                ("UpdateAttestationTaskActionByUser", null, dTORequest);
        }
        [WebInvoke(Method = "POST", UriTemplate = "GetEmployeeInformation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json)]
        public Dictionary<string, List<FilterData>> GetEmployeeInformation(long taskId,DataTableSearch search)
        {
            var param = new Dictionary<string, string>();
            TaskFilter taskFilter = new TaskFilter();
            taskFilter.TaskId = taskId;
            if (search != null)
                taskFilter.AccountType = search.AccountType;

            if (search != null && search.EndpointIds  != null && search.EndpointIds.Count > 0)
            {
                taskFilter.EpIds = search.EndpointIds;
            }
            return DomainApiClient<Dto.FilterData>.
                GetByAction<Dictionary<string, List<FilterData>>>
                ("GetEmployeeInformation", null, taskFilter);
        }

        
        [WebInvoke(Method = "POST", UriTemplate = "GetTaskHasUnsavedChanges", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        public bool GetTaskHasUnsavedChanges(long taskId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            parameter.Add("currentUser", "");
            var IsUnSavedChangesAvailable = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                    (DomainApiAction.Tasks.Task.GetTaskHasUnsavedChanges, parameter);
            return IsUnSavedChangesAvailable;
        }

        [WebInvoke(Method = "POST", UriTemplate = "SavedChanges", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        public bool SavedChanges(long taskId, string userName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            parameter.Add("currentUser", userName);
            var saved = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.SaveAttestationReview, parameter);

            return saved;
        }


        [WebInvoke(Method = "POST", UriTemplate = "RevertTaskInInterimStore", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        public bool RevertTaskInInterimStore(long taskId, string userName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            parameter.Add("currentUser", userName);
            return DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.RevertAttestationReview, parameter);
        }
        [WebInvoke(Method = "POST", UriTemplate = "GetAttestationTaskInformation", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        public AttestationTaskInformation GetAttestationTaskInformation(long taskId, string userName)
        {
            try
            {
                var task = DomainApiClient<Dto.Task>.GetById(taskId);
                Dto.AttestationCampaign campaign = null;
                if (task.AttestationCampaignId.HasValue)
                    campaign = DomainApiClient<Dto.AttestationCampaign>.GetById(task.AttestationCampaignId.Value);
                Dictionary<string, string> parameterId = new Dictionary<string, string>();
                parameterId.Add("id", Convert.ToString(task.AttestationId));
                Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>
                    (DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameterId);
                var currentApprover = attestation.WorkflowConfiguration.WorkflowConfigurationDetails.
                    FirstOrDefault(item => item.SequenceNumber == task.WorkflowStateId);
                var username = "-";
                if (task.ActionUserId.HasValue)
                {
                    username = DomainApiClient<Dto.User>.GetById(task.ActionUserId.Value).DisplayName;
                }
                else if (task.AssignedTo.HasValue)
                {
                    username = DomainApiClient<Dto.User>.GetById(task.AssignedTo.Value).DisplayName;
                }
                return new AttestationTaskInformation()
                {
                    Approver = username,
                    SubmittedDate = task.CreatedDate.AddDays(currentApprover.ResponseTimeInDays).ToString(),
                    RequestedDate = task.CreatedDate.ToString(),
                    AttestationType = attestation.AttestationType.ToString(),
                    AttestationDescription = attestation.AttestationDescription,
                    AttestationName = attestation.AttestationName,
                    Level = task.WorkflowStateId.ToString(),
                    CampaignName = campaign?.CampaignName,
                    Status = task.Action.ToString()
                };
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        [WebInvoke(Method = "POST", UriTemplate = "SubmitAttestationTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        public bool SubmitAttestationTask(long taskId, string userName,bool isSaved)
        {
            bool status = false;
            try
            {
                isSaved = true;
                if (CanSubmitTask(taskId) == false)
                {
                    isSaved = SaveTaskInInterimStore(taskId, userName);
                }
                var task = getCurrentTask(taskId);
                Dto.Presentation.PresentationWorkflowCriteriaBase _criteria = new Dto.Presentation.ApprovalActionCriteria()
                {
                    RequestType = RequestType.Attestation,
                    TaskId = taskId,
                    WorkflowInstanceId = task.WorkflowInstanceId
                };
                status = DomainApiClient<Object>.GetByAction<bool>("WorkflowController", DomainApiAction.WorkflowProcessor.SubmitWorkflowCriteria, null, _criteria);
            }
            catch (Exception ex)
            {
                return false;
            }

            return status;
        }
        bool SaveTaskInInterimStore(long taskId,string userName)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            parameter.Add("currentUser", userName);
            var saved = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.SaveAttestationReview, parameter);
            return saved;
        }
        bool CanSubmitTask(long taskId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            var canSubmit = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<bool>
                        (DomainApiAction.Tasks.Task.CanSubmitTask, parameter);
            return canSubmit;
        }
        Dto.Task getCurrentTask(long taskId)
        {
            return DomainApiClient<Dto.Task>.GetById(taskId);
        }

        [WebInvoke(Method = "POST", UriTemplate = "GetAccountTypes", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        public List<AccountType> GetAccountTypes(long taskId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("taskId", taskId.ToString());
            return DomainApiClient<Dto.FilterData>.GetByAction<List<AccountType>>("GetAccountTypes", parameter);
        }

        [WebInvoke(Method = "POST", UriTemplate = "GetAccountStatusInformation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json)]
        public List<FilterData> GetAccountStatusInformation(long taskId, long userId, DataTableSearch search)
        {
            var param = new Dictionary<string, string>();
            TaskFilter taskFilter = new TaskFilter();
            taskFilter.TaskId = taskId;
            if (search != null)
                taskFilter.AccountType = search.AccountType;

            if (search != null)
            {
                if(search.EndpointIds != null && search.EndpointIds.Count > 0)
                    taskFilter.EpIds = search.EndpointIds;

                taskFilter.UserId = userId;
                taskFilter.AccountName = search.Value;
            }
            return DomainApiClient<Dto.FilterData>.GetByAction<List<FilterData>>("GetAccountStatusInformation", null, taskFilter);
        }

        [WebInvoke(Method = "POST", UriTemplate = "DownloadUserComparisionData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        public byte[] DownloadUserComparisionData(long taskId)
        {
            DataTableSearch search = new DataTableSearch();
            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
            OfficeOpenXml.ExcelWorksheet excelSheet;
            //ExcelWorksheet workSheet = CreateWorksheet(excel, sheetName);
            search.AccountType = AccountType.User;
            excelSheet = ExportFileSystemHierarchyExcel(taskId, search, "UserAccount");
            if(excelSheet != null)
                excel.Workbook.Worksheets.Add("UserAccount", excelSheet);
            search.AccountType = AccountType.Secondary;
            excelSheet = ExportFileSystemHierarchyExcel(taskId, search, "SecondaryAccount");
            if (excelSheet != null)
                excel.Workbook.Worksheets.Add("SecondaryAccount", excelSheet);
            search.AccountType = AccountType.Shared;
            excelSheet = ExportFileSystemHierarchyExcel(taskId, search, "SharedAccount");
            if (excelSheet != null)
                excel.Workbook.Worksheets.Add("SharedAccount", excelSheet);
            search.AccountType = AccountType.Service;
            excelSheet = ExportFileSystemHierarchyExcel(taskId, search, "ServiceAccount");
            if (excelSheet != null)
                excel.Workbook.Worksheets.Add("ServiceAccount", excelSheet);
            var data = excel.GetAsByteArray();
            return data;
        }
        private OfficeOpenXml.ExcelWorksheet ExportFileSystemHierarchyExcel(long taskId, DataTableSearch search, string sheetName = "sheet1")
        {
            ExcelPackage package = new ExcelPackage();
            var task = DomainApiClient<Dto.Task>.GetById(taskId);
            // Create a new ExcelWorksheet separately
            ExcelWorksheet workSheet = CreateWorksheet(package, sheetName);
            var result = new Dictionary<string, List<FilterData>>();
            TaskFilter taskFilter = new TaskFilter
            {
                TaskId = taskId
            };
            if (search != null)
                taskFilter.AccountType = search.AccountType;

            if (search != null && search.EndpointIds != null && search.EndpointIds.Count > 0)
            {
                taskFilter.EpIds = search.EndpointIds;
            }
            result = DomainApiClient<Dto.FilterData>.
                    GetByAction<Dictionary<string, List<FilterData>>>
                    ("GetEmployeeInformation", null, taskFilter);
            if(result == null || result.Count == 0)
            {
                return null;
            }
            //var workSheet = excel.Workbook.Worksheets.Add("ATTESTATION REVIEW");
            //User header
            int currentRow = 1;
            int colStartPosition = 1;
            int colNum = colStartPosition;
            int secondRow = currentRow + 1;
            workSheet.Cells[currentRow, colNum].Value = "Application Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, colNum].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Cells[currentRow, colNum, secondRow, colNum].Merge = true;
            workSheet.Cells[currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            workSheet.Cells[currentRow, colNum].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            workSheet.Cells[currentRow, ++colNum].Value = "Role/Group";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, colNum, secondRow, colNum].Merge = true;
            workSheet.Cells[currentRow, colNum].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Cells[currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            workSheet.Cells[currentRow, colNum].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            workSheet.Cells[currentRow, ++colNum].Value = "Description";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, colNum, secondRow, colNum].Merge = true;
            workSheet.Cells[currentRow, colNum].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            workSheet.Cells[currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            workSheet.Cells[currentRow, colNum].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            //UserAccount Header Cell
            var totalCellCount = 0;
            foreach (var item in result)
            {
                var name = item.Key.Split('_');
                var index = 0;
                var currentColumn = ++colNum;

                if (item.Value.Count > 1)
                {
                    var totaluserName = item.Value.Select(i => i.UserName.ToUpper()).Distinct().ToList();
                    totalCellCount = totalCellCount + totaluserName.Count;
                    if (totaluserName.Count > 1)
                    {
                        index = currentColumn + totaluserName.Count - 1;
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Merge = true;
                        colNum = index;
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Value = name[0];
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[currentRow, currentColumn, currentRow, index].Style.Font.Bold = true;

                    }
                    else
                    {
                        workSheet.Cells[currentRow, currentColumn].Value = name[0];
                        workSheet.Cells[currentRow, currentColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        workSheet.Cells[currentRow, currentColumn].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        workSheet.Cells[currentRow, currentColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[currentRow, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;
                    }

                    foreach (var uname in totaluserName)
                    {
                        workSheet.Cells[secondRow, currentColumn].Value = uname;
                        workSheet.Cells[secondRow, currentColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[secondRow, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[secondRow, currentColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        workSheet.Cells[secondRow, currentColumn].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        ++currentColumn;
                    }
                    currentColumn = colNum;
                }
                else
                {

                    workSheet.Cells[currentRow, currentColumn].Value = name[0];
                    workSheet.Cells[currentRow, currentColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Cells[currentRow, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[currentRow, currentColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet.Cells[currentRow, currentColumn].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    workSheet.Cells[currentRow, currentColumn].Style.Font.Bold = true;

                    workSheet.Cells[secondRow, currentColumn].Value = item.Value.Select(i => i.UserName.ToUpper()).FirstOrDefault().ToString();
                    workSheet.Cells[secondRow, currentColumn].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Cells[secondRow, currentColumn].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[secondRow, currentColumn].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet.Cells[secondRow, currentColumn].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    totalCellCount += 1;
                }

            }

            //Entitlement Details
            UserEntitlementComparisionResult responses = new UserEntitlementComparisionResult();
            APIAttestationSearch aPIAttestation = new APIAttestationSearch();
            if (search != null)
            {
                aPIAttestation.EndpointIds = search != null ? search.EndpointIds : null;
                aPIAttestation.Value = search.Value;
                aPIAttestation.Regex = search.Regex;
                aPIAttestation.AccountType = search.AccountType;
            }
            aPIAttestation.TaskId = taskId;
            aPIAttestation.Draw = 1;
            aPIAttestation.Start = 0;
            var previousApprovers = DomainApiClient<Dto.FilterData>.
                GetByAction<PresentationGridPagedData<ViewUserEntitlementData>>
                ("GetAttestationUserComparisionData", null, aPIAttestation);

            //Entitlement Row Starts Here 
            currentRow = 3;
            colNum = 1;//assign first column
            foreach (var item in previousApprovers.Data)
            {
                workSheet.Cells[currentRow, colNum].Value = item.SystemName;
                workSheet.Cells[currentRow, ++colNum].Value = item.Entitlement;
                workSheet.Cells[currentRow, ++colNum].Value = item.EntitlementDescription;

                var dd = item.EmployeeNames;

                var userGroup = dd.Values.ToList();


                for (int i = 1; i <= totalCellCount; i++)
                {
                    var renderCell = ++colNum;
                    var forthColumn = workSheet.Cells[secondRow, renderCell].Value.ToString();
                    if (userGroup.Where(x => x.UserName == forthColumn && x.Approve == true).ToList().Count > 0)
                    {
                        if (task.Action == StatusType.Pending)
                        {
                            workSheet.Cells[currentRow, renderCell].Value = "To be Retained";
                        }
                        else if (task.Action == StatusType.Approved|| task.Action == StatusType.AutoCompleted)
                        {
                            workSheet.Cells[currentRow, renderCell].Value = "Retained";
                        }
                        workSheet.Cells[currentRow, renderCell].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[currentRow, renderCell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[currentRow, renderCell].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        workSheet.Cells[currentRow, renderCell].Style.Fill.BackgroundColor.SetColor(Color.DarkGreen);
                        workSheet.Cells[currentRow, renderCell].Style.Font.Color.SetColor(Color.White);
                    }
                    else if (userGroup.Where(x => x.UserName == forthColumn && x.Approve == false).ToList().Count > 0)
                    {
                        if (task.Action == StatusType.Pending)
                        {
                            workSheet.Cells[currentRow, renderCell].Value = "To be Revoked";
                        }
                        else if (task.Action == StatusType.Approved || task.Action == StatusType.AutoCompleted)
                        {
                            workSheet.Cells[currentRow, renderCell].Value = "Revoked";
                        }
                        workSheet.Cells[currentRow, renderCell].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[currentRow, renderCell].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[currentRow, renderCell].Style.Font.Color.SetColor(Color.White);
                        workSheet.Cells[currentRow, renderCell].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        workSheet.Cells[currentRow, renderCell].Style.Fill.BackgroundColor.SetColor(Color.Red);
                    }
                    else
                    {
                        workSheet.Cells[currentRow, renderCell].Value = "";
                    }
                }

                ++currentRow;
                colNum = 1;
            }
            for (int i = colStartPosition; i < totalCellCount + 4; i++)
            {
                workSheet.Column(i).AutoFit();
            }
            return workSheet;

            //var data = excel.GetAsByteArray();         
        }

        private ExcelWorksheet CreateWorksheet(ExcelPackage package, string name)
        {
            // Create a new ExcelWorksheet and return it
            return package.Workbook.Worksheets.Add(name);
        }
    }
}

