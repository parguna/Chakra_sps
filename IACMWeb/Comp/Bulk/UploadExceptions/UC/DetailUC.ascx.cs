using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Bulk.UploadExceptions.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Class Member Variable
        private string uploadPath = null;
        private string templatePath = null;
        private string fileName = null;
        private User userExists=null;
        private List<String[]> stringRows = new List<string[]>();
        private string[] codes = null;
        private string[] rowNos = null;
        public List<string> errorsWhileUploading = null;
        public const string errorUpload = "ErrorUpload";
        #endregion

        #region Public Properties
        public List<string> ErrorUpload
        {
            get
            {
                return (List<string>)ViewState[errorUpload];
            }
            set
            {
                ViewState[errorUpload] = value;
            }
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                uploadPath = "~/Comp/Bulk/Data/";
                templatePath = "~/Comp/Bulk/";
                this.MasterPage.ActionClicked += ActionClicked;
            }
            BindGrid();
        }

        /// <summary>
        /// Method to initialise the grid grdUploadErrors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdUploadErrors_Init(object sender, EventArgs e)
        {

            GridFilterMenu menu = grdUploadErrors.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" || 
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Method to handle any click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            try
            {
                if (e.ActionType == ACLAction.Save)
                {
                    if (RadUploadFile.UploadedFiles.Count > 0)
                        foreach (UploadedFile file in RadUploadFile.UploadedFiles)
                        {
                            fileName = file.GetName();
                            string filExtension = file.GetExtension();
                            if (filExtension == ".csv")
                            {
                                file.SaveAs(Server.MapPath(uploadPath) + fileName);
                                if (radNewObject.Checked || radAddObject.Checked)
                                {
                                    if (radNewObject.Checked)
                                        errorsWhileUploading = UploadExceptionsForCreate(Server.MapPath(uploadPath) + fileName);
                                    else if (radAddObject.Checked)
                                        errorsWhileUploading = UploadExceptionsForAddingItems(Server.MapPath(uploadPath) + fileName);
                                    ErrorUpload = errorsWhileUploading;
                                }
                                else MasterPage.ShowAlert(AlertType.Error, "Please select an upload option");
                            }
                            // Throw alert if the uploaded file is not a csv file
                            else
                            {
                                MasterPage.ShowAlert(AlertType.Warning, "Please upload csv files only");
                            }
                            if (ErrorUpload != null && ErrorUpload.Count > 0)
                            {
                                BindGrid();
                            }
                        }
                    else MasterPage.ShowAlert(AlertType.Error, "Please upload a file to save");
                }
                else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
                {
                    grdUploadErrors.ExportSettings.FileName = "UploadExceptions_Errors";
                    grdUploadErrors.ExportSettings.FileName = "ExceptionError";
                    Export(grdUploadErrors,e.ActionType);
                }
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, ex.Message);
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to bind data to grid grdUploadErrors
        /// </summary>
        private void BindGrid()
        {
            grdUploadErrors.DataSource = ErrorUpload != null ? ErrorUpload : new List<string>();
            grdUploadErrors.DataBind();

        }

        /// <summary>
        /// Function for creating new Exceptions
        /// </summary>
        /// <param name="filename">Path of the uploaded file </param>
        /// <returns>List of UploadErrors </returns>
        private List<string> UploadExceptionsForCreate(string filename)
        {
            codes = new string[16];
            rowNos = new string[16];
            bool status = Common.CompareFileHeaders(Server.MapPath(uploadPath) + fileName, Server.MapPath(templatePath) + 
                                "ExceptionUpload - New.csv");
            if (status == false)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please upload file as per the given template");
                return new List<string>();
            }
            else
            {
                stringRows = Common.ReadTheCSVFile(filename, false);
                int counter = 1;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                
                foreach (string[] stringRow in stringRows)
                {
                    counter++;
                    parameter.Clear();
                    string exceptionCode = stringRow[Constants.Compliance_ExceptionUpload_Column_ExceptionCode];
                    string exceptionDesc = stringRow[Constants.Compliance_ExceptionUpload_Column_ExceptionDescription];
                    string riskOwner = stringRow[Constants.Compliance_ExceptionUpload_Column_RiskOwnerId];
                    List<string[]> duplicateRows = stringRows.Where
                      (s => s[Constants.Compliance_ExceptionUpload_Column_ExceptionCode] == exceptionCode).ToList();
                    if (duplicateRows.Count > 1)
                    {
                        rowNos[0] = rowNos[0] + counter.ToString() + ",";
                    }
                    parameter.Add("name", riskOwner);
                    userExists = DomainApiClient<User>.GetByAction<User>(DomainApiAction.User.GetByUserName, parameter);
                    parameter.Clear();
                    parameter.Add("exceptionCode", exceptionCode);
                    SODException existingException = DomainApiClient<SODException>.GetByAction<SODException>
                        (DomainApiAction.SODException.Exception.GetByExceptionCode, parameter);
                    if (existingException != null)
                    {
                        rowNos[8] = rowNos[8] + counter.ToString() + ",";
                        codes[8] = codes[8] + exceptionCode + ",";
                    }
                    else if (!Common.CheckCode(exceptionCode))
                    {
                        rowNos[15] = rowNos[15] + counter.ToString() + ",";
                        codes[15] = codes[15] + exceptionCode + ",";
                    }
                    else if (userExists == null)
                    {
                        rowNos[9] = rowNos[9] + counter.ToString() + ",";
                        codes[9] = codes[9] + riskOwner + ",";
                    }
                    else
                    {
                        SODException newException = new SODException();
                        newException.ExceptionCode = exceptionCode;
                        newException.ExceptionDesc = exceptionDesc;
                        newException.RiskOwnerId = userExists.Id;
                        newException.ExceptionItems = new List<SODExceptionItem>();
                        DomainApiClient<SODException>.Save(newException);
                    }
                }
                return Common.GetErrorsToUpload(codes, rowNos);
            }
        }

        /// <summary>
        /// Function for adding ExceptionItems 
        /// </summary>
        /// <param name="filename">Path of the uploaded file </param>
        /// <returns> List Of UploadErrors </returns>
        private List<string> UploadExceptionsForAddingItems(string filename)
        {
            codes = new string[16];
            rowNos = new string[16];
            bool status = Common.CompareFileHeaders(Server.MapPath(uploadPath) + fileName, Server.MapPath(templatePath) + "ExceptionUpload - Add.csv");
            if (status == false)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please upload file as per the given template");
                return new List<string>();
            }
            else
            {
                stringRows = Common.ReadTheCSVFile(filename, false);
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                int counter = 1;
                foreach (string[] stringRow in stringRows)
                {
                    counter++;
                    bool ErrorFound = false;
                    string exceptionCode = stringRow[Constants.Compliance_ExceptionUpload_Column_ExceptionCode];
                    string userName = stringRow[Constants.Compliance_ExceptionUpload_Column_UserName];
                    string function1 = stringRow[Constants.Compliance_ExceptionUpload_Column_Function1];
                    long SODFuncItem1 = Convert.ToInt64(stringRow[Constants.Compliance_ExceptionUpload_Column_FuncItemId1]);
                    string function2 = stringRow[Constants.Compliance_ExceptionUpload_Column_Function2];
                    long SODFuncItem2 = Convert.ToInt64(stringRow[Constants.Compliance_ExceptionUpload_Column_FuncItemId2]);

                    List<string[]> duplicateRows = stringRows.Where(
                        s => s[Constants.Compliance_ExceptionUpload_Column_ExceptionCode] == exceptionCode &&
                             s[Constants.Compliance_ExceptionUpload_Column_UserName] == userName &&
                             s[Constants.Compliance_ExceptionUpload_Column_Function1] == function1 &&
                            Convert.ToInt64(s[Constants.Compliance_ExceptionUpload_Column_FuncItemId1]) == SODFuncItem1 &&
                            s[Constants.Compliance_ExceptionUpload_Column_Function2] == function2 &&
                            Convert.ToInt64(s[Constants.Compliance_ExceptionUpload_Column_FuncItemId2]) == SODFuncItem2)
                            .ToList();

                    if (duplicateRows.Count > 1)
                    {
                        rowNos[0] = rowNos[0] + counter.ToString() + ",";
                        ErrorFound = false;
                    }
                    parameter.Clear();
                    parameter.Add("exceptionCode", exceptionCode);
                    SODException existingException = DomainApiClient<SODException>.GetByAction<SODException>
                                                     (DomainApiAction.SODException.Exception.GetByExceptionCode, parameter);
                    parameter.Clear();
                    parameter.Add("name", userName);
                    User user = DomainApiClient<User>.GetByAction<User>(DomainApiAction.User.GetByUserName, parameter);

                    parameter.Clear();
                    parameter.Add("sodFunctionCode", function1);
                    SODFunction existingFunction1 = DomainApiClient<SODFunction>.GetByAction<SODFunction>
                                                    (DomainApiAction.SODFunction.Function.GetBySodFunctionCodeIncludingSodFunctionItems, parameter);
                    parameter.Clear();
                    parameter.Add("sodFunctionCode", function2);
                    SODFunction existingFunction2 = DomainApiClient<SODFunction >.GetByAction<SODFunction>
                                                    (DomainApiAction.SODFunction.Function.GetBySodFunctionCodeIncludingSodFunctionItems, parameter);

                    parameter.Clear();
                    parameter.Add("id", SODFuncItem1.ToString());
                    SODFunctionItem existingFunItem1 = DomainApiClient<SODFunctionItem>.GetByAction<SODFunctionItem>
                                                    (DomainApiAction.SODFunction.SODFunctionItem.GetByIdWithNavigation, parameter);
                    parameter.Clear();
                    parameter.Add("id", SODFuncItem2.ToString());
                    SODFunctionItem existingFunItem2 = DomainApiClient<SODFunctionItem>.GetByAction<SODFunctionItem>
                                                    (DomainApiAction.SODFunction.SODFunctionItem.GetByIdWithNavigation, parameter);
                     
                    if (existingException == null)
                    {
                        rowNos[10] = rowNos[10] + counter.ToString() + ",";
                        codes[10] = codes[10] + exceptionCode + ",";
                        ErrorFound = false;
                    }
                    
                    else if (user == null)
                    {
                        rowNos[11] = rowNos[11] + counter.ToString() + ",";
                        codes[11] = codes[11] + userName + ",";
                        ErrorFound = false;
                    }
                    
                    else if (existingFunction1 == null)
                    {
                        rowNos[2] = rowNos[2] + counter.ToString() + ",";
                        codes[2] = codes[2] + function1 + ",";
                        ErrorFound = false;
                    }
                    
                    else if (existingFunction2 == null)
                    {
                        rowNos[2] = rowNos[2] + counter.ToString() + ",";
                        codes[2] = codes[2] + function2 + ",";
                        ErrorFound = false;
                    }
                   
                    else if (existingFunItem1 == null)
                    {
                        rowNos[12] = rowNos[12] + counter.ToString() + ",";
                        codes[12] = codes[12] + SODFuncItem1 + ",";
                        ErrorFound = false;
                    }
                  
                    else if (existingFunItem2 == null)
                    {
                        rowNos[12] = rowNos[12] + counter.ToString() + ",";
                        codes[12] = codes[12] + SODFuncItem2 + ",";
                        ErrorFound = false;
                    }
                  
                    SODExceptionItem ifExistingExceptionItem = null;
                    ifExistingExceptionItem = existingException.ExceptionItems.FirstOrDefault(s => s.UserId == user.Id && 
                                                 s.FunctionId1 == existingFunction1.Id && s.FunctionItemId1 == existingFunItem1.Id && 
                                                 s.FunctionId2 == existingFunction2.Id && s.FunctionItemId2 == existingFunItem2.Id);
                   
                    if (ifExistingExceptionItem != null)
                    {
                        rowNos[13] = rowNos[13] + counter.ToString() + ",";
                        codes[13] = codes[13] + exceptionCode + ",";
                        ErrorFound = false;
                    }
                   
                    if (ErrorFound != true)
                    {                        
                        SODExceptionItem newExItem = new SODExceptionItem();
                        newExItem.ExceptionId = existingException.Id;
                        newExItem.UserId = user.Id;
                        newExItem.FunctionId1 = existingFunItem1.SodFunctionId;
                        newExItem.FunctionId2 = existingFunItem2.SodFunctionId;
                        newExItem.FunctionItemId1 = existingFunItem1.Id;
                        newExItem.FunctionItemId2 = existingFunItem2.Id;
                        existingException.ExceptionItems.Add(newExItem);
                        DomainApiClient<SODException>.Save(existingException);                      
                    }
                }
                return Common.GetErrorsToUpload(codes, rowNos);
            }
        }
        #endregion
    }
}