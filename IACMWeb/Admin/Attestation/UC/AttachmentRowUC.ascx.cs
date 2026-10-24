using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class AttachmentRowUC : UIBaseUC
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
        
        protected void lblFileName_Click(object sender, EventArgs e)
        {
            try
            {
                var control = ((LinkButton)sender);
                var attachmentId = control.CommandName;
                var fileName = control.Text;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("attachmentId", attachmentId);
                parameter.Add("fileName", fileName);
                Dto.AttestationCampaignAttachment requestAttachment = DomainApiClient<Dto.AttestationCampaignAttachment>.GetByAction<Dto.AttestationCampaignAttachment>(DomainApiAction.Attestation.GetAttachmentById, parameter);

                HttpResponse response = HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=" + requestAttachment.FileName);
                response.BinaryWrite(requestAttachment.AttachmentFile);
                response.End();
            }
            catch (System.Exception ex)
            {

            }
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
    }
}