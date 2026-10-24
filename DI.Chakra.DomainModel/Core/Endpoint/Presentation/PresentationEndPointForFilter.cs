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
    public class PresentationEndPointForFilter : EndPoint
    {
        public long Endpointsystemid { get; set; }
        public List<EntitlementType> EntitlementTypes { get; set; }
        public bool HasEntitlementTypes { get; set; }
    }
}
