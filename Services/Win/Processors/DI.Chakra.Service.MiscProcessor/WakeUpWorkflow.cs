using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Workflow.ServiceHelpers;
using DI.Chakra.Infrastructure;
using System.Net.Http;

namespace DI.Chakra.Services.MiscProcessor
{
    public class WakeUpWorkflow
    {
        public async void Browse()
        {

            try
            {
                string pages = AppSettingsHelper.WFService;

                foreach (var page in pages.Split(new char[] { '|' }))
                {
                    // ... Use HttpClient.
                    using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = await client.GetAsync(page))
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();

                        // ... Display the result.
                        if (result != null &&
                            result.Length >= 50)
                        {
                            //DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Misc service initiated workflow browsing");

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, ex.Message, ex);

            }

        }
    }
}
