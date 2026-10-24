using System;
using System.Linq;
using DI.Chakra.Dto;

using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace DI.Chakra.Web.Configuration.Role.UC
{
    public partial class ActionRightsUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private const string V_ACCESSRIGHTS = "ACCESSRIGHTS";
        public event UpdateAccessRight Update;
        public delegate void UpdateAccessRight(AccessRights AccessRights);
        #endregion

        #region Property
        public List<AccessRights> CurrentAccessRights
        {
            get
            {
                if (ViewState[V_ACCESSRIGHTS] == null)
                    return new List<AccessRights>();
                return (List<AccessRights>)ViewState[V_ACCESSRIGHTS];
            }
            set
            {
                ViewState[V_ACCESSRIGHTS] = value;
            }
        }
        #endregion

        #region Methods
        public void Initialize(long PageId)
        {
            BindActions(PageId);
        }
        private void BindActions(long PageId)
        {
            Permission permtn = new Permission();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("pageid", Convert.ToString(PageId));
            ActionRepeater.DataSource = DomainApiClient<Permission>.GetByAction<List<Permission>>(DomainApiAction.ACL.Permission.GetBypageId, parameter);
            ActionRepeater.DataBind();
        }
        #endregion

        #region Events
        protected void Action_CheckedChanged(object sender, EventArgs e)
        {
            IList<AccessRights> AR = new List<AccessRights>();
            AccessRights ExistRecord = new AccessRights();
            CheckBox Actions = (CheckBox)sender;
            ExistRecord = CurrentAccessRights.Where(lq => (lq.PagePermission.PermissionId == Convert.ToInt32(Actions.CssClass) || lq.PagePermission.PermissionId == -Convert.ToInt32(Actions.CssClass))).FirstOrDefault();
            long urlId = (ExistRecord.PagePermission.UrlId == null)? 0 : (long)ExistRecord.PagePermission.UrlId;

            AccessRights RightsFortheRole = null; PagePermission pagePermission = null;
            if (urlId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("urlId", urlId.ToString());
                //pagePermission = DomainApiClient<PagePermission>.GetByAction<PagePermission>(DomainApiAction.ACL.PagePermission.GetByUrlId, parameter);
            }
            AR = (this.Parent.Page as ChangeAccessRights).AccessRights;
            if (pagePermission != null)
                RightsFortheRole = AR.Where(l => (l.PagePermissionId == pagePermission.Id || l.PagePermissionId == -pagePermission.Id) && l.RoleId == ExistRecord.RoleId).FirstOrDefault();

            if (Actions.Checked)
            {
                ExistRecord.PagePermissionId = ExistRecord.PagePermissionId < 0 ? -ExistRecord.PagePermissionId : ExistRecord.PagePermissionId;
                if(pagePermission !=null && RightsFortheRole == null)
                    AR.Add(new AccessRights() { PagePermission = pagePermission, PagePermissionId = (int)-pagePermission.Id, RoleId = ExistRecord.RoleId });
            }
            else
            {
                ExistRecord.PagePermissionId = ExistRecord.PagePermissionId < 0 ? ExistRecord.PagePermissionId : -ExistRecord.PagePermissionId;
                if (pagePermission != null && RightsFortheRole != null)
                {
                    var obj = (from lst in AR
                           where lst.PagePermissionId == (int)-pagePermission.Id && lst.RoleId == ExistRecord.RoleId
                           select lst).FirstOrDefault();
                    AR.Remove(obj);
                }
            }
            Update(ExistRecord);
        }
        #endregion
    }
}