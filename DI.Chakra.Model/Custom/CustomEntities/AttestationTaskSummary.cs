using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace DI.Chakra.Model.CustomEntities
{
    public class AttestationStatusSummary
    {
        public int StatusCount { get; set; }

        public string Status { get; set; }

        public bool IsExploded { get; set; }

        public string BGColor { get; set; }

        public AttestationStatusSummary(int statusCount, string status, bool isExploded, string bgColor)
        {
            this.StatusCount = statusCount;
            this.Status = status;
            this.IsExploded = isExploded;
            this.BGColor = bgColor;
        }

        public AttestationStatusSummary()
        {

        }
    }
}
