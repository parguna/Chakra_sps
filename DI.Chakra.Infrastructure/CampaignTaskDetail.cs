using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class CampaignTaskDetail
    {
        public string TotalTask { get; set; }
        public string CompletedTask { get; set; }
        public string CurrentLevel { get; set; }
        public string TotalLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
