using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    public class WindowsShareFolderApproverGridData
    {
        public int approverId { get; set; }

        public int? approverUserId { get; set; }//From DI_M_User
        public int? approverGroupId { get; set; }//From DI_M_Group
        public bool isGroup { get; set; }

        public string approverUserName { get; set; }
        public string approverGroupName { get; set; }
    }
}
