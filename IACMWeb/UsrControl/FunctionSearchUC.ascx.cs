using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl
{
    public partial class FunctionSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private Dto.SODFunction _function = null;
        public Dto.SODFunction SelectedFunction
        {
            get
            {
                if (FunctionAutoCompleteBox.Entries.Count <= 0)
                    return null;

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", FunctionAutoCompleteBox.Entries[0].Value);
                _function = DomainApiClient<Dto.SODFunction>.GetByAction<Dto.SODFunction>
                    (DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, parameter);
                return _function;
            }
            set
            {
                _function = value;
                if (_function != null)
                    FunctionAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry
                        (_function.SodFunctionCode + " - " + _function.SodFunctionDescription, _function.Id.ToString()));
            }
        }

        public void Clear()
        {
            FunctionAutoCompleteBox.Entries.Clear();
        }
    }
}