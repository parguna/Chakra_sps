using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Report.Attestation.UC
{
    public partial class AttestationCampaignDetailUC : DI.Chakra.Web.UIBaseUC
    {        
        private Dto.Presentation.PresentationAttetstationCampaign _mCampaign = null;
        public Dto.Presentation.PresentationAttetstationCampaign Campaign
        {
            set
            {
                _mCampaign = value;
                SetValues(_mCampaign);
            }
        }

        private void SetValues(Dto.Presentation.PresentationAttetstationCampaign Campaign)
        {
            lblRemediation.Text = Campaign.RequiresAttestationRemediation.ToString();
            lblStatus.Text = Campaign.DefaultReviewStatus.ToString();
        }
    }
}