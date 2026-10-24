using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class UserSearchUC : DI.Chakra.Web.UIBaseUC
    {
        public delegate void SelectedUserChanged(Object sender);

        public event SelectedUserChanged UserChanged;

        private User _mUser = null;
        public bool DisableMyAutocompleteBox
        {
            set { UserNameAutoCompleteBox.Enabled = value; }
        }


        public string DataSourceMethod
        {
            set
            {
                UserNameAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }
        public bool AllowSingleUser
        {
            set
            {
                if (value == true)
                    UserNameAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Single;
                else
                    UserNameAutoCompleteBox.TextSettings.SelectionMode = RadAutoCompleteSelectionMode.Multiple;

            }
        }
        public bool IsRequired
        {
            set
            {
                UserNameAutoCompleteBox.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool Disabled
        {
            set
            {
                UserNameAutoCompleteBox.Enabled = !value;
            }
        }
        public bool AllUsers
        {
            set
            {
                if (value)
                {
                    string script = @"  function OnClientRequesting(sender, args)
                    {
                        var context = args.get_context();
                       context[""hidden""] = ""false"";
                    } ";
                    UserNameAutoCompleteBox.OnClientRequesting = script;
                }
            }
        }
        public short TabIndexUC
        {
            set
            {
                UserNameAutoCompleteBox.TabIndex = value;
            }
        }
        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("UserNameAutoCompleteBox")) ? true : false;
            }
        }

        public bool ConsiderPriorOnBoardDays
        {
            set
            {
                if (value)
                {
                    ConsiderJoindDate.Value = "true";
                    string script = @"  function OnClientRequesting(sender, args)
                    {
                        var context = args.get_context();
                       context[""ConsiderJoindDate""] = ""true"";
                    } ";
                    UserNameAutoCompleteBox.OnClientRequesting = script;
                }
            }
        }
        public User SelectedUser
        {
            get
            {
                if (UserNameAutoCompleteBox.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", (string.IsNullOrEmpty(UserNameAutoCompleteBox.Entries[0].Value)) ? "0" : UserNameAutoCompleteBox.Entries[0].Value);
                _mUser = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                return _mUser;
            }
            set
            {
                _mUser = value;
                if (_mUser != null)
                {
                    UserNameAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry($"{_mUser.DisplayName} [{_mUser.UserName}]", _mUser.Id.ToString()));
                }
                else
                {
                    UserNameAutoCompleteBox.Entries.Clear();
                }
            }
        }

        public void Clear()
        {
            UserNameAutoCompleteBox.Entries.Clear();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                UpdateOnSelect.Update();
            }
            //
        }

        protected void UserNameAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (UserChanged != null)
                UserChanged(sender);
            //if (UserNameAutoCompleteBox.TextSettings.SelectionMode == RadAutoCompleteSelectionMode.Single)
            //{
            //    int count = UserNameAutoCompleteBox.Entries.Count - 1;
            //    for (int i = 0; i < count; i++)
            //    {
            //        UserNameAutoCompleteBox.Entries.RemoveAt(i);
            //    }

            //}
        }

        protected void UserNameAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {

        }
    }
}