using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum ReportType
    {
        User,
        EndpointSystem,
        Attestation,
        Provisioning,
        Security,
        Audit,
        Compliance
    }

    public enum ReportStatus
    {
        Initiated,
        Created,
        MailSent,
        MailFailed
    }

    public enum ManagementReportType : byte
    {
        //[Description("None")]
        //None = 0,

        [Description("Daily Processing Report")]
        DailyProcessingReport = 1,

        [Description("Subsystem Discrepancy Report")]
        SubsystemDiscrepancyReport = 2,

        [Description("IDAS Discrepancy For Role Report")]
        IDASDiscrepancyForRoleReport = 3,


        [Description("IDAS Discrepancy For Account Report")]
        IDASDiscrepancyForAccountReport = 4,

        [Description("CSEL and GoldBar Exception Report")]
        CSELandGoldBarExceptionReport = 5,

        [Description("Review Status Summary Report")]
        ReviewStatusSummaryReport = 6,

        [Description("Work Request Aging Report")]
        WorkRequestAgingReport = 7
    }
}
