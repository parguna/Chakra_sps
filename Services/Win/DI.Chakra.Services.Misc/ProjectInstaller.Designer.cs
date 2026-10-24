namespace DI.Chakra.Services.Misc
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
            this.MiscServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MiscServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MiscServiceProcessInstaller
            // 
            this.MiscServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.MiscServiceProcessInstaller.Password = null;
            this.MiscServiceProcessInstaller.Username = null;
            // 
            // MiscServiceInstaller
            // 
            this.MiscServiceInstaller.Description = "Miscellaneous Service";
            this.MiscServiceInstaller.DisplayName = "Miscellaneous Service";
            this.MiscServiceInstaller.ServiceName = "MiscService";
            this.MiscServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MiscServiceProcessInstaller,
            this.MiscServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MiscServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MiscServiceInstaller;
    }
}