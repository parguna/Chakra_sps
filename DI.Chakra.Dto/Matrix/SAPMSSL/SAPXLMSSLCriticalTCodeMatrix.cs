using System;

namespace DI.Chakra.Dto
{
    public class SAPXLMSSLCriticalTCodeMatrix : DtoBase, IXLData,IIDMSReportPropertyExtractor
    {
        public string Transaction { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionOwner { get; set; }
        public DateTime? ValidFrom
        {
            get;
            set;
        }
        public DateTime? ValidTo
        {
            get;
            set;
        }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Transaction":
                    returnValue = Transaction ?? "";
                    break;
                case "TransactionDescription":
                    returnValue = TransactionDescription ?? "";
                    break;
                case "TransactionOwner":
                    returnValue = TransactionOwner ?? "";
                    break;
                case "ValidationMessage":
                    returnValue = ValidationMessage ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
