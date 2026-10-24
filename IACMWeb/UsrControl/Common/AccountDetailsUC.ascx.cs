using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.UsrControl.Common
{


    public partial class AccountDetailsUC : System.Web.UI.UserControl
    {
        private const string V_ID = "ENTITY_ID";

        private class Items
        {
            public string ItemName { get; set; }
            public string ItemValue { get; set; }
        }
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return long.Parse(ViewState[V_ID].ToString());
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void Initialize()
        {
            IList<Items> items = new List<Items>();
            Dictionary<string, string> parameter = new Dictionary<string, string>();

            parameter.Add("accountReconId", this.EntityId.ToString());
            string s = DomainApiClient<AccountReconcilation>.GetByAction<string>(DomainApiAction.Provisioning.AccountReconcilation.GetEndpointUserInfoJson, parameter);


            dynamic detail = JsonConvert.DeserializeObject(s);
            parameter.Clear();
            parameter.Add("endpointSystemId", detail.EndPointSystemId.ToString());
            List<EndpointUserViewConfiguration> epConfigs = DomainApiClient<EndpointUserViewConfiguration>.GetByAction<List<EndpointUserViewConfiguration>>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointUserViewConfiguration.GetByEndpointSystemId, parameter);

            foreach (var item in epConfigs)
            {
                bool negate = !string.IsNullOrWhiteSpace(item.Value) ? item.Value.StartsWith("!") : false;
                string val = !string.IsNullOrWhiteSpace(item.Value) ? item.Value.TrimStart('!') : item.Value;
                var actualValue = string.Empty;
                actualValue = detail[val];
                if (!string.IsNullOrWhiteSpace(actualValue))
                {
                    if (actualValue.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualValue = "Yes";
                    }
                    else if (actualValue.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                    {
                        actualValue = "No";
                    }

                    if (negate)
                    {
                        actualValue = actualValue == "Yes" ? "No" : "Yes";
                    }
                }
                items.Add(new Items { ItemName = item.Caption, ItemValue = actualValue });
            }
            DetailRepeater.DataSource = items;
            DetailRepeater.DataBind();
        }
    }
}