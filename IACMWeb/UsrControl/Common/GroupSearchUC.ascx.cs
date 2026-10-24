using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class GroupSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private Group _group = null;
        public Group SelectedGroup
        {
            get
            {
                if (GroupAutoCompleteBox.Entries.Count <= 0)
                    return null;

                _group = DomainApiClient<Dto.Group>.GetById(Convert.ToInt32(GroupAutoCompleteBox.Entries[0].Value));

                return _group;
            }
            set
            {
                _group = value;
                if (_group != null)
                    GroupAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(_group.GroupName + " - " + _group.GroupDescription, _group.Id.ToString()));
            }
        }

        public void Clear()
        {
            GroupAutoCompleteBox.Entries.Clear();
        }
    }
}