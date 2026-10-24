using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class CommentsRowUC : UIBaseUC
    {
        #region Memebers
        
        #endregion

        public long TaskId
        {
            get
            {
                if (ViewState["TaskId"] != null)
                {
                    return Convert.ToInt64(ViewState["TaskId"]);
                }
                return 0;
            }
            set
            {
                ViewState["TaskId"] = value;
            }
        }

        public long CampaignId
        {
            get
            {
                if (ViewState["CampaignId"] != null)
                {
                    return Convert.ToInt64(ViewState["CampaignId"]);
                }
                return 0;
            }
            set
            {
                ViewState["CampaignId"] = value;
            }
        }

        public bool AllowAddComments
        {
            get
            {
                if (ViewState["AllowAddComments"] != null)
                {
                    return Convert.ToBoolean(ViewState["AllowAddComments"]);
                }
                return false;
            }
            set
            {
                ViewState["AllowAddComments"] = value;
            }
        }

        public string CurrentUser
        {
            get
            {
                if (ViewState["CurrentUser"] != null)
                {
                    return Convert.ToString(ViewState["CurrentUser"]);
                }
                return "";
            }
            set
            {
                ViewState["CurrentUser"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
               
        }        
    }
}