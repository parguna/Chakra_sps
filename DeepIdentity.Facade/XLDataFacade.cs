using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.DataAccess;
using DeepIdentity.Model;

namespace DeepIdentity.Facade
{
    public class XLDataFacade
    {
        TextProcessor processor = null;

        public XLDataFacade()
        {
            processor = new TextProcessor();
        }

        public IList<IXLData> ProcessXL(string fileName, XLTemplates templateName, ref string errorMessage)
        {
            try
            {
                processor.FileName = fileName;
                processor.TemplateName = templateName;
                errorMessage = string.Empty;

                errorMessage = processor.StartProcess();

                if (processor.HasInvalidData)
                {
                    return processor.InvalidList;
                }
            }
            catch (Exception)
            {
                
                throw;
            }
           
            return null;
        }
        public IList<string> GetAllConfigurationOfApprover(string employeeId)
        {
            //try
            //{
            //    return processor.GetAllConfigurationOfApprover(employeeId);
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return new List<string>();
        }

    }
}
