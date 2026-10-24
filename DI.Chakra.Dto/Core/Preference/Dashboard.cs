using System;
using System.Collections.Generic;

using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class Dashboard:DtoBase
    {
        public string DashboardUniqueName { get; set; }

        public string DashboardName { get; set; }

        public string DashboardCategory { get; set; }

        public DashboardType DashBoardType { get; set; }

        public string TargetUrl { get; set; }

        public string NavigateUrl { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
