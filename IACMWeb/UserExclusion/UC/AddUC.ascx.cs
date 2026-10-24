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
    public partial class AddUC : UIBaseUC
    {
        List<Dto.Presentation.PresentationUserExclusion> UserExtList;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                //addemployeeEventList = new List<EmployeeEventTrigger>();
                //deleteemployeeEventList = new List<EmployeeEventTrigger>();
                // UserExtList.ForEach(x => x.HasAttachments = x.AttachmentFile.Count() > 0 ? true : false);

                BindEventType();
                EffectiveDate.MinDate = DateTime.Now;
            }
        }
        private void BindEventType()
        {
            ddlEventType.Items.Clear();
            Dto.IConfigurationMaster eventTypeList = GetConfigurationMaster("UserExclusionEventTypeList", Product.IM);
            List<string> eventTypeIds = eventTypeList.ConfigValue.Split(',').ToList();

            foreach (EmployeeEvents item in Enum.GetValues(typeof(EmployeeEvents)))
            {
                if (eventTypeIds.Any(i => Convert.ToInt32(i) == (int)item))
                {
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    ddlEventType.Items.Add(listItem);
                }
            }
            ddlEventType.Items.Insert(0, new ListItem("--Select--", ""));
        }
        private Dto.IConfigurationMaster GetConfigurationMaster(string ConfigKey, Product product)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("configKey", ConfigKey);
            parameter.Add("product", product.ToString());
            Dto.IConfigurationMaster config = DomainApiClient<Dto.ConfigurationMaster>.GetByAction<Dto.IConfigurationMaster>
                (DomainApiAction.Configuration.ConfigurationMaster.GetByConfigKey, parameter);
            return config;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "User Exclusion - Save");
                Save();
            }
        }

        private void Save()
        {
            UserExtList = DomainApiClient<Dto.Presentation.PresentationUserExclusion>.GetByAction<List<Dto.Presentation.PresentationUserExclusion>>(DomainApiAction.PresentationUserExclusion.GetAllUserExclusion, null);
            var selectedUser = UserSearchUC.SelectedUser;
            if (selectedUser != null && !(string.IsNullOrEmpty(ddlEventType.SelectedValue.ToString())) && EffectiveDate.SelectedDate != null && !string.IsNullOrEmpty(txtComments.Text) && AsyncUploadAttach.UploadedFiles.Count > 0)
            {
                //isdelete true is active status
                if (!UserExtList.Any(m => m.UserId == selectedUser.Id && m.IsDeleted && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString()))
                {
                    Dto.UserExclusion userExclusion = new Dto.UserExclusion();
                    userExclusion.UserId = UserSearchUC.SelectedUser.Id;
                    userExclusion.UserName = UserSearchUC.SelectedUser.UserName;
                    userExclusion.EventType = (EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue);
                    userExclusion.EffectiveDate = EffectiveDate.SelectedDate;
                    userExclusion.Remarks = txtComments.Text;
                    if (AsyncUploadAttach.UploadedFiles.Count > 0)
                    {
                        Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
                        byte[] fileData = new byte[file.ContentLength];
                        file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                        userExclusion.FileName = file.FileName;
                        userExclusion.AttachmentFile = fileData;
                        userExclusion.ContentType = file.ContentType;
                        userExclusion.HasAttachments = true;
                    }


                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", userExclusion.UserId.ToString());
                    parameter.Add("evnt", userExclusion.EventType.ToString());
                    var tmpemployeeEventList = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<EmployeeEventTrigger>>(DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);
                    if (tmpemployeeEventList.Count > 0)
                    {
                        foreach (Dto.EmployeeEventTrigger employeetrigger in tmpemployeeEventList)
                        {
                            if (employeetrigger.OldEffectiveDate.HasValue && userExclusion.EffectiveDate.HasValue &&
                                 userExclusion.EffectiveDate.Value > employeetrigger.OldEffectiveDate.Value)
                            {
                                employeetrigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                            }
                            else if (employeetrigger.OldEffectiveDate.HasValue)
                            {
                                employeetrigger.EffectiveDate = employeetrigger.OldEffectiveDate.Value;
                            }
                            Dictionary<string, string> parameters;
                            parameters = new Dictionary<string, string>();
                            parameters.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                            DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, employeetrigger);
                            //addemployeeEventList.Add(employeetrigger);
                        }
                    }
                    DomainApiClient<Dto.UserExclusion>.Save(userExclusion);
                }
                else if (UserExtList.Any(m => m.UserId == selectedUser.Id && m.IsDeleted && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString() && m.EffectiveDate != EffectiveDate.SelectedDate))
                {
                    Dto.Presentation.PresentationUserExclusion existingUser = UserExtList.Where(m => m.UserId == selectedUser.Id && m.IsDeleted && m.EventType == ((EmployeeEvents)Convert.ToInt32(ddlEventType.SelectedValue)).ToString()).FirstOrDefault();
                    if (existingUser != null)
                    {
                        Dto.UserExclusion existingUserExclustion = DomainApiClient<Dto.UserExclusion>.GetById(existingUser.Id);

                        existingUserExclustion.EffectiveDate = EffectiveDate.SelectedDate;
                        existingUserExclustion.Remarks = txtComments.Text;
                        if (AsyncUploadAttach.UploadedFiles.Count > 0)
                        {
                            Telerik.Web.UI.UploadedFile file = AsyncUploadAttach.UploadedFiles[0];
                            byte[] fileData = new byte[file.ContentLength];
                            file.InputStream.Read(fileData, 0, Convert.ToInt32(file.ContentLength));
                            existingUserExclustion.FileName = file.FileName;
                            existingUserExclustion.AttachmentFile = fileData;
                            existingUserExclustion.ContentType = file.ContentType;
                            existingUserExclustion.HasAttachments = true;
                        }


                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("userId", existingUserExclustion.UserId.ToString());
                        parameter.Add("evnt", existingUserExclustion.EventType.ToString());
                        var tmpemployeeEventList = DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<List<EmployeeEventTrigger>>(DomainApiAction.Provisioning.EmployeeEventTrigger.GetTriggerToExecute, parameter);
                        if (tmpemployeeEventList.Count > 0)
                        {
                            foreach (Dto.EmployeeEventTrigger employeetrigger in tmpemployeeEventList)
                            {
                                if (employeetrigger.OldEffectiveDate.HasValue && existingUserExclustion.EffectiveDate.HasValue &&
                                     existingUserExclustion.EffectiveDate.Value > employeetrigger.OldEffectiveDate.Value)
                                {
                                    employeetrigger.EffectiveDate = existingUserExclustion.EffectiveDate.Value;
                                }
                                else if (employeetrigger.OldEffectiveDate.HasValue)
                                {
                                    employeetrigger.EffectiveDate = employeetrigger.OldEffectiveDate.Value;
                                }

                                Dictionary<string, string> parameters;
                                parameters = new Dictionary<string, string>();
                                parameters.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                                DomainApiClient<Dto.EmployeeEventTrigger>.GetByAction<bool>("UdpateEventTrigger", parameters, employeetrigger);
                                // addemployeeEventList.Add(employeetrigger);
                            }
                        }
                        DomainApiClient<Dto.UserExclusion>.Save(existingUserExclustion);
                    }
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "User Exclusion already exists.");
                    return;
                }
                txtComments.Text = "";
            }
            else
            {
                string attchmsg = AsyncUploadAttach.UploadedFiles.Count > 0 ? " Reattach the file." : "";
                MasterPage.ShowAlert(AlertType.Error, "Required fields should not be empty." + attchmsg);
                return;
            }
            UserSearchUC.Clear();
            ddlEventType.ClearSelection();
            EffectiveDate.Clear();
            MasterPage.ShowAlert(AlertType.Success, "User Exclusion Added Successfuly");
            //Response.Redirect("~/UserExclusion/List.aspx");
        }
    }
}