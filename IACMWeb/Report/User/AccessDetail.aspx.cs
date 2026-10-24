using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DI.Chakra.Infrastructure;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using Application;
using System.Web.Services;

namespace DI.Chakra.Web.Report.User
{
    public partial class AccessDetail : DI.Chakra.Web.UIBasePage
    {
        private bool ExportMode;
        private bool ExportTCode;
        private static List<Dto.User> usrList;
        private static string selectedInstitute;
        private static long selectedDept;
        private bool ShowAll = false;
        private List<SAPTCodeInfo> sapTCodes = new List<SAPTCodeInfo>();
        private bool Flag = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ExportMode = false;
                ExportTCode = false;
                selectedInstitute = string.Empty;
                selectedDept = -1;
                long UserId = new SessionStatusImpl().User.Id;
                bool IsAdminUser = false, isIacmAdminUser = false;
            }

            if (ViewState["ShowAll"] != null)
            {
                ShowAll = (bool)ViewState["ShowAll"];
            }
            GetDeptID();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CompanyValidator.Visible = false;
            ShowAll = false;
            ViewState["ShowAll"] = ShowAll;
            ViewState["UserAccessDetails"] = null;
            if (UserAutoCompleteBox.Entries.Count > 0)
            {
                long userId = 0;
                bool cndn = long.TryParse(UserAutoCompleteBox.Entries[0].Value, out userId);

                if (cndn == true && userId > 1)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("userId", userId.ToString());
                    var list = DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
                        GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
                        (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, parameter);

                    var finallist = list.Where(lq => lq.ParentId < 0 || lq.ParentId == 0).Select(lq => lq).ToList();
                    UserRadGridTreeList.DataSource = finallist;
                    UserRadGridTreeList.DataBind();

                    ExportMode = false;
                    ExportTCode = false;
                    lblMsg.Text = string.Empty;
                }
            }
        }

        protected void btnShowALL_Click(object sender, EventArgs e)
        {
            ShowAll = true;
            ViewState["ShowAll"] = ShowAll;
            ViewState["UserAccessDetails"] = null;
            UserRadGridTreeList.DataSource = GetAccessDetailsForAllUsers();
            UserRadGridTreeList.DataBind();

            ExportMode = false;
            ExportTCode = false;
            lblMsg.Text = string.Empty;
        }

        private IList<Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> getLevel2DetailsForAuthorization(string paramSingleRoleCode, long paramEndPointSystemId)
        {
            return new List<Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>();
        }

        public List<Dto.Presentation.PresentationUserAccessDetail> GetAccessDetailsForAllUsers()
        {
            CompanyValidator.Visible = true;
            CompanyValidator.Validate();
            if (CompanyValidator.IsValid)
            {
                CompanyValidator.Visible = false;
                List<long> CompanyId = new List<long>();
                List<long> DepartmentId = new List<long>();
                List<long> DivisionId = new List<long>();
                List<long> OrganisationId = new List<long>();
                List<string> SectionId = new List<string>();

                if (HttpContext.Current.Session["CompanyID"] != null)
                {
                    CompanyId = (List<long>)(HttpContext.Current.Session["CompanyID"]);
                }
                if (HttpContext.Current.Session["DepartmentId"] != null)
                {
                    DepartmentId = (List<long>)(HttpContext.Current.Session["DepartmentId"]);
                }
                if (HttpContext.Current.Session["DivisionId"] != null)
                {
                    DivisionId = (List<long>)(HttpContext.Current.Session["DivisionId"]);
                }

                if (HttpContext.Current.Session["SectionId"] != null)
                {
                    SectionId = (List<string>)(HttpContext.Current.Session["SectionId"]);
                }
                long userId = new SessionStatusImpl().User.Id;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", userId.ToString());
                parameter.Add("searchString", "");
                parameter.Add("DepartmentId", DepartmentId.ToString());
                parameter.Add("DivisionId", DivisionId.ToString());
                parameter.Add("OrganisationId", OrganisationId.ToString());
                parameter.Add("SectionId", SectionId.ToString());
                Dto.Presentation.PresentationReportingEmployees presentationReportingEmployees = new Dto.Presentation.PresentationReportingEmployees()
                {
                    UserID = userId,
                    SearchString = "",
                    DepartmentId = DepartmentId,
                    DivisionId = DivisionId,
                    OrganisationId = OrganisationId,
                    SectionCode = SectionId

                };


                Dictionary<long, string> ReportingUsers =
                    DomainApiClient<Dto.User>.GetByAction<Dictionary<long, string>>(DomainApiAction.User.GetReportingEmployees, null, presentationReportingEmployees);

                List<Dto.Presentation.PresentationUserAccessDetail> UserAccessDetails = new List<Dto.Presentation.PresentationUserAccessDetail>();

                foreach (KeyValuePair<long, string> item in ReportingUsers)
                {
                    userId = item.Key;
                    if (userId > 1)
                    {
                        Dictionary<string, string> param = new Dictionary<string, string>();
                        param.Add("userId", userId.ToString());
                        var usrAccessList = DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
                            GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
                            (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, param);
                        UserAccessDetails = UserAccessDetails.Union(usrAccessList).ToList();
                    }
                }
                var finallist = UserAccessDetails.Where(lq => lq.ParentId < 0 || lq.ParentId == 0).Select(lq => lq).ToList();
                ViewState["UserAccessDetails"] = finallist;
                return finallist;
            }
            else
            {
                return new List<Dto.Presentation.PresentationUserAccessDetail>();
            }
        }

        private void GetDeptID()
        {
            List<long> DeptID = new List<long>();
            if (autoCompleteDepartment.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteDepartment.Entries)
                {
                    DeptID.Add(Convert.ToInt64(item.Value));
                }
                Session["DepartmentId"] = DeptID;
            }
            else
                Session["DepartmentId"] = null;
        }

        private void GetCompanyID()
        {
            List<long> CompanyID = new List<long>();
            if (autoCompleteCompany.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteCompany.Entries)
                {
                    CompanyID.Add(Convert.ToInt64(item.Value));
                }
                Session["CompanyID"] = CompanyID;
            }
            else
                Session["CompanyID"] = null;
        }


        private void GetSectionID()
        {
            List<string> SectionId = new List<string>();
            if (autoCompleteSection.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteSection.Entries)
                {
                    SectionId.Add(item.Value);
                }
                Session["SectionId"] = SectionId;
            }
            else
                Session["SectionId"] = null;
        }

        private void GetDivisionID()
        {
            List<long> DivisionId = new List<long>();
            if (autoCompleteDivision.Entries.Count > 0)
            {
                foreach (AutoCompleteBoxEntry item in autoCompleteDivision.Entries)
                {
                    DivisionId.Add(Convert.ToInt64(item.Value));
                }
                Session["DivisionId"] = DivisionId;
            }
            else
                Session["DivisionId"] = null;
        }

        [WebMethod]
        public static List<string> GetUser(string prefixText)
        {
            List<string> Users = new List<string>();
            int listMaxCnt = 0;
            if (usrList != null && usrList.Count > 0)
            {
                var query = from usr in usrList
                            where usr.UserName.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase)
                                && (string.IsNullOrEmpty(selectedInstitute))
                                && (usr.NOrganisationId == selectedDept || selectedDept == -1)
                                && listMaxCnt++ < 26
                            select new
                            {
                                Usernames = usr.UserName
                            };
                foreach (var u in query)
                {
                    Users.Add(u.Usernames.ToString());
                }
            }
            return Users;
        }

        protected void UserRadGridTreeList_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            long EndpointSystemID;
            EndPoint endPoint = null;
            switch (e.DetailTableView.Name)
            {
                case "Roles":
                    long parentID = (long)dataItem.GetDataKeyValue("Id");
                    string username = dataItem["userName"].Text;

                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("username", username);
                    Dto.User usr = DomainApiClient<Dto.User>.GetByAction<Dto.User>(DomainApiAction.User.GetByUserName, param);

                    if (usr != null)
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("userId", usr.Id.ToString());
                        var list = DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
                            GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
                            (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, parameter);

                        var finallist = list.Where(lq => lq.ParentId == parentID).Select(lq => lq).ToList();
                        e.DetailTableView.DataSource = finallist;
                        string rolesorgroup = finallist.Where(lq => lq.Type == "Role").Count() > 0 ? "Role" : "Group";

                        if (finallist.Count == 0) rolesorgroup = "Role / Group";
                        e.DetailTableView.Columns.FindByUniqueNameSafe("Name").HeaderText = rolesorgroup;
                    }

                    break;
                case "TCode":

                    EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");

                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add("EndpointSystemID", EndpointSystemID.ToString());
                    endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>
                        (DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, para);

                    if (endPoint != null && endPoint.EndpointSystemTypeId == (int)EndpointType.SAP)
                    {
                        if (ExportMode && !ExportTCode)
                        {
                            e.DetailTableView.Visible = false;
                            sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization", EndPointSystemId = EndpointSystemID, Role = "" });
                            e.DetailTableView.DataSource = sapTCodes;
                            e.DetailTableView.Visible = false;
                        }
                        else
                        {
                            string rolecode = (string)dataItem.GetDataKeyValue("Name");
                            EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");

                            Dictionary<string, string> parameter = new Dictionary<string, string>();
                            parameter.Add("rolecode", rolecode.ToString());
                            parameter.Add("EndpointSystemID", EndpointSystemID.ToString());
                            List<SAPTCodeInfo> sapTCodes = DomainApiClient<Dto.SAPTCodeInfo>.GetByAction<List<Dto.SAPTCodeInfo>>
                                (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetByRoleCodeIncludingRoles, parameter);

                            sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization", EndPointSystemId = EndpointSystemID, Role = rolecode });
                            e.DetailTableView.DataSource = sapTCodes;
                        }
                    }
                    else
                    {
                        sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization", EndPointSystemId = EndpointSystemID, Role = "" });
                        e.DetailTableView.DataSource = sapTCodes;
                        e.DetailTableView.Visible = false;
                    }

                    break;

                case "Authorization":
                    if (dataItem.GetDataKeyValue("EndpointSystemID") != null && ExportMode == false)
                    {
                        EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");
                        Dictionary<string, string> paramtr = new Dictionary<string, string>();
                        paramtr.Add("EndpointSystemID", EndpointSystemID.ToString());
                        EndPoint endPointSystem = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>
                            (DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, paramtr);

                        if (endPointSystem != null && endPointSystem.EndpointSystemTypeId == (int)EndpointType.SAP && ExportMode == false)
                        {
                            string rolecode = (string)dataItem.GetDataKeyValue("Role");
                            string TCODE_Code = (string)dataItem.GetDataKeyValue("TCODE_Code");
                            if (TCODE_Code == "Authorization")
                            {
                                EndpointSystemID = (long)dataItem.GetDataKeyValue("EndpointSystemID");

                                IList<Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> sapLevel2Details = this.getLevel2DetailsForAuthorization(rolecode, EndpointSystemID);
                                var l1EndpointSystemsSAP = (from dtls in sapLevel2Details
                                                            select new
                                                            {
                                                                sapLevel1DetailId = dtls.sapLevel1DetailId,
                                                                sapLevel1Detail = dtls.sapLevel1Detail,
                                                                sapLevel1DetailDesc = dtls.sapLevel1DetailDesc,
                                                                IsChanged = false
                                                            });

                                e.DetailTableView.DataSource = l1EndpointSystemsSAP;
                            }
                            else
                            {
                                IList<Dto.Presentation.PresentationAttestationReviewPageSAP_Level1> emptyDataSource = new List<Dto.Presentation.PresentationAttestationReviewPageSAP_Level1>();
                                e.DetailTableView.DataSource = emptyDataSource;
                                e.DetailTableView.Visible = false;
                            }
                        }
                        else
                        {
                            e.DetailTableView.Visible = false;
                        }
                    }
                    break;
            }
        }

        protected void UserRadGridTreeList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (ExportMode) return;
            if (ShowAll == true)
            {
                if (ViewState["UserAccessDetails"] != null)
                    UserRadGridTreeList.DataSource = ViewState["UserAccessDetails"];
                else
                    UserRadGridTreeList.DataSource = this.GetAccessDetailsForAllUsers();
            }
            else if (UserAutoCompleteBox.Entries.Count > 0)
            {
                long userId = 0;
                bool cndn = long.TryParse(UserAutoCompleteBox.Entries[0].Value, out userId);

                if (cndn == true && userId > 1)
                {
                    Dictionary<string, string> para = new Dictionary<string, string>();
                    para.Add("userId", userId.ToString());
                    var list = DomainApiClient<Dto.Presentation.PresentationUserAccessDetail>.
                        GetByAction<List<Dto.Presentation.PresentationUserAccessDetail>>
                        (DomainApiAction.Endpoint.Presentation.PresentationUserAccessDetail.GetUserAccessDetail, para);

                    var finallist = list.Where(lq => lq.ParentId < 0 || lq.ParentId == 0).Select(lq => lq).ToList();
                    UserRadGridTreeList.DataSource = finallist;
                }
            }
        }

        protected void CompanyValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            long UserId = new SessionStatusImpl().User.Id;
            bool IsAdminUser = false, isIacmAdminUser = false;
        }
    }
}