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
    public class PresentationSODEntitlementConflict
    {
        public string Role1 { get; set; }

        public string Role2 { get; set; }

        public string UserName { get; set; }

        public static List<PresentationSODEntitlementConflict> GetSODEntitlementConflicts(string Username, long ConflictId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationSODEntitlementConflictRepository(dataAccessTransaction).GetSODEntitlementConflicts(Username, ConflictId));
            }
        }

        public static PresentationSODEntitlementConflict ORMap(EntityModel.Presentation.PresentationSODEntitlementConflict presentationSODEntitlementConflict)
        {
            if (presentationSODEntitlementConflict == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSODEntitlementConflict, PresentationSODEntitlementConflict>(presentationSODEntitlementConflict);
        }

        public static List<PresentationSODEntitlementConflict> ORMapList(IList<EntityModel.Presentation.PresentationSODEntitlementConflict> presentationSODEntitlementConflict)
        {
            return presentationSODEntitlementConflict.Select(ORMap).ToList();
        }
    }
}
