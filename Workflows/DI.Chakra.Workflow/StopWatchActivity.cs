#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion 

namespace DeepIdentity.Workflow
{
  public  class StopWatchActivity:Activity
    {
        public int ModuleId { get; set; }
        public double StopTime { get; set; }
    }
}
