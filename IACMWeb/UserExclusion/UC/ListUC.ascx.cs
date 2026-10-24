using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UserExclusion.UC
{
    public partial class ListUC : UIBaseUC
    {
        #region Members

        private const string V_USEREXTLIST = "USEREXTLIST";
        private const string ADD_V_EMPEVENTTRIGGER = "ADDEMPEVENTTRIGGER";
        private const string DELETE_V_EMPEVENTTRIGGER = "DELETE_V_EMPEVENTTRIGGER";
        #endregion

        #region Properties

        public List<Dto.Presentation.PresentationUserExclusion> UserExtList
        {
            get
            {
                if (ViewState[V_USEREXTLIST] == null)
                    return null;
                return (List<Dto.Presentation.PresentationUserExclusion>)ViewState[V_USEREXTLIST];
            }
            set
            {
                ViewState[V_USEREXTLIST] = value;
            }
        }

        public List<Dto.EmployeeEventTrigger> addemployeeEventList
        {
            get
            {
                if (ViewState[ADD_V_EMPEVENTTRIGGER] == null)
                    return null;
                return (List<Dto.EmployeeEventTrigger>)ViewState[ADD_V_EMPEVENTTRIGGER];
            }
            set
            {
                ViewState[ADD_V_EMPEVENTTRIGGER] = value;
            }
        }

        public List<Dto.EmployeeEventTrigger> deleteemployeeEventList
        {
            get
            {
                if (ViewState[DELETE_V_EMPEVENTTRIGGER] == null)
                    return null;
                return (List<Dto.EmployeeEventTrigger>)ViewState[DELETE_V_EMPEVENTTRIGGER];
            }
            set
            {
                ViewState[DELETE_V_EMPEVENTTRIGGER] = value;
            }
        }

        #endregion

        #region Methods

        private void Save()
        {
            List<Dto.UserExclusion> saveUserExclusionList = new List<Dto.UserExclusion>();
            if (UserExtList.Count > 0)
            {
                foreach (Dto.Presentation.PresentationUserExclusion userlist in UserExtList)
                {
                    ////Dictionary<string, string> parameter = new Dictionary<string, string>();
                    ////parameter.Add("userId", userlist.UserId.ToString());
                    ////parameter.Add("evnt", userlist.EventType.ToString());
                    //var tmpemployeeEventList = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<EmployeeEventTrigger>>(DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);
                    // if (tmpemployeeEventList.Count > 0)
                    // {
                    //     foreach (Dto.EmployeeEventTrigger employeetrigger in tmpemployeeEventList)
                    //     {
                    //         if (!employeetrigger.OldEffectiveDate.HasValue)
                    //         {
                    //             employeetrigger.OldEffectiveDate = employeetrigger.EffectiveDate;
                    //         }
                    //         employeetrigger.EffectiveDate = userlist.EffectiveDate.Value;

                    //         employeeEventList.Add(employeetrigger);
                    //     }
                    // }
                    // else
                    // {
                    //     parameter.Clear();
                    //     parameter.Add("userId", userlist.UserId.ToString());
                    //     List<Dto.AccessRequest> accessRequestList = DomainApiClient<Dto.AccessRequest>.GetByAction<List<Dto.AccessRequest>>(DomainApiAction.Provisioning.AccessRequest.GetByUserId, parameter);
                    //     if (accessRequestList.Count > 0)
                    //     {
                    //         foreach (Dto.AccessRequest accessRequest in accessRequestList)
                    //         {
                    //             accessRequest.ActivationDate = userlist.EffectiveDate.Value;
                    //         }
                    //         DomainApiClient<Dto.AccessRequest>.SaveList(accessRequestList);
                    //     }
                    // }
                    var userExclusion = new Dto.UserExclusion();
                    userExclusion.Id = userlist.Id;
                    userExclusion.FileName = userlist.FileName;
                    userExclusion.CreatedBy = userlist.CreatedBy;
                    userExclusion.CreatedDate = userlist.CreatedDate;
                    userExclusion.ContentType = userlist.ContentType;
                    userExclusion.EffectiveDate = userlist.EffectiveDate;
                    userExclusion.EventType = (EmployeeEvents)Enum.Parse(typeof(EmployeeEvents), userlist.EventType);
                    userExclusion.UserId = userlist.UserId;
                    userExclusion.UserName = userlist.UserName;
                    userExclusion.Remarks = userlist.Remarks;
                    userExclusion.LastModifiedBy = userlist.LastModifiedBy;
                    userExclusion.LastModifiedDate = userlist.LastModifiedDate;
                    userExclusion.AttachmentFile = userlist.AttachmentFile;

                    saveUserExclusionList.Add(userExclusion);
                }
            }
            DomainApiClient<Dto.UserExclusion>.GetByAction<Dto.UserExclusion>(DomainApiAction.UserExclusion.SaveUserList, null, saveUserExclusionList);

            Dictionary<string, string> parameters;
            foreach (var trigger in deleteemployeeEventList)
            {
                parameters = new Dictionary<string, string>();
                parameters.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, trigger);
            }
            foreach (var trigger in addemployeeEventList)
            {
                parameters = new Dictionary<string, string>();
                parameters.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, trigger);
            }

            MasterPage.ShowAlert(AlertType.Success, "User Exclusion saved successfully.");

            UserExtList = DomainApiClient<Dto.Presentation.PresentationUserExclusion>.GetByAction<List<Dto.Presentation.PresentationUserExclusion>>(DomainApiAction.PresentationUserExclusion.GetAllUserExclusion, null);
            //UserExtList.ForEach(x => x.HasAttachments = x.AttachmentFile.Count() > 0 ? true : false);
            UserExclutionGrid.Rebind();
            //EffectiveDate.MinDate = DateTime.Now;
            //msgSave.Visible = false;
            addemployeeEventList = new List<EmployeeEventTrigger>();
        }

       
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                addemployeeEventList = new List<EmployeeEventTrigger>();
                deleteemployeeEventList = new List<EmployeeEventTrigger>();
                UserExtList = DomainApiClient<Dto.Presentation.PresentationUserExclusion>.GetByAction<List<Dto.Presentation.PresentationUserExclusion>>(DomainApiAction.PresentationUserExclusion.GetAllUserExclusion, null);
               // UserExtList.ForEach(x => x.HasAttachments = x.AttachmentFile.Count() > 0 ? true : false);

                //BindEventType();
                //EffectiveDate.MinDate = DateTime.Now;
                //msgSave.Visible = false;
            }
        }

        //protected void btnAdd_Click(object sender, EventArgs e)
        //{
        //    var selectedUser = UserSearchUC.SelectedUser;
        //    if (selectedUser != null && !(string.IsNullOrEmpty(ddlEventType.SelectedValue.ToString())) && EffectiveDate.SelectedDate != null && !string.IsNullOrEmpty(txtComments.Text) && AsyncUploadAttach.UploadedFiles.Count > 0)
        //    {
        //        if (!UserExtList.Any(m => m.UserId == selectedUser.Id && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString()))
        //        {
        //            Dto.Presentation.PresentationUserExclusion userExclusion = new Dto.Presentation.PresentationUserExclusion();
        //            userExclusion.UserId = UserSearchUC.SelectedUser.Id;
        //            userExclusion.UserName = UserSearchUC.SelectedUser.UserName;
        //            userExclusion.EventType = ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString();
        //            userExclusion.EffectiveDate = EffectiveDate.SelectedDate;
        //            userExclusion.Remarks = txtComments.Text;
        //            userExclusion.DisplayName = UserSearchUC.SelectedUser.DisplayName;
        //            if (AsyncUploadAttach.UploadedFiles.Count > 0)
        //            {
        //                Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
        //                byte[] fileData = new byte[file.ContentLength];
        //                file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
        //                userExclusion.FileName = file.FileName;
        //                userExclusion.AttachmentFile = fileData;
        //                userExclusion.ContentType = file.ContentType;
        //                userExclusion.HasAttachments = true;
        //            }


        //            Dictionary<string, string> parameter = new Dictionary<string, string>();
        //            parameter.Add("userId", userExclusion.UserId.ToString());
        //            parameter.Add("evnt", userExclusion.EventType.ToString());
        //            var tmpemployeeEventList = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<EmployeeEventTrigger>>(DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);
        //            if (tmpemployeeEventList.Count > 0)
        //            {
        //                foreach (Dto.EmployeeEventTrigger employeetrigger in tmpemployeeEventList)
        //                {
        //                    if (employeetrigger.OldEffectiveDate.HasValue && userExclusion.EffectiveDate.HasValue &&
        //                         userExclusion.EffectiveDate.Value > employeetrigger.OldEffectiveDate.Value)
        //                    {
        //                        employeetrigger.EffectiveDate = userExclusion.EffectiveDate.Value;
        //                    }
        //                    else if (employeetrigger.OldEffectiveDate.HasValue)
        //                    {
        //                        employeetrigger.EffectiveDate = employeetrigger.OldEffectiveDate.Value;
        //                    }
        //                    addemployeeEventList.Add(employeetrigger);
        //                }
        //            }

        //            UserExtList.Add(userExclusion);
        //            UserExclutionGrid.Rebind();
        //        }
        //        else if (UserExtList.Any(m => m.UserId == selectedUser.Id && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString() && m.EffectiveDate != EffectiveDate.SelectedDate))
        //        {
        //            Dto.Presentation.PresentationUserExclusion existingUser = UserExtList.Where(m => m.UserId == selectedUser.Id && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString()).FirstOrDefault();
        //            if (existingUser != null)
        //            {
        //                existingUser.EffectiveDate = EffectiveDate.SelectedDate;
        //                existingUser.Remarks = txtComments.Text;
        //                if (AsyncUploadAttach.UploadedFiles.Count > 0)
        //                {
        //                    Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
        //                    byte[] fileData = new byte[file.ContentLength];
        //                    file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
        //                    existingUser.FileName = file.FileName;
        //                    existingUser.AttachmentFile = fileData;
        //                    existingUser.ContentType = file.ContentType;
        //                    existingUser.HasAttachments = true;
        //                }


        //                Dictionary<string, string> parameter = new Dictionary<string, string>();
        //                parameter.Add("userId", existingUser.UserId.ToString());
        //                parameter.Add("evnt", existingUser.EventType.ToString());
        //                var tmpemployeeEventList = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<EmployeeEventTrigger>>(DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);
        //                if (tmpemployeeEventList.Count > 0)
        //                {
        //                    foreach (Dto.EmployeeEventTrigger employeetrigger in tmpemployeeEventList)
        //                    {
        //                        if (employeetrigger.OldEffectiveDate.HasValue && existingUser.EffectiveDate.HasValue &&
        //                             existingUser.EffectiveDate.Value > employeetrigger.OldEffectiveDate.Value)
        //                        {
        //                            employeetrigger.EffectiveDate = existingUser.EffectiveDate.Value;
        //                        }
        //                        else if (employeetrigger.OldEffectiveDate.HasValue)
        //                        {
        //                            employeetrigger.EffectiveDate = employeetrigger.OldEffectiveDate.Value;
        //                        }
        //                        addemployeeEventList.Add(employeetrigger);
        //                    }
        //                }

        //                UserExclutionGrid.Rebind();
        //            }
        //        }
        //        else
        //        {
        //            MasterPage.ShowAlert(AlertType.Error, "User already exists.");
        //            return;
        //        }
        //        txtComments.Text = "";
        //    }
        //    else
        //    {
        //        string attchmsg = AsyncUploadAttach.UploadedFiles.Count > 0 ? " Reattach the file." : "";
        //        MasterPage.ShowAlert(AlertType.Error, "Required fields should not be empty." + attchmsg);
        //        return;
        //    }
        //    msgSave.Visible = true;
        //    UserSearchUC.Clear();
        //    ddlEventType.ClearSelection();
        //    EffectiveDate.Clear();
        //}

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "User Exclusion - Save");
                Save();
            }
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "User Exclusion - Delete");
                delete();
            }
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                UserExclutionGrid.ExportSettings.FileName = "User Exclusion";
                UserExclutionGrid.Export(UserExclutionGrid, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                UserExclutionGrid.Export<IList<Dto.Presentation.PresentationUserExclusion>>(UserExtList, e.ActionType, "User Exclusion");
            }
        }

        #endregion
        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            long UserExclusionId = Convert.ToInt64(ContextObjectId.Value);

            bool doesExist = UserExtList.Any(user => user.Id == UserExclusionId);
            if (doesExist)
            {
               // UserExtList.RemoveAt(UserExtList.FindIndex(user => user.Id == UserExclusionId));
                Dto.UserExclusion userExclution = DomainApiClient<Dto.UserExclusion>.GetById(UserExclusionId);
                //isdelete true is active status
                if (userExclution != null && userExclution.IsDeleted)
                {
                    userExclution.IsDeleted = true;
                    DomainApiClient<Dto.UserExclusion>.Save(userExclution);
                    UserExtList = DomainApiClient<Dto.Presentation.PresentationUserExclusion>.GetByAction<List<Dto.Presentation.PresentationUserExclusion>>(DomainApiAction.PresentationUserExclusion.GetAllUserExclusion, null);
                    MasterPage.ShowAlert(AlertType.Success, "User Exclusion Deleted Successfully.");
                    UserExclutionGrid.Rebind();
                    this.ContextObjectId = null;
                }
                else if(userExclution != null && !userExclution.IsDeleted)
                {
                    UserExtList = DomainApiClient<Dto.Presentation.PresentationUserExclusion>.GetByAction<List<Dto.Presentation.PresentationUserExclusion>>(DomainApiAction.PresentationUserExclusion.GetAllUserExclusion, null);
                    MasterPage.ShowAlert(AlertType.Warning, "User Exclusion already Deleted.");                    
                    UserExclutionGrid.Rebind();
                    this.ContextObjectId = null;
                }
            }
           
        }
        protected void UserExclutionGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            UserExclutionGrid.DataSource = UserExtList;
        }

        protected void UserExclutionGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = (GridDataItem)e.Item;
                long UserExclusionId = Convert.ToInt64(item.GetDataKeyValue("Id"));
                bool doesExist = UserExtList.Any(user => user.Id == UserExclusionId);
                if (doesExist)
                {
                    UserExtList.RemoveAt(UserExtList.FindIndex(user => user.Id == UserExclusionId));
                    Dto.UserExclusion userExclution = DomainApiClient<Dto.UserExclusion>.GetById(UserExclusionId);
                    if (userExclution != null)
                    {
                        
                    }
                }
                UserExclutionGrid.Rebind();
            }
            else if (e.CommandName == "file")
            {
                GridDataItem ditem = (GridDataItem)e.Item;
                long userExclusionId = (long)ditem.GetDataKeyValue("Id");
                Dto.UserExclusion userExclusion = DomainApiClient<Dto.UserExclusion>.GetById(userExclusionId);
                if (userExclusion != null)
                    ShowDocument(userExclusion.FileName, userExclusion.AttachmentFile);
            }
            else if (e.CommandName == "ShowInfo")
            {
                GridDataItem item = e.Item as GridDataItem;
                long UserId = (long)item.GetDataKeyValue("UserId");
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", UserId.ToString());
                User User = DomainApiClient<Dto.User>.GetByAction<User>
                    (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                if (User == null)
                    return;
                NUserDetailUC.User = User;
                UserDetail.VisibleOnPageLoad = true;
            }
        }
        private void ShowDocument(string fileName, byte[] fileContent)
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

      

        protected void UpdatePanelDownload_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
     .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void UserExclutionGrid_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                ImageButton image = (ImageButton)item.FindControl("AttachmentImageButton");
                if (item["HasAttachments"].Text == Boolean.TrueString)
                {
                    image.Visible = true;
                }
                else
                    image.Visible = false;

                long userExclusionId = (long)item.GetDataKeyValue("Id");
                if (userExclusionId == 0)
                {
                    item.BackColor = System.Drawing.Color.IndianRed;
                    item.ForeColor = System.Drawing.Color.White;
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            UserDetail.VisibleOnPageLoad = false;
        }

        protected void UserExclutionGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = UserExclutionGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }
    }
}