namespace DI.Chakra.Services.Reporting
{
    partial class ReportInstaller
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
            this.ReportProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.ReportInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // ReportProcessInstaller1
            // 
            this.ReportProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.ReportProcessInstaller1.Password = null;
            this.ReportProcessInstaller1.Username = null;
            // 
            // ReportInstaller1
            // 
          
            this.ReportInstaller1.Description = "Reporting Service";
            this.ReportInstaller1.DisplayName = "Reporting Service";
            this.ReportInstaller1.ServiceName = "ReportingService";
            this.ReportInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // 
            // ReportInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ReportProcessInstaller1,
            this.ReportInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ReportProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller ReportInstaller1;
    }
}