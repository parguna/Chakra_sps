using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_Ruleset")]
    public class Ruleset
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public string Name{ get; set; }
       
        public int MajorVersion{ get; set; }
       
        public int MinorVersion{ get; set; }
       
        public string RuleSet{ get; set; }

        public bool Active { get; set; }

        public short Status { get; set; }
       
        public string AssemblyPath{ get; set; }

        public string ActivityName { get; set; }
       
        public DateTime ModifiedDate{ get; set; }
        
    }
}
