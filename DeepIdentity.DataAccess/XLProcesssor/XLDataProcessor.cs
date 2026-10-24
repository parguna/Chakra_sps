using System.Collections.Generic;
using System.Data;
using System.Linq;
using DeepIdentity.Model;

namespace DeepIdentity.DataAccess
{
    public class XLDataProcessor
    {
        private readonly IList<IXLData> _invalidList = new List<IXLData>();

        public XLTemplates TemplateName { get; set; }

        public string FileName { get; set; }

        public bool HasInvalidData
        {
            get { return InvalidList.Count > 0; }
        }

        public IList<IXLData> InvalidList
        {
            get { return _invalidList; }
        }

        public bool IsValidFile(ref string _validationMsg)
        {
            var ColumnList = new ExcelConnect().GetHeaderColumnListFromExcel(FileName);
            if (ColumnList.Contains("ValidationMessage"))
            {
                ColumnList.Remove("ValidationMessage");
            }

            var validator = XLDataValidator.GetXLDataValidator(TemplateName);

            if (!ColumnList.SequenceEqual(XLDataValidator.GetFields(TemplateName)))
            {
                _validationMsg = "InValid Excel File. Columns Mismatched";
                return false;
            }

            return true;
        }

        public void Truncate()
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            validator.TruncateTable();
        }

        public virtual bool Validate(IXLData data, XLDataValidator validator)
        {
            var validationMsg = string.Empty;

            if (!validator.Validate(ref validationMsg))
            {
                data.IsValid = false;
                data.ValidationMessage = validationMsg;
                _invalidList.Add(data);
                return false;
            }
            return true;
        }

        public IXLData GetObjectFromData(DataRow row)
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            return validator.GetObjectFromData(row);
        }

        public bool IsEmptyRow(IXLData data)
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            return validator.IsEmptyRow(data);
        }

        public virtual void Process(IXLData data, XLTemplates templateName)
        {
            var validator = XLDataValidator.GetXLDataValidator(templateName);
            validator.Data = data;

            var isValid = Validate(data, validator);

            if (isValid)
            {
                if (!validator.IsExists())
                    validator.Add();
            }
        }

        public virtual void PostProcess()
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            validator.PostProcess();
        }
    }
}