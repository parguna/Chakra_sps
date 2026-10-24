using Application;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Comp.Exception.Wizard
{
    public partial class ExceptionWizardUC : UIBaseUC
    {
        #region Class Member Variables
        private SessionStatusImpl session = new SessionStatusImpl();
        private const string V_DISPLAYMODE = "DISPLAYMODE";
        SODException _exception = null;
        #endregion

        #region Public Properties       
        public SODException Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.Add;
                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set { ViewState[V_DISPLAYMODE] = value; }
        }
        #endregion

        #region Helper
        private void SetPage()
        {
            long ExceptionId = Convert.ToInt64(this.ContextObjectId);
            SODException exception = GetSODExcpetion(ExceptionId);
            Exceptiondetails.SODException = exception;
            Exceptiondetails.EntityId = ExceptionId;
            UserDetails.ExceptionItems = exception.ExceptionItems;
        }

        private SODException GetSODExcpetion(long ExceptionId)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", ExceptionId.ToString());
            return DomainApiClient<SODException>.GetByAction<SODException>
                (DomainApiAction.SODException.Exception.GetByIdIncludeRiskOwner, parameter);
        }
        #endregion

        #region Page and Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitializePage();
        }

        protected void ExceptionWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            try
            {
                if ((UIMode)ViewState[V_DISPLAYMODE] != UIMode.View)
                {
                    SODException exception = null;
                    exception = Exceptiondetails.GetException();
                    if (string.IsNullOrEmpty(exception.ExceptionCode))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Exception Code should not be empty");
                        return;
                    }
                    if (Exceptiondetails.ExceptionOwner != null)
                    {                        
                        exception.RiskOwner = Exceptiondetails.ExceptionOwner;
                        exception.RiskOwnerId = Exceptiondetails.ExceptionOwner.Id;
                        exception.ExceptionItems = new List<SODExceptionItem>();
                        if (UserDetails.SelectedExceptions.Count == 0)
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Please add at least one user detail");
                            return;
                        }
                        else
                        {
                            foreach (var entry in UserDetails.SelectedExceptions)
                            {
                                {
                                    if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit)
                                    {
                                        entry.ExceptionId = Convert.ToInt64(this.ContextObjectId);
                                    }
                                    exception.ExceptionItems.Add(entry);
                                }
                            }
                        }
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Exception Owner should not be empty");
                        return;
                    }
                    if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit)
                    {
                        exception.Id = Convert.ToInt64(this.ContextObjectId);
                    }
                    else
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("exceptionCode", exception.ExceptionCode);
                        SODException Exception = DomainApiClient<SODException>.GetByAction<SODException>
                            (DomainApiAction.SODException.Exception.GetByExceptionCode, parameter);
                        if (Exception != null)
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Exception code already exists");
                            return;
                        }
                    }
                    if ((UIMode)ViewState[V_DISPLAYMODE] != UIMode.Edit && (UIMode)ViewState[V_DISPLAYMODE] != UIMode.View)
                    {
                        DomainApiClient<SODException>.Save(exception);                      
                    }
                    else
                    {
                        DomainApiClient<SODException>.Save(exception);
                    }
                    Response.Redirect("~//Comp/Exception//List.aspx");                 
                }
                else
                {
                    Response.Redirect("~//Comp/Exception//List.aspx");
                }
            }
            catch (System.Exception ex)
            {
            }
        }
        #endregion

        #region Initialise Section
        public void InitializePage()
        {
            if (!IsPostBack)
            {
                if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.Edit)
                {
                    SetPage();
                }
                else if ((UIMode)ViewState[V_DISPLAYMODE] == UIMode.View)
                {
                    SetPage();
                    Exceptiondetails.DisplayMode();
                    UserDetails.DisplayMode();
                }
                Exceptiondetails.UIMode = (UIMode)ViewState[V_DISPLAYMODE];
                UserDetails.UIMode = (UIMode)ViewState[V_DISPLAYMODE];
                Exceptiondetails.InitializePage();
                UserDetails.InitializePage();
            }
        }
        protected void ExceptionRadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Response.Redirect("~/Comp/Exception/List.aspx");
        }
        #endregion 
    }
}
