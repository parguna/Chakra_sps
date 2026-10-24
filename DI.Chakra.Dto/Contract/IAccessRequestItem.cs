using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public interface IAccessRequestItem
    {
        AccountType AccountType { get; set; }
        long EndpointUserId { get; set; }
        AccessRequestItemType AccessRequestItemType { get; set; }
        AccessItemType AccessItemType { get; set; }
        long KeyId { get; set; }
        long EndpointSystemId { get; set; }
        long EndpointSystemTypeId { get; set; }
        EndpointType EndpointType { get; set; }
        string ItemName { get; set; }
        string ItemDetail { get; set; }
        string ItemDescription { get; set; }
        EntitlementType EntitlementType { get; set; }

        long? ParentAccessRequestId { get; set; }

        bool IsApproved { get; set; }
        string AdditionalInfo { get; set; }
        ACRCategory ACRCategory { get; set; }

        bool Enabled { get; set; }

    }
}
