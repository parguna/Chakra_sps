using DeepIdentity.Infrastructure.Enums;
using System;

namespace DataCleansingTool
{
    public class UserMappingXLData
    {
        public string EndpointKey { get; set; }
        public string OwnerPernr { get; set; }
        public AccountType AccountType { get; set; }
    }

    public class InhouseUserMappingXLData
    {
        public string EndpointKey { get; set; }
        public string SystemCode { get; set; }
        public string OwnerPernr { get; set; }
        public AccountType AccountType { get; set; }
    }
}
