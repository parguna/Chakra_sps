#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion 

namespace DeepIdentity.Workflow
{
    public class ProcessActivity:Activity
    {
        public int ModuleId { get; set; } 
        public string ProcessName { get; set; }

    }
}
