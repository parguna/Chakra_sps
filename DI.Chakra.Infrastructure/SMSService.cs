using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class SMSService
    {
        public static void Send(string mobileNumber , string message)
        {

            if (string.IsNullOrEmpty(mobileNumber) || string.IsNullOrEmpty(message))
                return;

            try
            {


                string Content = "https://mx.fortdigital.net/http/send-message?username=@USERNAME@&password=@PASSWORD@&to=%2B65@MOBILENO@&from=DeepIdentityPasswordSync&message-type=sms.automatic&message=@MESSAGE@ ";

                StringBuilder bldrAD = new StringBuilder(Content);
                bldrAD.Replace("@USERNAME@", "deep");
                bldrAD.Replace("@PASSWORD@", "deep");
                bldrAD.Replace("@MOBILENO@", mobileNumber.Trim());
                bldrAD.Replace("@MESSAGE@", message);

                WebRequest request = WebRequest.Create(bldrAD.ToString());
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(bldrAD.ToString());
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
