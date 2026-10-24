using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Web.Attestation.List;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Dashboard.Attestation
{
    public partial class AttestationTypeUC : UIBaseUC
    {
        protected void ddlAttestationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAttestation(string.Empty);

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAttestationType();
            }
        }

        public event EventHandler SelectedIndexChanged;

        public void LoadAttestationType()
        {
            var attestationType = DomainApiClient<AttestationEndpoint>.GetByAction<List<string>>(DomainApiAction.Attestation.AttestationEndpoint.GetAttestationEndpointsForActiveCampaign, null);


            if (attestationType != null && attestationType.Count > 0)
            {
                ddlAttestationType.DataSource = attestationType;
                ddlAttestationType.DataBind();
                ddlAttestationType.Items.Insert(0, "Select");
                ddlAttestationType.Items.FindByValue(attestationType[0]).Selected = true;
                LoadAttestation(attestationType[0]);
            }
            else
            {
               // MasterPage.ShowAlert(AlertType.Warning, "Attestation Not found!");
            }
        }
        private void LoadAttestation(string endPointName)
        {
            List<string> attestation;
            if (!string.IsNullOrEmpty(endPointName))
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endPointTypeName", endPointName);
                attestation = DomainApiClient<AttestationEndpoint>.GetByAction< List<string>>(DomainApiAction.Attestation.AttestationEndpoint.GetAttestationByEndpointName,parameter);
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                endPointName = ddlAttestationType.SelectedItem.Text.Trim();
                parameter.Add("endPointTypeName", endPointName);
                attestation = DomainApiClient<AttestationEndpoint>.GetByAction<List<string>>(DomainApiAction.Attestation.AttestationEndpoint.GetAttestationByEndpointName, parameter);
            }

            //atnFacade = new AttestationFacade();
            if (attestation != null && attestation.Count > 0)
            {
                ddlAttestation.DataSource = attestation;
                ddlAttestation.DataBind();
                ddlAttestation.Items.Insert(0, "Select");
                ddlAttestation.Items.FindByValue(attestation[0]).Selected = true;
                Session["AtnDashboardDefault"] = attestation[0];
                ddlAttestation.SelectedIndex = 1;
                ddlAttestation_SelectedIndexChanged(ddlAttestation, null);
            }
        }

        protected void ddlAttestation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(sender, e);
            }
        }
    }
}