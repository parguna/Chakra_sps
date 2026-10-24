using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Preferences
{
    public partial class ItemsUC : UIBaseUC
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Repeater1.DataSource = DomainApiClient<AdminModeItem>.GetByAction<IList<AdminModeItem>>
                (DomainApiAction.Preference.AdminModeItem.GetAllIncludingPageUrl, null);
            Repeater1.DataBind();
        }

        public string SetPreferenceClass(AdminModeItemType prefType)
        {
            if (prefType == AdminModeItemType.Configuration)
            {
                return "tile-link tile-configuration";
            }
            else if (prefType == AdminModeItemType.Security)
            {
                return "tile-link tile-security";
            }
            else if (prefType == AdminModeItemType.Tool)
            {
                return "tile-link tile-tool";
            }
            else if (prefType == AdminModeItemType.Wizard)
            {
                return "tile-link tile-wizard";
            }
            return null;
        }
    }
}