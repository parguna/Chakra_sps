using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_ReconcilationExceptionAttributeChanges")]
    public class ExceptionAttributeChange : EntityBase, ICreatable, IUpdatable
    {
        public long ReconcilationId { get; set; }

        public long ExceptionAttributeId { get; set; }

        public string Attributes { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("ReconcilationId")]
        public AccountReconcilation AccountReconcilations { get; set; }

        [ForeignKey("ExceptionAttributeId")]
        public ExceptionAttribute ExceptionAttributes { get; set; }
    }
}
