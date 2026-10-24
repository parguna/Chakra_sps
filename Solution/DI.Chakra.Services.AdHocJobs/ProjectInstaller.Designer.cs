namespace DI.Chakra.Services.AdHocJobs
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AdHocJobServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AdHocJobServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AdHocJobServiceProcessInstaller
            // 
            this.AdHocJobServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.AdHocJobServiceProcessInstaller.Password = null;
            this.AdHocJobServiceProcessInstaller.Username = null;
            // 
            // AdHocJobServiceInstaller
            // 
            this.AdHocJobServiceInstaller.Description = "AdHocJob Service";
            this.AdHocJobServiceInstaller.DisplayName = "AdHocJob Service";
            this.AdHocJobServiceInstaller.ServiceName = "AdHocJobService";
            this.AdHocJobServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AdHocJobServiceProcessInstaller,
            this.AdHocJobServiceInstaller});

        }

        #endregion
        private System.ServiceProcess.ServiceInstaller AdHocJobServiceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller AdHocJobServiceProcessInstaller;
    }
}