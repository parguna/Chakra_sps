using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationJobListAttestation
    {
        public long AttestationProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public string occurence { get; set; }
        public string AttestationName { get; set; }
        public string Status { get; set; }
        public DateTime? StartDateTime { get; set; }
        public string ErrorMessage { get; set; }

        public string SchedulerDateTime { get; set; }

        public DateTime? LastRunDate { get; set; }

        public static List<PresentationJobListAttestation> GetAttestationProcessJobs(int pageSize, int pageNumber, string filter)  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetAttestationProcessJobs(pageSize, pageNumber, filter));
            }
        }
        public static List<PresentationJobListAttestation> GetAttestationJobs(string filter)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetAttestationJobs(filter));
            }
        }

        public static PresentationJobListAttestation ORMap(EntityModel.Presentation.PresentationJobListAttestation presentationJobListAttestationEntityModel)
        {
            if (presentationJobListAttestationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationJobListAttestation, PresentationJobListAttestation>(presentationJobListAttestationEntityModel);
        }

        public static List<PresentationJobListAttestation> ORMapList(IList<EntityModel.Presentation.PresentationJobListAttestation> presentationJobListAttestation)
        {
            return presentationJobListAttestation.Select(ORMap).ToList();
        }
    }
}
