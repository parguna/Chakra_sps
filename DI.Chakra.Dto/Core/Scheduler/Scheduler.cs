using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class Scheduler:DtoBase
    {
        public string ProcessName { get; set; }

        public DateTime StartDate { get; set; }

        public System.TimeSpan StartTime { get; set; }

        public Nullable<Occurence> Occurence { get; set; }

        public Nullable<int> ReccurEveryDays { get; set; }

        public Nullable<int> ReccurEveryWeeks { get; set; }

        public string WeekDay { get; set; }

        public Nullable<int> ReccurEveryMonths { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
