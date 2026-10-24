using System;
using System.Collections.Generic;

namespace DI.Chakra.Model
{
    [Serializable]
    public class RoleMatrixSimpleData
    {
        public RoleMatrixSimpleData()
        {
            this.Forms = new Dictionary<long, string>();
        }

        public Dictionary<long, string> Forms { get; set; }
    }
}
