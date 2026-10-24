using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserPreference
    {
        public long TabMenuId { get; set; }
        public string MenuName { get; set; }
        public int Position { get; set; }
        public string TargetUrl { get; set; }
    }
    public class DashBoardPreference
    {
        public long DashboardId { get; set; }
        //public string Title { get; set; }
        public string DashboardName { get; set; }
        public string DashboardCategory { get; set; }
        public int Position { get; set; }
        public string TargetUrl { get; set; }
        public string NavigateUrl { get; set; }
    }
}
