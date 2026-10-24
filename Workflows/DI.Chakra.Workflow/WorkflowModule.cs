#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace DeepIdentity.Workflow
{
   public  class WorkflowModule
    {
       public int        ModuleId { get; set; }
       public string   ModuleName { get; set; }
       public object  ModuleValue { get; set; }
       public string  Status { get; set; }

       public override string ToString()
       {
           return string.Format("{0}-{1}-{2}", ModuleId.ToString(), ModuleName.ToString(), ModuleValue.ToString());
       }

    }
}
