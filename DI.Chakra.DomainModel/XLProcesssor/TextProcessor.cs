using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DI.Chakra.DomainModel
{
    public class TextProcessor
    {
        private readonly XLDataProcessor _processor;
        private string _validationMsg = string.Empty;

        public TextProcessor()
        {
            _processor = new XLDataProcessor();
        }

        public string FileName { get; set; }
        public string xmlData { get; set; }
        public bool PartialUpload { get; set; }
        public XLTemplates TemplateName { get; set; }

        public long EndpointSystemId { get; set; }

        public bool HasInvalidData
        {
            get { return _processor.InvalidList.Count > 0; }
        }

        public bool HasvalidData
        {
            get { return _processor.ValidList.Count > 0; }
        }
        public IList<IXLData> InvalidList
        {
            get { return _processor.InvalidList; }
        }

        public List<IXLData> _ValidandInvalid;

        public IList<IXLData> ValidList
        {
            get { return _processor.ValidList; }
        }

        public string ValidateFileColumns()
        {
            _processor.TemplateName = TemplateName;
            _processor.FileName = FileName;
            _processor.xmlData = xmlData;

            if (xmlData == "" && !_processor.IsValidFile(ref _validationMsg))
                return _validationMsg;

            return string.Empty;
        }
        private DataTable GetDataTable(string xmlData)
        {
            try
            {
                StringReader theReader = new StringReader(xmlData);
                DataSet theDataSet = new DataSet();
                theDataSet.ReadXml(theReader);

                return theDataSet.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string StartProcess(string currentUser)
        {
            IXLData data;
            IList<IXLData> info = new List<IXLData>();
            _processor.TemplateName = TemplateName;
            _processor.FileName = FileName;
            _processor.xmlData = xmlData;
            _processor.UploadDateTime = DateTime.Now;
            _processor.PartialUpload = PartialUpload;
            _processor.canClearListobj = true;
            // if (!_processor.IsValidFile(ref _validationMsg))
            //  return _validationMsg;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("xmlData");
            if (xmlData == null || xmlData == "")
            {
                dt = new TextDataConnect().ReadCSV(FileName);
            }
            else
            {
                if (xmlData != "")
                {
                    dt = new DataTable("xmlData");
                    dt = GetDataTable(xmlData);
                }
            }

            bool fromFile = false;
            if (!string.IsNullOrWhiteSpace(FileName))
            {
                if (_processor.LstMatrixTemplates.Contains(TemplateName))
                    fromFile = true;
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!PartialUpload)
                        _processor.Truncate();
                }
            }

            var validator = XLDataValidator.GetXLDataValidator(TemplateName);

            foreach (DataRow dRow in dt.Rows)
            {
                if (TemplateName.ToString() == "ENTITLEMENTS")
                {
                    data = _processor.GetObjectFromData(dRow, EndpointSystemId, validator);
                }
                else
                {
                    data = _processor.GetObjectFromData(dRow, validator);
                }
                if (data != null)
                {
                    if (_processor.IsEmptyRow(data, validator))
                        return string.Empty;
                    _processor.Process(data, TemplateName, currentUser, validator, false, fromFile);
                    _processor.canClearListobj = false;
                    info.Add(data);
                }
            }

            if (TemplateName.ToString() == "PROVISIONINGDEFAULTS" || TemplateName.ToString() == "CATALOGMANAGEMENT"
                || _processor.LstMatrixTemplates.Contains(TemplateName))
            {
                if (_processor.ValidList.Count > 0 && _processor.InvalidList.Count > 0)
                {
                    _ValidandInvalid = new List<IXLData>();
                    _ValidandInvalid = info.ToList();
                }
            }

            //update  account recon and entitlement recon for valid records alone.
            _processor.PostProcess(currentUser);

            return string.Empty;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public IList<string> GetAllConfigurationOfApprover(string employeeId)
        {
            IList<string> config = new List<string>();
            XLDataValidator validator = null;
            foreach (XLTemplates item in Enum.GetValues(typeof(XLTemplates)))
            {
                validator = XLDataValidator.GetXLDataValidator(item);
                if (validator != null && validator.IsEmployeeConfiguredAsApprover(employeeId))
                    config.Add(UtilityMethods.GetEnumAttribute<XLTemplateFriendlyName>(item));
            }

            return config;
        }


        /* public string GetMessage(string menuname)
         {
             try
             {
                 using (var _context = new IACMDBContext())
                 {
                     return
                         _context.UILog.Where(lq => lq.MenuName == menuname)
                             .Select(lq => lq.MessageText)
                             .ToList()
                             .LastOrDefault();
                 }
             }
             catch
             {
                 throw;
             }
         }*/


    }
}
