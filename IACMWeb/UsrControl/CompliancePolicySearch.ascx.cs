using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl
{
    public partial class CompliancePolicySearch : DI.Chakra.Web.UIBaseUC
    {

        private CompliancePolicy _policy = null;
        public CompliancePolicy SelectedPolicy
        {
            get
            {
                if (PolicyAutoCompleteBox.Entries.Count <= 0)
                    return null;

                _policy = DomainApiClient<CompliancePolicy>.GetById(Convert.ToInt64(PolicyAutoCompleteBox.Entries[0].Value));

                return _policy;
            }
            set
            {
                _policy = value;
                if (_policy != null)
                    PolicyAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_policy.CompliancePolicyCode + " - " + _policy.CompliancePolicyDescription, _policy.Id.ToString()));
            }
        }

        public void Clear()
        {
            PolicyAutoCompleteBox.Entries.Clear();
        }
    }
}