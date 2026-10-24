using Application;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Exception.UC
{
    public partial class FunctionUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Member Variables
        private SODFunction _function = null;
        SessionStatusImpl session = new SessionStatusImpl();
        private SODFunctionItem _functionItem = null;
        #endregion

        #region Public properties
        public SODFunction SelectedFunction
        {
            get
            {
                if (FunctionAuto.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", (string.IsNullOrEmpty(FunctionAuto.Entries[0].Value)) ? "0" : FunctionAuto.Entries[0].Value);
                _function = DomainApiClient<SODFunction>.GetByAction<SODFunction>
                    (DomainApiAction.SODFunction.Function.GetByIdIncludeSodFunctionItem, parameter);
                return _function;
            }
            set
            {
                _function = value;
                if (_function != null)
                {
                    FunctionAuto.Entries.Add(new AutoCompleteBoxEntry
                        (_function.SodFunctionCode + " - " + _function.SodFunctionDescription,
                        _function.Id.ToString()));                   
                }
            }
        }

        public SODFunctionItem SelectedFunctionItem
        {
            get
            {
                if (FunctionAutoValue.Entries.Count <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", (string.IsNullOrEmpty(FunctionAutoValue.Entries[0].Value))?"0" : FunctionAutoValue.Entries[0].Value);
                _functionItem = DomainApiClient<SODFunctionItem>.GetByAction<SODFunctionItem>
                    (DomainApiAction.SODFunction.SODFunctionItem.GetByIdWithNavigation, parameter);
                return _functionItem;
            }
            set
            {
                _functionItem = value;
                if (_functionItem != null)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", (_functionItem.SodFunctionId.HasValue ? _functionItem.SodFunctionId.Value : 0).ToString());
                    _functionItem = DomainApiClient<SODFunctionItem>.GetByAction<SODFunctionItem>
                        (DomainApiAction.SODFunction.SODFunctionItem.GetByIdWithNavigation, parameter);
                }
                FunctionAutoValue.Entries.Add(new AutoCompleteBoxEntry(_functionItem.endpoint.DisplayName + " - " +
                    _functionItem.EndpointType.DisplayName, _functionItem.SodFunctionId.ToString()));
            }
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = this.ID;
            if (id == "FunSearch")
            {
                this.Function.Text = "Function 1";
                this.FunctionVal.Text = "Function Value 1";
            }
            else if (id == "FunSearch2")
            {
                this.Function.Text = "Function 2";
                this.FunctionVal.Text = "Function Value 2";
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to clear the AutoComplete box
        /// </summary>
        public void Clear()
        {
            FunctionAuto.Entries.Clear();
        }

        /// <summary>
        /// Method to clear the AutoComplete box
        /// </summary>
        public void ClearValueSearch()
        {
            FunctionAutoValue.Entries.Clear();
        }
        #endregion
    }
}