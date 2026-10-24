using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Dto.Presentation
{
    public class PresentationDashBoardPreference : DtoBase
    {
        public long DashboardId { get; set; }
        //public string Title { get; set; }
        public string DashboardName { get; set; }
        public string DashboardCategory { get; set; }
        public int Position { get; set; }
        public string TargetUrl { get; set; }
        public string NavigateUrl { get; set; }
        public DashboardType DashBoardType { get; set; }
    }
}
