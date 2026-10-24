using System;
using DI.Chakra.Dto;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Members.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Members
        private const string V_EntityID = "V_EntityID";
        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
                SetPage();
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            User User = DomainApiClient<Dto.User>.GetByAction<User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

            if (User == null)
                return;
            UserName.Text = User.UserName;
            EmailID.Text = User.EmailId;
            EmployeeID.Text = User.EmployeeId;
            lblStatus.Text = User.IsActive == true ? "Active" : "Inactive";
            lblDisplayName.Text = User.DisplayName;
            FName.Text = User.FirstName;
            LName.Text = User.LastName;
            IdentNumber.Text = IdentificationNumberConsrtuct(User.IdentificationNo);
            DateOnBoard.Text = User.JoinDate.HasValue ? User.JoinDate.Value.ToShortDateString() : "";
            DateOffBoard.Text = User.TerminationDate.HasValue ? User.TerminationDate.Value.ToShortDateString() : "";
            TelNumber.Text = User.TelephoneNumber;
            MbNumber.Text = User.MobileNumber;

            if (User.NOrganisation != null)
            {
                lblOrgName.Text = User.NOrganisation.Code + " - " + User.NOrganisation.Name;
            }

            if (User.NBusinessRole != null)
                lblPosition.Text = User.NBusinessRole.Code + " - " + User.NBusinessRole.Description;
           
        }

        private string IdentificationNumberConsrtuct(string identificationNumber)
        {
            var length = identificationNumber.Length;
            var result = new String('X', length - 5) + identificationNumber.Substring(length - 5);
            return result;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && ContextObjectId != null)
            {
                EntityId = Convert.ToInt64(ContextObjectId);
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Members/List.aspx");
        }

        //protected void Refresh_Click(object sender, EventArgs e)
        //{
        //    this.SetPage();
        //}
        #endregion
    }
}