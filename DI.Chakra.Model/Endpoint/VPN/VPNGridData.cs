using System;

namespace DI.Chakra.Model
{
    public class VPNGridData
    {
        public string ApplicationName { get; set; }
        public bool HasProductionAccess { get; set; }
        public bool HasQAAccess { get; set; }
        public bool HasDevAccess { get; set; }
        public string IsMapped { get; set; }
        public string Owner { get; set; }
    }
}
