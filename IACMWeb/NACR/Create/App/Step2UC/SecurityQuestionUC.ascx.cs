using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App.Step2UC
{
    public partial class SecurityQuestionUC : UIBaseStep2UC
    {
        private const string V_SpecificToADRequest = "V_SpecificToADRequest";
        public bool IsSpecificToADRequest
        {
            get
            {
                if (ViewState[V_SpecificToADRequest] == null)
                    return false;
                return (bool)ViewState[V_SpecificToADRequest];
            }
            set
            {
                ViewState[V_SpecificToADRequest] = value;
            }
        }

        public List<SecurityResponseDetail> SecurityResponseDetail { get; set; }

        public delegate void SecurityResponseHandeler(List<SecurityResponseDetail> detail);

        public event SecurityResponseHandeler SecurityResponseSaveClicked;

        public event System.EventHandler SecurityQuestionPopUp;

        string _linkText = "Service account compliance check";
        protected void Page_Load(object sender, EventArgs e)
        {
            securityQstnsBtn.Text = _linkText;
            //if (!string.IsNullOrWhiteSpace(hdnScrollControlId.Value))
            //    this.MaintainScroll(hdnScrollControlId.Value);
        }
        protected void MaintainScroll(string controlId)
        {
            string script = @" window.onload = function Maintain()
                              {document.getElementById('" + controlId + "').scrollIntoView(); }";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Maintain", script, true);
        }

        protected override void InitializeStep()
        {


        }


        protected void SQList_ItemDataBound(object sender, Telerik.Web.UI.RadListViewItemEventArgs e)
        {

            //Telerik.Web.UI.RadListViewDataItem dataItem = (Telerik.Web.UI.RadListViewDataItem)e.Item;

            //if (e.Item.ItemType == RadListViewItemType.DataItem || e.Item.ItemType == RadListViewItemType.AlternatingItem)
            //{
            //    Dto.SecurityResponseGridData yourDataSource = (Dto.SecurityResponseGridData)dataItem.DataItem;
            //    SecurityQuestionItemUC SecurityQuestionItemUC1 = (SecurityQuestionItemUC)e.Item.FindControl("SecurityQuestionItemUC");
            //    SecurityQuestionItemUC1.SecurityResponseDetailItem = yourDataSource;
            //    SecurityQuestionItemUC1.Initialize();
            //}
        }

        public override bool Validate()
        {
            return ValidateSQ();
        }
        public bool ValidateSQ()
        {
            var result = true;

            foreach (RadListViewDataItem dataItem in securityQuestionListView.Items)
            {
                if (dataItem.ItemType == RadListViewItemType.AlternatingItem || dataItem.ItemType == RadListViewItemType.DataItem)
                {
                    var lblComments = ((Label)dataItem.FindControl("lblComments"));
                    lblComments.Visible = false;
                    var Comments = ((TextBox)dataItem.FindControl("txtComments"));
                    Comments.BorderColor = System.Drawing.Color.Empty;
                    var radCmbCompliance = (RadComboBox)dataItem.FindControl("radCmbCompliance");
                    if (radCmbCompliance.SelectedValue != "0")
                    {
                        radCmbCompliance.BorderColor = System.Drawing.Color.Empty;
                        var userResponse = (UserResponse)Enum.Parse(typeof(UserResponse), (((RadComboBox)dataItem.FindControl("radCmbCompliance")).SelectedValue));

                        if (userResponse != UserResponse.Yes)
                        {
                            if (string.IsNullOrWhiteSpace(Comments.Text))
                            {
                                lblComments.Visible = true;
                                Comments.BorderColor = System.Drawing.Color.Red;
                                result = false;
                            }
                            else
                            {
                                Comments.BorderColor = System.Drawing.Color.Empty;
                            }
                        }
                    }
                    else
                    {
                        radCmbCompliance.BorderColor = System.Drawing.Color.Red;
                        result = false;
                    }
                }
            }
            return result;
        }



        public string LinkText
        {
            get
            {
                return _linkText;
            }
            set
            {
                _linkText = value;
            }
        }

        protected void securityQstnsBtn_Click(object sender, EventArgs e)
        {
            if (SecurityQuestionPopUp != null)
                SecurityQuestionPopUp(this, new EventArgs());
            ShowSecurityQuestions(true);
        }

        #region Security Questions Detail

        Dictionary<byte, string> GetComplianceData()
        {
            Dictionary<byte, string> data = new Dictionary<byte, string>();
            data.Add(0, "--Select--");
            byte[] Values = (byte[])(System.Enum.GetValues(typeof(UserResponse)));
            string[] Names = (string[])(System.Enum.GetNames(typeof(UserResponse)));

            for (int i = 0; i <= Names.Length - 1; i++)
            {
                data.Add(Values[i], Names[i]);
            }
            return data;
        }

        List<SecurityResponseGridData> GetSecurityResponses()
        {
            if (SecurityResponseDetail != null)
            {
                return SecurityResponseDetail.Select(item =>
             new SecurityResponseGridData() { ComplianceData = GetComplianceData(), Question = item.Question, QuestionID = item.QuestionID, Comments = item.Comments, Compliance = (byte)item.Compliance }
             ).ToList();
            }

            List<SecurityResponseDetail> svcQstns = DI.Chakra.Web.DomainApiClient<AdHocAccessRequest>.GetByAction<List<SecurityResponseDetail>>("GetServiceAccountQuestions", null);
            return svcQstns.Select(item =>
            new SecurityResponseGridData() { ComplianceData = GetComplianceData(), Question = item.Question, QuestionID = item.QuestionID }
            ).ToList();
        }

        List<SecurityResponseDetail> PoplateSecurityQuestionDetail()
        {

            SecurityResponseDetail = new List<SecurityResponseDetail>();

            foreach (RadListViewDataItem dataItem in securityQuestionListView.Items)
            {
                if (dataItem.ItemType == RadListViewItemType.AlternatingItem || dataItem.ItemType == RadListViewItemType.DataItem)
                {
                    SecurityResponseDetail detail = new SecurityResponseDetail();
                    detail.QuestionID = Convert.ToInt16(((Label)dataItem.FindControl("lblQstnID")).Text);
                    detail.Question = ((Label)dataItem.FindControl("lblDescription")).Text;
                    detail.Compliance = (UserResponse)Enum.Parse(typeof(UserResponse), (((RadComboBox)dataItem.FindControl("radCmbCompliance")).SelectedValue));
                    detail.Comments = ((TextBox)dataItem.FindControl("txtComments")).Text;
                    SecurityResponseDetail.Add(detail);
                }
            }

            //securityQuestionListView.Items.ForEach(item =>
            //{
            //    SecurityQuestionItemUC SecurityQuestionItem = (SecurityQuestionItemUC)item.FindControl("SecurityQuestionItemUC");
            //    SecurityResponseGridData gridItem = SecurityQuestionItem.SecurityResponseDetailItem;
            //    SecurityResponseDetails.Add(new SecurityResponseDetail() { Comments = gridItem.Comments, QuestionID = gridItem.QuestionID,Question= gridItem.Question, Compliance =  (UserResponse) gridItem.Compliance });
            //});



            return SecurityResponseDetail;
        }

        protected void SecqstnsbtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateSQ())
            {
                PoplateSecurityQuestionDetail();

                //SQDetails.VisibleOnPageLoad = false; 
                CloseWindowSQ();

                SecurityResponseSaveClicked(this.SecurityResponseDetail);
            }
            else
            {
                //errorMessage.Visible = true;
                //SQDetails.VisibleOnPageLoad = true;
            }

        }

        void ShowSecurityQuestions(bool show)
        {
            if (show)
            {
                //SQDetails.VisibleOnPageLoad = true;
                ShowWindowSQ();
                securityQuestionListView.DataSource = GetSecurityResponses();
            }
            else
            {
                //SQDetails.VisibleOnPageLoad = false;
                CloseWindowSQ();
            }
        }

        void ShowWindowSQ()
        {
            var key = "showWindowSQkey";
            string script = "Sys.Application.remove_load(showWindowSQ);Sys.Application.add_load(showWindowSQ);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }

        void CloseWindowSQ()
        {
            var key = "closeWindowSQkey";
            string script = "Sys.Application.remove_load(closeWindowSQ);Sys.Application.add_load(closeWindowSQ);";
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
        }


        #endregion

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //SQDetails.VisibleOnPageLoad = false;
            CloseWindowSQ();
        }

        protected void radCmbCompliance_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var ddl = (RadComboBox)sender;
            //hdnScrollControlId.Value = ddl.ClientID;
            var row = ddl.Parent;
            var radCmbCompliance = row.FindControl("radCmbCompliance") as RadComboBox;
            var txtComments = row.FindControl("txtComments") as TextBox;
            var lblComments = row.FindControl("lblComments") as Label;
            if (radCmbCompliance.SelectedValue != "0")
            {
                radCmbCompliance.BorderColor = System.Drawing.Color.Empty;
                var userResponse = (UserResponse)Enum.Parse(typeof(UserResponse), (radCmbCompliance.SelectedValue));

                if (userResponse != UserResponse.Yes)
                {
                    if (string.IsNullOrWhiteSpace(txtComments.Text))
                    {
                        lblComments.Visible = true;
                        txtComments.BorderColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        lblComments.Visible = false;
                        txtComments.BorderColor = System.Drawing.Color.Empty;
                    }
                }
                else
                {
                    txtComments.BorderColor = System.Drawing.Color.Empty;
                    lblComments.Visible = false;
                }
            }
            else
            {
                radCmbCompliance.BorderColor = System.Drawing.Color.Red;
            }
        }

        protected void txtComments_TextChanged(object sender, EventArgs e)
        {
            var txtControl = (TextBox)sender;
            var row = txtControl.Parent;
            var radCmbCompliance = row.FindControl("radCmbCompliance") as RadComboBox;
            var txtComments = row.FindControl("txtComments") as TextBox;
            var lblComments = row.FindControl("lblComments") as Label;
            if (radCmbCompliance.SelectedValue != "0")
            {
                radCmbCompliance.BorderColor = System.Drawing.Color.Empty;
                var userResponse = (UserResponse)Enum.Parse(typeof(UserResponse), (radCmbCompliance.SelectedValue));

                if (userResponse != UserResponse.Yes)
                {
                    if (string.IsNullOrWhiteSpace(txtComments.Text))
                    {
                        lblComments.Visible = true;

                        txtComments.BorderColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        lblComments.Visible = false;
                        txtComments.BorderColor = System.Drawing.Color.Empty;
                    }
                }
                else
                {
                    txtComments.BorderColor = System.Drawing.Color.Empty;
                    lblComments.Visible = false;
                }
            }
            else
            {
                radCmbCompliance.BorderColor = System.Drawing.Color.Red;
            }
        }

        //protected void radCmbCompliance_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        //{
        //    var cmb = (RadComboBox)sender;
        //    var row = cmb.Parent;
        //    var lblComments = ((Label)row.FindControl("lblComments"));
        //    var radCmbCompliance = ((RadComboBox)row.FindControl("radCmbCompliance"));
        //    var txtComments = ((TextBox)row.FindControl("txtComments"));
        //    lblComments.Visible = false;
        //    txtComments.BorderColor = System.Drawing.Color.Empty;
        //    if (radCmbCompliance.SelectedValue != "0")
        //    {
        //        radCmbCompliance.BorderColor = System.Drawing.Color.Empty;
        //        var userResponse = (UserResponse)Enum.Parse(typeof(UserResponse), (((RadComboBox)row.FindControl("radCmbCompliance")).SelectedValue));

        //        if (userResponse != UserResponse.Yes)
        //        {
        //            if (string.IsNullOrWhiteSpace(txtComments.Text))
        //            {
        //                lblComments.Visible = true;
        //                txtComments.BorderColor = System.Drawing.Color.Red;
        //            }
        //            else
        //            {
        //                txtComments.BorderColor = System.Drawing.Color.Empty;
        //            }
        //        }
        //    }
        //}
    }
}