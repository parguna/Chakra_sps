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
    public partial class RiskCodeUC : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private SODRiskCode _riskCode = null;
        public SODRiskCode SelectedRiskCode
        {
            get
            {
                if (RiskCodeAutobox.Entries.Count <= 0)
                    return null;
                _riskCode = DomainApiClient<SODRiskCode>.GetById(Convert.ToInt32(RiskCodeAutobox.Entries[0].Value));
                return _riskCode;
            }
            set
            {
                _riskCode = value;
                if (_riskCode != null)
                    RiskCodeAutobox.Entries.Add(new AutoCompleteBoxEntry(_riskCode.RiskCode + " - " + _riskCode.RiskDescription,_riskCode.RiskLevelId.ToString()));
            }
        }


        public void Clear()
        {
            RiskCodeAutobox.Entries.Clear();
        }
    }
}