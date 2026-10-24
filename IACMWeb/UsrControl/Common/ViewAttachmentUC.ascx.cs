using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ViewAttachmentUC : UIBaseUC
    {
        #region Private Members
        private int count;
        private long? _requestId;
        #endregion

        #region Public Properties
        public long? RequestId
        {
            set { _requestId = value; }
        } 

        public string AccessRequestSource { get; set; }
        public int attachmentCount
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        #endregion

        #region Page & Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
                if (Page.IsPostBack)
                count = getCount();
        }

        public int getCount()
        {
            if (_requestId != null)
            {
                string source = DI.Chakra.Infrastructure.Constants.AccessRequest;
                if (!string.IsNullOrEmpty(AccessRequestSource))
                {
                    source = AccessRequestSource;
                }

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("requestId", Convert.ToString(_requestId));
                parameter.Add("source", source);
                return DomainApiClient<Dto.Attachment>.GetByAction<List<Dto.Attachment>>(DomainApiAction.Attachment.GetByRequestId, parameter).Count();
            }
            return 0;
        }

        protected void radgridViewAttachment_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            LoadGrid();
        }

        public void LoadGrid()
        {
            if (_requestId != null)
            {
                string source = DI.Chakra.Infrastructure.Constants.AccessRequest;
                if (!string.IsNullOrEmpty(AccessRequestSource))
                {
                    source = AccessRequestSource;
                }
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("requestId", Convert.ToString(_requestId));
                parameter.Add("source", source);
                List<Dto.Attachment> requestAttachments = DomainApiClient<Dto.Attachment>.GetByAction<List<Dto.Attachment>>(DomainApiAction.Attachment.GetByRequestId, parameter);
                count = requestAttachments.Count;
                radgridViewAttachment.DataSource = requestAttachments;
            }
        }

        public void BindGrid()
        {
            LoadGrid();
            radgridViewAttachment.DataBind();
        }

        protected void radgridViewAttachment_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "file")
            {
                GridDataItem ditem = (GridDataItem)e.Item;
                this.ContextObjectId = (long)ditem.GetDataKeyValue("Id");

                if (_requestId != null)
                {
                    Dto.Attachment attachment = DomainApiClient<Dto.Attachment>.GetById(ContextObjectId.Value);
                    ShowDocument(attachment.FileName, attachment.AttachmentFile, attachment.ContentType);
				}
            }
        }

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void radgridViewAttachment_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                string name = item.GetDataKeyValue("FileName").ToString();
                string ext = name.Trim().Substring(name.LastIndexOf('.') + 1);
                ImageButton image = (ImageButton)item.FindControl("AttachmentImage");
                switch (ext)
                {
                    case "png":
                        image.ImageUrl = "~/Images/action-buttons/png.png";
                        break;
                    case "docx":
                        image.ImageUrl = "~/Images/action-buttons/docx.png";
                        break;
                    case "csv":
                        image.ImageUrl = "~/Images/action-buttons/csv.png";
                        break;
                    case "jpg":
                        image.ImageUrl = "~/Images/action-buttons/jpg.png";
                        break;
                    case "jpeg":
                        image.ImageUrl = "~/Images/action-buttons/jpg.png";
                        break;
                    case "pdf":
                        image.ImageUrl = "~/Images/action-buttons/pdf.png";
                        break;
                    case "xlsx":
                        image.ImageUrl = "~/Images/action-buttons/xlsx.png";
                        break;
                    case "txt":
                        image.ImageUrl = "~/Images/action-buttons/txt.png";
                        break;
                    default:
                        image.ImageUrl = "~/Images/action-buttons/attachment.png";
                        break;
                }
            }
        }
        #endregion

        #region Private Methods
        private void ShowDocument(string fileName, byte[] fileContent, string contenttype)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
            response.BinaryWrite(fileContent);
            response.End();
        }
        #endregion
    }
}