using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Triggers.UC
{
    public partial class ViewUserDetailUC : UIBaseUC
    {
        public User UserOldDetail
        {
            set
            {
                OrgDetailUCCurrent.UserDetail = value;
            }
        }
        public User UserNewDetail
        {
            set
            {
                OrgDetailUCNew.UserDetail = value;
            }
        }
    }
}