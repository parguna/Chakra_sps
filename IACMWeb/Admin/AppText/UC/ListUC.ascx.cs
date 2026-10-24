using System;
using System.Linq;
using System.Xml.Linq;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using Telerik.Web.UI;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.Admin.AppText.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Class Member Variable
        private const string FILE_PATH = "~/assets/apptexts/AppTexts.xml";
        XDocument sourceDocument = new XDocument();
        string selectedKey = null;
        GridDataItem gdi = null;
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            sourceDocument = XDocument.Load(this.Server.MapPath(FILE_PATH));
            this.MasterPage.ActionClicked += ActionClicked;
        }

        /// <summary>
        /// Method to handle grid item selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAppTexts_SelectedIndexChanged(object sender, EventArgs e)
        {
            gdi = grdAppTexts.SelectedItems[0] as GridDataItem;
            selectedKey = gdi.GetDataKeyValue("key").ToString();
        }

        /// <summary>
        /// Method to handle action click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Edit)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/AppText - Edit");
                if (selectedKey == null)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Please select an item");
                    return;
                }
                SetPage();
                modalPopup.VisibleOnPageLoad = true;
            }
        }

        /// <summary>
        /// Method to handle submit button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/AppText - Submit");
            if (UpdateChanges())
            {
                grdAppTexts.DataBind();

                modalPopup.VisibleOnPageLoad = false;
            }
        }

        /// <summary>
        /// Method to handle cancel button click event in the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
            grdAppTexts.SelectedIndexes.Clear();
        }

        /// <summary>
        /// Method to handle cancel button click event in the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/AppText - Cancel");
            Response.Redirect("~/Home/Home.aspx");
        }

        /// <summary>
        /// Metbod to initialise grdAppTexts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAppTexts_Init(object sender, EventArgs e)
        {

            GridFilterMenu menu = grdAppTexts.FilterMenu;
            int i = 0;
            while (i < menu.Items.Count)
            {
                if (menu.Items[i].Text == "NoFilter" || menu.Items[i].Text == "Contains" || menu.Items[i].Text == "EqualTo" ||
                    menu.Items[i].Text == "GreaterThan" || menu.Items[i].Text == "LessThan" || menu.Items[i].Text == "StartsWith" ||
                    menu.Items[i].Text == "EndsWith")
                {
                    i++;
                }
                else
                {
                    menu.Items.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to set the modal popup for the page
        /// </summary>
        private void SetPage()
        {
            XElement selectedText = GetText(sourceDocument, "Chakra", "add", selectedKey);
            if (null == selectedText)
                return;
            DetailUC.AppTextKey = selectedText.Attribute("key").Value;
            DetailUC.AppTexValue = selectedText.Attribute("value").Value;
        }

        /// <summary>
        /// Method to save the changes made to the text
        /// </summary>
        /// <returns></returns>
        private bool UpdateChanges()
        {
            try
            {
                XElement textToUpdate = GetText(sourceDocument, "Chakra", "add", DetailUC.AppTextKey);
                textToUpdate.Attribute("value").Value = DetailUC.AppTexValue;
                sourceDocument.Save(this.Server.MapPath(FILE_PATH));
                AppTextSource.ReloadAppText(); 
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method to read the xml and extraxt the selected element 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="rootNode"></param>
        /// <param name="childNode"></param>
        /// <param name="selectedKey"></param>
        /// <returns></returns>
        private XElement GetText(XDocument xmlDocument, XName rootNode, XName childNode, string selectedKey)
        {
            var Texts = xmlDocument.Element(rootNode).Elements(childNode);
            XElement selectedText = (from text in Texts
                                     where text.Attribute("key").Value == selectedKey
                                     select text).FirstOrDefault();
            return selectedText;
        }
        #endregion
    }
}