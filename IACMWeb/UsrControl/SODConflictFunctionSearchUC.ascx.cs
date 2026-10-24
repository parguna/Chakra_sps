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
    public partial class SODConflictFunctionSearchUC : DI.Chakra.Web.UIBaseUC
    {
        private SODConflictFunctions function = null;

        Dictionary<string, string> parameter = null;
        public SODConflictFunctions SelectedFunction
        {
            get
            {
                if (ConflictAutoCompleteBox.Entries.Count <= 0)
                    return null;

                parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToInt64(ConflictAutoCompleteBox.Entries[0].Value).ToString());
                function = DomainApiClient<Dto.SODConflictFunctions>.GetByAction<Dto.SODConflictFunctions>
                    (DomainApiAction.SODFunction.SODConflictFunction.GetByIdIncludingSODRiskCode, parameter);

                return function;
            }
            set
            {
                function = value;
                if (function != null)
                    ConflictAutoCompleteBox.Entries.Add(new AutoCompleteBoxEntry
                        (function.SodConflictCode + " - " + function.SodConflictDescription, function.Id.ToString()));
            }
        }

        public void Clear()
        {
            ConflictAutoCompleteBox.Entries.Clear();
        }
    }
}