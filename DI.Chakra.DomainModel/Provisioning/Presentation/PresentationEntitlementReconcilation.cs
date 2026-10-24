using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationEntitlementReconcilation
    {
        public string EndpointSystemType { get; private set; }
        public string EndpointSystemName { get; private set; }
        public string AccountName { get; private set; }
        public string Entitlement { get; private set; }
        public string EntitlementDescription { get; set; }
        public EntitlementType EntitlementType { get; private set; }
        public DateTime EnteredDate { get; private set; }
        public bool IsManuallyAdded { get; private set; }
        public string ResolvedBy { get; private set; }
        public string Comments { get; private set; }
        public ExceptionTypes ExceptionType { get; private set; }



        public static List<PresentationEntitlementReconcilation> GetAllByEndpointSystemType()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationEntitlementReconcilation ORMap(EntityModel.Presentation.PresentationEntitlementReconcilation presentationEntitleReconEntityModel)
        {
            if (presentationEntitleReconEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationEntitlementReconcilation, PresentationEntitlementReconcilation>(presentationEntitleReconEntityModel);
        }

        public static List<PresentationEntitlementReconcilation> ORMapList(IList<EntityModel.Presentation.PresentationEntitlementReconcilation> presentationEntitleReconEntityModels)
        {
            return presentationEntitleReconEntityModels.Select(ORMap).ToList();
        }
    }
}
