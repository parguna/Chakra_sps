#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion 

namespace DeepIdentity.Workflow
{
    public class MailActivity : Activity
    {
        public int ModuleId { get; set; }
        public string MailTemplateName { get; set; }

    }
}
