using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class AccessRequestSourcePieSeries
    {
        public AccessRequestSource Name { get; set; }
        public decimal Y { get; set; }
    }
    public class AccessRequestSourceChartData
    {
        public string UserName { get; set; }
        public string Description { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }

    }
    public class AccessRequestSourceByMonth
    {
        public AccessRequestSource Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }
    }

    public class AccessRequestProgressStatusByMonth
    {
        public AccessRequestProgress Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }
    }

    public class AdhocRequestAccountTyebyMonth
    {
        public AccountType Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }
    }
}
