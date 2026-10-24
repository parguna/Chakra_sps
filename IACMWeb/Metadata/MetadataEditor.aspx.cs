
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Metadata
{
    public partial class MetadataEditor : DI.Chakra.Web.UIBasePage
    {
        #region Members



        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillMetaDataList();
                drpMetadataType.SelectedIndex = 0;
                grdMetadata.DataBind();
            }
        }

        protected void grdMetadata_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            FillDataSouce();
        }

        protected void grdMetadata_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            bool _ChangeActive = false;

            if (e.CommandName == RadGrid.UpdateCommandName)
            {
                GridEditableItem edititem = (GridEditableItem)e.Item;
                MetaData oARP = new MetaData();
                oARP.Id = Convert.ToInt32(edititem.GetDataKeyValue("Id").ToString());
                oARP.Description = (edititem["Description"].Controls[0] as TextBox).Text;
                RadComboBox cmbIsActive = (RadComboBox)edititem["IsActive"].Controls[0];
                if (cmbIsActive.SelectedValue.ToUpper() == "TRUE" || cmbIsActive.SelectedValue.ToUpper() == "YES")
                    _ChangeActive = true;
                else
                    _ChangeActive = false;
                oARP.IsActive = _ChangeActive;
                oARP.Type = drpMetadataType.SelectedValue;

                DomainApiClient<Dto.MetaData>.GetByAction<Dto.MetaData>(DomainApiAction.MetaData.UpdateMetadata, null, oARP);
                FillDataSouce();
                MasterPage.ShowAlert(AlertType.Success, "Metadata Updated successfully");
            }
        }

        protected void grdMetadata_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                GridEditableItem editedItem = (GridEditableItem)e.Item;
                GridEditManager editMan = editedItem.EditManager;

                TextBox txtDescription = (TextBox)editedItem["Description"].Controls[0];
                txtDescription.Focus();
                DataTable dt = new DataTable();
                dt.Columns.Add("IsActive");
                dt.Rows.Add("Yes");
                dt.Rows.Add("No");

                RadComboBox cmbIsActive = (RadComboBox)editedItem["IsActive"].Controls[0];
                cmbIsActive.DataSource = dt;
                cmbIsActive.DataBind();
                cmbIsActive.SelectedIndex = cmbIsActive.SelectedValue == "True" ? 0 : 1;
                cmbIsActive.SelectedValue = cmbIsActive.SelectedValue == "True" ? "Yes" : "No";
            }
            if ((e.Item is GridDataItem) && (!e.Item.IsInEditMode))
            {
                string ChangeStatus = "Yes";
                GridDataItem item = (GridDataItem)e.Item;
                bool status = Convert.ToBoolean(item["IsActive"].Text.ToString());
                if (!status) ChangeStatus = "No";
                item["IsActive"].Text = ChangeStatus;
            }
        }

        protected void drpMetadataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillDataSouce();
            grdMetadata.Rebind();
        }

        #endregion

        #region Methods

        private void FillDataSouce()
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("MetadataType", drpMetadataType.SelectedValue.ToString());

            IList<Dto.MetaData> MetadataList = DomainApiClient<Dto.MetaData>.GetByAction<List<Dto.MetaData>>(DomainApiAction.MetaData.GetMetadata, parameter);


            grdMetadata.DataSource = MetadataList;
        }

        private void FillMetaDataList()
        {
            Dictionary<string, string> Metadata = new Dictionary<string, string>();
            Metadata.Add("ARPR", "AccessRequestProgress");
            Metadata.Add("ARSO", "AccessRequestSource");
            Metadata.Add("ARST", "AccessRequestStatus");
            Metadata.Add("ENST", "EndpointSystemType");
            drpMetadataType.DataSource = Metadata;
            drpMetadataType.DataValueField = "Key";
            drpMetadataType.DataTextField = "Value";
            drpMetadataType.DataBind();
        }

        #endregion
    }
}