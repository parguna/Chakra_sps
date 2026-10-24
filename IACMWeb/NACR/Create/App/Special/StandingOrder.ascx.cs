using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.NACR.Create.App.Special
{
    public partial class StandingOrder : UIBaseUC
    {
        private const string V_SHOWENDPOINT = "V_SHOWENDPOINT";        
        public bool ShowEndpoint
        {
            get
            {
                if (ViewState[V_SHOWENDPOINT] == null)
                    return false;
                return (bool)ViewState[V_SHOWENDPOINT];
            }
            set
            {
                ViewState[V_SHOWENDPOINT] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void BindApplicationType()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("ignoreTrustedSource", Boolean.FalseString);
            var endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<Dictionary<long, string>>
                (DomainApiAction.Endpoint.EndPoint.GetByTrustedSource, parameter);

            this.ApplicationTypeDropDownList.DataValueField = "Key";
            this.ApplicationTypeDropDownList.DataTextField = "Value";
            this.ApplicationTypeDropDownList.DataSource = endpoints;
            this.ApplicationTypeDropDownList.DataBind();
        }
        public void Initilize()
        {
            if (ShowEndpoint)
                selectEndpoint.Visible = true;
            else
                selectEndpoint.Visible = false;
            BindApplicationType();
            SODescription.Text = "";
        }

        public bool Validate(out string Message)
        {
            bool valid = true;
            Message = "";

            if (txtReferenceNumber.Text.Trim() == string.Empty)
            {
                valid = false;
                Message = "Please enter description";
            }

            if (!DatePickerValidTo.SelectedDate.HasValue)
            {
                valid = false;
                Message = "Please enter approved date";
            }

            if (SODescription.Text.Trim() == string.Empty)
            {
                valid = false;
                Message += "Please enter description";
            }

            return valid;
        }

        public string GetDetail(string EndPointName,long endpointSystemId, string EndPointCode)
        {
            dynamic detail = new ExpandoObject();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemId", endpointSystemId.ToString());
            string lastSequenceNumber = DomainApiClient<Dto.AccessRequest>.GetByAction<string>("GetLastSequenceNumber", parameter);
            string runningNumber = string.Format((int.Parse(lastSequenceNumber) + 1).ToString().PadLeft(3, '0'));
            
            string number = string.Format("{0}{1}-{2}", EndPointCode, DateTime.Now.Year, runningNumber.ToString());
            ExpandoHelper.AddProperty(detail, "SequenceNumber", number);
            

            ExpandoHelper.AddProperty(detail, "SubsystemName", ApplicationTypeDropDownList.SelectedItem.Text);
            ExpandoHelper.AddProperty(detail, "Description", SODescription.Text);
            ExpandoHelper.AddProperty(detail, "ReferenceNumber", txtReferenceNumber.Text);
            ExpandoHelper.AddProperty(detail, "ApprovedDate", DatePickerValidTo.SelectedDate.Value.ToShortDateString());

            return JsonConvert.SerializeObject(detail);

        }
    }
}