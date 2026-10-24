using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class SkypeForBusinessUnAssignedNumberMatrix : DtoBase, IIDMSReportPropertyExtractor
    {
        public int Number { get; set; }
        public int Priority { get; set; }
        public bool Assigned { get; set; }
        public bool Reserved { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public EndPoint EndPoint { get; set; }
        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "Number":
                    returnValue = Number.ToString();
                    break;
                case "Assigned":
                    returnValue = Assigned ? "Yes" : "No" ;
                    break;
                case "CreatedBy":
                    returnValue = CreatedBy ?? "";
                    break;
                case "Reserved":
                    returnValue = Convert.ToString(Reserved);
                    break;
               
                default:
                    break;
            }
            return returnValue;
        }
    }
}
