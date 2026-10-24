using AjaxControlToolkit;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.UsrControl
{
    public partial class EnterprisePasswordPolicy : UIBaseUC
    {
        #region Private Members
        PasswordPolicy passwordPolicy = null;
        AccordionPane aPane = new AccordionPane();
        private long _endPointsSystemId = -1;
        #endregion
        public long EndPointSystemID
        {
            get { return _endPointsSystemId; }
            set { _endPointsSystemId = value; }
        }

        public string Title
        {
            set { myModalLabel.InnerText = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // BindPolicy();
        }
        public void BindPolicy()
        {
            long ctrlID = 1;
            if (this.EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", this.EndPointSystemID.ToString());
                passwordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetByAction<PasswordPolicy>(DomainApiAction.Configuration.PasswordPolicy.GetByEndpointSystemId, parameter);
            }
            else if(this.EndPointSystemID == 0)
            {
                passwordPolicy = DomainApiClient<Dto.PasswordPolicy>.GetById(1);
            }
            if (passwordPolicy == null)
                return;

            //accordion.Panes.Clear();            
            //aPane = new AccordionPane();
            //aPane.ID = "Pane_AD_" + ctrlID.ToString();

            //DI.Chakra.Infrastructure.Enums.EndpointType _endpointSystemTypeName = pmFacade.GetEndPointTypeName(policy.EndpointSystemTypeId);
            //PasswordPolicy = LoadControl("~/UserControls/PasswordPolicy.ascx") as UserControls_PasswordPolicy;
            //PasswordPolicy.ID = "ADEndPointInfo" + ctrlID.ToString();
            PasswordPolicy1.GetPolicy(passwordPolicy.Id);
            //aPane.ContentContainer.Controls.Add(PasswordPolicy);
            //accordion.Panes.Add(aPane);
        }

    }
}