using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DI.Chakra.Model
{
    //[TableInfo("DI_LNS_GroupsHistory","DI_LNS_GroupsHistory"]
    [Table("DI_LNS_GroupsHistory")]
    public class LotusGroupHistoryInfo : IAggregateRoot
    {
        
        //public int GroupHistoryId { get; set; }
        [Key]
        public long Id { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public string RecordStatus { get; set; }
        public int? Version { get; set; }

        public static LotusGroupHistoryInfo CreateLotusGroupInfo()
        {
            LotusGroupHistoryInfo lotusGroupInfo = new LotusGroupHistoryInfo();

            return lotusGroupInfo;
        }

    }
    
}
