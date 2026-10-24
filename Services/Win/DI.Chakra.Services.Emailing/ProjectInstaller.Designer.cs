namespace DI.Chakra.Services.Emailing
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
            this.EmailingServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.EmailingServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // EmailingServiceProcessInstaller
            // 
            this.EmailingServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.EmailingServiceProcessInstaller.Password = null;
            this.EmailingServiceProcessInstaller.Username = null;
            // 
            // EmailingServiceInstaller
            // 
            this.EmailingServiceInstaller.Description = "Emailing Service";
            this.EmailingServiceInstaller.DisplayName = "Emailing Service";
            this.EmailingServiceInstaller.ServiceName = "EmailingService";
            this.EmailingServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.EmailingServiceProcessInstaller,
            this.EmailingServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller EmailingServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller EmailingServiceInstaller;
    }
}