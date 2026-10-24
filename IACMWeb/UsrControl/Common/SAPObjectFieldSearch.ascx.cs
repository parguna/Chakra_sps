using System;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class SAPObjectFieldSearch : DI.Chakra.Web.UIBaseUC
    {
        private Dto.SAPObjectInfo _object = null;
        private string  _field = null;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region Public properties
        public Dto.SAPObjectInfo SelectedObject
        {
            get
            {
                if (SapObject.Entries.Count <= 0)
                    return null;
                _object= DomainApiClient<Dto.SAPObjectInfo>.GetById(Convert.ToInt64(SapObject.Entries[0].Value));              
                return _object;
            }
            set
            {
                _object = value;
                if (_object != null)
                {
                    SapObject.Entries.Add(new AutoCompleteBoxEntry(_object.SapObjectCode, _object.Id.ToString()));
                }
            }
        }

        public string SelectedField
        {
            get
            {
                if (SapObjectFields.Entries.Count <= 0)
                    return null;
                _field = SapObjectFields.Entries[0].Value;
                return _field;
            }
            set
            {
                _field = value;
                SapObjectFields.Entries.Add(new AutoCompleteBoxEntry(_field,_field));
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Method to clear the AutoComplete box
        /// </summary>
        public void ClearObejcts()
        {
            SapObject.Entries.Clear();
        }

        public void ClearFields()
        {
            SapObjectFields.Entries.Clear();
        }
        #endregion
    }
}