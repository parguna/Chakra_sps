using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;


namespace DI.Chakra.Dto
{
    [Serializable]
    public class BulkRequestParameter
    {

        public BulkRequestParameter()
        {
            Locked = false;
        }
        public List<long> UserIds { get; set; }
        public long EndpointSystemId { get; set; }
        public bool Locked { get; set; }
        public EndpointType EndpointSystemTypeId { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public AccountType AccountType { get; set; }
        public DI.Chakra.Infrastructure.Enums.ActionType ActionType { get; set; }
        public Dictionary<string, string> keyValuePairs { get; set; }
    }
}
