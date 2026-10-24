using DI.Chakra.Infrastructure;

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DI.Chakra.Web;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using DI.Chakra.Logger;

public partial class Provisioning_XLUpload_XLUploadUC : DI.Chakra.Web.UIBaseUC
{
    private static string errMsg;

    private static IList<DI.Chakra.Dto.IXLData> invalid;
    private static IList<DI.Chakra.Dto.IXLData> valid;

    public event EventHandler UserControlButtonClicked;

    public IList<DI.Chakra.Dto.IXLData> InvalidList
    {
        get
        {
            return invalid;
        }
    }

    public IList<DI.Chakra.Dto.IXLData> ValidList
    {
        get
        {
            return valid;
        }
    }
    public XLTemplates TemplateName { get; set; }

    public long EndpointsystemId { get; set; }

    public string ErrorMessage
    {
        get { return errMsg; }
        set { errMsg = value; }
    }

    private void OnUserControlButtonClick()
    {
        if (UserControlButtonClicked != null)
        {
            UserControlButtonClicked(this, EventArgs.Empty);
        }
    }
    public bool IsPartialUpload
    {
        set
        {
            chkPartial.Checked = value;
            chkPartial.Enabled = !value;
        }
    }
    private string _path = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
    }
    public DataTable ReadCSV(string filename)
    {
        try
        {
            DataTable csvData = new DataTable("xmlData");

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.SetDelimiters(",");
                parser.TrimWhiteSpace = true;
                parser.HasFieldsEnclosedInQuotes = true;
                string[] colFields = parser.ReadFields();
                if (colFields != null)
                {
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!parser.EndOfData)
                    {
                        string[] fieldData = parser.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
              
                
            }

            return csvData;
        }
        catch (System.Exception ex)
        {
            DILog.UILogger.WriteLog(null, LogLevelType.Info, "exception : " + ex.Message);
            return null;
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        try
        {
            invalid = new List<IXLData>();
            valid = new List<IXLData>();
            if (!string.IsNullOrWhiteSpace(_path))
            {
                errMsg = string.Empty;
                DataTable dtXMLData = ReadCSV(_path);
                if (dtXMLData.Rows.Count > 0)
                {
                    //Set Null values are String empty (""). It will be used to include the empty node in XML while converting datatable into XML
                    SetValueForNull(dtXMLData);

                    string result;
                    if (TemplateName.ToString() == "SAP_SODConflictedBusinessRoles")
                    {

                        var res = from c in dtXMLData.AsEnumerable()
                                  group c by new
                                  {
                                      EndpointCode = c.Field<string>("Endpoint Code"),
                                      Technicalrole = c.Field<string>("Technicalrole"),
                                      ConflictingTechnicalrole = c.Field<string>("Conflicting Technicalrole")

                                  } into g
                                  where g.Count() > 1
                                  select new { g.Key.EndpointCode, g.Key.Technicalrole, g.Key.ConflictingTechnicalrole };

                        if (res.Count() > 0)
                        {
                            ErrorMessage = "There are duplicate records in file";
                            MasterPage.ShowAlert(AlertType.Error, ErrorMessage);
                            return;
                        }

                    }
                    using (StringWriter sw = new StringWriter())
                    {
                        dtXMLData.WriteXml(sw);
                        result = sw.ToString();
                    }
                    object objResult = (object)result;

                    Dictionary<string, string> parameterValidation = new Dictionary<string, string>();
                    parameterValidation.Add("fileName", _path);
                    parameterValidation.Add("templateName", TemplateName.ToString());
                    HeaderText.Disabled = true;
                    btnCancel.Disabled = true;

                    if (DomainApiAction.XLProcessor.LstMatrixTemplates.Contains(TemplateName))
                    {
                        errMsg = DomainApiClient<IXLData>.GetByAction<string>(DomainApiAction.XLProcessor.XLData.ValidateFileColumns, parameterValidation, objResult);
                    }

                    if (TemplateName.ToString() == "PROVISIONINGDEFAULTS" || TemplateName.ToString() == "CATALOGMANAGEMENT" || TemplateName.ToString() == "ENTITLEMENTS")
                    {
                        errMsg = DomainApiClient<IXLData>.GetByAction<string>(DomainApiAction.XLProcessor.XLData.ValidateFileColumns, parameterValidation, objResult);
                    }

                   
                    if ((errMsg == null) || (errMsg == ""))
                    {
                        Dictionary<string, string> parameterProcess = new Dictionary<string, string>();
                        parameterProcess.Add("fileName", _path);
                        parameterProcess.Add("templateName", TemplateName.ToString());
                        parameterProcess.Add("errorMessage", errMsg);
                        IList<DI.Chakra.Dto.IXLData> datas;
                        if (TemplateName.ToString() == "ENTITLEMENTS")
                        {
                            parameterProcess.Add("endpointSystemId", EndpointsystemId.ToString());
                            datas = DomainApiClient<IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXLForEntitlment, parameterProcess, objResult);
                        }
                        else
                        {
                            parameterProcess.Add("partialUpload", chkPartial.Checked.ToString());
                            datas = DomainApiClient<IXLData>.GetByAction<IList<DI.Chakra.Dto.IXLData>>(DomainApiAction.XLProcessor.XLData.ProcessXL, parameterProcess, objResult);
                        }
                        
                        if (TemplateName.ToString() == "PROVISIONINGDEFAULTS" || TemplateName.ToString() == "CATALOGMANAGEMENT")
                        {
                            if (datas != null && datas.Count > 0)
                            {

                                foreach (DI.Chakra.Dto.IXLData data in datas)
                                {
                                    string hasvalue = data.ValidationMessage;

                                    if (string.IsNullOrEmpty(hasvalue))
                                    {
                                        IXLData d = data;
                                        valid.Add(d);
                                    }
                                    else
                                    {
                                        IXLData d = data;
                                        invalid.Add(d);
                                    }
                                }
                            }
                        }
                       else
                        {
                            if (datas != null && datas.Count > 0)
                            {
                                foreach (DI.Chakra.Dto.IXLData data in datas)
                                {
                                    string hasvalue = data.ValidationMessage;

                                    if (string.IsNullOrWhiteSpace(hasvalue))
                                    {
                                        IXLData d = data;
                                        valid.Add(d);
                                    }
                                    else
                                    {
                                        IXLData d = data;
                                        invalid.Add(d);
                                    }
                                }
                            }
                        }
                    }
                    if (invalid.Count > 0 && (TemplateName == XLTemplates.Inhouse_Permissions || TemplateName == XLTemplates.Inhouse_UserInfo))
                    {
                        ErrorMessage = "There are some invalid entries";
                    }

                    OnUserControlButtonClick();
                    HeaderText.Disabled = false;
                    btnCancel.Disabled = false;
                }

                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No record to export");
                }
            }
               
             
        }
        catch (Exception ex)
        {
            HeaderText.Disabled = false;
            btnCancel.Disabled = false;
            return;
        }

    }
    public void SetValueForNull(DataTable dt)
    {
        int firstIndex;
        int secondIndex;
        for (firstIndex = 0; firstIndex < dt.Columns.Count; firstIndex++)
        {
            for (secondIndex = 0; secondIndex < dt.Rows.Count; secondIndex++)
            {
                if (dt.Columns[firstIndex].DataType.ToString() == "System.Int32" || dt.Columns[firstIndex].DataType.ToString() == "System.Single" || dt.Columns[firstIndex].DataType.ToString() == "System.Double" || dt.Columns[firstIndex].DataType.ToString() == "System.Decimal")
                {
                    if (dt.Rows[secondIndex][firstIndex] == DBNull.Value)
                        dt.Rows[secondIndex][firstIndex] = 0;
                }
                else if (dt.Columns[firstIndex].DataType.ToString() == "System.String")
                {
                    if (dt.Rows[secondIndex][firstIndex] == DBNull.Value || dt.Rows[secondIndex][firstIndex].ToString().Trim() == "")
                        dt.Rows[secondIndex][firstIndex] = "";
                }
            }
        }
    }
    [DllImport("advapi32.DLL", SetLastError = true)]
    public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
    protected void FileRadAsyncUpload_FileUploaded(object sender, Telerik.Web.UI.FileUploadedEventArgs e)
    {
        try
        {
            string relativePath = "~/";
            var filename = e.File.FileName;
            string dirPath = string.Empty;
            dirPath = MapPath(relativePath) + @"XLUploadFiles\";
            _path = dirPath + filename;

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            if (e.File.GetExtension() != ".csv")
            {
                MasterPage.ShowAlert(AlertType.Success, "File Format is Wrong");
            }

            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
            e.File.SaveAs(_path, true);

        }
        catch (Exception ex)
        {
            //HeaderText.Disabled = false;
            //btnCancel.Disabled = false;
            //return;     
            DILog.UILogger.WriteLog(null, LogLevelType.Info, "exception : " + ex.Message);
        }
    }
}
