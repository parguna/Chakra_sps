using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_Scheduler")]
    public class Scheduler : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string ProcessName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public Nullable<Occurence> Occurence { get; set; }
        public Nullable<int> ReccurEveryDays { get; set; }
        public Nullable<int> ReccurEveryWeeks { get; set; }
        public string WeekDay { get; set; }
        public Nullable<int> ReccurEveryMonths { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }


        public override string ToString()
        {
            return this.Id.ToString();
        }
    }

    

}
