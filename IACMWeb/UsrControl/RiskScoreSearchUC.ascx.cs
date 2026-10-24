using DI.Chakra.Dto;
using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl
{
    public partial class RiskScoreSearchUC : DI.Chakra.Web.UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private SODRiskScore _riskScore=null;

        public SODRiskScore SelectedRiskScore
        {
            get{
                if (RiskAutoCompleteBox.Entries.Count <= 0)
                    return null;
                _riskScore = DomainApiClient<Dto.SODRiskScore>.GetById(Convert.ToInt64(RiskAutoCompleteBox.Entries[0].Value));
                //SODRiskScore.GetById(Convert.ToInt64(RiskAutoCompleteBox.Entries[0].Value));

                return _riskScore;
            }
            set{
                _riskScore = value;
                Clear();
                if (_riskScore != null)
                    RiskAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_riskScore.RiskScore + " - " + _riskScore.RiskScoreDescription, _riskScore.Id.ToString()));
            }
        }

        public void Clear()
        {
            RiskAutoCompleteBox.Entries.Clear();
        }
    }
}