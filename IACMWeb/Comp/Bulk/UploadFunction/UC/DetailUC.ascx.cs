using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Bulk.UploadFunction.UC
{
    public partial class DetailUC : UIBaseUC
    {
        #region Class Member Variable
        SODFunction functn = new SODFunction();
        private string uploadPath = null;
        private string templatePath = null;
        private string fileName = null;
        private SessionStatusImpl session = new SessionStatusImpl(); 
        private List<string[]> stringRows = new List<string[]>();
        private string[] codes=null;
        private string[] rowNos=null;       
        public List<string> errorsWhileUploading=null;
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
        /// Method to initialise grid grdUploadErrors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdUploadErrors_Init(object sender, EventArgs e)
        {
            GridFilterMenu menu = grdUploadErrors.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || 
                    menu.Items[i].Text == "EqualTo" || menu.Items[i].Text == "GreaterThan" ||
                    menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" ||
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
                    // Check if any file has been uploaded
                    if (RadUploadFile.UploadedFiles.Count > 0)
                        foreach (UploadedFile file in RadUploadFile.UploadedFiles)
                        {
                            fileName = file.GetName();
                            string filExtension = file.GetExtension();
                            // Check the uploaded file extension
                            if (filExtension == ".csv")
                            {
                                if (radNewObject.Checked || radAddObject.Checked)
                                {
                                    file.SaveAs(Server.MapPath(uploadPath) + fileName);
                                    if (radNewObject.Checked)
                                        errorsWhileUploading = UploadFunctionsForCreate(Server.MapPath(uploadPath) + fileName);
                                    else if (radAddObject.Checked)
                                        errorsWhileUploading = UploadFunctionsForAddingItems(Server.MapPath(uploadPath) + fileName);
                                    ErrorUpload = errorsWhileUploading;
                                }
                                else MasterPage.ShowAlert(AlertType.Error, "Please select an upload option");
                            }
                            // Throw alert if the uploaded file is not a csv file
                            else
                            {
                                MasterPage.ShowAlert(AlertType.Warning, "Please Upload csv files only");
                            }
                            if (errorsWhileUploading != null && errorsWhileUploading.Count > 0)
                            {
                                BindGrid();
                            }
                        }
                    // Throw alert if no file is uploaded 
                    else MasterPage.ShowAlert(AlertType.Error, "Please upload a file to save");
                }
                else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
                {
                    grdUploadErrors.ExportSettings.FileName = "UploadFunctions_Errors";
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
        /// Method for creating new Functions
        /// </summary>
        /// <param name="filename">Path of the uploaded file </param>
        /// <returns>List of UploadErrors </returns>
        private List<string> UploadFunctionsForCreate(string filename)
        {
            codes = new string[16];
            rowNos = new string[16];
            bool status = Common.CompareFileHeaders(Server.MapPath(uploadPath) + fileName, 
                            Server.MapPath(templatePath) + "FunctionUpload - New.csv");
            if (status == false)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please upload file as per the given template");
                return new List<string>();
            }
            else
            {
                // Parse the uploaded csv file containing functions
                stringRows = Common.ReadTheCSVFile(filename, false);
                int counter = 1;
                foreach (string[] stringRow in stringRows)
                {
                    counter++;
                    string functionCode = stringRow[Constants.Compliance_FunctionUpload_Column_FunctionCode];
                    string functionDesc = stringRow[Constants.Compliance_FunctionUpload_Column_FunctionDescription];
                    // Check for duplicate records in the uploaded file.
                    List<string[]> duplicateRows = stringRows.Where
                        (s => s[Constants.Compliance_FunctionUpload_Column_FunctionCode] == functionCode).ToList();
                    // Throw error if duplicate records exist.
                    if (duplicateRows.Count > 1)
                    {
                        rowNos[0] = rowNos[0] + counter.ToString() + ",";
                    }
                    else
                    {                       
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("sodFunctionCode", functionCode);
                        SODFunction existingFunction = DomainApiClient<SODFunction>.GetByAction< SODFunction>
                            (DomainApiAction.SODFunction.Function.GetBySodFunctionCodeIncludingSodFunctionItems,parameter);
                        
                        if (existingFunction != null)
                        {
                            rowNos[1] = rowNos[1] + counter.ToString() + ",";
                            codes[1] = codes[1] + existingFunction.SodFunctionCode + ",";
                        }
                        else if (!Common.CheckCode(functionCode))
                        {
                            rowNos[14] = rowNos[14] + counter.ToString() + ",";
                            codes[14] = codes[14] + functionCode + ",";
                        }
                        //Save the function if its new
                        else
                        {
                            SODFunction newFunction = new SODFunction();
                            newFunction.SodFunctionCode = functionCode;
                            newFunction.SodFunctionDescription = functionDesc;
                            newFunction.IsActive = true;
                            newFunction.SodFunctionItems = new List<SODFunctionItem>();
                            DomainApiClient<SODFunction>.Save(newFunction);
                        }
                    }
                }
                return Common.GetErrorsToUpload(codes, rowNos);
            }
        }

        /// <summary>
        /// Method to bind data to grid grdUploadErrors
        /// </summary>
        private void BindGrid()
        {
            grdUploadErrors.DataSource = ErrorUpload != null ? ErrorUpload : new List<string>();
            grdUploadErrors.DataBind();
        }
        
        /// <summary>
        /// Method for adding FunctionItems 
        /// </summary>
        /// <param name="filename">Path of the uploaded file </param>
        /// <returns> List of UploadErrors </returns>
        private List<string> UploadFunctionsForAddingItems(string filename)
        {
            codes = new string[16];
            rowNos = new string[16];
            bool status = Common.CompareFileHeaders(Server.MapPath(uploadPath) + fileName, 
                            Server.MapPath(templatePath) + "FunctionUpload - Add.csv");
            if (status == false)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please upload file as per the given template");
                return new List<string>();
            }
            else
            {
                // Parse the uploaded csv file containing functions
                stringRows = Common.ReadTheCSVFile(filename, false);
                int counter = 1;
                foreach (string[] stringRow in stringRows)
                {
                    counter++;
                    bool ErrorFound = false;
                    string roleId = null;
                    string functionCode = stringRow[Constants.Compliance_FunctionUpload_Column_FunctionCode];
                    string endpointSystemType = stringRow[Constants.Compliance_FunctionUpload_Column_EndpointSystemType];
                    string endpointSystem = stringRow[Constants.Compliance_FunctionUpload_Column_EndpointSystem];
                    string functionType = stringRow[Constants.Compliance_FunctionUpload_Column_FunctionType];
                    string functionValue = stringRow[Constants.Compliance_FunctionUpload_Column_FunctionValue];

                    // Check for duplicate records in the uploaded file.
                    List<string[]> duplicateRows = stringRows.Where(
                        s => s[Constants.Compliance_FunctionUpload_Column_FunctionCode] == functionCode &&
                             s[Constants.Compliance_FunctionUpload_Column_EndpointSystemType] == endpointSystemType &&
                             s[Constants.Compliance_FunctionUpload_Column_EndpointSystem] == endpointSystem &&
                            s[Constants.Compliance_FunctionUpload_Column_FunctionType] == functionType &&
                            s[Constants.Compliance_FunctionUpload_Column_FunctionValue] == functionValue)
                            .ToList();
                    // Throw error if duplicate records exist.
                    if (duplicateRows.Count > 1)
                    {
                        rowNos[0] = rowNos[0] + counter.ToString() + ",";
                        ErrorFound = true;
                    }
                    else
                    {
                        // Check if function already exists.
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("sodFunctionCode", functionCode);
                        SODFunction existingFunction = DomainApiClient<SODFunction>.GetByAction<SODFunction>
                            (DomainApiAction.SODFunction.Function.GetBySodFunctionCodeIncludingSodFunctionItems, parameter);

                        parameter.Clear();
                        parameter.Add("displayName", endpointSystem);
                        EndPoint endpoint = DomainApiClient<EndPoint>.GetByAction<EndPoint>
                            (DomainApiAction.Endpoint.EndPoint.GetByDisplayNameIncludingEndPointSystemType, parameter);

                        // Throw error if function does not exist.
                        if (existingFunction == null)
                        {
                            rowNos[2] = rowNos[2] + counter.ToString() + ",";
                            codes[2] = codes[2] + functionCode + ",";
                            ErrorFound = true;
                        }
                        // Check if endpoint already exists.
                        else
                        {
                            if (endpoint != null)
                            {
                                //Check Function Value based on endpoint ,functiontype and objectCode.
                                bool FunctionValueFound = false;                                
                                if (endpointSystemType == EndpointType.SAP.ToString())
                                {
                                    if (string.Compare(functionType, "Role", true) == 0)
                                    {
                                        Dictionary<string, string> sparameter = new Dictionary<string, string>();
                                        sparameter.Add("roleCode", functionValue);
                                        sparameter.Add("endPointSystemId", endpoint.Id.ToString());
                                        SAPRoleInfo saprole = DomainApiClient<SAPRoleInfo>.GetByAction<SAPRoleInfo>
                                            (DomainApiAction.Endpoint.SAP.SAPRoleinfo.GetByRoleCode, sparameter);

                                        if (saprole != null)
                                        {
                                            roleId = saprole.Id.ToString();
                                            FunctionValueFound = true;
                                        }
                                    }
                                    else if (string.Compare(functionType, "Tcodes", true) == 0)
                                    {
                                        Dictionary<string, string> sparameter = new Dictionary<string, string>();
                                        sparameter.Add("tCode", functionValue);
                                        SAPTCodeInfo Tcode = DomainApiClient<SAPTCodeInfo>.GetByAction<SAPTCodeInfo>
                                            (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetByTCode, sparameter);

                                        if (Tcode != null)
                                        {
                                            roleId = Tcode.Id.ToString();
                                            FunctionValueFound = true;
                                        }
                                    }
                                    else if (string.Compare(functionType, "Profile", true) == 0)
                                    {
                                        Dictionary<string, string> spparameter = new Dictionary<string, string>();
                                        spparameter.Add("profileCode", functionValue);
                                        spparameter.Add("endPointSystemId", endpoint.Id.ToString());
                                        SAPProfileInfo sapprofile = DomainApiClient<SAPProfileInfo>.GetByAction<SAPProfileInfo>
                                            (DomainApiAction.Endpoint.SAP.SAPProfileInfo.GetByProfileCode, spparameter);

                                        if (sapprofile != null)
                                        {
                                            roleId = sapprofile.Id.ToString();
                                            FunctionValueFound = true;
                                        }
                                    }
                                }
                                else if (endpointSystemType == EndpointType.ActiveDirectory.ToString())
                                {
                                    if (string.Compare(functionType, "Group", true) == 0)
                                    {
                                        Dictionary<string, string> aparameter = new Dictionary<string, string>();
                                        aparameter.Add("cn", functionValue);
                                        ADSGroupInfo adsgroup = DomainApiClient<ADSGroupInfo>.GetByAction<ADSGroupInfo>
                                            (DomainApiAction.Endpoint.ActiveDirectory.ADSGroupInfo.GetByCn, aparameter);

                                        if (adsgroup != null)
                                        {
                                            roleId = adsgroup.Id.ToString();
                                            FunctionValueFound = true;
                                        }
                                    }
                                }
                               
                                // Throw error if Function Value does not exist and don't save the function item.
                                if (!FunctionValueFound)
                                {
                                    rowNos[5] = rowNos[5] + counter.ToString() + ",";
                                    codes[5] = codes[5] + functionValue + ",";
                                    ErrorFound = true;
                                }
                                if (FunctionValueFound)
                                {
                                    // Check if the function item alreaddy exists for the function.
                                    SODFunctionItem fItem = null;
                                    fItem = existingFunction.SodFunctionItems.FirstOrDefault(s => s.SodRoleDesc == functionValue && 
                                            s.SodFunctionType == functionType && s.Endpointsystemid == endpoint.Id && 
                                            s.EndpointSystemTypeId == endpoint.EndpointSystemTypeId);

                                    // Throw error if function item already exists and don't save the function item.
                                    if (fItem != null)
                                    {
                                        rowNos[4] = rowNos[4] + counter.ToString() + ",";
                                        codes[4] = codes[4] + functionCode + ",";
                                        ErrorFound = true;
                                    }
                                }
                            }
                            // Throw error if endpoint does not exist and don't save the function item.
                            else
                            {
                                rowNos[3] = rowNos[3] + counter.ToString() + ",";
                                codes[3] = codes[3] + endpointSystem + ",";
                                ErrorFound = true;
                            }
                        }
                        // Save the function item.
                        if (ErrorFound != true)
                        {
                            SODFunctionItem newItem = new SODFunctionItem();
                            newItem.SodFunctionId = existingFunction.Id;
                            newItem.Endpointsystemid = endpoint.Id;
                            newItem.EndpointSystemTypeId = endpoint.EndpointSystemTypeId;
                            newItem.SodFunctionType = functionType;
                            newItem.SodRoleDesc = functionValue;
                            newItem.SodRoleId = roleId;
                            existingFunction.SodFunctionItems.Add(newItem);
                            DomainApiClient<SODFunction>.Save(existingFunction);
                        }
                    }
                }
                return Common.GetErrorsToUpload(codes, rowNos);
            }
        }
        #endregion
    }
}
