using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
   public class PresentationAttetstationCampaignRepository : RepositoryBase<AttestationCampaign>
    {

        public PresentationAttetstationCampaignRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public IList<PresentationAttetstationCampaign> GetAttestationOverallStatus()
        {
            try
            {
                var rawOutput = (from attcmp in CurrentContext.AttestationCampaigns
                                 where attcmp.ParentCampaign == null && (attcmp.Status != AttestationCampaignStatus.Completed  && attcmp.Status != AttestationCampaignStatus.Initiated)                            
                                 select new PresentationAttetstationCampaign
                                 {
                                     AttestationName = attcmp.Attestation.AttestationName,
                                     //AttestationType = attcmp.Attestation.AttestationType.ToString(),
                                     CreatedBy = attcmp.CreatedBy,
                                     AttestationCampaignId = attcmp.Id,
                                     CampaignName = attcmp.CampaignName,
                                     Description = attcmp.Description,
                                     Comments = attcmp.Comments,
                                     CreatedDate = attcmp.CreatedDate,
                                     CloseDate = attcmp.CloseDate,
                                     OverallStatus = attcmp.Status.ToString(),
                                     CampaignSplit = attcmp.ChildSplitCount,
                                     RequiresAttestationRemediation = attcmp.RequiresAttestationRemediation,
                                     DefaultReviewStatus = attcmp.DefaultReviewStatus,
                                 }).Distinct().ToList();

                rawOutput = rawOutput.OrderBy(i => i.CreatedDate).ToList();
                return rawOutput;
            }
            catch (Exception ex)
            {

            }

            return new List<PresentationAttetstationCampaign>();
        }

        public IList<PresentationAttetstationCampaign> GetCampaignStatus(long AttestationCampaignId)
        {
            try
            {
                var parentCampaign = CurrentContext.AttestationCampaigns.Where(a => a.Id == AttestationCampaignId).FirstOrDefault();
                List<PresentationAttetstationCampaign> rawOutput = new List<PresentationAttetstationCampaign>();

                if (parentCampaign != null && parentCampaign.ChildSplitCount > 0)
                {
                    var campaigns = CurrentContext.AttestationCampaigns.Where(i => i.ParentCampaign != null && i.ParentCampaign == AttestationCampaignId);
                    var task = (from tsk in CurrentContext.Tasks
                                join cmp in campaigns on tsk.AttestationCampaignId equals cmp.Id
                                where tsk.AttestationCampaignId != null && tsk.AccessRequestId == null 
                                select new { tsk.Id, tsk.WorkflowStateId, tsk.AttestationCampaignId, LastModifiedDate = tsk.IsCompleted ? tsk.LastModifiedDate :null})
                                .GroupBy(i => i.AttestationCampaignId)
                                .Select(i => new { Id = i.Max(j => j.Id), WorkflowStateId = i.Max(j => j.WorkflowStateId), LastModifiedDate = i.Max(j=> j.LastModifiedDate) });

                    rawOutput = (from attcmp in campaigns
                                 join att in CurrentContext.Attestations on attcmp.AttestationID equals att.Id
                                 join snapShot in CurrentContext.AttestationSnapshots on attcmp.Id equals snapShot.AttestationCampaignId
                                 join CampaignTask in CurrentContext.AttestationSnapshotActions on snapShot.Id equals CampaignTask.AttestationSnapshotId
                                 join tasks in task on CampaignTask.TaskId equals tasks.Id
                                 where (attcmp.Status == AttestationCampaignStatus.WorkInProgress || attcmp.Status == AttestationCampaignStatus.Remediation || attcmp.Status == AttestationCampaignStatus.Completed)
                                 select new PresentationAttetstationCampaign
                                 {
                                     AttestationName = attcmp.Attestation.AttestationName,
                                     AttestationType = attcmp.Attestation.AttestationType.ToString(),
                                     CreatedBy = attcmp.CreatedBy,
                                     AttestationCampaignId = attcmp.Id,
                                     CampaignName = attcmp.CampaignName,
                                     Description = attcmp.Description,
                                     Comments = attcmp.Comments,
                                     CreatedDate = attcmp.CreatedDate,
                                     CloseDate = attcmp.CloseDate,
                                     Level = tasks.WorkflowStateId,
                                     OverallStatus = attcmp.Status.ToString(),
                                     TotalItem = tasks.Id,
                                     RequiresAttestationRemediation = attcmp.RequiresAttestationRemediation,
                                     DefaultReviewStatus = attcmp.DefaultReviewStatus,
                                     SubmittedDate = tasks.LastModifiedDate,
                                 }).ToList();
                }
                else if(parentCampaign != null && parentCampaign.ChildSplitCount == 0 && parentCampaign.ParentCampaign.HasValue)
                {
                    rawOutput = (from attcmp in CurrentContext.AttestationCampaigns
                                 where attcmp.ParentCampaign == parentCampaign.ParentCampaign.Value
                                 join CampaignTask in CurrentContext.AttestationSnapshotActions.Include("AttestationSnapshot")
                                 on attcmp.Id equals CampaignTask.AttestationSnapshot.AttestationCampaignId
                                 join att in CurrentContext.Attestations
                                 on attcmp.AttestationID equals att.Id
                                 join task in CurrentContext.Tasks.Where(i=> i.AttestationCampaignId != null)
                                 on CampaignTask.TaskId equals task.Id
                                 where (attcmp.Status == AttestationCampaignStatus.WorkInProgress || attcmp.Status == AttestationCampaignStatus.Remediation)
                                 select new PresentationAttetstationCampaign
                                 {
                                     AttestationName = attcmp.Attestation.AttestationName,
                                     AttestationType = attcmp.Attestation.AttestationType.ToString(),
                                     CreatedBy = attcmp.CreatedBy,
                                     AttestationCampaignId = attcmp.Id,
                                     CampaignName = attcmp.CampaignName,
                                     Description = attcmp.Description,
                                     Comments = attcmp.Comments,
                                     CreatedDate = attcmp.CreatedDate,
                                     CloseDate = attcmp.CloseDate,
                                     Level = task.WorkflowStateId,
                                     OverallStatus = attcmp.Status.ToString(),
                                     TotalItem = task.Id,
                                     RequiresAttestationRemediation = attcmp.RequiresAttestationRemediation,
                                     DefaultReviewStatus = attcmp.DefaultReviewStatus,
                                     SubmittedDate = task.IsCompleted ? task.LastModifiedDate : null,
                                 }).ToList();
                }
                else if (parentCampaign != null && parentCampaign.ChildSplitCount == 0)
                {
                    rawOutput = (from attcmp in CurrentContext.AttestationCampaigns
                                 where attcmp.ParentCampaign == null && attcmp.Id == AttestationCampaignId
                                 join CampaignTask in CurrentContext.AttestationSnapshotActions.Include("AttestationSnapshot")
                                 on attcmp.Id equals CampaignTask.AttestationSnapshot.AttestationCampaignId
                                 join att in CurrentContext.Attestations
                                 on attcmp.AttestationID equals att.Id
                                 join task in CurrentContext.Tasks
                                 on CampaignTask.TaskId equals task.Id
                                 where task.Action == StatusType.Pending || attcmp.Status == AttestationCampaignStatus.Remediation
                                 select new PresentationAttetstationCampaign
                                 {
                                     AttestationName = attcmp.Attestation.AttestationName,
                                     AttestationType = attcmp.Attestation.AttestationType.ToString(),
                                     CreatedBy = attcmp.CreatedBy,
                                     AttestationCampaignId = attcmp.Id,
                                     CampaignName = attcmp.CampaignName,
                                     Description = attcmp.Description,
                                     Comments = attcmp.Comments,
                                     CreatedDate = attcmp.CreatedDate,
                                     CloseDate = attcmp.CloseDate,
                                     Level = task.WorkflowStateId,
                                     OverallStatus = attcmp.Status.ToString(),
                                     TotalItem = task.Id,
                                     RequiresAttestationRemediation = attcmp.RequiresAttestationRemediation,
                                     DefaultReviewStatus = attcmp.DefaultReviewStatus,
                                     SubmittedDate = task.IsCompleted ? task.LastModifiedDate : null,
                                 }).ToList();
                }

                var output = (from presentation in rawOutput
                              group presentation by new { presentation.AttestationCampaignId, presentation.Level } into grpPresentation
                              select new PresentationAttetstationCampaign
                              {
                                  AttestationName = grpPresentation.Select(a => a.AttestationName).FirstOrDefault(),
                                  AttestationType = grpPresentation.Select(a => a.AttestationType).FirstOrDefault(),
                                  CreatedBy = grpPresentation.Select(a => a.CreatedBy).FirstOrDefault(),
                                  AttestationCampaignId = grpPresentation.Key.AttestationCampaignId,
                                  CampaignName = grpPresentation.Select(a => a.CampaignName).FirstOrDefault(),
                                  Description = grpPresentation.Select(a => a.Description).FirstOrDefault(),
                                  Comments = grpPresentation.Select(a => a.Comments).FirstOrDefault(),
                                  CreatedDate = grpPresentation.Select(a => a.CreatedDate).FirstOrDefault(),
                                  CloseDate = grpPresentation.Select(a => a.CloseDate).FirstOrDefault(),
                                  Level = grpPresentation.Key.Level,
                                  OverallStatus = grpPresentation.Select(a => a.OverallStatus).FirstOrDefault(),
                                  TotalItem = grpPresentation.Select(a => a.TotalItem).Distinct().Count(),
                                  RequiresAttestationRemediation = grpPresentation.Select(a => a.RequiresAttestationRemediation).FirstOrDefault(),
                                  DefaultReviewStatus = grpPresentation.Select(a => a.DefaultReviewStatus).FirstOrDefault(),
                                  SubmittedDate = grpPresentation.Select(i=> i.SubmittedDate).FirstOrDefault()
                              }).ToList();

                output = output.OrderBy(i => i.CreatedDate).ToList();
                return output;
            }
            catch (Exception ex)
            {

            }

            return new List<PresentationAttetstationCampaign>();
        }

        public IList<PresentationAttetstationCampaign> GetCampaignReport()
        {
            return (from attcmp in CurrentContext.AttestationCampaigns.Include("Attestation")
                    where attcmp.EndDate != null
                    orderby attcmp.EndDate descending
                    select new PresentationAttetstationCampaign
                    {
                        AttestationName = attcmp.Attestation.AttestationName,
                        CreatedBy = attcmp.CreatedBy,
                        AttestationCampaignId = attcmp.Id,
                        CampaignName = attcmp.CampaignName,
                        Description = attcmp.Description,
                        Comments = attcmp.Comments,
                        CreatedDate = attcmp.CreatedDate,
                        EndDate = attcmp.EndDate,
                        SnapshotFileName = attcmp.SnapshotFileName,
                        SnapshotFilePath = attcmp.SnapshotFileName,
                    }).ToList();
        }

        public PresentationGridPagedData<PresentationAttetstationCampaign> GetPresentationGridPagedData(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
            var query = (from attcmp in CurrentContext.AttestationCampaigns.Include("Attestation")
                         join attReport in CurrentContext.AttestationCampaignReports on attcmp.Id equals attReport.AttestationCampaignId
                         where attReport.ReportType == AttestationReportType.ExcelFlat && attcmp.EndDate != null && attReport.AttestationCampaignId != null
                         orderby attcmp.EndDate descending
                        select new PresentationAttetstationCampaign
                        {
                            AttestationName = attcmp.Attestation.AttestationName,
                            CreatedBy = attcmp.CreatedBy,
                            AttestationCampaignId = attcmp.Id,
                            CampaignName = attcmp.CampaignName,
                            Description = attcmp.Description,
                            Comments = attcmp.Comments,
                            CreatedDate = attcmp.CreatedDate,
                            EndDate = attcmp.EndDate,
                            SnapshotFileName = attcmp.SnapshotFileName,
                            SnapshotFilePath = attcmp.SnapshotFileName,
                        });
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }
            dataResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.CreatedDate);
            }

            if (maximumRows == -1)
                dataResult.Data = query.ToList();
            else
                dataResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();
            return dataResult;
        }
        public PresentationGridPagedData<PresentationAttetstationCampaign> GetUserProfileComparisionCampaignReport(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
            var query = (from attcmp in CurrentContext.AttestationCampaigns.Include("Attestation")
                         join attReport in CurrentContext.AttestationCampaignReports on attcmp.Id equals attReport.AttestationCampaignId
                         where attReport.ReportType == AttestationReportType.ExcelHierarchical && attcmp.EndDate != null && attReport.AttestationCampaignId != null
                         orderby attcmp.EndDate descending
                         select new PresentationAttetstationCampaign
                         {
                             AttestationName = attcmp.Attestation.AttestationName,
                             CreatedBy = attcmp.CreatedBy,
                             AttestationCampaignId = attcmp.Id,
                             CampaignName = attcmp.CampaignName,
                             Description = attcmp.Description,
                             Comments = attcmp.Comments,
                             CreatedDate = attcmp.CreatedDate,
                             EndDate = attcmp.EndDate,
                             SnapshotFileName = attcmp.SnapshotFileName,
                             SnapshotFilePath = attcmp.SnapshotFileName,
                         });
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }
            dataResult.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.CreatedDate);
            }

            if (maximumRows == -1)
                dataResult.Data = query.ToList();
            else
                dataResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();
            return dataResult;
        }
    }
}
