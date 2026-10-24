namespace DI.Chakra.Services.Compliance
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
            this.ComplainceServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.ComplianceServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ComplainceServiceProcessInstaller
            // 
            this.ComplainceServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.ComplainceServiceProcessInstaller.Password = null;
            this.ComplainceServiceProcessInstaller.Username = null;
            // 
            // ComplianceServiceInstaller
            // 
            this.ComplianceServiceInstaller.Description = "Compliance Service";
            this.ComplianceServiceInstaller.DisplayName = "Compliance Service";
            this.ComplianceServiceInstaller.ServiceName = "DiscoveryService";
            this.ComplianceServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ComplainceServiceProcessInstaller,
            this.ComplianceServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ComplainceServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller ComplianceServiceInstaller;
    }
}