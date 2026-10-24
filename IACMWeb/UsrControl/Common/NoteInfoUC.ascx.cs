using System;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class NoteInfoUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ENTITY_ID";
        private const string V_REQUESTEEUSERID = "REQUESTEEUSERID";
        private const string V_ENDPOINTID = "ENDPOINTID";
        #endregion

        #region Properties

        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return long.Parse(ViewState[V_ID].ToString());
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public long userId
        {
            get
            {
                if (ViewState[V_REQUESTEEUSERID] == null)
                    return -1;
                return long.Parse(ViewState[V_REQUESTEEUSERID].ToString());
            }
            set
            {
                ViewState[V_REQUESTEEUSERID] = value;
            }
        }
        public long endpointId
        {
            get
            {
                if (ViewState[V_ENDPOINTID] == null)
                    return -1;
                return long.Parse(ViewState[V_ENDPOINTID].ToString());
            }
            set
            {
                ViewState[V_ENDPOINTID] = value;
            }
        }
        #endregion

        #region Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }            
        }

        public void Initialize()
        {
            SetPage();
        }

        private void SetPage()
        {
            if (userId != -1)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();

                parameter.Add("userId", this.userId.ToString());
                parameter.Add("endpointId", this.endpointId.ToString());
                var infos = DomainApiClient<User>.GetByAction<List<string>>(DomainApiAction.User.GetRequesteeInfo, parameter);

                if(infos != null && infos.Count > 0)
                {
                    string infoText = string.Empty;
                    foreach (var info in infos)
                        infoText += string.Format("<p>{0}</P><br />", info);
                    userInfoDiv.InnerHtml = infoText;
                }
                else
                    userInfoMainDiv.Visible = false;
            }
            else
                userInfoMainDiv.Visible = false;
        }
        #endregion

        #region Events
        
        #endregion
    }
}