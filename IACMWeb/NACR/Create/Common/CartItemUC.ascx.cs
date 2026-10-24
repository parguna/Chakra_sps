using Application;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.Common
{
    public partial class CartItemUC : DI.Chakra.Web.UIBaseUC
    {
        private const string V_DISPLAYDELETE = "V_DISPLAYDELETE";
        private const string V_COMPLIANCEREPORT = "V_COMPLIANCEREPORT";
        private const string V_INTRIMID = "V_INTRIMID";
        private const string V_INTERIMCLEANUP = "V_INTERIMCLEANUP";
        private const string V_INTRIMATTACHMENTID = "V_INTRIMATTACHMENTID";
        private const string V_CARTITEMCOUNT = "V_CARTITEMCOUNT";
        private SessionStatusImpl Session = new SessionStatusImpl();
        public delegate void RefreshCart();
        public event RefreshCart RefreshCartGrid;
        private Dto.AdHocAccessRequest AdhocaccessRequest = new Dto.AdHocAccessRequest();
        public bool canDenyRequest { get; set; }

        public List<UserSODComplianceReport> complianceReport
        {
            get
            {
                if (ViewState[V_COMPLIANCEREPORT] == null)
                    return null;
                return (List<UserSODComplianceReport>)ViewState[V_COMPLIANCEREPORT];
            }
            set
            {
                ViewState[V_COMPLIANCEREPORT] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public long InterimId
        {
            get
            {
                if (ViewState[V_INTRIMID] == null)
                    return 0;
                return (long)ViewState[V_INTRIMID];
            }
            set
            {
                ViewState[V_INTRIMID] = value;
            }
        }

        public long InterimCleanUp
        {
            get
            {
                if (ViewState[V_INTERIMCLEANUP] == null)
                    return 0;
                return (long)ViewState[V_INTERIMCLEANUP];
            }
            set
            {
                ViewState[V_INTERIMCLEANUP] = value;
            }
        }

        public long CartItemCount
        {
            get
            {
                if (ViewState[V_CARTITEMCOUNT] == null)
                    return 0;
                return (long)ViewState[V_CARTITEMCOUNT];
            }
            set
            {
                ViewState[V_CARTITEMCOUNT] = value;
            }
        }

        public long InterimAttachmentId
        {
            get
            {
                if (ViewState[V_INTRIMATTACHMENTID] == null)
                    return 0;
                return (long)ViewState[V_INTRIMATTACHMENTID];
            }
            set
            {
                ViewState[V_INTRIMATTACHMENTID] = value;
            }
        }
        public bool DisplayDelete
        {
            get
            {
                if (ViewState[V_DISPLAYDELETE] == null)
                    return true;
                return (bool)ViewState[V_DISPLAYDELETE];
            }
            set
            {
                ViewState[V_DISPLAYDELETE] = value;
            }
        }

        private void BindRequests()
        {
            CartItemCount = 0;
            long RequesterUserId = 0;
            long RequesteeUserId = 0;
            IList<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest> listadhoc = GetListFromInterimStore();
            CartItemCount = listadhoc.Count;
            if (listadhoc != null && listadhoc.Count > 0)
            {
                RequesteeUserId = listadhoc.FirstOrDefault().AdhocAccessRequest.RequesteeUserId;
                RequesterUserId = listadhoc.FirstOrDefault().AdhocAccessRequest.RequesterUserId;
            }

            //foreach (Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest request in listadhoc)
            //{
            //    RequesteeUserId = request.AdhocAccessRequest.RequesteeUserId;
            //    RequesterUserId = request.AdhocAccessRequest.RequesterUserId;
            //    request.AdhocAccessRequest.DisplayName = DomainApiClient<Dto.User>.GetById(request.AdhocAccessRequest.RequesteeUserId).DisplayName;
            //}
            gridAllRequests.DataSource = listadhoc;
            if (RequesterUserId <= 0)
                return;

            UserSODConflict.UserId = RequesterUserId;
            UserSODConflict.RequesteeId = RequesteeUserId;
            UserSODConflict.BindConflicts();
            complianceReport = UserSODConflict.report;
            //canDenyRequest = UserSODConflict.isDenyRequest;
        }

        public void Initialize()
        {
            try
            {

                BindRequests();
                gridAllRequests.DataBind();
                gridAllRequests.Rebind();
            }
            catch (Exception ex)
            {
                throw;
            }
            //string script = "function f(){$find(\"" + this.gridAllRequests.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
        }
        protected void panel_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
       .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        public List<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest> GetListFromInterimStore()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("userId", Session.User.Id.ToString());
            parameter.Add("isSaved", "true");
            parameter.Add("itemType", InterimStoreItemType.AccessRequest.ToString());

            List<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest> listAdHocAccessRequests = DomainApiClient<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest>.GetByAction<List<Dto.AdHocRequest.Presentation.PresentationInterimAdhocRequest>>
                (DomainApiAction.PresentationInterimAdhocRequest.GetByUser, parameter);

            return listAdHocAccessRequests;
        }

        //public List<Attachment> GetUploadedFiles()
        //{
        //    List<Attachment> attachments = new List<Attachment>();
        //    foreach (Telerik.Web.UI.UploadedFile file in AsyncUploadAttach.UploadedFiles)
        //    {
        //        Attachment na = new Attachment();
        //        na.RequestTypeId = (long)RequestType.Provisioning;
        //        na.ContentType = file.ContentType;
        //        na.FileName = file.FileName;
        //        byte[] fileData = new byte[file.ContentLength];
        //        file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
        //        na.AttachmentFile = fileData;
        //        attachments.Add(na);
        //    }
        //    return attachments;
        //}

        protected void gridAllRequests_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindRequests();
        }

        protected void gridAllRequests_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            long interimstoreid = Convert.ToInt64(gridAllRequests.Items[e.Item.ItemIndex]["Id"].Text);
            DomainApiClient<Dto.InterimStore>.DeleteById(interimstoreid);
            BindRequests();
            gridAllRequests.Rebind();
            //ViewState["V_TOTALITEMS"] = gridAllRequests.Items.Count;
        }

        protected void gridAllRequests_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Details")
            {
                long interimstoreid = Convert.ToInt64(gridAllRequests.Items[e.Item.ItemIndex]["Id"].Text);

                Dto.InterimStore interimStore = DomainApiClient<Dto.InterimStore>.GetById(interimstoreid);

                AdHocAccessRequest adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(interimStore.XmlData);
                AdhocMoreDetails.RaiseModelPopup(adhocaccessrequest);
                detailWindow.VisibleOnPageLoad = true;

            }
            else if (e.CommandName == "ShowWarningInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                string roleCode = item["Entitlement"].Text.ToString();
                UserSODConflict.EntitlementConflict = roleCode;
                UserSODConflict.UserId = (long)item.GetDataKeyValue("AdhocAccessRequest.RequesterUserId");
                UserSODConflict.RequesteeId = (long)item.GetDataKeyValue("AdhocAccessRequest.RequesteeUserId");
                UserSODConflict.BindConflicts();
                SODConflictWindow.VisibleOnPageLoad = true;
            }
            else if (e.CommandName == "file")
            {
                long interimstoreid = Convert.ToInt64(gridAllRequests.Items[e.Item.ItemIndex]["Id"].Text);
                this.InterimId = interimstoreid;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("taskId", interimstoreid.ToString());
                parameter.Add("isSaved", Boolean.TrueString);
                parameter.Add("itemType", InterimStoreItemType.Attachment.ToString());

                Dto.InterimStore interimStoreAttachment = DomainApiClient<Dto.InterimStore>.GetByAction<Dto.InterimStore>
                 (DomainApiAction.Tasks.InterimStore.GetByTaskId, parameter);
                removeAttach.Visible = false;
                AsyncUploadAttach.Enabled = true;
                if (interimStoreAttachment != null)
                {
                    InterimAttachmentId = interimStoreAttachment.Id;
                    var attachment = UtilityMethods.Deserialize<PresentationInterimAttachment>(interimStoreAttachment.XmlData);
                    attachedFileName.Text = attachment.FileName;
                    removeAttach.Visible = true;
                    AsyncUploadAttach.Enabled = false;
                }
                AddAttachmentWindow.VisibleOnPageLoad = true;
            }
        }

        protected void gridAllRequests_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                AdHocActionType ActionType = (AdHocActionType)dataItem.GetDataKeyValue("AdhocAccessRequest.AdhocActionType");
                long EndpointSystemId = (long)dataItem.GetDataKeyValue("AdhocAccessRequest.EndpointSystemId");
                long RequesteeUserId = (long)dataItem.GetDataKeyValue("AdhocAccessRequest.RequesteeUserId");
                GridColumn DeleteColumn = gridAllRequests.MasterTableView.GetColumn("DeleteColumn");
                GridColumn chkTemp = gridAllRequests.MasterTableView.GetColumn("CheckTemp");
                if (DeleteColumn != null)
                {
                    DeleteColumn.Visible = DisplayDelete;

                }
                if(chkTemp != null)
                {
                    chkTemp.Visible = DisplayDelete;
                }
                var details = dataItem.FindControl("Details") as ImageButton;
                if (details != null)
                {
                    details.Visible = ActionType != AdHocActionType.DeleteAccount;
                }
                GridColumn DetailColumn = gridAllRequests.MasterTableView.GetColumn("Details");
                if (DetailColumn != null)
                    DetailColumn.Visible = DisplayDelete;
                //Following is commented as its no longer required for V5
                //if (ActionType == AdHocActionType.NewAccount)
                //{

                //    //var DataSource = (IList<PresentationInterimAdhocRequest>)gridAllRequests.DataSource;
                //    //bool IsDeleteDisplay = DataSource.Where(lq => lq.AdhocAccessRequest.EndpointSystemId == EndpointSystemId &&
                //    //lq.AdhocAccessRequest.RequesteeUserId == RequesteeUserId && lq.AdhocAccessRequest.AdhocActionType != AdHocActionType.NewAccount)
                //    //.Count() > 0 ? false : true;
                //    //if (DeleteColumn != null && DisplayDelete)
                //    //    (dataItem["DeleteColumn"].Controls[0] as ImageButton).Enabled = IsDeleteDisplay;
                //}

                // dataItem["TemplateInfoColumn"].Controls[1].Visible = false;
                if (ActionType == AdHocActionType.AmendmentAdd)
                {
                    UserSODConflict.EntitlementConflict = dataItem["Entitlement"].Text.ToString();
                    ////if (dataItem["TemplateInfoColumn"].Controls.Count > 1)
                    ////{
                    ////    dataItem["TemplateInfoColumn"].Controls[1].Visible = UserSODConflict.GetRisks().Count > 0;
                    ////}
                }

                //Following is commented as its no longer required for V5
                //if (ActionType == AdHocActionType.AmendmentRemove)
                //{
                //    var DataSource = (IList<PresentationInterimAdhocRequest>)gridAllRequests.DataSource;
                //    bool IsDeleteDisplay = DataSource.Where(lq => lq.AdhocAccessRequest.EndpointSystemId == EndpointSystemId &&
                //    lq.AdhocAccessRequest.RequesteeUserId == RequesteeUserId && lq.AdhocAccessRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                //    .Count() > 0 ? false : true;
                //    if (DeleteColumn != null && DisplayDelete)
                //        (dataItem["DeleteColumn"].Controls[0] as ImageButton).Enabled = IsDeleteDisplay;
                //}


                //GridColumn riskColumn = gridAllRequests.MasterTableView.GetColumn("TemplateInfoColumn");
                //if (riskColumn != null)
                //    riskColumn.Visible = DisplayDelete;
            }
            else
            {
                //GridDataItem dataItem = e.Item as GridDataItem;
                //long validTo = (long)dataItem.GetDataKeyValue("AdhocAccessRequest.RequesteeUserId");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SODConflictWindow.VisibleOnPageLoad = false;
        }

        protected void Save_Click(object sender, EventArgs e)
        {
            if (AsyncUploadAttach.UploadedFiles.Count == 0)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Mandatory fields should be required");
            }
            else
            {
                long interimstoreid = this.InterimId;
                InterimStore interimStore = new InterimStore();
                interimStore.TaskId = interimstoreid;
                PresentationInterimAttachment presentationIterimAttch = new PresentationInterimAttachment();

                Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
                byte[] fileData = new byte[file.ContentLength];
                file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                presentationIterimAttch.FileName = file.FileName;
                presentationIterimAttch.AttachmentFile = fileData;
                presentationIterimAttch.ContentType = file.ContentType;
                presentationIterimAttch.CreatedDate = DateTime.Now;
                presentationIterimAttch.CreatedBy = Session.User.DisplayName;

                interimStore.InterimStoreItemType = InterimStoreItemType.Attachment;
                interimStore.UserId = Session.User.Id;
                interimStore.IsSaved = true;
                interimStore.XmlData = UtilityMethods.Serialize<Dto.Presentation.PresentationInterimAttachment>(presentationIterimAttch);
                DomainApiClient<Dto.InterimStore>.Save(interimStore);
                AddAttachmentWindow.VisibleOnPageLoad = false;
                MasterPage.ShowAlert(AlertType.Success, "Attachment uploaded successfully.");
                BindRequests();
                gridAllRequests.Rebind();
            }
        }

        protected void detail_OK_Click(object sender, EventArgs e)
        {
            detailWindow.VisibleOnPageLoad = false;
        }
        protected void Cancel_Click(object sender, EventArgs e)
        {
            AddAttachmentWindow.VisibleOnPageLoad = false;
        }

        protected void removeButton_Click(object sender, EventArgs e)
        {
            long interimstoreid = this.InterimAttachmentId;
            DomainApiClient<Dto.InterimStore>.DeleteById(interimstoreid);
            removeAttach.Visible = false;
            AsyncUploadAttach.Enabled = true;
        }

        protected void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            var x = sender as CheckBox;
            if (x != null)
            {
                bool checkStatus = x.Checked;
                foreach (GridDataItem item in gridAllRequests.MasterTableView.Items)
                {
                    CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                    if (chkbx.Visible)
                    {
                        chkbx.Checked = checkStatus;
                        item.Selected = chkbx.Checked;
                    }
                }

            }
        }

        protected void ClientSelectColumn_CheckedChanged(object sender, EventArgs e)
        {
            int gridcount = gridAllRequests.MasterTableView.Items.Count;
            int selectedcount = 0;
            foreach (GridDataItem item in gridAllRequests.MasterTableView.Items)
            {
                CheckBox chkbx = (CheckBox)item["CheckTemp"].FindControl("ClientSelectColumn");
                if (chkbx.Checked)
                    selectedcount++;
                item.Selected = chkbx.Checked;
            }
            foreach (GridItem item in gridAllRequests.MasterTableView.GetItems(GridItemType.Header))
            {
                CheckBox chkbx = (CheckBox)item.FindControl("CheckBox2");
                if (selectedcount == gridcount)
                {
                    chkbx.Checked = true;
                }
                else
                {
                    chkbx.Checked = false;
                }
            }
           
        }

        public List<long> GetSelectedItems()
        {
            List<long> selectedItems = new List<long>();
            foreach (GridDataItem item in gridAllRequests.MasterTableView.Items)
            {
                CheckBox chkbx = item["CheckTemp"].FindControl("ClientSelectColumn") as CheckBox;
                if (chkbx != null && chkbx.Checked)
                {
                    long id;
                    if(long.TryParse(item.GetDataKeyValue("Id").ToString(),out id) && id> 0)
                    {
                        selectedItems.Add(id);
                    }
                }
                    
            }
            return selectedItems;
        }

        public bool checkBox2CheckedStatus()
        {
            bool blncheckBox2Checked = false;
            foreach (GridItem item in gridAllRequests.MasterTableView.GetItems(GridItemType.Header))
            {
                CheckBox chkbx = item.FindControl("CheckBox2") as CheckBox;
                if (chkbx != null && chkbx.Checked)
                {
                    blncheckBox2Checked = true;
                }
            }
            return blncheckBox2Checked;
        }
        public bool IsCartItemStillExists()
        {
            bool exists = false;
            try
            {
                var listadhoc = GetListFromInterimStore();
                exists = listadhoc.Count > 0;
            }
            catch (Exception ex)
            {
            }

            return exists;
        }
    }
}
