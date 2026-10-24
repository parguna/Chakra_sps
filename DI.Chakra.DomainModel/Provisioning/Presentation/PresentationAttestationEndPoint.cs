using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestationEndPoint
    {
        public long? Id { get; set; }
        public string TargetSystem { get; set; }

        public string ApplicationName { get; set; }

        public static IList<PresentationAttestationEndPoint> GetAttestationEndPoints(long AttestationId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationAttestationEndPointRepository(dataAccessTransaction).GetAttestationEndPoints(AttestationId));
            }
        }
        public static PresentationAttestationEndPoint ORMap(EntityModel.Presentation.PresentationAttestationEndPoint presentationAttestationEndPoint)
        {
            if (presentationAttestationEndPoint == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAttestationEndPoint, PresentationAttestationEndPoint>(presentationAttestationEndPoint);
        }

        public static List<PresentationAttestationEndPoint> ORMapList(IList<EntityModel.Presentation.PresentationAttestationEndPoint> presentationAttestationEndPoint)
        {
            return presentationAttestationEndPoint.Select(ORMap).ToList();
        }
    }
}
