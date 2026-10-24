using System;


namespace DI.Chakra.Web.Admin.AppSetting.UC
{
    public partial class ProvEngineSetupUC : UIBaseUC
    {
        public string RunOnBoardTriggerStr
        {
            set
            {
                lblrunon.Text = value;
                Txtonboard.Attributes.Add("title", value);
            }
        }

        public bool RunOnBoardTrigger
        {
            get
            {
                return CanRunOnBoardTrigger.Checked;
            }
            set
            {
                CanRunOnBoardTrigger.Checked = value;
            }
        }

        public string RunOnBoardTriggerText
        {
            get
            {
                return CanRunOnBoardTrigger.ID;
            }
        }

        public string RunOffBoardTriggerStr
        {
            set
            {
                lbloffbtrigg.Text = value;
                Txtoffboard.Attributes.Add("Title", value);
            }
        }
        public bool RunOffBoardTrigger
        {
            get
            {
                return CanRunOffBoardTrigger.Checked;
            }
            set
            {
                CanRunOffBoardTrigger.Checked = value;
            }
        }

        public string RunOffBoardTriggerText
        {
            get
            {
                return CanRunOffBoardTrigger.ID;
            }
        }

        public string RunTransferTriggerStr
        {
            set
            {
                lbltranstrigg.Text = value;
                Txttransfer.Attributes.Add("Title", value);
            }
        }


        public bool RunTransferTrigger
        {
            get
            {
                return CanRunTransferTrigger.Checked;
            }
            set
            {
                CanRunTransferTrigger.Checked = value;
            }
        }

        public string RunTransferTriggerText
        {
            get
            {
                return CanRunTransferTrigger.ID;
            }
        }

        public string RunReOrgTriggerStr
        {
            set
            {
                lblrunreorg.Text = value;
                Txtrunorg.Attributes.Add("Title", value);
            }
        }

        public bool RunReOrgTrigger
        {
            get
            {
                return CanRunReOrgTrigger.Checked;
            }
            set
            {
                CanRunReOrgTrigger.Checked = value;
            }
        }

        public string RunReOrgTriggerText
        {
            get
            {
                return CanRunReOrgTrigger.ID;
            }
        }
        public string RunPropagationTriggerStr
        {
            set
            {
                lblrunprop.Text = value;
                Txtprop.Attributes.Add("Title", value);
            }
        }
        public bool RunPropagationTrigger
        {
            get
            {
                return CanRunPropagationTrigger.Checked;
            }
            set
            {
                CanRunPropagationTrigger.Checked = value;
            }
        }

        public string RunPropagationTriggerText
        {
            get
            {
                return CanRunPropagationTrigger.ID;
            }
        }

        public string RunPromotionTriggerStr
        {
            set
            {
                lblPromotionTrigg.Text = value;
                TxtPromotion.Attributes.Add("Title", value);
            }
        }
        public bool RunPromotionTrigger
        {
            get
            {
                return CanRunPromotionTrigger.Checked;
            }
            set
            {
                CanRunPromotionTrigger.Checked = value;
            }
        }

        public string RunPromotionTriggerText
        {
            get
            {
                return CanRunPromotionTrigger.ID;
            }
        }

        public string RunRehireTriggerStr
        {
            set
            {
                lblRehiretrigg.Text = value;
                TxtReHire.Attributes.Add("Title", value);
            }
        }
        public bool RunRehireTrigger
        {
            get
            {
                return CanRunRehireTrigger.Checked;
            }
            set
            {
                CanRunRehireTrigger.Checked = value;
            }
        }

        public string CanRunRehireTriggerText
        {
            get
            {
                return CanRunRehireTrigger.ID;
            }
        }

        public string RunAttRemediationStr
        {
            set
            {
                lblrunatt.Text = value;
                Txtrunatt.Attributes.Add("Title", value);
            }
        }
        public bool RunAttRemediation
        {
            get
            {
                return runAttestationRemediation.Checked;
            }
            set
            {
                runAttestationRemediation.Checked = value;
            }
        }

        public string RunAttRemediationText
        {
            get
            {
                return runAttestationRemediation.ID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}