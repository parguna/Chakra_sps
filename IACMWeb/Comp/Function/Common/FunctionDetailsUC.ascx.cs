using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;

namespace DI.Chakra.Web.Comp.Function.Common
{
    /// <summary>
    /// User control responsible for capturing Function Details in Complaince Module Function Wizard.
    /// Author : Harihara - 3/11/2015.
    /// </summary>
    public partial class FunctionDetailsUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Member Variable
        private SODFunction function = null;
        SessionStatusImpl session = new SessionStatusImpl();
        private string V_SODFunction = "SODFunction";
        private UIMode uiMode;
        #endregion

        #region Public Property        
        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODFunction SodFunction
        {
            get
            {
                if (ViewState[V_SODFunction] == null)
                    return null;
                return (SODFunction)ViewState[V_SODFunction];
            }
            set
            {
                ViewState[V_SODFunction] = value;
            }
        }
        #endregion

        #region Intialize Section
        /// <summary>
        /// Method to Initialise Page
        /// </summary>
        public void InitilizePage()
        {
            rblActive.Checked = true;
            if (UIMode==UIMode.Edit || UIMode==UIMode.View|| UIMode==UIMode.Clone)
            {
                txtFunctionCode.Text = UIMode == UIMode.Clone ? "Copy of " + SodFunction.SodFunctionCode : SodFunction.SodFunctionCode;
                txtFunctionDescription.Text = SodFunction.SodFunctionDescription;
                if (SodFunction.IsActive.Value)
                {
                    rblActive.Checked = true;
                }
                else
                {
                    rblInActive.Checked = true;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set mode of display
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        /// <summary>
        /// Method to get user inputs as SodFunction
        /// </summary>
        /// <returns></returns>
        public SODFunction GetSodFunction()
        {
            SODFunction function = new SODFunction();
            function.IsActive = rblActive.Checked;
            function.SodFunctionCode = txtFunctionCode.Text;
            function.SodFunctionDescription = txtFunctionDescription.Text;
            return function;
        }
        #endregion
    }
}