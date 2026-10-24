using Application;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using DI.Chakra.Infrastructure;


namespace DI.Chakra.Web.Preferences
{
    public partial class Pref : UIBasePage
    {
        private SessionStatusImpl sessionStatus = new SessionStatusImpl();
        IList<Dto.QuickLink> QckLinkLst = new List<Dto.QuickLink>();
        long role_id = 0;
        string category = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Dto.Role role= DomainApiClient<Dto.Role>.GetById(sessionStatus.User.Roles.Select(y => y.Id).FirstOrDefault());
            //if (role != null)
            //{
            //    role_id = role.Id;
            //    Dictionary<String, string> parameter = new Dictionary<String, string>();
            //    parameter.Add("roleId", role_id.ToString());
            //    QckLinkLst = DomainApiClient<Dto.QuickLink>.GetByAction<List<Dto.QuickLink>>
            //        (DomainApiAction.Preference.QuickLink.QL.GetByRoleId, parameter);
            //    var temp = QckLinkLst.GroupBy(x => x.Category).Select(x => x.First());
            //    rptParent.DataSource = temp;
            //    rptParent.DataBind();
            //}
            //else
            //{
            //    return;
            //}

            // get all roles for user
            var rolesList = sessionStatus.User.Roles;

            if (rolesList != null)
            {
                // iterate the roles
                foreach (var role in rolesList)
                {
                    if (role != null)
                    {
                        role_id = role.Id;
                        Dictionary<String, string> parameter = new Dictionary<String, string>();
                        parameter.Add("roleId", role_id.ToString());
                        // get the quicklinks for this role
                        var qLinksForRole = DomainApiClient<Dto.QuickLink>.GetByAction<List<Dto.QuickLink>>
                            (DomainApiAction.Preference.QuickLink.QL.GetByRoleId, parameter);
                        if (qLinksForRole != null)
                        {
                            // iterate all the quicklinks for this role
                            foreach (var qLink in qLinksForRole)
                            {
                                // if not already added, add the quick link
                                if (!QckLinkLst.Any(q => q.Id == qLink.Id))
                                {
                                    QckLinkLst.Add(qLink);
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                var temp = QckLinkLst.GroupBy(x => x.Category).Select(x => x.First());
                rptParent.DataSource = temp;
                rptParent.DataBind();
            }
        }

        protected void rptParent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                dynamic listitem = e.Item.DataItem as dynamic;
                category = listitem.Category;
                Repeater childRepeater = (Repeater)e.Item.FindControl("rptChild");
                childRepeater.DataSource = QckLinkLst.Where(it => it.Category == category);
                childRepeater.DataBind();
            }
        }

        protected void lnkItem_Click(object sender, EventArgs e)
        {
            if (Session["QuickLink"].ToString() == "DefaultRoleQuickLink")
            {
                MasterPage.ShowAlert(AlertType.Error, "Maximum 5 quick links allowed delete some item");
            }
            else
            {
                LinkButton lbtn = (LinkButton)sender;
                string id = lbtn.CommandArgument;
                Dictionary<String, string> parameter = new Dictionary<String, string>();
                parameter.Add("id", id);
                var quckid = DomainApiClient<Dto.QuickLink>.GetByAction<Dto.QuickLink>
                    (DomainApiAction.Preference.QuickLink.QL.GetByName, parameter);
                if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
                {
                    Dto.UserQuickLink selectedmenu = new Dto.UserQuickLink();
                    selectedmenu.QuickLinkId = quckid.Id;
                    selectedmenu.Name = quckid.Name;
                    selectedmenu.UserId = sessionStatus.User.Id;
                    selectedmenu.Position = selectedmenu.Position;

                    Dictionary<String, string> qparameter = new Dictionary<String, string>();
                    qparameter.Add("userId", sessionStatus.User.Id.ToString());
                    List<Dto.UserQuickLink> linkcount = DomainApiClient<Dto.UserQuickLink>.
                        GetByAction<List<Dto.UserQuickLink>>(DomainApiAction.Preference.UserQuickLink.GetByUserId, qparameter);

                    if (linkcount.Count < 5)
                    {
                        DomainApiClient<Dto.UserQuickLink>.Save(selectedmenu);

                        if (sessionStatus.User != null && sessionStatus.User.Roles.Count > 0)
                        {
                            Dictionary<string, string> para = new Dictionary<string, string>();
                            para.Add("userId", sessionStatus.User.Id.ToString());
                            IList<Dto.UserPreference> menus =
                                DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                                (DomainApiAction.Preference.UserPreference.GetUserPreferenceMenus, para);

                            if (menus.Count > 0)
                            {
                                Session["QuickNav"] = menus;
                                Session["QuickLink"] = "UserQuickLink";
                            }
                            else
                            {
                                Dictionary<string, string> paramtr = new Dictionary<string, string>();
                                paramtr.Add("roleId", sessionStatus.User.Roles[0].Id.ToString());
                                menus = DomainApiClient<Dto.UserPreference>.GetByAction<IList<Dto.UserPreference>>
                                          (DomainApiAction.Preference.UserPreference.GetDefaultMenus, paramtr);

                                Session["QuickNav"] = menus;
                                Session["QuickLink"] = "DefaultRoleQuickLink";
                                if (menus == null || menus.Count < 5)
                                {
                                    Session["QuickLink"] = "UserQuickLink";
                                }
                            }
                        }
                        Response.Redirect("~/Preferences/pref.aspx");
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Maximum 5 quick links allowed");
                    }
                }
            }
        }

        protected void rptChild_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                IList<Dto.UserPreference> menus1;
                LinkButton GetLnkButtonItem = (LinkButton)e.Item.FindControl("lnkitem");
                Dictionary<String, string> parameter = new Dictionary<String, string>();
                parameter.Add("userId", sessionStatus.User.Id.ToString());
                menus1 = DomainApiClient<Dto.UserPreference>.GetByAction<List<Dto.UserPreference>>
                    (DomainApiAction.Preference.UserPreference.GetUserPreferenceMenus, parameter);
                if (menus1.Count == 0)
                {
                    Dictionary<String, string> parameters = new Dictionary<String, string>();
                    parameters.Add("roleId", role_id.ToString());
                    menus1 = DomainApiClient<Dto.UserPreference>.GetByAction<List<Dto.UserPreference>>
                        (DomainApiAction.Preference.UserPreference.GetDefaultMenus, parameters);
                }
                menus1 = menus1.Where(m => m.MenuName == GetLnkButtonItem.CommandArgument).ToList();
                if (menus1.Count > 0)
                {
                    GetLnkButtonItem.Enabled = false;
                }
                else
                {
                    GetLnkButtonItem.Enabled = true;
                }
            }
        }
    }
}

