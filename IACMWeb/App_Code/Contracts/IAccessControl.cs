using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Web
{
    public interface IAccessControl
    {
        void ActionClicked(object sender, AccessActionEventArgs e);
    }
}
