using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.IO;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Comp.Mitigation.Common
{
    public partial class MitigationCodeUC : UIBaseUC
    {
        #region Class Member Variables
        SessionStatusImpl session = new SessionStatusImpl();
        SODMitigation mitigation;      
        private bool isEdit = false;
        private bool isView = false;
        private const string V_ID = "ENTITY_ID";
        private UIMode uiMode;
        #endregion

        #region Public Properties
        public string FilePath
        {
            get
            { return (string)ViewState["FileName"]; }
            set { ViewState["FileName"] = value; }
        }

        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }     
        }

        public SODMitigation Mitigaiton
        {
            get { return mitigation; }
            set { mitigation = value; }
        }

        public string FilePathValue
        {
            get{
                if((String.IsNullOrEmpty( (string)Session["FileInfo"])))
                {
                    Session["FileInfo"] = this.FileUpload.HasFile ? Path.GetFileName(this.FileUpload.FileName) : "";
                }
                return (string)Session["FileInfo"]
                ;}
            set{
                if (Session["FileInfo"] == null)
                    value = this.FileUpload.HasFile ? Path.GetFileName(this.FileUpload.FileName) : "";
                Session["FileInfo"] = value;}
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IntializePage()
        {
            if (UIMode==UIMode.Edit || UIMode==UIMode.View)
            {
                this.txtMitigationCode.Text = this.Mitigaiton.MitigationCode;
                this.txtMitigationDesc.Text = this.Mitigaiton.MitigationDescription;
                this.txtSteps.Text = this.Mitigaiton.MitigationSteps;
                if (this.Mitigaiton.IsActive.HasValue ? this.Mitigaiton.IsActive.Value : false)
                    this.radActive.Checked = true;
                else
                {
                    this.radActive.Checked = false;
                    this.radClosed.Checked = true;
                }
                this.DocumentName.Text = this.Mitigaiton.DocumentName;
                FilePath = this.Mitigaiton.DocumentUploadedPath;              
            }
            if(UIMode!=UIMode.Edit&&!IsPostBack)
                Session["FileInfo"] = this.Mitigaiton!=null?this.Mitigaiton.DocumentUploadedPath:"";
            if (IsPostBack && this.FileUpload.HasFile)
            {
                string FileInfo = this.FileUpload.HasFile ? Path.GetFileName(this.FileUpload.FileName):"";
                Session["FileInfo"] = FileInfo;
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Method to get the mitigation code details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetMitigCodeDetails(ref SODMitigation SODMitigation)
        {
            SODMitigation.MitigationCode = this.txtMitigationCode.Text;
            SODMitigation.MitigationDescription = this.txtMitigationDesc.Text;
            SODMitigation.MitigationSteps = this.txtSteps.Text;
            if (this.radActive.Checked)
                SODMitigation.IsActive = true;
            else
                SODMitigation.IsActive = false;
            SODMitigation.DocumentUploadedDate = DateTime.Now.Date;
            string FileInfo = Session["FileInfo"]!=null?Session["FileInfo"].ToString():"";
            if (!string.IsNullOrEmpty(FileInfo) && !FileUpload.HasFile)
            {
                SODMitigation.DocumentUploadedPath = FileInfo;
                SODMitigation.DocumentName = String.IsNullOrEmpty(this.DocumentName.Text) ? FileInfo : this.DocumentName.Text;
            }
            else if (this.FileUpload.HasFile)
            {
                try
                {
                    string fileName = Path.GetFileName(this.FileUpload.FileName);
                    this.FileUpload.SaveAs(Server.MapPath("~/") + fileName);
                    SODMitigation.DocumentName = String.IsNullOrEmpty(this.DocumentName.Text) ? fileName : this.DocumentName.Text;
                    SODMitigation.DocumentUploadedPath = this.FileUpload.PostedFile != null ? this.FileUpload.PostedFile.FileName : 
                        this.FileUpload.FileName;
                }
                catch (System.Web.HttpException ex)
                {
                }
            }
            else
                SODMitigation.DocumentUploadedPath = FilePath;            
        }
        #endregion

        #region Public Methods
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion
        
        #region Page and Control Events
        /// <summary>
        /// Method to load the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Event to handle the Active Radio box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radActive_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.radClosed.Checked = false;
            }
        }

        /// <summary>
        /// Event to handle the Closed Radio box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radClosed_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.radActive.Checked = false;
            }
        }
        #endregion
    }
}