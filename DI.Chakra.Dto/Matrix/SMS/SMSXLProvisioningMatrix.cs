using System;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class SMSXLProvisioningMatrix:DtoBase, IXLData
    {
        public string PostalCode { get; set; }
        public string Location { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string DefaultGroups { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }
}
