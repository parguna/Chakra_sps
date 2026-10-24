using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationAttestationTaskSummary : Task
    {
        public int StatusCount { get; set; }

        public string Status { get; set; }

        public bool IsExploded { get; set; }

        public string BGColor { get; set; }
    }
}
