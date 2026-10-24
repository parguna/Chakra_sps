using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class AccountNavigatorHelper
    {
        public AccountNavigatorHelper()
        {
            FilterFieldAndValue = new Dictionary<string, string>();
        }
        public long ApplicationId { get; set; }
        public long ApplicationTypeId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SelectedRowNumber { get; set; }
        public string FilterExpression { get; set; }
        public Dictionary<string, string> FilterFieldAndValue { get; set; }
        public bool MultipleAccount { get; set; }
        public string MappingType { get; set; }
        public string ComboFilterExpression { get; set; }
    }
}
