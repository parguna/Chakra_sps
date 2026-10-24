using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Exception.Common
{
    public partial class UserDetailsUC :UIBaseUC
    {
        #region Class Member Variables
        SODExceptionItem _exceptionItem = null;
        SessionStatusImpl session = new SessionStatusImpl();
        private const string V_SELECTEDEXCEPTOINS = "SELECTEDEXCEPTIONS";
        private bool IsSaved = false;
        private List<SODExceptionItem> _exceptionItems=new List<SODExceptionItem>();
        private UIMode uiMode;
        #endregion

        #region public Properties
        public List<SODExceptionItem> SelectedExceptions
        {
            get
            {
                if (ViewState[V_SELECTEDEXCEPTOINS] != null)
                {
                    return (List<SODExceptionItem>)ViewState[V_SELECTEDEXCEPTOINS];
                }
                return new List<SODExceptionItem>();
            }
            set
            {
                ViewState[V_SELECTEDEXCEPTOINS] = value;
            }
        }

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODExceptionItem ExceptionItem
        {
            get { return _exceptionItem; }
            set
            {
                _exceptionItem = value == null ?
                    value = new SODExceptionItem()
                    {
                        user = UserDetails,
                        function1 = SodFunciton1,
                        function2 = SodFunction2,
                        functionitem1 = SodFunctionValue1,
                        functionitem2 = SodFunctionValue2
                    } : value;
            }
        }

        public List<SODExceptionItem> ExceptionItems
        {
            get { return _exceptionItems; }
            set { _exceptionItems = value; }
        }

        public SODFunction SodFunciton1
        {
            get { return FunSearch.SelectedFunction; }
            set { FunSearch.SelectedFunction = value; }
        }

        public SODFunction SodFunction2
        {
            get { return FunSearch2.SelectedFunction; }
            set { FunSearch2.SelectedFunction = value; }
        }

        public User UserDetails
        {
            get { return usrSearch.SelectedUser; }
            set { usrSearch.SelectedUser = value; }
        }

        public SODFunctionItem SodFunctionValue1
        {
            get { return FunSearch.SelectedFunctionItem; }
            set { FunSearch.SelectedFunctionItem = value; }
        }

        public SODFunctionItem SodFunctionValue2
        {
            get { return FunSearch2.SelectedFunctionItem; }
            set { FunSearch2.SelectedFunctionItem = value; }
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
        }

        /// <summary>
        /// Method to handle the Add item in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExceptionFunction_Click(object sender, EventArgs e)
        {        
            this.ExceptionItems = this.SelectedExceptions;
            if (CheckNullEntries())
            {
                if (FunSearch.SelectedFunctionItem.Id != FunSearch2.SelectedFunctionItem.Id)
                {
                    ExceptionItems.Add(new Dto.SODExceptionItem
                    {
                        user = usrSearch.SelectedUser,
                        UserId = usrSearch.SelectedUser.Id,
                        function1 = FunSearch.SelectedFunction,
                        FunctionId1 = FunSearch.SelectedFunction.Id,
                        function2 = FunSearch2.SelectedFunction,
                        FunctionId2 = FunSearch2.SelectedFunction.Id,
                        functionitem1 = FunSearch.SelectedFunctionItem,
                        FunctionItemId1 = FunSearch.SelectedFunctionItem.Id,
                        functionitem2 = FunSearch2.SelectedFunctionItem,
                        FunctionItemId2 = FunSearch2.SelectedFunctionItem.Id
                    });
                    this.usrSearch.Clear();
                    this.FunSearch.Clear();
                    this.FunSearch2.Clear();
                    this.FunSearch.ClearValueSearch();
                    this.FunSearch2.ClearValueSearch();
                    this.SelectedExceptions = ExceptionItems;
                    BindExceptionItemGrid();
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "Function Value 1 and Function Value 2 should not be same");
                }
            }
            else
                MasterPage.ShowAlert(AlertType.Error, "All the user fields are mandatory");
        }

        /// <summary>
        /// Method to handle ItemCommand event for grdExceptionItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdExceptionItem_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                long key = (long)Convert.ChangeType(item.GetDataKeyValue("Id"), typeof(long));
                SelectedExceptions.RemoveAll(x => x.Id == key);
                BindExceptionItemGrid();
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to Bind Exception Items in the Grid
        /// </summary>
        private void BindExceptionItemGrid()
        {
            this.grdExceptionItem.DataSource = this.SelectedExceptions;
            this.grdExceptionItem.DataBind();
        }

        /// <summary>
        /// Method to check null entries
        /// </summary>
        /// <returns></returns>
        private bool CheckNullEntries()
        {
            if (usrSearch.SelectedUser != null && FunSearch.SelectedFunction != null && FunSearch.SelectedFunctionItem!=null &&
                FunSearch2.SelectedFunction!=null && FunSearch2.SelectedFunctionItem != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Method to Get the Exception Item 
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public SODExceptionItem GetExceptionItem(long ItemId)
        {
            SODExceptionItem sodException = new SODExceptionItem();
            sodException.Id = Convert.ToInt64(ItemId);
            sodException.ExceptionId = Convert.ToInt64(ItemId);
            sodException.user = usrSearch.SelectedUser;
            sodException.UserId = usrSearch.SelectedUser.Id;
            sodException.function1 = FunSearch.SelectedFunction;
            sodException.FunctionId1 = FunSearch.SelectedFunction.Id ;
            sodException.function2 = FunSearch2.SelectedFunction;
            sodException.FunctionId2 = FunSearch2.SelectedFunction.Id;
            sodException.functionitem1 = FunSearch.SelectedFunctionItem;
            sodException.FunctionItemId1 = FunSearch.SelectedFunctionItem.SodFunctionId;
            sodException.functionitem2 = FunSearch2.SelectedFunctionItem;
            sodException.FunctionItemId2 = FunSearch2.SelectedFunctionItem.SodFunctionId;
            return sodException;
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to Initialize the page
        /// </summary>
        public void InitializePage()
        {
            if (UIMode==UIMode.Edit || UIMode==UIMode.View)
            {
                if (!IsPostBack)
                    this.SelectedExceptions = ExceptionItems;
                BindExceptionItemGrid();
            }
            else
            {
                BindExceptionItemGrid();
            }       
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to open the screen in the View Mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion     
    }
}