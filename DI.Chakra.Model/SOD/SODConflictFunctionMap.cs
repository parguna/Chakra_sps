using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODConflictFunctionMap")]
    public class SODConflictFunctionMap
    {
        [Key]
        public long Id { get; set; }

        public long SodConflictFunctionId { get; set; }
        public long SodFunctionId { get; set; }

        public virtual SodFunction Function { get; set; }

    }
}
