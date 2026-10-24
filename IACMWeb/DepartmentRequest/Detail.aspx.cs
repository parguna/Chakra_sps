using System;
using System.Collections.Generic;
using System.Web.UI;

namespace DI.Chakra.Web.DepartmentRequest
{
    public partial class Detail : DI.Chakra.Web.UIBasePage, IContextObjectRequired
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //int AdHocAccessRequestId = Convert.ToInt32(Request.QueryString["AdHocAccessRequestId"]);
                //RequestDetailUC.IsPageCalledFromDashboard = true;
                //RequestDetailUC.AdHocAccessRequestId = AdHocAccessRequestId;

                try
                {
                    if (Request.QueryString["Id"] != null)
                    {
                        var id = Request.QueryString["Id"].ToString();
                        if (!string.IsNullOrEmpty(id))
                        {
                            ContextObjectId = Convert.ToInt64(id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //suppress the exception.
                }
            }
        }
    }
}