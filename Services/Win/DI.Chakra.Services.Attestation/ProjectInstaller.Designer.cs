namespace DI.Chakra.Services.Attestation
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
            this.AttestationServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AttestationServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AttestationServiceProcessInstaller
            // 
            this.AttestationServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.AttestationServiceProcessInstaller.Password = null;
            this.AttestationServiceProcessInstaller.Username = null;
            // 
            // AttestationServiceInstaller
            // 
            this.AttestationServiceInstaller.Description = "Attestation Service";
            this.AttestationServiceInstaller.DisplayName = "Attestation Service";
            this.AttestationServiceInstaller.ServiceName = "AttestationService";
            this.AttestationServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AttestationServiceProcessInstaller,
            this.AttestationServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AttestationServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AttestationServiceInstaller;
    }
}