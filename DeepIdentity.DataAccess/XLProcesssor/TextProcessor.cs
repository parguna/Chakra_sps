using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;

namespace DeepIdentity.DataAccess
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
        public XLTemplates TemplateName { get; set; }

        public bool HasInvalidData
        {
            get { return _processor.InvalidList.Count > 0; }
        }

        public IList<IXLData> InvalidList
        {
            get { return _processor.InvalidList; }
        }

        public string StartProcess()
        {
            IXLData data;
            _processor.TemplateName = TemplateName;
            _processor.FileName = FileName;
            if (!_processor.IsValidFile(ref _validationMsg))
                return _validationMsg;

            var dt = new TextDataConnect().ReadCSV(FileName);

            if (dt != null && dt.Rows.Count > 0)
                _processor.Truncate();

            foreach (DataRow dRow in dt.Rows)
            {
                data = _processor.GetObjectFromData(dRow);
                if (data != null)
                {
                    if (_processor.IsEmptyRow(data))
                        return string.Empty;
                    _processor.Process(data, TemplateName);
                }
            }
            _processor.PostProcess();

            return string.Empty;
        }

        public IList<string> GetAllConfigurationOfApprover(string employeeId)
        {
            IList<string> config = new List<string>();
            XLDataValidator validator = null;
            foreach (XLTemplates item in Enum.GetValues(typeof (XLTemplates)))
            {
                validator = XLDataValidator.GetXLDataValidator(item);

                if (validator.IsEmployeeConfiguredAsApprover(employeeId))
                    config.Add(UtilityMethods.GetEnumAttribute<XLTemplateFriendlyName>(item));
            }

            return config;
        }


        public string GetMessage(string menuname)
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
        }
    }
}