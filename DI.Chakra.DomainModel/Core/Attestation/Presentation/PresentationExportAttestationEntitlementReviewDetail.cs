using AutoMapper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DI.Chakra.DomainModel.Presentation
{
    [Serializable]
    public class PresentationExportAttestationEntitlementReviewDetail
    {
        public long Id { get; set; }
        public string AttestationName { get; set; }
        public string CampaignName { get; set; }

        public string EndpointSystemName { get; set; }

        public string EndpointType { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

        public string Detail { get; set; }

        public string AccountType { get; set; }

        public string EntitlementName { get; set; }

        public string EntitlementType { get; set; }

        public bool Approved { get; set; }

        public bool Rejected { get; set; }

        public string Comments { get; set; }

        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }
        public string ActionReserved1 { get; set; }
        public string ActionReserved2 { get; set; }
        public string ActionReserved3 { get; set; }

        public long TaskId { get; set; }

        public long Level { get; set; }

        public bool IsChanged { get; set; }
        public bool IsActionable { get; set; }
        public bool IsVisible { get; set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public byte[] excelFile { get; set; }

        static List<AttestationSnapshotAction> needToOrder = new List<AttestationSnapshotAction>();
        public string ParentObject { get; set; }
        public static IEnumerable<AttestationSnapshotAction> GetList(string parentEntity = null, string accountName = null)
        {

            foreach (var item in needToOrder
               .Where(x => x.AttestationSnapshot.ParentEntity == parentEntity &&
               (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(x.AttestationSnapshot.AccountName) || x.AttestationSnapshot.AccountName == accountName))
               .OrderBy(x => x.AttestationSnapshot.Level)
               .ToList())
            {
                if (string.IsNullOrEmpty(item.AttestationSnapshot.AccountName))
                {
                    yield return new AttestationSnapshotAction
                    {
                        AttestationSnapshot = item.AttestationSnapshot,
                        Approve = item.Approve,
                        AttestationSnapshotId = item.AttestationSnapshotId,
                        Comment = item.Comment,
                        Id = item.Id,
                    };
                }
                else
                    yield return item;

                foreach (var child in GetList(item.AttestationSnapshot.Entity, item.AttestationSnapshot.AccountName))
                {
                    yield return child;
                }

            }
        }

        public static IEnumerable<AttestationSnapshotAction> GetList()
        {
            List<AttestationSnapshotAction> ReportingOrder = new List<AttestationSnapshotAction>();
            long levelToLoad = 1;
            if (needToOrder != null && needToOrder.Count > 0)
            {
                levelToLoad = needToOrder.Select(a => a.AttestationSnapshot.Level).Distinct().Min();
            }
            var level1 = needToOrder.Where(a => a.AttestationSnapshot.Level == levelToLoad).ToList();
            if (level1 != null && level1.Count > 0)
            {
                foreach (var level in level1)
                {
                    ReportingOrder.Add(level);
                    //check for child
                    ReportingOrder.AddRange(GetChildList(level));
                }
            }
            return ReportingOrder;
        }

        static IEnumerable<AttestationSnapshotAction> GetChildList(AttestationSnapshotAction parent)
        {
            List<AttestationSnapshotAction> ReportingOrder = new List<AttestationSnapshotAction>();
            if (parent != null && parent.AttestationSnapshot != null)
            {
                var parentEntity = parent.AttestationSnapshot.Entity;
                var accountName = parent.AttestationSnapshot.AccountName;

                var childs = needToOrder.Where(x => x.AttestationSnapshot.EndpointId == parent.AttestationSnapshot.EndpointId && x.AttestationSnapshot != null && x.AttestationSnapshot.ParentEntity == parentEntity &&
                x.AttestationSnapshot.Level == (parent.AttestationSnapshot.Level + 1) &&
               (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(x.AttestationSnapshot.AccountName) || x.AttestationSnapshot.AccountName == accountName))
               .OrderBy(x => x.AttestationSnapshot.Level).ToList();

                if (childs != null && childs.Count > 0)
                {
                    foreach (var child in childs)
                    {
                        child.ParentObjects = parentEntity;
                        if (child.IsActionable == false && parent.Revoke)
                        {
                            child.Revoke = parent.Revoke;
                            child.Approve = false;
                            if (string.IsNullOrWhiteSpace(child.LastModifiedBy))
                            {
                                child.LastModifiedBy = parent.LastModifiedBy;
                            }
                        }
                        else if(child.IsActionable == false && parent.Approve && (child.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.TechnicalRole || child.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.IndirectRole))
                        {
                            child.Revoke = parent.Revoke;
                            child.Approve = parent.Approve;
                            if (string.IsNullOrWhiteSpace(child.LastModifiedBy))
                            {
                                child.LastModifiedBy = parent.LastModifiedBy;
                            }
                        }
                        ReportingOrder.Add(child);
                        var childRows = GetChildList(child);
                        if (childRows != null && childRows.Count() > 0)
                            ReportingOrder.AddRange(childRows);
                    }
                }
            }

            return ReportingOrder;
        }

        public static List<PresentationExportAttestationEntitlementReviewDetail> GetAttestationTaskReviewForExport(long campaignId)
        {
            try
            {
                List<PresentationExportAttestationEntitlementReviewDetail> output = new List<PresentationExportAttestationEntitlementReviewDetail>();
                var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdForReport(0, campaignId, true, true);
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion(campaignId);

                //user details
                if (task != null && task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                {
                    needToOrder = task.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.EndpointId).ThenBy(a => a.AttestationSnapshot.Level).ThenByDescending(a => a.IsChanged).ToList();
                    task.AttestationSnapshotActions = GetList().ToList();
                    output.AddRange((from action in task.AttestationSnapshotActions
                                     select new PresentationExportAttestationEntitlementReviewDetail
                                     {
                                         Id = action.Id,
                                         AttestationName = campaignDetails.Attestation.AttestationName,
                                         CampaignName = campaignDetails.CampaignName,
                                         EndpointSystemName = action.AttestationSnapshot.EndpointName,
                                         EndpointType = action.AttestationSnapshot.EndpointType.ToString(),
                                         UserName = string.IsNullOrEmpty(action.AttestationSnapshot.DIUserName) ? action.AttestationSnapshot.Name : action.AttestationSnapshot.DIUserName,
                                         Name = string.IsNullOrEmpty(action.AttestationSnapshot.DIUserDisplayName) ? action.AttestationSnapshot.Description : action.AttestationSnapshot.DIUserDisplayName,
                                         Detail = action.AttestationSnapshot.Description,
                                         AccountType = action.AttestationSnapshot.AccountType.ToString(),
                                         EntitlementName = action.AttestationSnapshot.Entity,
                                         EntitlementType = action.AttestationSnapshot.EntitlementType.ToString(),
                                         Approved = action.Approve,
                                         Rejected = action.Revoke,
                                         TaskId = action.TaskId,
                                         Comments = action.Comment,
                                         Reserved1 = action.AttestationSnapshot.Reserved1,
                                         Reserved2 = action.AttestationSnapshot.Reserved2,
                                         Reserved3 = action.AttestationSnapshot.Reserved3,
                                         Reserved4 = action.AttestationSnapshot.Reserved4,
                                         Reserved5 = action.AttestationSnapshot.Reserved5,
                                         Reserved6 = action.AttestationSnapshot.Reserved6,
                                         Reserved7 = action.AttestationSnapshot.Reserved7,
                                         Reserved8 = action.AttestationSnapshot.Reserved8,
                                         Reserved9 = action.AttestationSnapshot.Reserved9,
                                         Reserved10 = action.Reserved1,
                                         Level = action.AttestationSnapshot.Level,
                                         IsChanged = action.IsChanged,
                                         IsActionable = action.IsActionable,
                                         IsVisible = action.IsVisible,
                                         CreatedBy = action.CreatedBy,
                                         LastModifiedBy = action.LastModifiedBy,
                                         ParentObject = action.ParentObjects
                                     }).ToList());
                }
                needToOrder = new List<AttestationSnapshotAction>();
                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, "Error while getting attestation review for export", ex);
                throw;
            }
        }

        public static PresentationExportAttestationReportForUI GetAttestationTaskReviewForExportFlat(long taskId, bool isInitialLoad, string currentUser)
        {
            try
            {
                PresentationExportAttestationReportForUI output = new PresentationExportAttestationReportForUI();
                byte[] report = null;
                var orgTask = Task.GetById(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, (long)orgTask.AttestationCampaignId);
                    List<long> availableEndpoints = new List<long>();
                    List<long> availableEndpointTypes = new List<long>();
                    //merge the 2 objects(interim + snapshot) and construct the view for export, might be a costly operation

                    var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);
                    if (!orgTask.IsCompleted)
                    {
                        var availableLevels = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().ToList();
                        availableEndpoints = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.EndpointId).Distinct().ToList();
                        var nonActionableSnapshot = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdForReport(taskId, (long)orgTask.AttestationCampaignId, true);
                        task.AttestationSnapshotActions.AddRange(
                            nonActionableSnapshot.AttestationSnapshotActions.Where(a => !availableLevels.Contains(a.AttestationSnapshot.Level)
                            && availableEndpoints.Contains(a.AttestationSnapshot.EndpointId))
                            );
                    }

                    if (availableEndpoints.Count > 0)
                    {
                        foreach (var availableEndpoint in availableEndpoints)
                        {
                            var endpoint = EndPoint.GetById(availableEndpoint);
                            if (!availableEndpointTypes.Any(a => a == endpoint.EndpointSystemTypeId.Value))
                            {
                                availableEndpointTypes.Add(endpoint.EndpointSystemTypeId.Value);
                            }
                        }
                    }

                    //user details
                    if (task != null && task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        try
                        {
                            var actions = task.AttestationSnapshotActions;
                            needToOrder = task.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.EndpointId).ThenBy(a => a.AttestationSnapshot.Level).ThenByDescending(a => a.IsChanged).ToList();
                            task.AttestationSnapshotActions = GetList().ToList();

                            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();

                            var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_", campaignDetails.Attestation.AttestationType.ToString().ToUpper(), "ATTESTATION_REVIEW"));

                            //Header
                            // loop top level users
                            // add workcell entry for top level
                            // TOP User header
                            int currentRow = 1;
                            int colStartPosition = 1;
                            int colNum = colStartPosition;

                            bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
                            bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
                            bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;
                            string accountType = string.Empty;
                            showReserved1 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved1));
                            showReserved2 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved2));
                            showReserved3 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved3));
                            showReserved4 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved4));
                            //SP:Siva not required Mainly for SRM internal Purpose
                            //showReserved5 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved5));
                            //showReserved6 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved6));

                            showReserved7 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved7));
                            //showReserved8 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved8));

                            showReserved9 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved9));
                            showReserved10 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved10));

                            showActionReserved1 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                            showActionReserved2 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                            showActionReserved3 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved3));

                            workSheet.Cells[currentRow, colNum].Value = "Attestation Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Campaign Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Campaign Start Date";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Campaign Remarks";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "System Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Login Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Display Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Account Type";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                                workSheet.Cells[currentRow, ++colNum].Value = "Type";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                                workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                                workSheet.Cells[currentRow, ++colNum].Value = "Detail";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                
                                
                            }

                            #region reserved columns
                            if (showReserved1)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Additional Info";
