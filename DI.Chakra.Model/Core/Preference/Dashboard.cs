using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_Dashboard")]
    [Serializable]
    public class Dashboard : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string DashboardName { get; set; }
        public string DashboardCategory { get; set; }
        public DashboardType DashBoardType { get; set; }
        public string TargetUrl { get; set; }
        public string NavigateUrl { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
