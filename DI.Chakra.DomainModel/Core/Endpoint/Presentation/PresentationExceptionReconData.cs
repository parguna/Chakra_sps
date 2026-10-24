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
    public class PresentationExceptionReconData
    {
        public string EndpointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string ExceptionType { get; set; }

        public long? UserId { get; set; }

        public int? Version { get; set; }

        public long EndpointSystemId { get; set; }

        public long EndpointUserId { get; set; }


        public static PresentationExceptionReconData ORMap(EntityModel.Presentation.PresentationExceptionReconData presentationExceptionReconData)
        {
            if (presentationExceptionReconData == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationExceptionReconData, PresentationExceptionReconData>(presentationExceptionReconData);
        }

        public static List<PresentationExceptionReconData> ORMapList(IList<EntityModel.Presentation.PresentationExceptionReconData> presentationExceptionReconDatum)
        {
            return presentationExceptionReconDatum.Select(ORMap).ToList();
        }


        //public List<PresentationExceptionReconData> GetExceptionReconData(long endpointSystemId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        EndPoint endpoint = EndPoint.GetById(endpointSystemId);

        //        if (endpoint.EndpointSystemTypeId == (long)EndpointType.ActiveDirectory)
        //            return ORMapList(new EndPointRepository(dataAccessTransaction).GetADSExceptionReconByendPointSystemId(endpointSystemId));
        //        else if (endpoint.EndpointSystemTypeId == (long)EndpointType.LotusNotes)
        //            return ORMapList(new EndPointRepository(dataAccessTransaction).GetLotusNotesExceptionReconByendPointSystemId(endpointSystemId));
        //        else if (endpoint.EndpointSystemTypeId == (long)EndpointType.SAP)
        //            return ORMapList(new EndPointRepository(dataAccessTransaction).GetSAPExceptionReconByendPointSystemId(endpointSystemId));
        //        else
        //            return null;
        //    }
        //}
    }
}
