using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;

namespace DI.Chakra.Web.Comp.Exception.Common
{
    public partial class ExceptionDetailsUC : UIBaseUC
    {
        #region Class Memeber Variables
        SODException _exception = null;
        SessionStatusImpl session = new SessionStatusImpl();
        private const string V_ID = "ENTITY_ID";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODException SODException
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public User ExceptionOwner
        {
            get
            {
                return usrSearch.SelectedUser;
            }
            set
            {
                usrSearch.SelectedUser= value;
            }
        }

        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to Initialize the Page
        /// </summary>
        public void InitializePage()
        {
            if (UIMode == UIMode.Edit || UIMode == UIMode.View)
            {
                txtExceptionCode.Text = SODException.ExceptionCode;
                txtExceptionDescription.Text = SODException.ExceptionDesc;
                ExceptionOwner = SODException.RiskOwner;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to open the screen in View Mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        /// <summary>
        /// Method to Get the Exception
        /// </summary>
        /// <returns></returns>
        public SODException GetException()
        {
            SODException exception = new SODException();
            exception.ExceptionCode = txtExceptionCode.Text;
            exception.ExceptionDesc = txtExceptionDescription.Text;
            return exception;
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to Load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        #endregion
    }
}