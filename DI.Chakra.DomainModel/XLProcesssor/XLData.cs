using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.DomainModel
{
    public class XLData
    {
        public static string ValidateFileColumns(object xmlData, string fileName, XLTemplates templateName)
        {
            TextProcessor processor = new TextProcessor();
            processor.FileName = fileName;
            processor.xmlData = xmlData.ToString();
            processor.TemplateName = templateName;
            return processor.ValidateFileColumns();
        }
        public static string ValidateFileColumnsForEntitlment(object xmlData, string fileName, XLTemplates templateName, long endpointSystemId)
        {
            TextProcessor processor = new TextProcessor();
            processor.FileName = fileName;
            processor.xmlData = xmlData.ToString();
            processor.TemplateName = templateName;
            processor.EndpointSystemId = endpointSystemId;
            return processor.ValidateFileColumns();
        }
        public static IList<IXLData> ProcessXL(object xmlData, string fileName, XLTemplates templateName, ref string errorMessage,string currentUser, bool partialUpload)
        {
            TextProcessor processor = new TextProcessor();
            processor.FileName = fileName;
            processor.xmlData = xmlData.ToString();
            processor.TemplateName = templateName;
            processor.PartialUpload = partialUpload;
            errorMessage = string.Empty;
            errorMessage = processor.StartProcess(currentUser);
            if(processor._ValidandInvalid != null)
            {
                return processor._ValidandInvalid;
            }
            if (processor.HasInvalidData)
            {
                return processor.InvalidList;
            }
            if (processor.HasvalidData)
            {
                return processor.ValidList;
            }
            return null;
        }

        public static IList<IXLData> ProcessXLData(IXLData data, XLTemplates templateName, string currentUser, bool isEditMode)
        {
            XLDataProcessor processor = new XLDataProcessor();
            processor.Process(data, templateName, currentUser, isEditMode);
            if (processor.ValidList.Count > 0)
                return processor.ValidList;
            else
                return processor.InvalidList;
        }
        public static IList<IXLData> ProcessXLForEntitlment(object xmlData, string fileName, XLTemplates templateName, ref string errorMessage, string currentUser, long endpointSystemId)
        {
            TextProcessor processor = new TextProcessor();
            processor.FileName = fileName;
            processor.xmlData = xmlData.ToString();
            processor.TemplateName = templateName;
            processor.EndpointSystemId = endpointSystemId;
            errorMessage = string.Empty;
            errorMessage = processor.StartProcess(currentUser);
            if (processor._ValidandInvalid != null)
            {
                return processor._ValidandInvalid;
            }
            if (processor.HasInvalidData)
            {
                return processor.InvalidList;
            }
            if (processor.HasvalidData)
            {
                return processor.ValidList;
            }
            return null;
        }
        public static IList<string> GetAllConfigurationOfApprover(string employeeId)
        {
            try
            {
                TextProcessor processor = new TextProcessor();
                return processor.GetAllConfigurationOfApprover(employeeId);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static string GetWhoUpdatedData(XLTemplates template)
        {
            var validator = XLDataValidator.GetXLDataValidator(template);
            return validator.GetWhoUpdatedData();
        }
    }
}
