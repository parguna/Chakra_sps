using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web
{
    public partial class DownloadTCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var applicationType = Request.QueryString["applicationType"];
            var applnType = (ApplicationType)Convert.ToInt32(applicationType);
            var parameter = new Dictionary<string, string>();
            parameter.Add("applicationType", applicationType);
            var data = DomainApiClient<Dto.Attestation>
                .GetByAction<IDMSHttpResponse<byte[]>>("DownloadTCodeReport", parameter);

            if (data != null)
            {
                Session["ExportStatus"] = "true";
                HttpResponse response = Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                response.AddHeader("Content-Length", data.Data.Length.ToString());
                response.AddHeader("Content-Disposition", "attachment;filename=" + $"{applnType.ToString()}_RoleToTCode.xls");
                response.BinaryWrite(data.Data);
                response.End();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}