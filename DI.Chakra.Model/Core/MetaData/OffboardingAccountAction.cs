using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_OffboardingAccountAction")]
    public class OffboardingAccountAction
    {
        [Key]
        public OffboardingAccountActionType Id { get; set; }
        public string Desc { get; set; }
    }
}
