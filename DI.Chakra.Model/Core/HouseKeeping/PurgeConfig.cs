using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.Core.House_Keeping
{
    //[Table("DI_M_PurgeConfig")]
    //public class PurgeConfig : IAggregateRoot
    //{
    //    [Key]
    //    public long Id { get; set; }
    //    public string TableName { get; set; }
    //    public string ObjectName { get; set; }
    //    public string Description { get; set; }
    //    public int DaysToKeep { get; set; }
    //    public string Status { get; set; }
    //    public PurgeMode PurgeMode { get; set; }
    //    public string CreatedBy { get; set; }
    //    public Nullable<System.DateTime> CreatedDate { get; set; }
    //    public string LastModifiedBy { get; set; }
    //    public Nullable<System.DateTime> LastModifiedDate { get; set; }
    //    public Nullable<System.DateTime> LastRunOn { get; set; }
    //    public string PurgeModeString
    //    {
    //        get
    //        {
    //            return UtilityMethods.GetEnumAttribute<Description>((PurgeMode)this.PurgeMode);
    //        }
    //    }
    //}
}
