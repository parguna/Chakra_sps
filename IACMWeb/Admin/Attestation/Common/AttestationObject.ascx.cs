using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.Attestation.Common
{
    public partial class AttestationObject : UIBaseUC
    {
        public List<Dto.AttestationHierarchy> AttestationHierarchy = new List<Dto.AttestationHierarchy>();

        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;


        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (!IsPostBack)
            {
                
                
            }
        }

        public void BindRepeater()
        {
            long EndpointId = (long)Session["EndpointID"];
            var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", EndpointId);
            AttestationHierarchy = (List<Dto.AttestationHierarchy>)Session[sessionFlag];
            Dictionary<string, string> parameterList = new Dictionary<string, string>();
            parameterList.Add("EndpointSystemId", EndpointId.ToString());
            if(Session["SelectedAttestationType"] == null)
                parameterList.Add("attestationType", AttestationType.User.ToString());
            else
                parameterList.Add("attestationType", ((AttestationType)Session["SelectedAttestationType"]).ToString());
            List<Dto.EndpointHierarchy> endpointHierarchy = DomainApiClient<Dto.EndpointHierarchy>.
                                           GetByAction<List<Dto.EndpointHierarchy>>(DomainApiAction.Endpoint.EndPoint.GetEndpointHierarchyByEndpoint, parameterList);
            AttestationObjectRepeater.DataSource = AttestationHierarchy.Where(hierarchy => hierarchy.EndpointId == EndpointId).ToList(); ;
            AttestationObjectRepeater.DataBind();
            Session[sessionFlag] = AttestationHierarchy.Where(hierarchy => hierarchy.EndpointId == EndpointId).ToList();
            foreach (RepeaterItem item in AttestationObjectRepeater.Items)
            {
                DropDownList drpAction = (DropDownList)item.FindControl("defaultaction");
                Label lblObject = (Label)item.FindControl("lblObject");
                drpAction.DataSource = Enum.GetValues(typeof(AttestationDefaultReviewStatus)).Cast<AttestationDefaultReviewStatus>();
                drpAction.DataBind();
                Dto.AttestationHierarchy aHierarchy = AttestationHierarchy.FirstOrDefault(hierarchy => hierarchy.EndpointHierarchy.EntitlementType.ToString() == lblObject.Text);

                drpAction.SelectedValue = aHierarchy.DefaultAttestationAction.ToString();                
                Dto.EndpointHierarchy extendableSetting = endpointHierarchy.FirstOrDefault(hierarchy => hierarchy.EntitlementType.ToString() == lblObject.Text);
                CheckBox chkActionable = (CheckBox)item.FindControl("chkAction");
                if (extendableSetting == null || !extendableSetting.IsActionable)
                    chkActionable.Visible = false;
                CheckBox chkVisible = (CheckBox)item.FindControl("chkVisible");
                if (extendableSetting == null || !extendableSetting.IsActive)
                    chkVisible.Enabled = false;

                CheckBox chkExtendable = (CheckBox)item.FindControl("chkExtendable");
                if (chkExtendable != null )
                {
                    chkExtendable.Checked = aHierarchy.IsActionExtendable;
                    chkExtendable.Visible = chkActionable.Checked;
                }
                else
                {
                    chkExtendable.Visible = true;
                }
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
           
           
            if (Add_CancelButtonClicked != null)
            {
                Add_CancelButtonClicked(sender, e);
            }

            
        }

        

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            long EndpointId = (long)Session["EndpointID"];
            var sessionFlag = string.Format("AttestationHierarchy_EP_{0}", EndpointId);
            List<Dto.AttestationHierarchy> lstHierarchy = (List<Dto.AttestationHierarchy>)Session[sessionFlag];
            foreach (RepeaterItem item in AttestationObjectRepeater.Items)
            {
                CheckBox chkAction = (CheckBox) item.FindControl("chkAction");
                CheckBox chkVisible = (CheckBox) item.FindControl("chkVisible");
                CheckBox chkExtendable = (CheckBox) item.FindControl("chkExtendable");
                if(chkExtendable != null && (chkAction.Visible == false || chkAction.Checked == false))
                {
                    chkExtendable.Checked = false;
                }
                Label lblObject = (Label)item.FindControl("lblObject");
                Label lblLevel = (Label) item.FindControl("lblLevel");
                DropDownList drpAction = (DropDownList) item.FindControl("defaultaction");

                Dto.AttestationHierarchy hierarchy = lstHierarchy.FirstOrDefault(hier => hier.EndpointHierarchy.EntitlementType.ToString() == lblObject.Text && hier.EndpointHierarchy.Level == Convert.ToInt64(lblLevel.Text));
                hierarchy.IsActionAllowed = chkAction.Checked;
                hierarchy.IsActionExtendable = chkExtendable.Checked;
                hierarchy.IsVisible = chkVisible.Checked;
                hierarchy.DefaultAttestationAction = (AttestationDefaultReviewStatus)Enum.Parse(typeof(AttestationDefaultReviewStatus), drpAction.SelectedValue);
            }
            Session[sessionFlag] = lstHierarchy;
            if (Add_SubmitButtonClicked != null)
            {
                Add_SubmitButtonClicked(sender, e);
            }
        }

        protected void chkAction_CheckedChanged(object sender, EventArgs e)
        {
            var chkAction = sender as CheckBox;
            if(chkAction != null)
            {
                RepeaterItem item = chkAction.NamingContainer as RepeaterItem;
                if(item != null)
                {
                    CheckBox chkExtendable = item.FindControl("chkExtendable") as CheckBox;
                    CheckBox chkVisible    = item.FindControl("chkVisible") as CheckBox;
                    DropDownList ddldefaultaction = item.FindControl("defaultaction") as DropDownList;
                    if (chkExtendable != null)
                    {
                        chkExtendable.Visible = chkAction.Checked;
                        chkExtendable.Checked = false;
                        if (chkVisible.Checked == false)
                        {
                            chkVisible.Checked = chkAction.Checked;
                            ddldefaultaction.Visible = chkVisible.Checked;
                        }
                    }
                }
            }
        }

        protected void chkVisible_CheckedChanged(object sender, EventArgs e)
        {
            var chkvisible = sender as CheckBox;
            if (chkvisible != null)
            {
                RepeaterItem item = chkvisible.NamingContainer as RepeaterItem;
                if (item != null)
                {
                    CheckBox chkAction = item.FindControl("chkAction") as CheckBox;
                    CheckBox chkExtendable = item.FindControl("chkExtendable") as CheckBox;
                    DropDownList ddldefaultaction = item.FindControl("defaultaction") as DropDownList;
                    if (chkAction != null && chkExtendable != null)
                    {
                        ddldefaultaction.Visible = chkvisible.Checked;
                        if (chkAction.Checked)
                        {
                            chkAction.Checked = chkvisible.Checked;
                            chkExtendable.Visible = chkAction.Checked;
                            chkExtendable.Checked = false;
                        }
                    }
                }
            }
        }
    }
}