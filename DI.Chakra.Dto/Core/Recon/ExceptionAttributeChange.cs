using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class ExceptionAttributeChange : DtoBase
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

        public AccountReconcilation AccountReconcilations { get; set; }

        public ExceptionAttribute ExceptionAttributes { get; set; }

    }
}
