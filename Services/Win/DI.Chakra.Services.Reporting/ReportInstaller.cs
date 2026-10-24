using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace DI.Chakra.Services.Reporting
{
    [RunInstaller(true)]
    public partial class ReportInstaller : System.Configuration.Install.Installer
    {
        public ReportInstaller()
        {
            InitializeComponent();
        }
    }
}
