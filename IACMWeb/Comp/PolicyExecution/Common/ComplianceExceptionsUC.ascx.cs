using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Comp.PolicyExecution.Common
{
    public partial class ComplianceExceptionsUC : UIBaseUC
    {
        #region Class Member Variables

        private SODPolicyExecutionDefinition sodDefinition = new SODPolicyExecutionDefinition();        
        private const string V_ID = "ENTITY_ID";
        private const string V_SELECTEDEXCEPTIONS = "SELECTEDEXCEPTIONS";
        private UIMode uiMode;
        private const string V_SODFunction = "SODPolicyExecutionDefinition";
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

        public Dictionary<long, string> SelectedExceptions
        {
            get
            {
                if (ViewState[V_SELECTEDEXCEPTIONS] != null)
                {
                    return (Dictionary<long, string>)ViewState[V_SELECTEDEXCEPTIONS];
                }
                return new Dictionary<long, string>();
            }
            set
            {
                ViewState[V_SELECTEDEXCEPTIONS] = value;
            }
        }

        public SODPolicyExecutionDefinition PolicyDefinition
        {
            get
            {
                if (ViewState[V_SODFunction] == null)
                    return null;
                return (SODPolicyExecutionDefinition)ViewState[V_SODFunction];
            }
            set
            {
                ViewState[V_SODFunction] = value;
            }
        }
        #endregion

        #region Page and Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnException_Click(object sender, EventArgs e)
        {
            SODException exception = ExceptionSearch.SelectedSodException;
            if (exception != null)
            {
                AddExceptions(exception.Id, exception.ExceptionCode + " - " + exception.ExceptionDesc);
            }
            BindExceptionsGrid();
        }

        protected void grdExceptions_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Key"), typeof(long));
                var exceptions = this.SelectedExceptions;
                exceptions.Remove(key);
                this.SelectedExceptions = exceptions;
                BindExceptionsGrid();
            }
        }
        #endregion
        
        #region Initialise Section
        public void InitilizePage()
        {
            if (UIMode==UIMode.Edit || UIMode==UIMode.View)
            {
                if (PolicyDefinition != null && PolicyDefinition.Exceptions.Count > 0)
                {
                    foreach (SODException exception in PolicyDefinition.Exceptions)
                    {
                        AddExceptions(exception.Id, exception.ExceptionCode + " - " + exception.ExceptionDesc);
                    }
                }
            }
            BindExceptionsGrid();
        }
        #endregion

        #region Public Methods
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion
        
        #region Helpers
        private void BindExceptionsGrid()
        {
            grdExceptions.DataSource = this.SelectedExceptions;
            grdExceptions.DataBind();
            ExceptionSearch.Clear();
        }

        private void AddExceptions(long id, string exceptionName)
        {
            var exceptions = this.SelectedExceptions;
            if (!exceptions.ContainsKey(id))
            {
                exceptions.Add(id, exceptionName);
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Warning, "Exception already exists");
            }
            this.SelectedExceptions = exceptions;
        }
        #endregion
    }
}