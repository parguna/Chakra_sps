using Application;
using System;

namespace DI.Chakra.Web.Prov.Task
{
    public partial class List : DI.Chakra.Web.UIBasePage, IContextObjectEnabled
    {
        SessionStatusImpl _m_session = new SessionStatusImpl();

        protected void Page_Load(object sender, EventArgs e)
        {
            _m_session.RedirectFrom = "Tasks";
        }
    }
}