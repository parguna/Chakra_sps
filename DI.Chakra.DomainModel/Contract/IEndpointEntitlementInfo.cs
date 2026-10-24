using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IEndpointEntitlementInfo
    {
        long EndPointSystemId { get; set; }
        long Id { get; set; }
        string EntitlementName { get; }
        string EntitlementDesc { get; }
        EntitlementType EntitlementType { get; }
    }
}
