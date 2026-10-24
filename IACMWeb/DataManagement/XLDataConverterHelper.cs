using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;

namespace DI.Chakra.Web
{
    public class XLDataConverterHelper
    {
        public string xldatasource = "xldatasource";
        public string FormatLeadingZeros(string Value, int Padinglength)
        {
            try
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    if (Value.Length < Padinglength)
                    {
                        char pad = '0';
                        string output = Value.PadLeft(Padinglength, pad);
                        return output;
                    }
                    else
                    {
                        return Value;
                    }
                }
                else return string.Empty;
            }
            catch (Exception ex)
            {
                return Value;
            }
        }
        public void InvisibleBetween(GridFilterMenu menu)
        {
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "Between")
                {
                    menu.Items.RemoveAt(i);
                }
                if (menu.Items[i].Text == "NotBetween")
                {
                    menu.Items.RemoveAt(i);
                }

                i++;
            }
        }
        public long GetEndpointSystemId(ApplicationType applicationType)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("applicationType", Convert.ToInt32(applicationType).ToString());
            Dto.EndPoint endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, parameters);
            return endPoint.Id;
        }
        public string GetWhoUpdatedData(XLTemplates template)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("templateName", template.ToString());
            var message = DomainApiClient<Dto.IXLData>.GetByAction<string>(DomainApiAction.XLProcessor.XLData.GetWhoUpdatedData, parameter);
            return message;
        }

        public DateTime? GetDateFromString(object date)
        {
            if (date == null)
                return null;

            if (string.IsNullOrWhiteSpace(date.ToString()))
                return null;
            DateTime d;
            var parsed = DateTime.TryParse(date.ToString(), out d);
            if (parsed)
                return d;
            return null;
        }
    }
}