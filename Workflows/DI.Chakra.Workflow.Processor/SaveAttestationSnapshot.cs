
using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DI.Chakra.Workflow.Processor
{
    public class SaveAttestationSnapshot
    {
        Dictionary<long, string> TaskEndorsedBy = new Dictionary<long, string>();
        Dictionary<long, int> TaskState = new Dictionary<long, int>();
        List<PresentationSnapshotActionHistory> history = new List<PresentationSnapshotActionHistory>();
        public void Execute(Task task, long campaignId)
        {
            try
            {
                var tTask = Task.GetById(task.Id);

                if (tTask == null)
                {
                    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Attestation Campaign task empty {task?.Id} campaignId:{campaignId}");
                    return;
                }

                if (tTask.Action == StatusType.Rejected || tTask.Action == StatusType.Withdrawn)
                {
                    //DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Info, $"Attestation Campaign task  {task?.Id} withdrawn for Campaign Id:{campaignId}");
                    return;
                }
                AttestationCampaign campaign = AttestationCampaign.GetById(campaignId);
                if (campaign == null)
                {
                    DILog.AttestationLogger.WriteLog(null, LogLevelType.Error, $"Error while executing the RemediateAttestation activity - Cannot get campaign! Details: TaskID: {task?.Id}, CampaignID: {campaignId}");
                    return;
                }
                string fileName = campaign.CampaignName;
                //try
                //{
                //    //Directory.CreateDirectory(string.Format(@"{0}\\{1}", MasterConfigHelper.AttestationSnapshotFilePath, campaign.CampaignName));


                //    //Export an attestation pdf of the completed attestation
                //    //ExportPDFToFilePath(task.Id, campaign);
                //    //ExportCSVToFilePath(task.Id, campaign);
                //    //ExportFlatExcelToFilePath(task.Id, campaign);
                //    //ExportHierarchyExcelToFilePath(task.Id, campaign);
                //}
                //catch (Exception ex)
                //{
                //    DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, ex.Message, ex);
                //}
                //Fetch all the history
                //history = DI.Chakra.DomainModel.Presentation.PresentationSnapshotActionHistory.GetHistoryByCampaignId(campaign.Id);

                //CreateAndSaveAttestationCampaignReport(AttestationReportType.PDF, task.Id, campaign);
                //CreateAndSaveAttestationCampaignReport(AttestationReportType.CSV, task.Id, campaign);
                //CreateAndSaveAttestationCampaignReport(AttestationReportType.ExcelFlat, task.Id, campaign);
                CreateReportRecord(AttestationReportType.ExcelFlat, task.Id, campaign.Id);
                //CreateAndSaveAttestationCampaignReport(AttestationReportType.ExcelHierarchical, task.Id, campaign);
                CreateReportRecord(AttestationReportType.ExcelHierarchical, task.Id, campaign.Id);
                // update snapshot filename
                AttestationCampaign.UpdateCampaignFileName(campaign.Id, fileName);
            }
            catch (Exception ex)
            {
                string message = "Error while executing the SaveAttestationSnapshot activity.";
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, message, ex);
            }
            finally
            {
                //history.Clear();
                TaskEndorsedBy.Clear();
                TaskState.Clear();
            }
        }
        public void GetHistoryRecordByCampaign(long campaignId)
        {
            history = new List<PresentationSnapshotActionHistory>();
            history = DI.Chakra.DomainModel.Presentation.PresentationSnapshotActionHistory.GetHistoryByCampaignId(campaignId);
        }
        public byte[] ExportUserComparisionByReportType(AttestationReportType attestationReportType, long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            if (attestationReportType == AttestationReportType.ExcelFlat)
            {
                report = ExportFlatExcelToByteArray(taskId, campaign);
            }
            else if (attestationReportType == AttestationReportType.ExcelHierarchical)
            {
                report = ExportUserProfileComparisionArray(taskId, campaign);
            }
            return report;
        }

        public byte[] ExportUserProfileComparisionArray(long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            try
            {
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                ExportUserProfileComparision(excel, taskId, campaign);
                report = excel.GetAsByteArray();
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export flat excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
            return report;
        }
        public void CreateReportRecord(AttestationReportType reportType, long taskId, long campaignId)
        {
            try
            {
                AttestationCampaign.ExecuteQuery(string.Format(@"IF NOT EXISTS(Select 1 from DI_M_AttestationReportList WHERE taskId = {0} AND CampaignId ={1} and ReportType={2} and Attestationtype ={3})
                BEGIN
                INSERT INTO DI_M_AttestationReportList(taskId,CampaignId,ReportType,CreatedDate,CreatedBy,Attestationtype)
                values({0},{1},{2},Getdate(),'System',{3})   
                END", taskId, campaignId, (int)reportType, (int)AttestationType.User));
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Exception, $"Error while creating CreateReportRecord", ex);
            }
        }
        public void CreateAndSaveAttestationCampaignReport(AttestationReportType reportType, long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            string filename = campaign.CampaignName;

            if (reportType == AttestationReportType.PDF)
            {
                report = ExportPDFAsByteArray(taskId, campaign);
                filename = String.Format("{0}.{1}", campaign.CampaignName, "pdf");
            }
            else if (reportType == AttestationReportType.CSV)
            {
                report = ExportCSVAsByteArray(taskId, campaign);
                filename = String.Format("{0}.{1}", campaign.CampaignName, "csv");
            }
            else if (reportType == AttestationReportType.ExcelFlat)
            {
                //report = ExportFlatExcelToByteArray(taskId, campaign);
                //filename = String.Format("{0}_Flat.{1}", campaign.CampaignName, "xlsx");
            }
            else if (reportType == AttestationReportType.ExcelHierarchical)
            {
                report = ExportHierarchyExcelAsByteArray(taskId, campaign);
                filename = String.Format("{0}_Hierarchy.{1}", campaign.CampaignName, "xlsx");
            }

            if (report != null)
            {

                try
                {
                    var isReportThere = AttestationCampaignReport.GetByAttestationCampaignId(campaign.Id);

                    if (isReportThere == null || !isReportThere.Any(a => a.ReportType == reportType))
                    {
                        AttestationCampaignReport cReport = new AttestationCampaignReport()
                        {
                            AttachmentFile = report,
                            AttestationCampaignId = campaign.Id,
                            AttestationCampaignName = campaign.CampaignName,
                            ContentType = reportType.ToString(),
                            FileName = filename,
                            ReportType = reportType,
                        };
                        cReport.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        var cReport = isReportThere.Where(a => a.ReportType == reportType).FirstOrDefault();
                        if (cReport != null)
                        {
                            cReport.AttachmentFile = report;
                            cReport.FileName = filename;
                            cReport.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DILog.AttestationLogger.WriteLog(campaign.Id, LogLevelType.Exception, "Error while craeting Attestation report file.", ex);
                }
            }

        }

        private PdfPCell GetPDFCell(string cellText, bool isheader, bool isChanged = false)
        {
            cellText = string.IsNullOrEmpty(cellText) ? "" : cellText;
            PdfPCell cell = new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Helvetica", 10, Font.NORMAL, new Color(102, 102, 102))));
            if (isheader)
            {
                cell = new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Helvetica", 10, Font.BOLD, new Color(102, 102, 102))));
                cell.BackgroundColor = new Color(197, 197, 197);
            }
            else if (isChanged)
            {
                cell.BackgroundColor = new Color(228, 236, 245);
            }
            return cell;
        }

        private PdfPCell GetPDFCell(Paragraph cellText, bool isheader, bool isChanged = false)
        {
            cellText.Font = FontFactory.GetFont("Helvetica", 10, Font.NORMAL, new Color(102, 102, 102));
            PdfPCell cell = new PdfPCell(cellText);

            if (isChanged)
            {
                cell.BackgroundColor = new Color(228, 236, 245);
            }
            return cell;
        }

        private PdfPCell GetPDFCellBorderLess(string cellText, bool isValue, bool useBoldCase = true)
        {
            cellText = string.IsNullOrEmpty(cellText) ? "" : cellText;
            PdfPCell cell = new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Helvetica", 10,
                useBoldCase ? Font.BOLD : Font.NORMAL,
                isValue ? new Color(32, 123, 189) : new Color(102, 102, 102))));
            cell.BorderWidth = 0;
            cell.BorderColor = Color.WHITE;
            cell.Border = Rectangle.NO_BORDER;
            return cell;
        }

        private void ExportPDFToFilePath(long taskId, AttestationCampaign campaign)
        {
            try
            {
                var doc1 = new Document(PageSize.A4.Rotate()); // landscape mode
                Directory.CreateDirectory(MasterConfigHelper.AttestationSnapshotFilePath);
                PdfWriter.GetInstance(doc1, new FileStream(String.Format("{0}\\{1}\\{1}.pdf", MasterConfigHelper.AttestationSnapshotFilePath, campaign.CampaignName), FileMode.Create)); // TODO: Change storage location

                ExportPDF(doc1, taskId, campaign);
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export pdf activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private void ExportPDF(Document doc1, long taskId, AttestationCampaign campaign)
        {
            try
            {
                var taskDetail = Task.GetById(taskId, true);
                var attestationCampaign = Attestation.GetById(campaign.AttestationID);
                //pdf header information
                doc1.Open();

                //Create Chunk for underline
                Chunk chkHeader = new Chunk(string.Format("{0} - {1}", taskDetail.Attestation.AttestationName, "Report").ToUpper(),
                    FontFactory.GetFont("Helvetica", 12, Font.BOLD, new Color(32, 123, 189)));
                //chkHeader.SetUnderline(1f, -2f);
                //Add Chunk to paragraph
                Paragraph pHeader = new Paragraph(chkHeader);
                pHeader.Alignment = Element.ALIGN_CENTER;
                doc1.Add(pHeader);
                doc1.Add(new Paragraph("\n"));
                Chunk linebreak = new Chunk(new LineSeparator(1f, 100f, Color.GRAY, Element.ALIGN_CENTER, -1));
                doc1.Add(linebreak);
                doc1.Add(new Paragraph("\n"));

                //doc1.Add(new Phrase(campaignName, FontFactory.GetFont("Helvetica", 12, Font.BOLD, new Color(102, 102, 102))));
                //doc1.Add(new Paragraph("\n"));

                //get list of columns to show
                //PresentationExportAttestationEntitlementReviewDetail entity = new PresentationExportAttestationEntitlementReviewDetail();
                //var displayCOlumns = entity.GetType().GetProperties().ToList();

                //Attestation Details
                PdfPTable detail = new PdfPTable(4);
                float[] detailWidths = new float[] { 20f, 60f, 15f, 30f };
                detail.SetTotalWidth(detailWidths);
                detail.DefaultCell.FixedHeight = 100f;
                detail.WidthPercentage = 100;
                detail.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                detail.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                detail.DefaultCell.BorderColor = Color.WHITE;
                detail.DefaultCell.Border = Rectangle.NO_BORDER;
                detail.DefaultCell.BorderWidth = 0;

                detail.AddCell(GetPDFCellBorderLess("Campaign Name", false));
                detail.AddCell(GetPDFCellBorderLess(taskDetail.AttestationCampaign.CampaignName.ToUpper(), true));
                detail.AddCell(GetPDFCellBorderLess("Start Date", false));
                detail.AddCell(GetPDFCellBorderLess(taskDetail.AttestationCampaign.CreatedDate.ToString("dd/MM/yyyy").ToUpper(), true));

                bool displayHeader = true;
                //var allTasksForThisCampaign = Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();


                // check whether we are in first level of workflow
                bool isFirstLevel = taskDetail.WorkflowStateId == (byte)WorkflowState.Level1 || taskDetail.WorkflowStateId == (byte)WorkflowState.None;

                //Endpoint & Enddate
                var attestationEndpoint = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(campaign.AttestationID).ToList();
                List<long> availableEndpointTypes = new List<long>();
                foreach (var endpoint in attestationEndpoint)
                {
                    if (!availableEndpointTypes.Any(a => a == (long)endpoint.EndpointSystemType))
                    {
                        availableEndpointTypes.Add((long)endpoint.EndpointSystemType);
                    }

                    if (displayHeader)
                        detail.AddCell(GetPDFCellBorderLess("Endpoint", false));
                    else
                        detail.AddCell(GetPDFCellBorderLess("", false));

                    detail.AddCell(GetPDFCellBorderLess(endpoint.EndPoint != null ? endpoint.EndPoint.DisplayName : "", true));

                    if (displayHeader)
                    {
                        detail.AddCell(GetPDFCellBorderLess("End Date", false));
                        detail.AddCell(GetPDFCellBorderLess(taskDetail.AttestationCampaign.EndDate.HasValue ? taskDetail.AttestationCampaign.EndDate.Value.ToString("dd/MM/yyyy").ToUpper() : DateTime.Now.ToString("dd/MM/yyyy").ToUpper(), true));
                    }
                    else
                    {
                        detail.AddCell(GetPDFCellBorderLess("", false));
                        detail.AddCell(GetPDFCellBorderLess("", true));
                    }

                    //hereafter we dont need any more headers.
                    displayHeader = false;
                }

                displayHeader = true;
                var allTasksForThisCampaign = Task.GetByCampaignIdAndAllState((long)taskDetail.AttestationCampaignId).ToList();
                //multiple reviewers & its comments
                foreach (var task in allTasksForThisCampaign)
                {
                    var taskStatus = task.Action;

                    // check whether task is autocompleted
                    bool isautoCompletedTask = task.Action == StatusType.AutoCompleted;

                    var attestedUserId = task.ActionUserId.HasValue ? task.ActionUserId.Value : 0;
                    attestedUserId = (task.IsGroupTask && task.AssignedTo.HasValue && !task.IsDelegated) ? task.AssignedTo.Value : attestedUserId;
                    var attestedUser = User.GetById(attestedUserId);

                    // check whether we got a valid user
                    var attDisplayName = attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System";

                    // if the task is at the first level and is Auto Completed
                    if (isautoCompletedTask && isFirstLevel)
                    {
                        // set reviewer and endorser to System
                        attDisplayName = "System";
                    }

                    if (!TaskEndorsedBy.ContainsKey(task.Id))
                        TaskEndorsedBy.Add(task.Id, attDisplayName);

                    if (!TaskState.ContainsKey(task.Id))
                        TaskState.Add(task.Id, task.WorkflowStateId);


                    if (displayHeader)
                        detail.AddCell(GetPDFCellBorderLess("Endorsed BY", false));
                    else
                        detail.AddCell(GetPDFCellBorderLess("", false));

                    detail.AddCell(GetPDFCellBorderLess(attDisplayName, true));

                    ////Empty cell.
                    //detail.AddCell(GetPDFCellBorderLess("", false));
                    //detail.AddCell(GetPDFCellBorderLess("", true));

                    ////doc1.Add(detail);
                    ////Create new table only for comments
                    //detail = new PdfPTable(2);
                    //detailWidths = new float[] { 20f, 105f };
                    //detail.SetTotalWidth(detailWidths);
                    //detail.DefaultCell.FixedHeight = 100f;
                    //detail.WidthPercentage = 100;
                    //detail.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //detail.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                    //detail.DefaultCell.BorderColor = Color.WHITE;
                    //detail.DefaultCell.Border = Rectangle.NO_BORDER;
                    //detail.DefaultCell.BorderWidth = 0;

                    if (displayHeader)
                    {
                        detail.AddCell(GetPDFCellBorderLess("Comments", false));
                    }
                    else
                    {
                        detail.AddCell(GetPDFCellBorderLess("", false));
                    }

                    var campaignComments = string.IsNullOrWhiteSpace(campaign.Comments) ? "" : campaign.Comments;
                    var reviewer = string.Format("Update By:[{0}]", attDisplayName);
                    var commentsByReviewer = "";
                    if (campaignComments.Contains(reviewer))
                    {
                        var reviewerComments = campaignComments.Split(new string[] { "!~" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var comment in reviewerComments.Where(i => i.Contains(reviewer, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            commentsByReviewer += comment.Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "") + Environment.NewLine;
                            //var output = comment.Split(new string[] { " AM]"," PM]" }, StringSplitOptions.RemoveEmptyEntries);
                            //if (output.Length > 0)
                            //{
                            //    commentsByReviewer += output[output.Length - 1].Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "");
                            //    commentsByReviewer += Environment.NewLine;
                            //}
                        }
                        //var reviewerComments = campaignComments.Split(new string[] { reviewer }, StringSplitOptions.RemoveEmptyEntries);

                        //foreach (var comment in reviewerComments)
                        //{
                        //    var output = comment.Split(']');
                        //    if (output.Length > 0)
                        //    {
                        //        commentsByReviewer += output[output.Length - 1].Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "");
                        //        commentsByReviewer += Environment.NewLine;
                        //    }
                        //}
                    }
                    else
                    {
                        commentsByReviewer = campaignComments;
                    }
                    commentsByReviewer = commentsByReviewer.Replace("!~", "").Trim(new Char[] { '\r', '\n' });

                    var cell = GetPDFCellBorderLess(string.IsNullOrWhiteSpace(commentsByReviewer) ? "" : commentsByReviewer, false, false);
                    detail.AddCell(cell);


                    //hereafter we dont need any more headers.
                    displayHeader = false;
                }

                //save the header alone with the attestation detail
                doc1.Add(detail);
                doc1.Add(new Paragraph("\n"));

                var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport(campaign.Id);
                var attestationUserTask = attestationUserDetails.Where(a => a.TaskId > 0 && a.EntitlementType == EntitlementType.Account.ToString()).Select(a => a.TaskId).Distinct();
                bool mulitpleLevel = false;
                bool mulitpleEndpoint = attestationEndpoint.Count > 1;

                if (!mulitpleLevel && attestationUserTask.Except(allTasksForThisCampaign.Select(a => a.Id).Distinct()).Any())
                    mulitpleLevel = true;

                //default column list
                int columnCount = 7;

                if (mulitpleEndpoint)
                    columnCount += 1;
                if (mulitpleLevel)
                    columnCount += 1;

                bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
                bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
                bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;

                showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                #region SRM Usage
                //showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                //showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                #endregion
                showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));
                //showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                showReserved8 = false;
                showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));

                if (showActionReserved1)
                {
                    columnCount += 1;
                }

                if (showActionReserved2)
                {
                    columnCount += 1;
                }

                if (showActionReserved3)
                {
                    columnCount += 1;
                }

                if (showReserved1)
                {
                    columnCount += 1;
                }

                if (showReserved2)
                {
                    columnCount += 1;
                }

                if (showReserved3)
                {
                    columnCount += 1;
                }

                if (showReserved4)
                {
                    columnCount += 1;
                }

                if (showReserved5)
                {
                    columnCount += 1;
                }

                if (showReserved6)
                {
                    columnCount += 1;
                }

                if (showReserved7)
                {
                    columnCount += 1;
                }

                if (showReserved8)
                {
                    columnCount += 1;
                }

                if (showReserved9)
                {
                    columnCount += 1;
                }

                if (showReserved10)
                {
                    columnCount += 1;
                }

                PdfPTable table = new PdfPTable(columnCount);
                table.HeaderRows = 1;
                table.WidthPercentage = 100;
                List<float> snapshotWidths = new List<float> { 2f, 3f, 2f, 2f, 2f, 3f, 2f };

                //set the size for additional columns
                if (columnCount > 7)
                {
                    for (int extraColumn = 7; extraColumn < columnCount; extraColumn++)
                    {
                        snapshotWidths.Add(2f);
                    }
                }

                snapshotWidths = snapshotWidths.Take(columnCount).ToList();
                table.SetTotalWidth(snapshotWidths.ToArray());
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.BorderColor = new Color(221, 221, 221);

                int currentRow = 1;
                int colStartPosition = 1;
                int colNum = colStartPosition;

                if (mulitpleEndpoint)
                    table.AddCell(GetPDFCell("System Name", true));

                if (attestationCampaign.AttestationType == AttestationType.User)
                {
                    table.AddCell(GetPDFCell("Login Name", true));
                    table.AddCell(GetPDFCell("Display Name", true));
                }
                //else if (attestationCampaign.AttestationType == AttestationType.Role)
                //{
                //    table.AddCell(GetPDFCell("Name", true));
                //    table.AddCell(GetPDFCell("Description", true));
                //}
                //else if (attestationCampaign.AttestationType == AttestationType.Privileged)
                //{
                //    table.AddCell(GetPDFCell("Applied On", true));
                //    table.AddCell(GetPDFCell("Activity", true));
                //}

                if (attestationCampaign.AttestationType == AttestationType.User)
                {
                    table.AddCell(GetPDFCell("Entitlement", true));
                    table.AddCell(GetPDFCell("Type", true));
                    table.AddCell(GetPDFCell("Action", true));
                }

                #region reserved columns
                if (showReserved1)
                {
                    var cellHeader = "Additional Info";
                    if (attestationCampaign.AttestationType == AttestationType.User)
                    {
                        //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                        //{
                        //    cellHeader = "View";
                        //}
                        //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                        //{
                        //    cellHeader = "MenuName";
                        //}
                    }
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved2)
                {
                    var cellHeader = "Is Locked";
                    if (attestationCampaign.AttestationType == AttestationType.User)
                    {
                        //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.CasinoManagementSystem))
                        //{
                        //    cellHeader = "Create";
                        //}
                        //if (availableEndpointTypes.Contains((long)Infrastructure.Enums.EndpointType.PSFinance))
                        //{
                        //    cellHeader = "BarName";
                        //}
                    }
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved3)
                {
                    var cellHeader = "Valid To";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved4)
                {
                    var cellHeader = "Detail 4";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved5)
                {
                    var cellHeader = "Detail 5";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved6)
                {
                    var cellHeader = "Detail 6";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved7)
                {
                    var cellHeader = "Detail 7";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved8)
                {
                    var cellHeader = "Detail 8";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved9)
                {
                    var cellHeader = "Detail 9";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showReserved10)
                {
                    var cellHeader = "Detail 10";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showActionReserved1)
                {
                    var cellHeader = "Action Detail 1";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showActionReserved2)
                {
                    var cellHeader = "Action Detail 2";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }

                if (showActionReserved3)
                {
                    var cellHeader = "Action Detail 3";
                    table.AddCell(GetPDFCell(cellHeader, true));
                }
                #endregion

                table.AddCell(GetPDFCell("Reviewed BY", true));
                table.AddCell(GetPDFCell("Comments", true));

                if (mulitpleLevel)
                    table.AddCell(GetPDFCell("Attestation Level", true));


                currentRow++;

                foreach (PresentationExportAttestationEntitlementReviewDetail action in attestationUserDetails)
                {
                    colStartPosition = 1;
                    colNum = colStartPosition;
                    if (action.IsVisible)
                    {
                        // add workcell entry for top level
                        if (mulitpleEndpoint)
                            table.AddCell(GetPDFCell(action.EndpointSystemName, false, action.IsChanged));
                        table.AddCell(GetPDFCell(action.UserName, false, action.IsChanged));
                        table.AddCell(GetPDFCell(action.Name, false, action.IsChanged));

                        if (attestationCampaign.AttestationType == AttestationType.User)
                        {
                            table.AddCell(GetPDFCell(action.EntitlementName, false, action.IsChanged));
                            table.AddCell(GetPDFCell(action.EntitlementType, false, action.IsChanged));

                            if (action.Approved)
                            {
                                table.AddCell(GetPDFCell(action.IsActionable ? "Retained" : "", false, action.IsChanged));
                            }
                            else
                            {
                                table.AddCell(GetPDFCell(action.IsActionable ? "Revoked" : "", false, action.IsChanged));
                            }
                        }

                        if (showReserved1)
                        {
                            table.AddCell(GetPDFCell(action.Reserved1, false, action.IsChanged));
                        }

                        if (showReserved2)
                        {
                            table.AddCell(GetPDFCell(action.Reserved2, false, action.IsChanged));
                        }

                        if (showReserved3)
                        {
                            table.AddCell(GetPDFCell(action.Reserved3, false, action.IsChanged));
                        }

                        if (showReserved4)
                        {
                            table.AddCell(GetPDFCell(action.Reserved4, false, action.IsChanged));
                        }

                        if (showReserved5)
                        {
                            table.AddCell(GetPDFCell(action.Reserved5, false, action.IsChanged));
                        }

                        if (showReserved6)
                        {
                            table.AddCell(GetPDFCell(action.Reserved6, false, action.IsChanged));
                        }

                        if (showReserved7)
                        {
                            table.AddCell(GetPDFCell(action.Reserved7, false, action.IsChanged));
                        }

                        if (showReserved8)
                        {
                            table.AddCell(GetPDFCell(action.Reserved8, false, action.IsChanged));
                        }

                        if (showReserved9)
                        {
                            table.AddCell(GetPDFCell(action.Reserved9, false, action.IsChanged));
                        }

                        if (showReserved10)
                        {
                            table.AddCell(GetPDFCell(action.Reserved10, false, action.IsChanged));
                        }

                        if (showActionReserved1)
                        {
                            table.AddCell(GetPDFCell(action.ActionReserved1, false, action.IsChanged));
                        }

                        if (showActionReserved2)
                        {
                            table.AddCell(GetPDFCell(action.ActionReserved2, false, action.IsChanged));
                        }

                        if (showActionReserved3)
                        {
                            table.AddCell(GetPDFCell(action.ActionReserved3, false, action.IsChanged));
                        }

                        var lstModifedBy = action.LastModifiedBy;
                        table.AddCell(GetPDFCell(string.IsNullOrEmpty(lstModifedBy) ? action.CreatedBy : lstModifedBy, false, action.IsChanged));

                        if (!string.IsNullOrEmpty(action.Comments) && action.Comments.Split(new string[] { "\\r\\n" }, StringSplitOptions.None).Length > 1)
                        {
                            var p = new Paragraph();
                            p.Font = FontFactory.GetFont("Helvetica", 10, Font.NORMAL, new Color(102, 102, 102));
                            foreach (var msg in action.Comments.Split(new string[] { "\\r\\n" }, StringSplitOptions.None))
                            {
                                if (string.IsNullOrEmpty(msg.Trim())) continue;
                                p.Add(string.Format("{0} \n", msg));
                            }

                            table.AddCell(GetPDFCell(p, false, action.IsChanged));
                        }
                        else
                        {
                            table.AddCell(GetPDFCell(string.IsNullOrEmpty(action.Comments) ? "" : action.Comments.Replace("\\r\\n", ""), false, action.IsChanged));
                        }

                        if (mulitpleLevel)
                        {
                            int state = 0;
                            if (TaskState.ContainsKey(action.TaskId))
                            {
                                state = TaskState[action.TaskId];
                            }
                            else
                            {
                                if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                                {
                                    var actionTask = Task.GetById(action.TaskId);
                                    if (actionTask != null)
                                    {
                                        var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                        attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                        var attestedUser = User.GetById(attestedUserId);

                                        TaskEndorsedBy.Add(action.TaskId, attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System");
                                        state = actionTask.WorkflowStateId;
                                        TaskState.Add(action.TaskId, state);
                                    }
                                }
                            }
                            table.AddCell(GetPDFCell(state > 0 ? state.ToString() : "", false, action.IsChanged));
                        }

                        currentRow++;
                    }
                }
                doc1.Add(table);

                doc1.Close();
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export pdf activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private void ExportCSVToFilePath(long taskId, AttestationCampaign campaign)
        {
            try
            {
                Directory.CreateDirectory(MasterConfigHelper.AttestationSnapshotFilePath);
                StreamWriter objWriter;

                objWriter = new StreamWriter(String.Format("{0}{1}.csv", MasterConfigHelper.AttestationSnapshotFilePath, campaign.CampaignName));

                objWriter.Write(ExportCSV(taskId, campaign).ToString());

                objWriter.Close();
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export csv activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private StringBuilder ExportCSV(long taskId, AttestationCampaign campaign)
        {
            try
            {
                StringBuilder objWriter = new StringBuilder();
                var taskDetail = Task.GetById(taskId, true);
                var allTasksForThisCampaign = Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();
                var attestationCampaign = Attestation.GetById(campaign.AttestationID);

                var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport(campaign.Id);

                var attestationUserTask = attestationUserDetails.Where(a => a.TaskId > 0 && a.EntitlementType == EntitlementType.Account.ToString()).Select(a => a.TaskId).Distinct();

                bool mulitpleLevel = false;
                if (!mulitpleLevel && attestationUserTask.Except(allTasksForThisCampaign.Select(a => a.Id).Distinct()).Any())
                    mulitpleLevel = true;

                bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
                bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
                bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;

                showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                #region SRM
                //showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                //showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));

                #endregion
                //showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));

                //Endpoint & Enddate
                var attestationEndpoint = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(campaign.AttestationID).ToList();
                List<long> availableEndpointTypes = new List<long>();
                foreach (var endpoint in attestationEndpoint)
                {
                    if (!availableEndpointTypes.Any(a => a == (long)endpoint.EndpointSystemType))
                    {
                        availableEndpointTypes.Add((long)endpoint.EndpointSystemType);
                    }
                }

                //Header
                string header = "Attestation Name,Campaign Name,System Name,Login Name,Display Name,Entitlement,Type,Action";

                //if (attestationCampaign.AttestationType == AttestationType.Role)
                //{
                //    header = "Attestation Name,Campaign Name,System Name,Name,Description,Entitlement,Type,Action";
                //}

                //if (attestationCampaign.AttestationType == AttestationType.Privileged)
                //{
                //    header = "Attestation Name,Campaign Name,System Name,Applied On,Activity,Entitlement,Type,Action";
                //}

                #region reserved columns
                if (showReserved1)
                {
                    var tmpHeader = "Additional Info";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved2)
                {
                    var tmpHeader = "Is Locked";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved3)
                {
                    var tmpHeader = "Valid To";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved4)
                {
                    var tmpHeader = "Detail 4";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved5)
                {
                    var tmpHeader = "Detail 5";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved6)
                {
                    var tmpHeader = "Detail 6";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showReserved7)
                {
                    header += string.Format(",{0}", "Detail 7");
                }

                if (showReserved8)
                {
                    header += string.Format(",{0}", "Detail 8");
                }

                if (showReserved9)
                {
                    header += string.Format(",{0}", "Detail 9");
                }

                if (showReserved10)
                {
                    var tmpHeader = "Detail 10";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showActionReserved1)
                {
                    var tmpHeader = "Action Detail 1";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showActionReserved2)
                {
                    var tmpHeader = "Action Detail 2";
                    header += string.Format(",{0}", tmpHeader);
                }

                if (showActionReserved3)
                {
                    var tmpHeader = "Action Detail 3";
                    header += string.Format(",{0}", tmpHeader);
                }
                #endregion

                if (mulitpleLevel)
                    header += ",Reviewed By,Comments,Endorsed By,Attestation Level";
                else
                    header += ",Reviewed By,Comments,Endorsed By";

                header += ",Level1 Approver, Reviewed Date,Level2 Approver, Reviewed Date,Level3 Approver, Reviewed Date";

                objWriter.AppendLine(header);

                var previousParentEndsorer = "";
                string currentDisplayName = string.Empty;
                string previousDisplayName = string.Empty;
                string currentUserName = string.Empty;
                string previousUserName = string.Empty;
                foreach (PresentationExportAttestationEntitlementReviewDetail action in attestationUserDetails)
                {
                    List<string> towrite = new List<string>();
                    if (action.IsVisible)
                    {
                        towrite.Add(action.AttestationName);
                        towrite.Add(action.CampaignName);
                        towrite.Add(action.EndpointSystemName);
                        var userName = action.UserName;
                        if (action.EntitlementType == EntitlementType.TCodes.ToString() || action.EntitlementType == EntitlementType.IndirectRole.ToString())
                        {
                        }
                        else
                        {
                            previousUserName = userName;
                        }
                        userName = previousUserName;

                        towrite.Add(userName.Contains(",") ? string.Format("\"{0}\"", userName) : userName);
                        var name = action.Name;
                        if (action.EntitlementType == EntitlementType.TCodes.ToString() || action.EntitlementType == EntitlementType.IndirectRole.ToString())
                        {
                        }
                        else
                        {
                            previousDisplayName = name;
                        }
                        name = previousDisplayName;

                        if (!string.IsNullOrWhiteSpace(name))
                            towrite.Add(name.Contains(",") ? string.Format("\"{0}\"", name) : name);
                        var entitlementName = action.EntitlementName;
                        towrite.Add(entitlementName.Contains(",") ? string.Format("\"{0}\"", entitlementName) : entitlementName);
                        towrite.Add(action.EntitlementType);
                        if (attestationCampaign.AttestationType == AttestationType.User)
                        {
                            if (action.Approved)
                            {
                                towrite.Add(action.IsActionable ? "Retained" : "");
                            }
                            else
                            {
                                towrite.Add(action.IsActionable ? "Revoked" : "");
                            }
                        }

                        if (showReserved1)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved2)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved3)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved4)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved5)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved6)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved7)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved8)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved9)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showReserved10)
                        {
                            towrite.Add(action.Reserved1);
                        }

                        if (showActionReserved1)
                        {
                            towrite.Add(action.ActionReserved1);
                        }

                        if (showActionReserved2)
                        {
                            towrite.Add(action.ActionReserved2);
                        }

                        if (showActionReserved3)
                        {
                            towrite.Add(action.ActionReserved3);
                        }

                        var lstModifedBy = action.LastModifiedBy; var createdBy = action.CreatedBy;
                        lstModifedBy = string.IsNullOrEmpty(lstModifedBy) ? "" : lstModifedBy;
                        lstModifedBy = lstModifedBy.Contains(",") ? string.Format("\"{0}\"", lstModifedBy) : lstModifedBy;
                        createdBy = string.IsNullOrEmpty(createdBy) ? "" : createdBy;
                        createdBy = createdBy.Contains(",") ? string.Format("\"{0}\"", createdBy) : createdBy;
                        towrite.Add(string.IsNullOrEmpty(lstModifedBy) ? createdBy : lstModifedBy);

                        var comments = string.IsNullOrEmpty(action.Comments) ? "" : action.Comments.Replace("\\r\\n", " \n ").Trim().Trim('\n');
                        comments = string.IsNullOrEmpty(comments) ? "" : comments;
                        towrite.Add(comments.Contains(",") ? string.Format("\"{0}\"", comments) : (comments.Contains("\n") ? string.Format("\"{0}\"", comments) : comments));

                        string approver = "";
                        if (TaskEndorsedBy.ContainsKey(action.TaskId))
                        {
                            approver = TaskEndorsedBy[action.TaskId];
                        }
                        else
                        {
                            if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                            {
                                var actionTask = Task.GetById(action.TaskId);
                                if (actionTask != null)
                                {
                                    var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                    attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                    var attestedUser = User.GetById(attestedUserId);

                                    approver = attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System";
                                    TaskEndorsedBy.Add(action.TaskId, approver);
                                    TaskState.Add(action.TaskId, actionTask.WorkflowStateId);
                                }
                            }
                        }
                        approver = approver.Contains(",") ? string.Format("\"{0}\"", approver) : approver;
                        if (!string.IsNullOrEmpty(approver))
                            previousParentEndsorer = approver;
                        else
                            approver = previousParentEndsorer;

                        towrite.Add(approver);

                        if (mulitpleLevel)
                        {
                            int state = 0;
                            if (TaskState.ContainsKey(action.TaskId))
                            {
                                state = TaskState[action.TaskId];
                            }
                            else
                            {
                                if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                                {
                                    var actionTask = Task.GetById(action.TaskId);
                                    if (actionTask != null)
                                    {
                                        var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                        attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                        var attestedUser = User.GetById(attestedUserId);

                                        TaskEndorsedBy.Add(action.TaskId, attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System");
                                        state = actionTask.WorkflowStateId;
                                        TaskState.Add(action.TaskId, state);
                                    }
                                }
                            }

                            towrite.Add(state > 0 ? state.ToString() : "");
                        }
                        var level1 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 1);
                        if (level1 != null)
                        {

                            if (!string.IsNullOrWhiteSpace(level1.DisplayName))
                            {
                                towrite.Add($"{level1.DisplayName}({level1.UserName})");
                            }
                            else
                            {
                                towrite.Add("");
                            }
                            towrite.Add($"{level1.ReviewedDate?.ToString("dd/MM/yyyy")}");
                        }
                        else
                        {
                            towrite.Add("");
                            towrite.Add("");
                        }
                        var level2 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 2);
                        if (level2 != null)
                        {
                            if (!string.IsNullOrWhiteSpace(level2.DisplayName))
                            {
                                towrite.Add($"{level2.DisplayName}({level2.UserName})");
                            }
                            else
                            {
                                towrite.Add("");
                            }
                            towrite.Add($"{level2.ReviewedDate?.ToString("dd/MM/yyyy")}");
                        }
                        else
                        {
                            towrite.Add("");
                            towrite.Add("");
                        }
                        var level3 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 3);
                        if (level3 != null)
                        {

                            if (!string.IsNullOrWhiteSpace(level3.DisplayName))
                            {
                                towrite.Add($"{level3.DisplayName}({level3.UserName})");
                            }
                            else
                            {
                                towrite.Add("");
                            }
                            towrite.Add($"{level3.ReviewedDate?.ToString("dd/MM/yyyy") ?? ""}");
                        }
                        else
                        {
                            towrite.Add("");
                            towrite.Add("");
                        }
                        objWriter.AppendLine(string.Join(",", towrite));
                    }
                }
                return objWriter;
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export csv activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
            return new StringBuilder();
        }

        public void ExportFlatExcelToFilePath(long taskId, AttestationCampaign campaign)
        {
            try
            {
                Directory.CreateDirectory(MasterConfigHelper.AttestationSnapshotFilePath);

                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                ExportFlatExcel(excel, taskId, campaign);

                // ToDO: Save to ftp location or Database
                excel.SaveAs(new FileInfo(String.Format("{0}\\{1}\\{1}_Flat.xlsx", MasterConfigHelper.AttestationSnapshotFilePath, campaign.CampaignName)));
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export flat excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private void ExportFlatExcel(OfficeOpenXml.ExcelPackage excel, long taskId, AttestationCampaign campaign)
        {
            try
            {
                string accountType = string.Empty;
                var taskDetail = Task.GetById(taskId, true);
                var allTasksForThisCampaign = Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();
                List<long> availableEndpoints = new List<long>();
                List<long> availableEndpointTypes = new List<long>();
                var orgTask = Task.GetById(taskId);
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);

                var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_ATTESTATION_REVIEW", campaignDetails.Attestation.AttestationType.ToString().ToUpper()));

                var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport(campaign.Id);
                var attestationUserTask = attestationUserDetails.Where(a => a.TaskId > 0 && a.EntitlementType == EntitlementType.Account.ToString()).Select(a => a.TaskId).Distinct();
                bool mulitpleLevel = false;

                //if (!mulitpleLevel && attestationUserTask.Except(allTasksForThisCampaign.Select(a => a.Id).Distinct()).Any())
                //    mulitpleLevel = true;

                var attestationEndpoint = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(campaign.AttestationID).ToList();
                if (attestationEndpoint != null && attestationEndpoint.Count > 0)
                {
                    foreach (var availableEndpoint in attestationEndpoint)
                    {
                        var endpoint = EndPoint.GetById(availableEndpoint.EndpointSystemId);

                        if (!availableEndpoints.Any(a => a == endpoint.Id))
                        {
                            availableEndpoints.Add(endpoint.Id);
                        }

                        if (!availableEndpointTypes.Any(a => a == endpoint.EndpointSystemTypeId.Value))
                        {
                            availableEndpointTypes.Add(endpoint.EndpointSystemTypeId.Value);
                        }
                    }
                }

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

                showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                #region SRM
                //showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                //showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                #endregion
                showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));
                //showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));

                workSheet.Cells[currentRow, colNum].Value = "Attestation Name";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Campaign Name";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Campaign Start Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //workSheet.Cells[currentRow, ++colNum].Value = "Campaign End Date";
                //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
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

                if (showReserved1)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Additional Info";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                workSheet.Cells[currentRow, ++colNum].Value = "Parent Object";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                if (showReserved3)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Valid To";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved2)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Is Locked";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }
                workSheet.Cells[currentRow, ++colNum].Value = "Action";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                workSheet.Cells[currentRow, ++colNum].Value = "Comments";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                workSheet.Cells[currentRow, ++colNum].Value = "Modified By";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //Commented for SP Requirement
                //workSheet.Cells[currentRow, ++colNum].Value = "Endorsed By";
                //workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                #region reserved columns
                if (showReserved4)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 4";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved5)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 5";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved6)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 6";
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
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showActionReserved1)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 1";
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
                if (mulitpleLevel)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Attestation Level";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }
                workSheet.Cells[currentRow, ++colNum].Value = "Level1 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Level1 Delegation Comments";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                workSheet.Cells[currentRow, ++colNum].Value = "Level2 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Level2 Delegation Comments";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                workSheet.Cells[currentRow, ++colNum].Value = "Level3 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Level3 Delegation Comments";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                //move the row index next to header
                currentRow++;
                var previousParentEndsorer = "";
                string currentDisplayName = string.Empty;
                string previousDisplayName = string.Empty;
                string currentUserName = string.Empty;
                string previousUserName = string.Empty;

                string level1Approver = string.Empty;
                string level2Approver = string.Empty;
                string level3Approver = string.Empty;

                string level1ReviewedDate = string.Empty;
                string level2ReviewedDate = string.Empty;
                string level3ReviewedDate = string.Empty;

                string level1DelegationComments = string.Empty;
                string level2DelegationComments = string.Empty;
                string level3DelegationComments = string.Empty;
                PresentationSnapshotActionHistory level1 = null;
                PresentationSnapshotActionHistory level2 = null;
                PresentationSnapshotActionHistory level3 = null;
                PresentationExportAttestationEntitlementReviewDetail parentAction = null;
                foreach (PresentationExportAttestationEntitlementReviewDetail action in attestationUserDetails)
                {
                    colStartPosition = 1;
                    colNum = colStartPosition;
                    if (action.IsVisible)
                    {


                        if (action.EntitlementType == EntitlementType.Account.ToString())
                        {
                            currentDisplayName = action.Name;
                            currentUserName = action.UserName;
                            accountType = action.AccountType;
                            parentAction = null;

                        }

                        if (action.EntitlementType != EntitlementType.Account.ToString())
                        {
                            if (action.EntitlementType == Infrastructure.Enums.EntitlementType.BusinessRole.ToString() || action.EntitlementType == Infrastructure.Enums.EntitlementType.Position.ToString())
                            {
                                parentAction = action;
                            }
                            if (action.EntitlementType == Infrastructure.Enums.EntitlementType.TechnicalRole.ToString() || action.EntitlementType == Infrastructure.Enums.EntitlementType.IndirectRole.ToString() || action.EntitlementType == Infrastructure.Enums.EntitlementType.TCodes.ToString())
                            {
                                if (parentAction != null && action.ParentObject == parentAction.EntitlementName)
                                {
                                    action.Approved = parentAction.Approved;
                                }
                            }
                        }
                        else
                        {
                            previousDisplayName = currentDisplayName;
                            previousUserName = currentUserName;
                        }

                        // add workcell entry for top level
                        workSheet.Cells[currentRow, colNum].Value = action.AttestationName;
                        workSheet.Cells[currentRow, ++colNum].Value = action.CampaignName;
                        workSheet.Cells[currentRow, ++colNum].Value = campaign.CreatedDate.ToString(Constants.DateFormatForSP);
                        //workSheet.Cells[currentRow, ++colNum].Value = campaign.EndDate.HasValue ? campaign.EndDate.Value.ToString(Constants.DateFormatForSP) : "";
                        workSheet.Cells[currentRow, ++colNum].Value = campaign.Comments?.Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "").Replace("!~", Environment.NewLine);
                        workSheet.Cells[currentRow, ++colNum].Value = action.EndpointSystemName;
                        workSheet.Cells[currentRow, ++colNum].Value = previousUserName;
                        workSheet.Cells[currentRow, ++colNum].Value = previousDisplayName;

                        if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = accountType;
                            workSheet.Cells[currentRow, ++colNum].Value = action.EntitlementType;
                            workSheet.Cells[currentRow, ++colNum].Value = action.EntitlementName;
                            workSheet.Cells[currentRow, ++colNum].Value = action.Detail;
                        }

                        string approver = "";
                        if (TaskEndorsedBy.ContainsKey(action.TaskId))
                        {
                            approver = TaskEndorsedBy[action.TaskId];
                        }
                        else
                        {
                            if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                            {
                                var actionTask = Task.GetById(action.TaskId);
                                if (actionTask != null)
                                {
                                    var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                    attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                    var attestedUser = User.GetById(attestedUserId);

                                    approver = attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System";
                                    TaskEndorsedBy.Add(action.TaskId, approver);
                                    TaskState.Add(action.TaskId, actionTask.WorkflowStateId);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(approver))
                            previousParentEndsorer = approver;
                        else
                            approver = previousParentEndsorer;
                        if (showReserved1)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved1;
                        }
                        workSheet.Cells[currentRow, ++colNum].Value = action.ParentObject;

                        if (showReserved3)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved3;
                        }
                        if (showReserved2)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved2;
                        }


                        if (action.Approved)
                        {
                            //COmmented for SP requirement
                            //workSheet.Cells[currentRow, ++colNum].Value = action.IsActionable ? "Retained" : "";
                            workSheet.Cells[currentRow, ++colNum].Value = "Retained";
                        }
                        else
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                        }
                        workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(action.Comments) ? "" : action.Comments.Replace("\\r\\n", " \n ").Trim().Trim('\n');
                        workSheet.Cells[currentRow, colNum].Style.WrapText = false;

                        var lstModifedBy = action.LastModifiedBy;
                        workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(lstModifedBy) ? approver : lstModifedBy;

                        #region Reserved Field Value




                        if (showReserved4)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved4;
                        }

                        if (showReserved5)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved5;
                        }

                        if (showReserved6)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved6;
                        }

                        if (showReserved7)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved7;
                        }

                        if (showReserved8)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved8;
                        }

                        if (showReserved9)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved9;
                        }

                        if (showReserved10)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved1;
                        }

                        if (showActionReserved1)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved2;
                        }

                        if (showActionReserved2)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved3;
                        }

                        if (showActionReserved3)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved3;
                        }
                        #endregion

                        if (mulitpleLevel)
                        {
                            int state = 0;
                            if (TaskState.ContainsKey(action.TaskId))
                            {
                                state = TaskState[action.TaskId];
                            }
                            else
                            {
                                if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                                {
                                    var actionTask = Task.GetById(action.TaskId);
                                    if (actionTask != null)
                                    {
                                        var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                        attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                        var attestedUser = User.GetById(attestedUserId);

                                        TaskEndorsedBy.Add(action.TaskId, attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System");
                                        state = actionTask.WorkflowStateId;
                                        TaskState.Add(action.TaskId, state);
                                    }
                                }
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = state > 0 ? state.ToString() : "";
                        }

                        level1 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 1);
                        level2 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 2);
                        level3 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 3);

                        if (level1 != null)
                        {
                            level1Approver = $"{level1.DisplayName}({level1.UserName})";
                            level1ReviewedDate = $"{level1.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                            level1DelegationComments = $"{level1.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                        }
                        if (level2 != null)
                        {
                            level2Approver = $"{level2.DisplayName}({level2.UserName})";
                            level2ReviewedDate = $"{level2.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                            level2DelegationComments = $"{level2.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                        }
                        if (level3 != null)
                        {
                            level3Approver = $"{level3.DisplayName}({level3.UserName})";
                            level3ReviewedDate = $"{level3.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                            level3DelegationComments = $"{level3.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                        }

                        workSheet.Cells[currentRow, ++colNum].Value = level1Approver;
                        workSheet.Cells[currentRow, ++colNum].Value = level1ReviewedDate;
                        workSheet.Cells[currentRow, ++colNum].Value = level1DelegationComments;

                        workSheet.Cells[currentRow, ++colNum].Value = level2Approver;
                        workSheet.Cells[currentRow, ++colNum].Value = level2ReviewedDate;
                        workSheet.Cells[currentRow, ++colNum].Value = level2DelegationComments;


                        workSheet.Cells[currentRow, ++colNum].Value = level3Approver;
                        workSheet.Cells[currentRow, ++colNum].Value = level3ReviewedDate;
                        workSheet.Cells[currentRow, ++colNum].Value = level3DelegationComments;

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

            }
            catch (Exception ex)
            {
                string message = "Error while executing the export flat excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private void ExportUserProfileComparision(OfficeOpenXml.ExcelPackage excel, long taskId, AttestationCampaign campaign)
        {
            string accountType = string.Empty;
            var taskDetail = Task.GetById(taskId, true);
            var allTasksForThisCampaign = Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();
            List<long> availableEndpoints = new List<long>();
            List<long> availableEndpointTypes = new List<long>();
            var orgTask = Task.GetById(taskId);
            var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);
            var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport((long)taskDetail.AttestationCampaignId).Where(x => x.Level == 2).ToList();
            var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_ATTESTATION_USER_COMAPARISION_REVIEW", campaignDetails.Attestation.AttestationType.ToString().ToUpper()));
            var currentRow = 1;
            var colNum = 1;
            workSheet.Cells[currentRow, colNum].Value = "Application Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Account Type";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "UserName";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Account Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Role/Group";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Description";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

            workSheet.Cells[currentRow, ++colNum].Value = "Retained/Revoked";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

            workSheet.Cells[currentRow, ++colNum].Value = "Level1 Approver";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Level1 Delegation Comments";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

            workSheet.Cells[currentRow, ++colNum].Value = "Level2 Approver";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Level2 Delegation Comments";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

            workSheet.Cells[currentRow, ++colNum].Value = "Level3 Approver";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Level3 Delegation Comments";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            ++currentRow;

            string level1Approver = string.Empty;
            string level2Approver = string.Empty;
            string level3Approver = string.Empty;

            string level1ReviewedDate = string.Empty;
            string level2ReviewedDate = string.Empty;
            string level3ReviewedDate = string.Empty;

            string level1DelegationComments = string.Empty;
            string level2DelegationComments = string.Empty;
            string level3DelegationComments = string.Empty;
            PresentationSnapshotActionHistory level1 = null;
            PresentationSnapshotActionHistory level2 = null;
            PresentationSnapshotActionHistory level3 = null;
            var campaignId = (long)taskDetail.AttestationCampaignId;

            history = DI.Chakra.DomainModel.Presentation.PresentationSnapshotActionHistory.GetHistoryByCampaignId(campaignId);
            int pageSize = 100;
            int pageNumber = 1;
            bool isRecordExists = true;
            while (isRecordExists)
            {
                var userDetails = PresentationUserEntitlementData.GetAttestationTaskEntitlementUserComparisionByCampaignId(pageNumber, pageSize, campaignId);
                if(userDetails == null || userDetails.Count == 0)
                {
                    isRecordExists = false;
                    break;
                }
                pageNumber = pageNumber+1;
                foreach (var item in userDetails)
                {
                    colNum = 1;

                    workSheet.Cells[currentRow, colNum].Value = item.EndpointSystemName;
                    workSheet.Cells[currentRow, ++colNum].Value = item.AccountType;
                    workSheet.Cells[currentRow, ++colNum].Value = item.Name;
                    workSheet.Cells[currentRow, ++colNum].Value = item.UserName;
                    workSheet.Cells[currentRow, ++colNum].Value = item.Entitlement;
                    workSheet.Cells[currentRow, ++colNum].Value = item.EntitlementDescription;
                    if (item.Approve)
                    {
                        workSheet.Cells[currentRow, ++colNum].Value = "Retained";
                        workSheet.Cells[currentRow, colNum].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                    }
                    else
                    {
                        workSheet.Cells[currentRow, ++colNum].Value = "Revoked";
                        workSheet.Cells[currentRow, colNum].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                    }
                    workSheet.Cells[currentRow, colNum].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Cells[currentRow, colNum].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

                    level1 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == item.SnapshotActionId && i.Version == 1);
                    level2 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == item.SnapshotActionId && i.Version == 2);
                    level3 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == item.SnapshotActionId && i.Version == 3);

                    if (level1 != null)
                    {
                        level1Approver = $"{level1.DisplayName}({level1.UserName})";
                        level1ReviewedDate = $"{level1.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                        level1DelegationComments = $"{level1.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                    }
                    if (level2 != null)
                    {
                        level2Approver = $"{level2.DisplayName}({level2.UserName})";
                        level2ReviewedDate = $"{level2.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                        level2DelegationComments = $"{level2.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                    }
                    if (level3 != null)
                    {
                        level3Approver = $"{level3.DisplayName}({level3.UserName})";
                        level3ReviewedDate = $"{level3.ReviewedDate?.ToString(Constants.DateFormatForSP)}";
                        level3DelegationComments = $"{level3.DelegationComments?.Replace("<br/>", Environment.NewLine) ?? ""}";
                    }

                    workSheet.Cells[currentRow, ++colNum].Value = level1Approver;
                    workSheet.Cells[currentRow, ++colNum].Value = level1ReviewedDate;
                    workSheet.Cells[currentRow, ++colNum].Value = level1DelegationComments;

                    workSheet.Cells[currentRow, ++colNum].Value = level2Approver;
                    workSheet.Cells[currentRow, ++colNum].Value = level2ReviewedDate;
                    workSheet.Cells[currentRow, ++colNum].Value = level2DelegationComments;


                    workSheet.Cells[currentRow, ++colNum].Value = level3Approver;
                    workSheet.Cells[currentRow, ++colNum].Value = level3ReviewedDate;
                    workSheet.Cells[currentRow, ++colNum].Value = level3DelegationComments;

                    currentRow++;
                }
            }
            for (int i = 1; i <= colNum; i++)
            {
                workSheet.Column(i).AutoFit();
            }
        }

        private void ExportHierarchyExcelToFilePath(long taskId, AttestationCampaign campaign)
        {
            try
            {
                Directory.CreateDirectory(MasterConfigHelper.AttestationSnapshotFilePath);

                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                ExportHierarchyExcel(excel, taskId, campaign);

                // ToDO: Save to ftp location or Database
                excel.SaveAs(new FileInfo(String.Format("{0}\\{1}\\{1}_Hierarchy.xlsx", MasterConfigHelper.AttestationSnapshotFilePath, campaign.CampaignName)));
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export hierarchy excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private void ExportHierarchyExcel(OfficeOpenXml.ExcelPackage excel, long taskId, AttestationCampaign campaign)
        {
            try
            {
                var displayName = string.Empty;
                var userName = string.Empty;
                var taskDetail = Task.GetById(taskId, true);
                var allTasksForThisCampaign = Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();
                List<long> availableEndpoints = new List<long>();
                List<long> availableEndpointTypes = new List<long>();
                var orgTask = Task.GetById(taskId);
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);
                var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_", campaignDetails.Attestation.AttestationType.ToString().ToUpper(), "ATTESTATION_REVIEW"));

                var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport(campaign.Id);
                var attestationUserTask = attestationUserDetails.Where(a => a.TaskId > 0 && a.EntitlementType == EntitlementType.Account.ToString()).Select(a => a.TaskId).Distinct();
                bool mulitpleLevel = false;

                if (!mulitpleLevel && attestationUserTask.Except(allTasksForThisCampaign.Select(a => a.Id).Distinct()).Any())
                    mulitpleLevel = true;

                var attestationEndpoint = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(campaign.AttestationID).ToList();
                if (attestationEndpoint != null && attestationEndpoint.Count > 0)
                {
                    foreach (var availableEndpoint in attestationEndpoint)
                    {
                        var endpoint = EndPoint.GetById(availableEndpoint.EndpointSystemId);

                        if (!availableEndpoints.Any(a => a == endpoint.Id))
                        {
                            availableEndpoints.Add(endpoint.Id);
                        }

                        if (!availableEndpointTypes.Any(a => a == endpoint.EndpointSystemTypeId.Value))
                        {
                            availableEndpointTypes.Add(endpoint.EndpointSystemTypeId.Value);
                        }
                    }
                }

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

                showReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved1));
                showReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved2));
                showReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved3));
                showReserved4 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved4));
                #region SRM
                //showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
                //showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
                #endregion
                showReserved7 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved7));
                //showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
                showReserved9 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved9));
                showReserved10 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved10));

                showActionReserved1 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved1));
                showActionReserved2 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved2));
                showActionReserved3 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.ActionReserved3));

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
                //else if (campaignDetails.Attestation.AttestationType == AttestationType.Role)
                //{
                //    workSheet.Cells[currentRow, ++colNum].Value = "Name";
                //    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //    workSheet.Cells[currentRow, ++colNum].Value = "Description";
                //    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //}
                //else if (campaignDetails.Attestation.AttestationType == AttestationType.Privileged)
                //{
                //    workSheet.Cells[currentRow, ++colNum].Value = "Applied On";
                //    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //    workSheet.Cells[currentRow, ++colNum].Value = "Activity";
                //    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //}

                if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
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
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved2)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Is Locked";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved3)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Valid To";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved4)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 4";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved5)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 5";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showReserved6)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Detail 6";
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
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }

                if (showActionReserved1)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Action Detail 1";
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

                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed By";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Comments";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Endorsed By";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                if (mulitpleLevel)
                {
                    workSheet.Cells[currentRow, ++colNum].Value = "Attestation Level";
                    workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                }
                workSheet.Cells[currentRow, ++colNum].Value = "Level1 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Level2 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Level3 Approver";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Reviewed Date";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                //move the row index next to header
                currentRow++;
                var previousParentEndsorer = "";
                string currentDisplayName = string.Empty;
                string previousDisplayName = string.Empty;
                string currentUserName = string.Empty;
                string previousUserName = string.Empty;
                foreach (PresentationExportAttestationEntitlementReviewDetail action in attestationUserDetails)
                {
                    colStartPosition = 1;
                    colNum = colStartPosition;
                    if (action.IsVisible)
                    {
                        int level = (int)action.Level;

                        currentDisplayName = action.Name;
                        currentUserName = action.UserName;

                        if (action.EntitlementType == EntitlementType.TCodes.ToString() || action.EntitlementType == EntitlementType.IndirectRole.ToString())
                        {
                        }
                        else
                        {
                            previousDisplayName = currentDisplayName;
                            previousUserName = currentUserName;
                        }

                        // add workcell entry for top level
                        workSheet.Cells[currentRow, colNum].Value = level == 1 ? action.AttestationName : "";
                        workSheet.Cells[currentRow, ++colNum].Value = level == 1 ? action.CampaignName : "";
                        workSheet.Cells[currentRow, ++colNum].Value = level == 1 ? action.EndpointSystemName : "";
                        workSheet.Cells[currentRow, ++colNum].Value = previousUserName;
                        workSheet.Cells[currentRow, ++colNum].Value = previousDisplayName;

                        if (campaignDetails.Attestation.AttestationType == AttestationType.User)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.EntitlementName;
                            workSheet.Cells[currentRow, ++colNum].Value = action.EntitlementType;
                            workSheet.Cells[currentRow, ++colNum].Value = action.IsActionable ? action.Approved.ToString() : "";
                            workSheet.Cells[currentRow, ++colNum].Value = action.IsActionable ? action.Rejected.ToString() : "";
                        }

                        if (showReserved1)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved1;
                        }

                        if (showReserved2)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved2;
                        }

                        if (showReserved3)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved3;
                        }

                        if (showReserved4)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved4;
                        }

                        if (showReserved5)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved5;
                        }

                        if (showReserved6)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved6;
                        }

                        if (showReserved7)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved7;
                        }

                        if (showReserved8)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved8;
                        }

                        if (showReserved9)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved9;
                        }

                        if (showReserved10)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.Reserved10;
                        }

                        if (showActionReserved1)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved1;
                        }

                        if (showActionReserved2)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved2;
                        }

                        if (showActionReserved3)
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = action.ActionReserved3;
                        }

                        var lstModifedBy = action.LastModifiedBy;
                        workSheet.Cells[currentRow, ++colNum].Value = string.IsNullOrEmpty(lstModifedBy) ? action.CreatedBy : lstModifedBy;

                        workSheet.Cells[currentRow, ++colNum].Value = action.IsActionable ? (string.IsNullOrEmpty(action.Comments) ? "" : action.Comments.Replace("\\r\\n", " \n ").Trim().Trim('\n')) : "";
                        workSheet.Cells[currentRow, colNum].Style.WrapText = true;

                        string approver = "";
                        if (TaskEndorsedBy.ContainsKey(action.TaskId))
                        {
                            approver = TaskEndorsedBy[action.TaskId];
                        }
                        else
                        {
                            if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                            {
                                var actionTask = Task.GetById(action.TaskId);
                                if (actionTask != null)
                                {
                                    var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                    attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                    var attestedUser = User.GetById(attestedUserId);

                                    approver = attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System";
                                    TaskEndorsedBy.Add(action.TaskId, approver);
                                    TaskState.Add(action.TaskId, actionTask.WorkflowStateId);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(approver))
                            previousParentEndsorer = approver;
                        else
                            approver = previousParentEndsorer;

                        workSheet.Cells[currentRow, ++colNum].Value = approver;

                        if (mulitpleLevel)
                        {
                            int state = 0;
                            if (TaskState.ContainsKey(action.TaskId))
                            {
                                state = TaskState[action.TaskId];
                            }
                            else
                            {
                                if (action.TaskId > 0 && action.EntitlementType == EntitlementType.Account.ToString())
                                {
                                    var actionTask = Task.GetById(action.TaskId);
                                    if (actionTask != null)
                                    {
                                        var attestedUserId = actionTask.ActionUserId.HasValue ? actionTask.ActionUserId.Value : 0;
                                        attestedUserId = (actionTask.IsGroupTask && actionTask.AssignedTo.HasValue && !actionTask.IsDelegated) ? actionTask.AssignedTo.Value : attestedUserId;
                                        var attestedUser = User.GetById(attestedUserId);

                                        TaskEndorsedBy.Add(action.TaskId, attestedUser != null ? ($"{attestedUser.DisplayName}({attestedUser.UserName})") : "System");
                                        state = actionTask.WorkflowStateId;
                                        TaskState.Add(action.TaskId, state);
                                    }
                                }
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = state > 0 ? state.ToString() : "";
                        }
                        var level1 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 1);
                        if (level1 != null)
                        {

                            if (!string.IsNullOrWhiteSpace(level1.DisplayName))
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"{level1.DisplayName}({level1.UserName})";
                            }
                            else
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"";
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = $"{level1.ReviewedDate?.ToString("dd/MM/yyyy")}";
                        }
                        else
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                        }
                        var level2 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 2);
                        if (level2 != null)
                        {
                            if (!string.IsNullOrWhiteSpace(level2.DisplayName))
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"{level2.DisplayName}({level2.UserName})";
                            }
                            else
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"";
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = $"{level2.ReviewedDate?.ToString("dd/MM/yyyy")}";
                        }
                        else
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                        }
                        var level3 = history.FirstOrDefault(i => i.AttestationSnapshotActionId == action.Id && i.Version == 3);
                        if (level3 != null)
                        {

                            if (!string.IsNullOrWhiteSpace(level3.DisplayName))
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"{level3.DisplayName}({level3.UserName})";
                            }
                            else
                            {
                                workSheet.Cells[currentRow, ++colNum].Value = $"";
                            }
                            workSheet.Cells[currentRow, ++colNum].Value = $"{level3.ReviewedDate?.ToString("dd/MM/yyyy")}";
                        }
                        else
                        {
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                            workSheet.Cells[currentRow, ++colNum].Value = "";
                        }
                        if (action.IsChanged)
                        {
                            workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            workSheet.Cells[currentRow, 1, currentRow, colNum].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(228, 236, 245));
                        }

                        if (level >= 2)
                        {
                            workSheet.Row(currentRow).OutlineLevel = level;
                            workSheet.Row(currentRow).Collapsed = true;
                        }

                        currentRow++;
                    }
                }

                // Fit the columns according to its content
                for (int i = colStartPosition; i < colNum; i++)
                {
                    workSheet.Column(i).AutoFit();
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export hierarchy excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
        }

        private byte[] ExportHierarchyExcelAsByteArray(long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            try
            {
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                ExportHierarchyExcel(excel, taskId, campaign);

                // ToDO: Save to ftp location or Database
                report = excel.GetAsByteArray();
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export flat excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
            return report;
        }

        private byte[] ExportCSVAsByteArray(long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            try
            {
                report = Encoding.ASCII.GetBytes(ExportCSV(taskId, campaign).ToString());
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export csv activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }

            return report;
        }

        private byte[] ExportFlatExcelToByteArray(long taskId, AttestationCampaign campaign)
        {
            byte[] report = null;
            try
            {
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                ExportFlatExcel(excel, taskId, campaign);

                report = excel.GetAsByteArray();
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export flat excel activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }

            return report;
        }

        public byte[] ExportPDFAsByteArray(long taskId, AttestationCampaign campaign)
        {
            byte[] result = null;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var doc1 = new Document(PageSize.A4.Rotate()); // landscape mode
                    PdfWriter writer = PdfWriter.GetInstance(doc1, ms);
                    writer.PageEvent = new PageEventHelper();
                    ExportPDF(doc1, taskId, campaign);
                    result = ms.GetBuffer();
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the export pdf activity.";
                DILog.AttestationLogger.WriteLog(campaign == null ? 0 : campaign.Id, LogLevelType.Exception, message, ex);
            }
            return result;
        }

    }
    public class PageEventHelper : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;
        protected BaseFont RunDateFont;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(100, 100);
            template.BoundingBox = new Rectangle(-20, -20, 100, 100);
            try
            {
                RunDateFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            }
            catch (Exception ex)
            {
            }
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            try
            {
                base.OnEndPage(writer, document);

                int pageN = writer.PageNumber;
                String text = "Page " + pageN.ToString() + " of ";
                float len = this.RunDateFont.GetWidthPoint(text, 10);

                iTextSharp.text.Rectangle pageSize = document.PageSize;

                cb.SetRGBColorFill(102, 102, 102);

                cb.BeginText();
                cb.SetFontAndSize(this.RunDateFont, 10);
                cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetTop(30));
                //cb.SetTextMatrix(document.RightMargin, pageSize.GetTop(document.TopMargin));  
                cb.ShowText(text);

                cb.EndText();

                cb.AddTemplate(template, pageSize.GetRight(100) + len, pageSize.GetTop(30));
            }
            catch (Exception ex)
            {
            }
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            try
            {
                base.OnCloseDocument(writer, document);

                template.BeginText();
                template.SetFontAndSize(this.RunDateFont, 10);
                template.SetTextMatrix(0, 0);
                template.ShowText(" " + (writer.PageNumber - 1));
                template.EndText();
            }
            catch (Exception ex)
            {

            }
        }


    }
}
