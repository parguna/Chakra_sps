using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using DI.Chakra.DomainModel.Core;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttetstationCampaign
    {
        public long AttestationCampaignId { get; set; }
        public long AttestationID { get; set; }
        public string AttestationName { get; set; }
        public string AttestationType { get; set; }
        public Guid WorkflowInstanceID { get; set; }

        public string CampaignName { get; set; }
        public string Name { get; set; }
        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }

        public int Status { get; set; }
        public int Level { get; set; }
        public long TotalItem { get; set; }
        public string OverallStatus { get; set; }
        public long CampaignSplit { get; set; }
        public string SnapshotFileName { get; set; }
        public string SnapshotFilePath { get; set; }
        public bool RequiresAttestationRemediation { get; set; }
        public Nullable<System.DateTime> SubmittedDate { get; set; }

        public static List<PresentationAttetstationCampaign> GetCampaignStatus(long AttestationCampaignId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new PresentationAttetstationCampaignRepository(dataAccessTransaction).GetCampaignStatus(AttestationCampaignId));
            }
        }

        public static List<PresentationAttetstationCampaign> GetAttestationOverallStatus()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new PresentationAttetstationCampaignRepository(dataAccessTransaction).GetAttestationOverallStatus());
            }
        }

        public static List<PresentationAttetstationCampaign> GetCampaignReport()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new PresentationAttetstationCampaignRepository(dataAccessTransaction).GetCampaignReport());
            }
        }



        public static List<PresentationAttetstationCampaign> ORMapList(IList<EntityModel.Presentation.PresentationAttetstationCampaign> PresentationAttetstationCampaign)
        {
            return PresentationAttetstationCampaign.Select(ORMap).ToList();
        }

        public static PresentationAttetstationCampaign ORMap(EntityModel.Presentation.PresentationAttetstationCampaign PresentationAttetstationCampaign)
        {
            if (PresentationAttetstationCampaign == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttetstationCampaign, PresentationAttetstationCampaign>(PresentationAttetstationCampaign);
        }
        public static PresentationGridPagedData<PresentationAttetstationCampaign> GetPresentationGridPagedData(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
                var result = new PresentationAttetstationCampaignRepository(dataAccessTransaction).GetPresentationGridPagedData(startRowIndex, maximumRows, filterExpression, sortExpression);
                dataResult.Data = ORMapList(result.Data);
                dataResult.TotalCount = result.TotalCount;
                return dataResult;
            }
            
        }
        public static PresentationGridPagedData<PresentationAttetstationCampaign> GetUserProfileComparisionCampaignReport(int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var dataResult = new PresentationGridPagedData<PresentationAttetstationCampaign>();
                var result = new PresentationAttetstationCampaignRepository(dataAccessTransaction).GetUserProfileComparisionCampaignReport(startRowIndex, maximumRows, filterExpression, sortExpression);
                dataResult.Data = ORMapList(result.Data);
                dataResult.TotalCount = result.TotalCount;
                return dataResult;
            }

        }
    }
}
