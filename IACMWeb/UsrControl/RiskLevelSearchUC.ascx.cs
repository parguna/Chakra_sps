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
    public partial class RiskLevelSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private SODRiskLevel riskLevel = null;
        public SODRiskLevel SelectedRiskLevel
        {
            get
            {
                if (RiskLevelAutoCompleteBox.Entries.Count <= 0)
                    return null;

                riskLevel = DomainApiClient<SODRiskLevel>.GetById(Convert.ToInt64(RiskLevelAutoCompleteBox.Entries[0].Value));

                return riskLevel;
            }
            set
            {
                riskLevel = value;
                Clear();
                if (riskLevel != null)
                    RiskLevelAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(riskLevel.RiskLevelCode + " - " + riskLevel.RiskLevelDescription, riskLevel.Id.ToString()));
            }
        }
      
      
        public void Clear()
        {
            RiskLevelAutoCompleteBox.Entries.Clear();
        }
    }
}