//                                if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
//                                {
//                                    workSheet.Cells[currentRow, colNum].Value = "Operator Id";
//                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved2)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 2";
//                                if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
//                                {
//                                    workSheet.Cells[currentRow, colNum].Value = "Activity DateTime";
//                                }
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            

                            if (showReserved4)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 4";
                                if ( campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                   
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved5)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 5";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved6)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 6";
                                if ( campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved7)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 7";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved8)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 8";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved9)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 9";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved10)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 10";
//                                if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
//                                {
//                                    workSheet.Cells[currentRow, colNum].Value = "Detail 10";
//                                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
//                                }
                            }

                            if (showActionReserved1)
                            {
                                //if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
                                //{
                                //    workSheet.Cells[currentRow, ++colNum].Value = "Request Id";
                                //}
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showActionReserved2)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 2";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showActionReserved3)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 3";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }
                            #endregion
                            workSheet.Cells[currentRow, ++colNum].Value = "Parent Object";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            if (showReserved3)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Valid To";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {

                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = "Retain";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Revoke";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Comments";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            string currentDisplayName = string.Empty;
                            string previousDisplayName = string.Empty;
                            var usrUserName = string.Empty;
                            //move the row index next to header
                            currentRow++;
                            AttestationSnapshotAction parentAction = null;
                            for (int i = 0; i < task.AttestationSnapshotActions.Count; i++)
                            {
                                AttestationSnapshotAction action = task.AttestationSnapshotActions[i];
                                if (action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.Account)
                                {
                                    parentAction = null;
                                }
                                if (i > 0 && (action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.BusinessRole || action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.Position))
                                {
                                    parentAction = action;
                                }
                                if(action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.TCodes || action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.TechnicalRole || action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.IndirectRole)
                                {
                                    if(parentAction != null && action.ParentObjects == parentAction.AttestationSnapshot.Name)
                                    {
                                        action.Approve = parentAction.Approve;
                                        action.Revoke = parentAction.Revoke;
                                    }
                                }
                                
                            //}
                            //foreach (AttestationSnapshotAction action in task.AttestationSnapshotActions)
                            //{
                                colStartPosition = 1;
                                colNum = colStartPosition;
                                if (action.IsVisible)
                                {
                                    currentDisplayName = action.AttestationSnapshot.DIUserDisplayName;
                                    if (action.AttestationSnapshot.EntitlementType == Infrastructure.Enums.EntitlementType.Account)
                                    {
                                        previousDisplayName = currentDisplayName;
                                        usrUserName = action.AttestationSnapshot.DIUserName;
                                        accountType = action.AttestationSnapshot.AccountType.ToString();
                                    }
                                    // add workcell entry for top level
                                    workSheet.Cells[currentRow, colNum].Value = campaignDetails.Attestation.AttestationName;
                                    workSheet.Cells[currentRow, ++colNum].Value = campaignDetails.CampaignName;
                                    workSheet.Cells[currentRow, ++colNum].Value = campaignDetails.CreatedDate.ToString(Constants.DateFormatForSP);
                                    workSheet.Cells[currentRow, ++colNum].Value = campaignDetails.Comments?.Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "").Replace("!~", Environment.NewLine);
                                    workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.EndpointName;
                                    workSheet.Cells[currentRow, ++colNum].Value = usrUserName;
                                    workSheet.Cells[currentRow, ++colNum].Value = previousDisplayName;

                                    if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = accountType;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.EntitlementType.ToString();
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Entity;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Description;
                                        
                                    }

                                    //var lstModifedBy = action.LastModifiedBy;
                                    //workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(lstModifedBy) ? action.CreatedBy : lstModifedBy;

                                    //workSheet.Cells[currentRow, ++colNum].Value = lstModifedBy;    

                                    if (showReserved1)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved1;
                                    }

                                    if (showReserved2)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved2;
                                    }

                                    

                                    if (showReserved4)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved4;
                                    }

                                    if (showReserved5)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved5;
                                    }

                                    if (showReserved6)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved6;
                                    }

                                    if (showReserved7)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved7;
                                    }

                                    if (showReserved8)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved8;
                                    }

                                    if (showReserved9)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved9;
                                    }

                                    if (showReserved10)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved10;
                                    }

                                    if (showActionReserved1)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.Reserved1;
                                    }

                                    if (showActionReserved2)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.Reserved2;
                                    }

                                    if (showActionReserved3)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.Reserved3;
                                    }
                                    workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.ParentEntity;
                                    if (showReserved3)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved3;
                                    }
                                    workSheet.Cells[currentRow, ++colNum].Value = action.Approve;
                                    workSheet.Cells[currentRow, ++colNum].Value = action.Revoke;
                                    workSheet.Cells[currentRow, ++colNum].Value = action.Comment;

                                    if (action.IsChanged)
                                    {
                                        workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(228, 236, 245));
                                    }

                                    currentRow++;
                                }
                            }
                            workSheet.Cells.AutoFitColumns();
                            // Fit the columns according to its content
                            //for (int i = colStartPosition; i < colNum; i++)
                            //{
                            //    workSheet.Column(i).AutoFit();
                            //}

                            report = excel.GetAsByteArray();
                        }
                        catch (Exception ex)
                        {
                            string message = "Error while executing the export flat excel activity from UI.";
                            DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                        }
                    }
                }
                output.excelFile = report;
                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for export from UI", ex);
                throw;
            }
        }

        public static PresentationExportAttestationReportForUI GetAttestationTaskReviewForExportHierarchy(long taskId, bool isInitialLoad, string currentUser)
        {
            try
            {
                PresentationExportAttestationReportForUI output = new PresentationExportAttestationReportForUI();
                byte[] report = null;
                var orgTask = Task.GetById(taskId);
                if (orgTask != null && orgTask.AttestationCampaignId.HasValue)
                {
                    var task = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskId(taskId, (long)orgTask.AttestationCampaignId);
                    List<long> availableEndpoints = new List<long>();
                    List<long> availableEndpointTypes = new List<long>();
                    //merge the 2 objects(interim + snapshot) and construct the view for export, might be a costly operation

                    var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);
                    if (!orgTask.IsCompleted)
                    {
                        var availableLevels = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.Level).Distinct().ToList();
                        availableEndpoints = task.AttestationSnapshotActions.Select(a => a.AttestationSnapshot.EndpointId).Distinct().ToList();
                        var nonActionableSnapshot = PresentationAttestationSnapshotActionsForTask.GetAttestationSnapshotByTaskIdForReport(taskId, (long)orgTask.AttestationCampaignId, true);
                        task.AttestationSnapshotActions.AddRange(
                            nonActionableSnapshot.AttestationSnapshotActions.Where(a => !availableLevels.Contains(a.AttestationSnapshot.Level)
                            && availableEndpoints.Contains(a.AttestationSnapshot.EndpointId))
                            );
                    }

                    if (availableEndpoints.Count > 0)
                    {
                        foreach (var availableEndpoint in availableEndpoints)
                        {
                            var endpoint = EndPoint.GetById(availableEndpoint);
                            if (!availableEndpointTypes.Any(a => a == endpoint.EndpointSystemTypeId.Value))
                            {
                                availableEndpointTypes.Add(endpoint.EndpointSystemTypeId.Value);
                            }
                        }
                    }

                    //user details
                    if (task != null && task.AttestationSnapshotActions != null && task.AttestationSnapshotActions.Count > 0)
                    {
                        try
                        {
                            var actions = task.AttestationSnapshotActions;
                            needToOrder = task.AttestationSnapshotActions.OrderBy(a => a.AttestationSnapshot.EndpointId).ThenBy(a => a.AttestationSnapshot.Level).ThenByDescending(a => a.IsChanged).ToList();
                            task.AttestationSnapshotActions = GetList().ToList();

                            OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();

                            var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_", campaignDetails.Attestation.AttestationType.ToString().ToUpper(), "ATTESTATION_REVIEW"));

                            //Header
                            // loop top level users
                            // add workcell entry for top level
                            // TOP User header
                            int currentRow = 1;
                            int colStartPosition = 1;
                            int colNum = colStartPosition;

                            bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
                            bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
                            bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;

                            showReserved1 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved1));
                            showReserved2 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved2));
                            showReserved3 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved3));
                            showReserved4 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved4));
                            //SP:Siva Not required
                            //showReserved5 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved5));
                            //showReserved6 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved6));
                            showReserved7 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved7));
                            //SP:Siva Not required
                            //showReserved8 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved8));
                            showReserved9 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved9));
                            showReserved10 = actions.Any(a => !string.IsNullOrEmpty(a.AttestationSnapshot.Reserved10));

                            showActionReserved1 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                            showActionReserved2 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                            showActionReserved3 = actions.Any(a => !string.IsNullOrEmpty(a.Reserved3));

                            workSheet.Cells[currentRow, colNum].Value = "Attestation Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "Campaign Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            workSheet.Cells[currentRow, ++colNum].Value = "System Name";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Login Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Display Name";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }
                            

                            if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Account Type";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                //NEWLY Added for SP requirement
                                workSheet.Cells[currentRow, ++colNum].Value = "Parent Object";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Type";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Retain";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                workSheet.Cells[currentRow, ++colNum].Value = "Revoke";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            #region reserved columns
                            if (showReserved1)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Additional Info";
                                //if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
                                //{
                                //    workSheet.Cells[currentRow, colNum].Value = "Operator Id";
                                //}
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "View";
                                    //}
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "MenuName";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved2)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 2";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Create";
                                    //}
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "BarName";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved3)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Valid To";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Modify";
                                    //}
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "ContentReference";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved4)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 4";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Delete";
                                    //}
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Description";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved5)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 5";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Print";
                                    //}
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "IsDisplay";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved6)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 6";
                                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                {
                                    //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                                    //{
                                    //    workSheet.Cells[currentRow, colNum].Value = "Export";
                                    //}
                                }
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved7)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 7";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved8)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 8";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved9)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 9";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showReserved10)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Detail 10";
                                //if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
                                //{
                                //    workSheet.Cells[currentRow, colNum].Value = "Detail 10";
                                //    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                                //}
                            }

                            if (showActionReserved1)
                            {
                                //if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
                                //{
                                //    workSheet.Cells[currentRow, ++colNum].Value = "Request Id";
                                //}
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showActionReserved2)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 2";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }

                            if (showActionReserved3)
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 3";
                                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            }
                            #endregion

                            workSheet.Cells[currentRow, ++colNum].Value = "Comments";
                            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                            string currentDisplayName = string.Empty;
                            string previousDisplayName = string.Empty;
                            //move the row index next to header
                            currentRow++;
                            foreach (AttestationSnapshotAction action in task.AttestationSnapshotActions)
                            {
                                currentDisplayName = action.AttestationSnapshot.DIUserDisplayName;
                                if (!string.IsNullOrWhiteSpace(currentDisplayName))
                                {
                                    previousDisplayName = currentDisplayName;
                                }

                                colStartPosition = 1;
                                colNum = colStartPosition;
                                if (action.IsVisible)
                                {
                                    var usrDispName = action.AttestationSnapshot.DIUserDisplayName;
                                    
                                    // add workcell entry for top level
                                    workSheet.Cells[currentRow, colNum].Value = campaignDetails.Attestation.AttestationName;
                                    workSheet.Cells[currentRow, ++colNum].Value = campaignDetails.CampaignName;
                                    workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.EndpointName;
                                    workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(action.AttestationSnapshot.DIUserName) ? "" : action.AttestationSnapshot.DIUserName;
                                    workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(action.AttestationSnapshot.DIUserDisplayName) ? previousDisplayName : action.AttestationSnapshot.DIUserDisplayName;

                                    if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Description;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.AccountType.ToString();
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Entity;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.ParentEntity;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.EntitlementType.ToString();
                                        workSheet.Cells[currentRow, ++colNum].Value = action.Approve;
                                        workSheet.Cells[currentRow, ++colNum].Value = action.Revoke;
                                    }

                                    //var lstModifedBy = action.LastModifiedBy;
                                    //workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(lstModifedBy) ? action.CreatedBy : lstModifedBy;

                                    //workSheet.Cells[currentRow, ++colNum].Value = lstModifedBy;        

                                    if (showReserved1)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved1;
                                    }

                                    if (showReserved2)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved2;
                                    }

                                    if (showReserved3)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved3;
                                    }

                                    if (showReserved4)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved4;
                                    }

                                    if (showReserved5)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved5;
                                    }

                                    if (showReserved6)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved6;
                                    }

                                    if (showReserved7)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved7;
                                    }

                                    if (showReserved8)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved8;
                                    }

                                    if (showReserved9)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved9;
                                    }

                                    if (showReserved10)
                                    {
                                        workSheet.Cells[currentRow, ++colNum].Value = action.AttestationSnapshot.Reserved10;
                                    }

                                    workSheet.Cells[currentRow, ++colNum].Value = action.Comment;

                                    var level = action.AttestationSnapshot.Level;
                                    if (level >= 2)
                                    {                                       
                                        workSheet.Row(currentRow).OutlineLevel = (int)level;
                                        workSheet.Row(currentRow).Collapsed = true;
                                    }

                                    if (action.IsChanged)
                                    {
                                        workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(228, 236, 245));
                                    }

                                    currentRow++;
                                }
                            }

                            // Fit the columns according to its content
                            for (int i = colStartPosition; i < colNum; i++)
                            {
                                workSheet.Column(i).AutoFit();
                            }

                            report = excel.GetAsByteArray();
                        }
                        catch (Exception ex)
                        {
                            string message = "Error while executing the export flat excel activity from UI.";
                            DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
                        }
                    }
                }
                output.excelFile = report;
                return output;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(taskId, LogLevelType.Exception, "Error while getting attestation review for export from UI", ex);
                throw;
            }
        }        
    }
}
