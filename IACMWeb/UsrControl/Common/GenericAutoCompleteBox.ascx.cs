using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class GenericAutoCompleteBox : DI.Chakra.Web.UIBaseUC
    {
        public event AutoCompleteEntryEventHandler EntryAdded; 
        public event AutoCompleteEntryEventHandler EntryRemoved;
        public bool EnabledAutoCompleteBox
        {
            get { return AutoCompleteBoxCompany.Enabled; }
            set { AutoCompleteBoxCompany.Enabled = value; }
        }
   
        
        public string ServiceMethodName
        {
            get { return AutoCompleteBoxCompany.WebServiceSettings.Method; }
            set { AutoCompleteBoxCompany.WebServiceSettings.Method = value; }
        }
        public string ServicePath
        {
            get { return AutoCompleteBoxCompany.WebServiceSettings.Path; }
            set { AutoCompleteBoxCompany.WebServiceSettings.Path = value; }
        }
        public void SetValue(string Text, string Value)
        {
            AutoCompleteBoxCompany.Entries.Add(new AutoCompleteBoxEntry(Text, Value));
        }
        public AutoCompleteBoxEntry GetValue()
        {
            if (AutoCompleteBoxCompany.Entries.Count > 0)
                return AutoCompleteBoxCompany.Entries[0];
            else
                return null;
        }
        public void Clear()
        {
            AutoCompleteBoxCompany.Entries.Clear();
        }
        public bool Disabled
        {
            set
            {
                AutoCompleteBoxCompany.Enabled = !value;
            }
        }
        public bool ReadOnly
        {
            set { AutoCompleteBoxCompany.Enabled = !value; }
            get { return AutoCompleteBoxCompany.Enabled; }
        }

        protected void AutoCompleteBox_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            EntryAdded?.Invoke(sender, e);
        }

        protected void AutoCompleteBox_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            EntryRemoved?.Invoke(sender, e);
        }
    }
}