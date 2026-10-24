using DI.Chakra.DomainModel;
using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Workflow.Processor;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor
{
    public class AttestationReportProcess
    {
        readonly string _masterConfigHelperServiceUser;
        readonly SchedulerLogger _logger = null;
        bool _disposed = false;
        readonly ISchedulerFactory _schedulerFactory;
        readonly string _CronScheduleString;
        readonly string attestationReportIdentity = "AttestationReport";

        public AttestationReportProcess()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
            _logger = new SchedulerLogger();
            _schedulerFactory = new StdSchedulerFactory();
            _CronScheduleString = MasterConfigHelper.CRON_Expression_AttestationReportTimer;
        }

        public async void Start()
        {
            try
            {
                IScheduler scheduler = await _schedulerFactory.GetScheduler();
                _logger.WriteLog(LogLevelType.Info, $"AttestationReportProcess started");
                await scheduler.Start();

                ITrigger trigger = TriggerBuilder.Create()
                     .WithIdentity(attestationReportIdentity)
                     .WithCronSchedule(_CronScheduleString)   // "0 59 23 L * ?")
                     .StartAt(DateTime.Now)
                     .WithPriority(1)
                     .Build();

                IJobDetail job = JobBuilder.Create<AttestationReportJob>().WithIdentity(attestationReportIdentity).Build();
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"AttestationReportProcess start failed.{ex.Message}", ex);
            }
        }
        public async void Stop()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                // get a scheduler
                IScheduler sched = await schedFact.GetScheduler();
                //sched.Standby();
                await sched.UnscheduleJob(new TriggerKey(attestationReportIdentity));
                _logger.WriteLog(LogLevelType.Info, $"AttestationReportProcess stopped");
            }
            catch (Exception ex)
            {
                _logger.WriteLog(null, LogLevelType.Error, $"AttestationReportProcess Stop failed.{ex.Message}", ex);
            }
        }
        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }
            _disposed = true;
        }
    }
    public class AttestationReportJob : IJob
    {
        Dictionary<long, string> TaskEndorsedBy = new Dictionary<long, string>();
        Dictionary<long, int> TaskState = new Dictionary<long, int>();
        List<PresentationSnapshotActionHistory> history = new List<PresentationSnapshotActionHistory>();

        readonly string _masterConfigHelperServiceUser;
        public AttestationReportJob()
        {
            _masterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            Action action = () =>
            {
                DI.Chakra.Logger.SchedulerLogger _logger = new SchedulerLogger();
                try
                {
                    _logger.WriteLog(LogLevelType.Info, $"AttestationReportJob executing");
                    GenerateAttestationReport();
                    GenerateAttestationParentCampaignReport();
                    _logger.WriteLog(LogLevelType.Info, $"AttestationReportJob completed");
                }
                catch (Exception ex)
                {
                    _logger.WriteLog(null, LogLevelType.Error, $"AttestationReportJob Execute failed.{ex.Message}", ex);
                }
            };
            return System.Threading.Tasks.Task.Run(action);
        }

        public void GenerateAttestationParentCampaignReport()
        {
            List<AttestationCampaign> attestationCampaigns = null;
            List<AttestationCampaign> pendingAttestationCampaign = null;
            try
            {
                if (MasterConfigHelper.CanGenerateAttestationReport)
                {
                    bool readyForGenerate = false;
                    attestationCampaigns = AttestationCampaign.GetParentCampaignForReport();
                    byte[] report = null;
                    foreach (var attestationCampaign in attestationCampaigns)
                    {
                        report = null;
                        readyForGenerate = false;
                        try
                        {

                            var cmgs = AttestationCampaign.GetChildCampaignsIncludingStatus(attestationCampaign.Id);
                            if (cmgs.Count > 0 && cmgs.All(i => i.Status == AttestationCampaignStatus.Completed && i.Comments.Contains("Attestation Campaign has been cancelled")))
                            {
                                continue;
                            }
                            pendingAttestationCampaign = AttestationCampaign.GetChildCampaigns(attestationCampaign.Id);
                            pendingAttestationCampaign = pendingAttestationCampaign.Where(a => a.Status != AttestationCampaignStatus.IgnoreNoUserFound).ToList();
                            pendingAttestationCampaign = pendingAttestationCampaign.Where(a => a.Status != AttestationCampaignStatus.Completed).ToList();
                            pendingAttestationCampaign = pendingAttestationCampaign.Where(a => a.Status != AttestationCampaignStatus.Remediation).ToList();
                            if (pendingAttestationCampaign == null || pendingAttestationCampaign.Count() == 0)
                            {
                                readyForGenerate = true;
                                report = null;
                                string filename = attestationCampaign.CampaignName;
                                #region Compariosn
                                //UserComparision Excel
                                try
                                {
                                    report = null; filename = "";
                                    using (OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage())
                                    {
                                        ExportFlatExcelUserProfileComparision(excel, attestationCampaign);
                                        report = excel.GetAsByteArray();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = "Error while executing the Export FlatExcel UserProfileComparision excel activity.";
                                    DILog.AttestationLogger.WriteLog(attestationCampaign == null ? 0 : attestationCampaign.Id, LogLevelType.Exception, message, ex);
                                }
                                filename = String.Format("{0}_UserComparision.{1}", attestationCampaign.CampaignName, "xlsx");
                                if (report != null)
                                {

                                    try
                                    {
                                        var reportType = AttestationReportType.ExcelHierarchical;
                                        var isReportThere = AttestationCampaignReport.GetByAttestationCampaignId(attestationCampaign.Id);

                                        if (isReportThere == null || !isReportThere.Any(a => a.ReportType == reportType))
                                        {
                                            AttestationCampaignReport cReport = new AttestationCampaignReport()
                                            {
                                                AttachmentFile = report,
                                                AttestationCampaignId = attestationCampaign.Id,
                                                AttestationCampaignName = attestationCampaign.CampaignName,
                                                ContentType = reportType.ToString(),
                                                FileName = filename,
                                                ReportType = reportType,
                                            };
                                            cReport.Save(MasterConfigHelper.ServiceUser);
                                        }
                                        else
                                        {
                                            var cReport = isReportThere.FirstOrDefault(a => a.ReportType == reportType);
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
                                        DILog.AttestationLogger.WriteLog(attestationCampaign.Id, LogLevelType.Exception, "Error while craeting Attestation report file.", ex);
                                    }
                                }
                                #endregion

                                #region Flat File Report
                                try
                                {
                                    using (OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage())
                                    {
                                        ExportFlatExcel(excel, attestationCampaign);
                                        report = excel.GetAsByteArray();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string message = "Error while executing the export flat excel activity.";
                                    DILog.AttestationLogger.WriteLog(attestationCampaign == null ? 0 : attestationCampaign.Id, LogLevelType.Exception, message, ex);
                                }
                                filename = String.Format("{0}_Flat.{1}", attestationCampaign.CampaignName, "xlsx");
                                if (report != null)
                                {
                                    try
                                    {
                                        var reportType = AttestationReportType.ExcelFlat;
                                        var isReportThere = AttestationCampaignReport.GetByAttestationCampaignId(attestationCampaign.Id);

                                        if (isReportThere == null || !isReportThere.Any(a => a.ReportType == reportType))
                                        {
                                            AttestationCampaignReport cReport = new AttestationCampaignReport()
                                            {
                                                AttachmentFile = report,
                                                AttestationCampaignId = attestationCampaign.Id,
                                                AttestationCampaignName = attestationCampaign.CampaignName,
                                                ContentType = reportType.ToString(),
                                                FileName = filename,
                                                ReportType = reportType,
                                            };
                                            cReport.Save(MasterConfigHelper.ServiceUser);
                                        }
                                        else
                                        {
                                            var cReport = isReportThere.FirstOrDefault(a => a.ReportType == reportType);
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
                                        DILog.AttestationLogger.WriteLog(attestationCampaign.Id, LogLevelType.Exception, "Error while craeting Attestation report file.", ex);
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"GenerateAttestationReport campaign:{attestationCampaign.CampaignName} report not ready for generation.");
                            }
                        }
                        catch (Exception ex)
                        {
                            DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"GenerateAttestationParentCampaignReport failed for campaign:{attestationCampaign.CampaignName}.{ex.Message}", ex);
                        }
                        finally
                        {
                            pendingAttestationCampaign?.Clear();
                            pendingAttestationCampaign = null;

                            report = null;
                            if (readyForGenerate)
                                DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"GenerateAttestationReport completed for campaign:{attestationCampaign.CampaignName}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"GenerateAttestationParentCampaignReport trigger failed.{ex.Message}", ex);
            }
            finally
            {
                attestationCampaigns?.Clear();
                attestationCampaigns = null;
            }
        }
        private void WriteAttestationLog(LogLevelType logLevelType, string message, Exception ex)
        {
            DILog.SchedulerLogger.WriteLog(logLevelType, message, ex);
        }
        private void ExportFlatExcel(OfficeOpenXml.ExcelPackage excel, AttestationCampaign attCampaign)
        {

            List<long> availableEndpoints = new List<long>();
            List<long> availableEndpointTypes = new List<long>();
            var campaignId = attCampaign.Id;
            var pCampaign = AttestationCampaign.GetByCampaignId(attCampaign.Id);
            List<AttestationCampaign> childCampaigns = new List<AttestationCampaign>();
            var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_ATTESTATION_REVIEW", pCampaign.Attestation.AttestationType.ToString().ToUpper()));
            var attestationEndpoint = AttestationEndpoint.GetByAttestationIdIncludeEndpoint(pCampaign.AttestationID).ToList();
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
            //workSheet.Cells[currentRow,1, currentRow, 30].Value = $"{attCampaign.CampaignName}";
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.Font.Size = 12;
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.Font.Bold = true;
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            //workSheet.Cells[currentRow, 1, currentRow, 30].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
            //workSheet.Cells[currentRow, 1, currentRow, 30].Merge = true;

            //currentRow = currentRow + 1;

            int colStartPosition = 1;
            int colNum = colStartPosition;
            #region Header Section
            bool showReserved1 = false; bool showReserved2 = false; bool showReserved3 = false; bool showReserved4 = false; bool showReserved5 = false;
            bool showReserved6 = false; bool showReserved7 = false; bool showReserved8 = false; bool showReserved9 = false; bool showReserved10 = false;
            bool showActionReserved1 = false; bool showActionReserved2 = false; bool showActionReserved3 = false;

            showReserved1 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved1");
            showReserved2 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved2");
            showReserved3 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved3");
            showReserved4 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved4");
            #region SRM
            //showReserved5 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved5));
            //showReserved6 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved6));
            #endregion
            showReserved7 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved7");
            //showReserved8 = attestationUserDetails.Any(a => !string.IsNullOrEmpty(a.Reserved8));
            showReserved9 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved9");
            showReserved10 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "Reserved10");

            //showActionReserved1 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "ActionReserved1");
            //showActionReserved2 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "ActionReserved2");
            //showActionReserved3 = AttestationSnapshot.IsResevedFiledHasValue(campaignId, "ActionReserved3");
            bool mulitpleLevel = false;

            //if (!mulitpleLevel && attestationUserTask.Except(allTasksForThisCampaign.Select(a => a.Id).Distinct()).Any())
            //    mulitpleLevel = true;

            workSheet.Cells[currentRow, colNum].Value = "Parent Campaign Name";
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

            workSheet.Cells[currentRow, ++colNum].Value = "Login Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Display Name";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;

            workSheet.Cells[currentRow, ++colNum].Value = "Account Type";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Type";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Entitlement";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
            workSheet.Cells[currentRow, ++colNum].Value = "Detail";
            workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
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
            #endregion
            childCampaigns = AttestationCampaign.GetCompletedChildCampaigns(campaignId).OrderBy(i => i.EndDate).ToList();
            string usrisplayName = string.Empty;
            string usrUserName = string.Empty;

            string level1Approver = string.Empty;
            string level2Approver = string.Empty;
            string level3Approver = string.Empty;

            string level1ReviewedDate = string.Empty;
            string level2ReviewedDate = string.Empty;
            string level3ReviewedDate = string.Empty;

            string level1DelegationComments = string.Empty;
            string level2DelegationComments = string.Empty;
            string level3DelegationComments = string.Empty;

            string accountType = string.Empty;
            PresentationSnapshotActionHistory level1 = null;
            PresentationSnapshotActionHistory level2 = null;
            PresentationSnapshotActionHistory level3 = null;
            foreach (var campaign in childCampaigns)
            {
                history = DI.Chakra.DomainModel.Presentation.PresentationSnapshotActionHistory.GetHistoryByCampaignId(campaign.Id);
                var taskDetail = DomainModel.Task.GetByCampaignIdAndMaxState(campaign.Id);
                if (taskDetail == null || campaign.Comments.Contains("Attestation Campaign has been cancelled.", StringComparison.InvariantCultureIgnoreCase)) continue;

                var allTasksForThisCampaign = DomainModel.Task.GetByCampaignIdAndWorkFlowState((long)taskDetail.AttestationCampaignId, taskDetail.WorkflowStateId).ToList();
                var orgTask = DomainModel.Task.GetById(taskDetail.Id);
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)orgTask.AttestationCampaignId);

                var attestationUserDetails = PresentationExportAttestationEntitlementReviewDetail.GetAttestationTaskReviewForExport(campaign.Id);
                var attestationUserTask = attestationUserDetails.Where(a => a.TaskId > 0 && a.EntitlementType == EntitlementType.Account.ToString()).Select(a => a.TaskId).Distinct();

                var previousParentEndsorer = "";
                string parentEntity = string.Empty;
                usrisplayName = string.Empty;
                usrUserName = string.Empty;


                level1Approver = string.Empty;
                level2Approver = string.Empty;
                level3Approver = string.Empty;

                level1ReviewedDate = string.Empty;
                level2ReviewedDate = string.Empty;
                level3ReviewedDate = string.Empty;

                level1DelegationComments = string.Empty;
                level2DelegationComments = string.Empty;
                level3DelegationComments = string.Empty;
                level1 = null;
                level2 = null;
                level3 = null;
                accountType = string.Empty;
                foreach (PresentationExportAttestationEntitlementReviewDetail action in attestationUserDetails)
                {
                    colStartPosition = 1;
                    colNum = colStartPosition;
                    if (action.IsVisible)
                    {
                        if (action.EntitlementType == EntitlementType.Account.ToString())
                        {
                            usrisplayName = action.Name;
                            usrUserName = action.UserName;
                            accountType = action.AccountType;
                        }
                        // add workcell entry for top level
                        workSheet.Cells[currentRow, colNum].Value = attCampaign.CampaignName;
                        workSheet.Cells[currentRow, ++colNum].Value = action.CampaignName;
                        workSheet.Cells[currentRow, ++colNum].Value = campaign.CreatedDate.ToString(Constants.DateFormatForSP);
                        //workSheet.Cells[currentRow, ++colNum].Value = campaign.EndDate.HasValue ? campaign.EndDate.Value.ToString(Constants.DateFormatForSP):"";
                        workSheet.Cells[currentRow, ++colNum].Value = campaign.Comments?.Trim(new Char[] { '\r', '\n' }).Replace("\r\n", "").Replace("\\r\\n", "").Replace("!~", Environment.NewLine);
                        workSheet.Cells[currentRow, ++colNum].Value = action.EndpointSystemName;
                        workSheet.Cells[currentRow, ++colNum].Value = usrUserName;
                        workSheet.Cells[currentRow, ++colNum].Value = usrisplayName;

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
                                var actionTask = DomainModel.Task.GetById(action.TaskId);
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
                        //workSheet.Cells[currentRow, ++colNum].Value = approver;
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
                                    var actionTask = DomainModel.Task.GetById(action.TaskId);
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
                for (int i = colStartPosition; i <= colNum; i++)
                {
                    workSheet.Column(i).AutoFit();
                }
            }


        }

        private void ExportFlatExcelUserProfileComparision(OfficeOpenXml.ExcelPackage excel, AttestationCampaign attCampaign)
        {
            try
            {
                var campaignDetails = AttestationCampaign.GetByCampaignIdIncludeAttestaion((long)attCampaign.Id);
                var workSheet = excel.Workbook.Worksheets.Add(string.Format("{0}_ATTESTATION_USER_COMAPARISION_REVIEW", campaignDetails.Attestation.AttestationType.ToString().ToUpper()));
                var currentRow = 1;
                var colNum = 1;
                workSheet.Cells[currentRow, colNum].Value = "Campaign Name";
                workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                workSheet.Cells[currentRow, ++colNum].Value = "Application Name";
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
                var childCampaigns = AttestationCampaign.GetCompletedChildCampaigns(attCampaign.Id).OrderBy(i => i.EndDate).ToList();
                const uint COWAIT_ALERTABLE = 0x00000004;
                const uint COWAIT_WAITALL = 0x00000001;
                foreach (var campaign in childCampaigns)
                {
                    history = DI.Chakra.DomainModel.Presentation.PresentationSnapshotActionHistory.GetHistoryByCampaignId(campaign.Id);
                    var taskDetail = DomainModel.Task.GetByCampaignIdAndMaxState(campaign.Id);
                    if (taskDetail == null || campaign.Comments.Contains("Attestation Campaign has been cancelled.", StringComparison.InvariantCultureIgnoreCase)) continue;
                    List<long> availableEndpoints = new List<long>();
                    List<long> availableEndpointTypes = new List<long>();
                    int pageSize = 100;
                    int pageNumber = 1;
                    bool isRecordExists = true;
                    while (isRecordExists)
                    {
                        var userDetails = PresentationUserEntitlementData.GetAttestationTaskEntitlementUserComparisionByCampaignId(pageNumber, pageSize, campaign.Id);
                        if (userDetails == null || userDetails.Count == 0)
                        {
                            isRecordExists = false;
                            break;
                        }
                        pageNumber = pageNumber + 1;
                        foreach (var item in userDetails)
                        {
                            colNum = 1;
                            workSheet.Cells[currentRow, colNum].Value = campaign.CampaignName;
                            workSheet.Cells[currentRow, ++colNum].Value = item.EndpointSystemName;
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
                }
                for (int i = 1; i <= colNum; i++)
                {
                    workSheet.Column(i).AutoFit();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
        public void GenerateAttestationReport()
        {
            DataTable dataTable = null;
            //FacadeHelper facadeHelper = null;
            try
            {
                var serviceUser = MasterConfigHelper.ServiceUser;
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, $"GenerateAttestationReport started", null);
                {
                    dataTable = AttestationCampaign.GetDataTableByQuery("Select id,TaskId,CampaignId,ReportType,Attestationtype,CreatedDate from DI_M_AttestationReportList WHERE ISACTIVE=1");
                    if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        WriteAttestationLog(LogLevelType.Info, $"GenerateAttestationReport count:{dataTable.Rows.Count}", null);
                        long id = 0;
                        long taskId = 0;
                        long campaignId = 0;
                        AttestationType attestationtype = AttestationType.User;
                        AttestationReportType attestationReportType = AttestationReportType.ExcelFlat;
                        AttestationCampaign campaign = null;
                        SaveAttestationSnapshot saveAttestationSnapshot = null;
                        foreach (System.Data.DataRow row in dataTable.Rows)
                        {
                            try
                            {
                                id = Convert.ToInt64(row["id"].ToString());
                                taskId = Convert.ToInt64(row["TaskId"].ToString());
                                campaignId = Convert.ToInt64(row["CampaignId"].ToString());
                                if (!Enum.TryParse<AttestationType>(row["Attestationtype"].ToString(), out attestationtype))
                                {
                                    continue;
                                }
                                if (!Enum.TryParse<AttestationReportType>(row["ReportType"].ToString(), out attestationReportType))
                                {
                                    continue;
                                }
                                campaign = AttestationCampaign.GetById(campaignId);
                                if (campaign == null) continue;

                                saveAttestationSnapshot = new SaveAttestationSnapshot();
                                byte[] report = null;
                                string filename = campaign.CampaignName;
                                saveAttestationSnapshot.GetHistoryRecordByCampaign(campaignId);
                                switch (attestationReportType)
                                {
                                    case AttestationReportType.ExcelFlat:
                                        WriteAttestationLog(LogLevelType.Info, $"Create Campain:{campaign.CampaignName} report for campaignid:{campaignId} taskId:{taskId}", null);
                                        report = saveAttestationSnapshot.ExportUserComparisionByReportType(attestationReportType, taskId, campaign);
                                        filename = String.Format("{0}.{1}", campaign.CampaignName, "xlsx");
                                        break;
                                    //User Comparision
                                    case AttestationReportType.ExcelHierarchical:
                                        report = saveAttestationSnapshot.ExportUserComparisionByReportType(attestationReportType, taskId, campaign);
                                        filename = String.Format("UserProfileComparision_{0}.{1}", campaign.CampaignName, "xlsx");
                                        break;
                                    default:
                                        report = null;
                                        break;
                                }
                                if (report != null)
                                {
                                    WriteAttestationLog(LogLevelType.Info, $"Create {attestationReportType.ToString()} report for Campain:{campaign.CampaignName} End at:{DateTime.Now.ToString()}", null);
                                    var attestationCampaignReports = AttestationCampaignReport.GetByAttestationCampaignId(campaignId);
                                    var exisitingReport = attestationCampaignReports.FirstOrDefault(i => i.ReportType == attestationReportType);
                                    if (exisitingReport != null)
                                    {
                                        exisitingReport.FileName = filename;
                                        exisitingReport.AttachmentFile = report;
                                    }
                                    else
                                    {
                                        exisitingReport = new AttestationCampaignReport()
                                        {
                                            AttachmentFile = report,
                                            AttestationCampaignId = campaign.Id,
                                            AttestationCampaignName = campaign.CampaignName,
                                            ContentType = attestationReportType.ToString(),
                                            FileName = filename.Replace(")", "").Replace("(", "").Replace(" / ", "").Replace(@"\", ""),
                                            ReportType = attestationReportType,
                                            CreatedBy = serviceUser,
                                            CreatedDate = DateTime.Now
                                        };
                                    }
                                    WriteAttestationLog(LogLevelType.Info, $"Create {attestationReportType.ToString()} report for Campain:{campaign.CampaignName} started at:{DateTime.Now.ToString()}", null);
                                    exisitingReport.Save(serviceUser);
                                    AttestationCampaignReport.ExecuteQuery($"Update DI_M_AttestationReportList Set ISACTIVE=0 WHERE id = {id}");
                                    WriteAttestationLog(LogLevelType.Info, $"Create {attestationReportType.ToString()} report for Campain:{campaign.CampaignName} End at:{DateTime.Now.ToString()}", null);
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteAttestationLog(LogLevelType.Info, $"failed to generate report {attestationReportType.ToString()} {campaign?.CampaignName}", ex);
                            }
                            finally
                            {
                                saveAttestationSnapshot = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteAttestationLog(LogLevelType.Exception, "Error While GenerateAttestationReport.", ex);
            }
            finally
            {
                dataTable = null;
                WriteAttestationLog(LogLevelType.Info, "GenerateAttestationReport Completed", null);
            }
        }
    }
}
