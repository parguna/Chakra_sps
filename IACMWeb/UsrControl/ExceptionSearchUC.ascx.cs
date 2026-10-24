using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl
{
    public partial class ExceptionSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private SODException sodException = null;
        public SODException SelectedSodException
        {
            get
            {
                if (ExceptionAutoCompleteBox.Entries.Count <= 0)
                    return null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", ExceptionAutoCompleteBox.Entries[0].Value);
                sodException = DomainApiClient<SODException>.GetByAction<SODException>
                    (DomainApiAction.SODException.Exception.GetByIdIncludeRiskOwner, parameter);
                return sodException;
            }
            set
            {
                sodException = value;
                if (sodException != null)
                    ExceptionAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry
                        (sodException.ExceptionCode + " - " + sodException.ExceptionDesc, sodException.Id.ToString()));
            }
        }

        public void Clear()
        {
            ExceptionAutoCompleteBox.Entries.Clear();
        }
    }
}