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
    public class PresentationAttestationTaskSummary : Task
    {
        public int StatusCount { get; set; }

        public string Status { get; set; }

        public bool IsExploded { get; set; }

        public string BGColor { get; set; }

        public static PresentationAttestationTaskSummary ORMap(EntityModel.Presentation.PresentationAttestationTaskSummary presentationAttestationTaskSummaryEntityModel)
        {
            if (presentationAttestationTaskSummaryEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttestationTaskSummary, PresentationAttestationTaskSummary>(presentationAttestationTaskSummaryEntityModel);
        }

        public static List<PresentationAttestationTaskSummary> ORMapList(IList<EntityModel.Presentation.PresentationAttestationTaskSummary> presentationAttestationTaskSummaryEntityModels)
        {
            return presentationAttestationTaskSummaryEntityModels.Select(ORMap).ToList();
        }

        public static List<PresentationAttestationTaskSummary> GetByEndPointTypeName(string endPointTypeName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PresentationAttestationTaskSummary> attestationTaskSummaryList = ORMapList(new AttestationTaskSummaryRepository(dataAccessTransaction)
                    .GetByEndPointTypeName(endPointTypeName));
                return attestationTaskSummaryList;
            }
        }
    }
}
