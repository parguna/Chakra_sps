using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.Admin.House_Keeping.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        private const string V_ISEDIT = "ISEDIT";
        private const string V_VIEWONLY = "VIEWONLY";
        SessionStatusImpl sessionStatus = new SessionStatusImpl();

        public bool IsEdit
        {
            get
            {
                if (ViewState[V_ISEDIT] == null)
                    return false;
                return (bool)ViewState[V_ISEDIT];
            }
            set
            {
                ViewState[V_ISEDIT] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                PurgeConfig.DataBind();
            }
        }

        private void bindpurgeconfig()
        {
            PurgeConfig.DataSource = DomainApiClient<PurgeConfig>.GetAll();
        }

        protected void PurgeConfig_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            bindpurgeconfig();
        }

        protected void btnCancel_Click1(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/HouseKeeping/List.aspx");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "IDAS Housekeeping - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                DetailUC.Clearvalue();
                DetailUC.update_btn = "Add";
                DetailUC.header = "Add Purge Configuration";
                IsEdit = false;
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!ContextObjectId.HasValue || ContextObjectId == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                    return;
                }
                DetailUC.update_btn = "Update";
                DetailUC.header = "Edit Purge Configuration";
                IsEdit = true;
                SetPage();
                modalPopup.VisibleOnPageLoad = true;
            }
        }

        private void SetPage()
        {
            var Config = DomainApiClient<PurgeConfig>.GetById(this.ContextObjectId.Value);

            if (null == Config)
                return;
            DetailUC.TableName = Config.TableName;
            //DetailUC.ObjectName = Config.ObjectName;
            DetailUC.Description = Config.Description;
            DetailUC.DaysToKeep = (int)Config.DaysToKeep;
            DetailUC.PurgeModeConfig = Convert.ToInt32((PurgeMode)Enum.Parse(typeof(PurgeMode), Config.PurgeMode.ToString()));
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select a Value.");
                return;
            }
            DomainApiClient<PurgeConfig>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Value Deleted Successfully");
            PurgeConfig.Rebind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;            
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Save();
            PurgeConfig.Rebind();
            //MasterPage.ShowAlert(AlertType.Success, "Saved successfully");
        }
        private void Save()
        {
            string errorMessage = "";
            bool toSave = false;
            string obj = (DetailUC.TableName == "--Select--" && DetailUC.DaysToKeep <= 0 && DetailUC.PurgeModeConfig == 0) ?
                      "ALL" : (DetailUC.TableName == "--Select--" ? "TableName" : 
                      ((DetailUC.DaysToKeep <= 0) ? "DaysToKeep" : 
                      (DetailUC.PurgeModeConfig == 0 ? "PurgeModeConfig" : String.Empty)));

            switch (obj)
            {
                case "TableName":
                    errorMessage = "Table Name should not be Empty";
                    toSave = false;
                    break;
                //case "ObjectName":
                //    errorMessage = "Object Name should not be Empty";
                //    toSave = false;
                //    break;
                case "DaysToKeep":
                    errorMessage = "Days To Keep should not be Empty";
                    toSave = false;
                    break;
                case "PurgeModeConfig":
                    errorMessage = "Purge Mode should not be Empty";
                    toSave = false;
                    break;
                case "ALL":
                    modalPopup.VisibleOnPageLoad = false;
                    errorMessage = "Table Name, Days To Keep and Purge Mode should not be empty.";
                    toSave = false;
                    break;
                default:
                    toSave = true;
                    break;
            }

            string strTB = DetailUC.TableName;
            if (DoesExists(strTB.Trim(), (this.ContextObjectId.HasValue ? this.ContextObjectId.Value : 0)))
            {
                errorMessage = "Table Name Already Exists";
                toSave = false;
            }

            if (toSave)
            {
                //string strOB = DetailUC.ObjectName;
                string strDESC = DetailUC.Description;
                int intDTK = DetailUC.DaysToKeep;
                string strPM = DetailUC.PurgeModeConfig.ToString();
                strPM = Convert.ToString(Convert.ToInt32(strPM) - 1);

                PurgeConfig purgeConfig = new PurgeConfig();
                purgeConfig.Id = this.ContextObjectId.HasValue ? Convert.ToInt64(this.ContextObjectId.Value) : 0;
                purgeConfig.TableName = strTB.Trim();
                //purgeConfig.ObjectName = strOB.Trim();
                purgeConfig.Description = strDESC.Trim();
                purgeConfig.DaysToKeep = intDTK;
                purgeConfig.PurgeMode = (PurgeMode)Enum.Parse(typeof(PurgeMode), strPM);
                if(purgeConfig.Id > 0)
                {
                    var oldObj = DomainApiClient<PurgeConfig>.GetById(purgeConfig.Id);
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Housekeeping - Old value: TableName: {oldObj.TableName} PurgeMode:{oldObj.PurgeMode} DaysToKeep: {oldObj.DaysToKeep} Description:{oldObj.Description} ");
                }
                DomainApiClient<PurgeConfig>.Save(purgeConfig);
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"Housekeeping - New value: TableName: {purgeConfig.TableName} PurgeMode:{purgeConfig.PurgeMode} DaysToKeep: {purgeConfig.DaysToKeep} Description:{purgeConfig.Description} ");
                Response.Redirect("~/Admin/HouseKeeping/List.aspx");
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, errorMessage);
            }
        }

        protected void PurgeConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = PurgeConfig.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        private bool DoesExists(string strTB, long purgeID)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("tableName", strTB);
            parameter.Add("purgeID", purgeID.ToString());
            return (DomainApiClient<Dto.PurgeConfig>.GetByAction<bool>
                (DomainApiAction.HouseKeeping.PurgeConfig.DoesExists, parameter));
        }
    }
}