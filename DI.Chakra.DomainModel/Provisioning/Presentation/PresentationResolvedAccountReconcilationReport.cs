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
    public class PresentationResolvedAccountReconcilationReport
    {
        public string EndpointSystemType { get; set; }
        public string EndpointSystemName { get; set; }
        public string AccountName { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool IsManuallyAdded { get; set; }
        public string ResolvedBy { get; set; }
        public string Comments { get; set; }
        public ExceptionTypes ExceptionType { get; set; }


        public List<PresentationResolvedAccountReconcilationReport> GetResolvedExceptions()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetResolvedExceptions());
            }
        }

        public static PresentationResolvedAccountReconcilationReport ORMap(EntityModel.Presentation.PresentationResolvedAccountReconcilationReport presentationResAccReconEntityModel)
        {
            if (presentationResAccReconEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationResolvedAccountReconcilationReport, PresentationResolvedAccountReconcilationReport>(presentationResAccReconEntityModel);
        }

        public static List<PresentationResolvedAccountReconcilationReport> ORMapList(IList<EntityModel.Presentation.PresentationResolvedAccountReconcilationReport> presentationResAccReconEntityModels)
        {
            return presentationResAccReconEntityModels.Select(ORMap).ToList();
        }

    }
}
