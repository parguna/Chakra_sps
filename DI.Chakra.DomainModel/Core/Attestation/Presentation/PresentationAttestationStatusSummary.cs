using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestationStatusSummary:Attestation
    {
        public int StatusCount { get; set; }

        public string Status { get; set; }

        public bool IsExploded { get; set; }

        public string BGColor { get; set; }



        public static PresentationAttestationStatusSummary ORMap(EntityModel.Presentation.PresentationAttestationStatusSummary presentationAttestationTaskSummaryEntityModel)
        {
            if (presentationAttestationTaskSummaryEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttestationStatusSummary, PresentationAttestationStatusSummary>(presentationAttestationTaskSummaryEntityModel);
        }

        public static List<PresentationAttestationStatusSummary> ORMapList(IList<EntityModel.Presentation.PresentationAttestationStatusSummary> presentationAttestationTaskSummaryEntityModels)
        {
            return presentationAttestationTaskSummaryEntityModels.Select(ORMap).ToList();
        }


        public static IList<PresentationAttestationStatusSummary> GetRevokeStatus(string endPointTypeName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<PresentationAttestationStatusSummary> attestationTaskSummaryList = ORMapList(new AttestationRepository(dataAccessTransaction)
                    .GetByEndPointTypeName(endPointTypeName));

                return attestationTaskSummaryList;
            }
                
        }

        public static IList<PresentationAttestationStatusSummary> GetSummaryByEndPointTypeName(string endPointTypeName)
        {
            List<PresentationAttestationStatusSummary> lstRevokeSummary = new List<PresentationAttestationStatusSummary>();
            var lqRevokeSummary = GetRevokeStatus(endPointTypeName);
            foreach (var grpRevokeSummary in lqRevokeSummary.GroupBy(x=>x.Status))
            {
                int statusCount = grpRevokeSummary.Count();
                string status = grpRevokeSummary.FirstOrDefault().Status;
                bool isExploded = grpRevokeSummary.FirstOrDefault().IsExploded;
                string bgColor = grpRevokeSummary.FirstOrDefault().BGColor;
                lstRevokeSummary.Add(new PresentationAttestationStatusSummary {StatusCount=statusCount,Status=status,IsExploded=isExploded,BGColor=bgColor });
            }
            return lstRevokeSummary;
            
        }

        public static IList<PresentationAttestationStatusSummary> GetTaskSummary(string endPointTypeName)
        {
            List<PresentationAttestationStatusSummary> lstTaskSummary = new List<PresentationAttestationStatusSummary>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                List<PresentationAttestationStatusSummary> lqTaskSummary = ORMapList(new AttestationRepository(dataAccessTransaction)
                    .GetTaskSummaryByEndPointTypeName(endPointTypeName));

                foreach (var grpTaskSummary in lqTaskSummary.GroupBy(x => x.Status))
                {
                    int statusCount = grpTaskSummary.Count();
                    string status = grpTaskSummary.FirstOrDefault().Status;
                    bool isExploded = grpTaskSummary.FirstOrDefault().IsExploded;
                    string bgColor = grpTaskSummary.FirstOrDefault().BGColor;

                    lstTaskSummary.Add(new PresentationAttestationStatusSummary { StatusCount = statusCount, Status = status, IsExploded = isExploded, BGColor = bgColor });
                }

                return lstTaskSummary;
            }
        }
    }
}
