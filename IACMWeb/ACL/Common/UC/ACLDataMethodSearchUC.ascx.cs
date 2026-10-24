using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.ACL.Common.UC
{
    public partial class ACLDataMethodSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private Dto.TabMenu tabmenu = null;
        private Dto.WebPage page = null;
        private Dto.PageUrl pageurl = null;

        public delegate void AutoCompleteOnEntryAdded(Dto.TabMenu SelectedTabMenu);
        public event AutoCompleteOnEntryAdded AutocompleteSelection;

        public delegate void AutoCompleteOnEntryRemoved();
        public event AutoCompleteOnEntryRemoved OnEntryRemoved;

        public bool DisableMyAutocompleteBox
        {
            set { ACLMethodAutoCompleteBox.Enabled = value; }
        }

        public string DataSourceMethod
        {
            set
            {
                ACLMethodAutoCompleteBox.WebServiceSettings.Method = value;
            }
        }

        public bool Disabled
        {
            set
            {
                ACLMethodAutoCompleteBox.Enabled = !value;
            }
        }

        public bool IsEntryModified
        {
            get
            {
                return (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].
                    Contains("ACLMethodAutoCompleteBox")) ? true : false;
            }
        }

        public RadAutoCompleteBox ChildAutoCompleteBox
        {
            get { return ACLMethodAutoCompleteBox; }
        }

        public void Clear()
        {
            ACLMethodAutoCompleteBox.Entries.Clear();
        }

        protected void ACLMethodAutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            if (this.ACLMethodAutoCompleteBox.Entries.Count > 0)
            {
                string AutocompleteEntry = this.ACLMethodAutoCompleteBox.Entries[0].Value;
                if (ACLMethodAutoCompleteBox.Entries.Count == 2)
                {
                    ACLMethodAutoCompleteBox.Entries.Remove(ACLMethodAutoCompleteBox.Entries[1]);
                }
            }
            if (AutocompleteSelection != null)
                AutocompleteSelection(SelectedParentTabMenu);
        }

        protected void ACLMethodAutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            if (OnEntryRemoved != null)
                OnEntryRemoved();
        }


        public Dto.TabMenu SelectedParentTabMenu
        {
            get
            {
                if (ACLMethodAutoCompleteBox.Entries.Count <= 0)
                    return null;
                tabmenu = DomainApiClient<Dto.TabMenu>.GetById(Convert.ToInt64(ACLMethodAutoCompleteBox.Entries[0].Value));
                return tabmenu;
            }
            set
            {
                tabmenu = value;
                if (tabmenu != null)
                    ACLMethodAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(tabmenu.MenuName, tabmenu.Id.ToString()));
            }
        }

        public Dto.WebPage SelectedPage
        {
            get
            {
                if (ACLMethodAutoCompleteBox.Entries.Count <= 0)
                    return null;
                page = DomainApiClient<Dto.WebPage>.GetById(Convert.ToInt64(ACLMethodAutoCompleteBox.Entries[0].Value));
                return page;
            }
            set
            {
                page = value;
                if (page != null)
                    ACLMethodAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(page.Name, page.Id.ToString()));
            }
        }

        public Dto.PageUrl SelectedUrl
        {
            get
            {
                if (ACLMethodAutoCompleteBox.Entries.Count <= 0)
                    return null;

                pageurl = DomainApiClient<Dto.PageUrl>.GetById(Convert.ToInt64(ACLMethodAutoCompleteBox.Entries[0].Value));
                return pageurl;
            }
            set
            {
                pageurl = value;
                if (pageurl != null)
                    ACLMethodAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry(pageurl.Url, pageurl.Id.ToString()));
            }
        }
    }
}