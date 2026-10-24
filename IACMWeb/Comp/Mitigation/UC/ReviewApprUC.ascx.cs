using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Mitigation.UC
{
    public partial class ReviewApprUC :UIBaseUC
    {
        #region Class Member Variables
        private User _mUser = null;
        private Group _mGroup = null;
        #endregion

        #region Public Properties
        public string DataMethodField
        {
            set
            {
                this.ApproverReviwer.WebServiceSettings.Method = value;
            }
        }

        public User SelectedUser
        {
            get
            {
                if (ApproverReviwer.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", ApproverReviwer.Entries[0].Value.ToString());
                
                _mUser = DomainApiClient<User>.GetByAction<User>(DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);
                return _mUser;
            }
            set
            {
                _mUser = value;
                if (_mUser != null)
                    ApproverReviwer.Entries.Add(new AutoCompleteBoxEntry(_mUser.DisplayName, _mUser.Id.ToString()));
            }
        }

        public Group SelectedGroup
        {
            get
            {
                if (ApproverReviwer.Entries.Count <= 0)
                    return null;
                _mGroup = DomainApiClient<Dto.Group>.GetById( Convert.ToInt64(ApproverReviwer.Entries[0].Value.ToString()));
                return _mGroup;
            }
            set
            {
                _mGroup = value;
                if (_mGroup != null)
                    ApproverReviwer.Entries.Add(new AutoCompleteBoxEntry(_mGroup.GroupName, _mGroup.Id.ToString()));
            }
        }
        #endregion
         
        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #endregion

        #region  Public Methods
        /// <summary>
        /// Method to clear the Auto complete box entries
        /// </summary>
        public void ClearAppUser()
        {
            ApproverReviwer.Entries.Clear();
        }
        #endregion
    }
}