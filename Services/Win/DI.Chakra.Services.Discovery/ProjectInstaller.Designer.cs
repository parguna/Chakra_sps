namespace DI.Chakra.Services.Discovery
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
            this.DiscoveryServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.DiscoveryServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // DiscoveryServiceProcessInstaller
            // 
            this.DiscoveryServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.DiscoveryServiceProcessInstaller.Password = null;
            this.DiscoveryServiceProcessInstaller.Username = null;
            // 
            // DiscoveryServiceInstaller
            // 
            this.DiscoveryServiceInstaller.Description = "Discovery Service";
            this.DiscoveryServiceInstaller.DisplayName = "Discovery Service";
            this.DiscoveryServiceInstaller.ServiceName = "DiscoveryService";
            this.DiscoveryServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DiscoveryServiceProcessInstaller,
            this.DiscoveryServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller DiscoveryServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller DiscoveryServiceInstaller;
    }
}