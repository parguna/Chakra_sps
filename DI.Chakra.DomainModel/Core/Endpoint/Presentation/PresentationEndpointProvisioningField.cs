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
    public class PresentationEndpointProvisioningField
    {
        public long Id { get; set; }
        public long RowID { get; set; }
        public Nullable<long> ParentRowID { get; set; }
        public long EndPointSystemId { get; set; }
        public long EndPointSystemTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool EndPointHasValues { get; set; }

        public static IList<PresentationEndpointProvisioningField> GetEndpointProvisiongField() 
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationEndpointProvisioningFieldRepository(dataAccessTransaction).GetEndpointProvisiongField());
            }
        }

        public static PresentationEndpointProvisioningField ORMap(EntityModel.Presentation.PresentationEndpointProvisioningField presentationAttestation)
        {
            if (presentationAttestation == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationEndpointProvisioningField, PresentationEndpointProvisioningField>(presentationAttestation);
        }

        public static List<PresentationEndpointProvisioningField> ORMapList(IList<EntityModel.Presentation.PresentationEndpointProvisioningField> presentationAttestation)
        {
            return presentationAttestation.Select(ORMap).ToList();
        }
    }
}
