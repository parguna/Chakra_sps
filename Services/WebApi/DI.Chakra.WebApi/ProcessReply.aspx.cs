using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChakraService
{
    public partial class ProcessReply : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form.Get("txtUserName") == null)
                return;

            Process();
        }

        private void Process()
        {
            string userName = Request.Form.Get("txtUserName").ToString();
            string taskId = Request.Form.Get("txtTaskId").ToString();
            string comments = Request.Form.Get("txtComments").ToString();



            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.15.76:85/");

            client.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));

            dynamic taskObject = new JObject();
            taskObject.TaskId = taskId;
            taskObject.UserName = userName;
            taskObject.Action = "Approve";
            taskObject.Comment = comments;

            var response = client.PostAsync("api/Task/ApproveTask",
                        new StringContent(JsonConvert.SerializeObject(taskObject).ToString(),
                        Encoding.UTF8, "application/json")).Result;
        }
    }
}