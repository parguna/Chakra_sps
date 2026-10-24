namespace DI.Chakra.Services.Provisioning
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
            this.ProvisioningServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ProvisioningServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ProvisioningServiceProcessInstaller
            // 
            this.ProvisioningServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.ProvisioningServiceProcessInstaller.Password = null;
            this.ProvisioningServiceProcessInstaller.Username = null;
            // 
            // ProvisioningServiceInstaller
            // 
            this.ProvisioningServiceInstaller.Description = "Provisioning Service";
            this.ProvisioningServiceInstaller.DisplayName = "Provisioning Service";
            this.ProvisioningServiceInstaller.ServiceName = "ProvisioningService";
            this.ProvisioningServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ProvisioningServiceProcessInstaller,
            this.ProvisioningServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ProvisioningServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ProvisioningServiceInstaller;
    }
}