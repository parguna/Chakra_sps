using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_VoicePolicy")]
    public class SkypeForBusinessVoicePolicy : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Identity { get; set; }
        public string PstnUsages { get; set; }
        public string CustomCallForwardingSimulRingUsages { get; set; }
        public string Description { get; set; }
        public bool AllowSimulRing { get; set; }
        public bool AllowCallForwarding { get; set; }
        public bool AllowPSTNReRouting { get; set; }
        public string Name { get; set; }
        public bool EnableDelegation { get; set; }
        public bool EnableTeamCall { get; set; }
        public bool EnableCallTransfer { get; set; }
        public bool EnableCallPark { get; set; }
        public bool EnableBusyOptions { get; set; }
        public bool EnableMaliciousCallTracing { get; set; }
        public bool EnableBWPolicyOverride { get; set; }
        public bool PreventPSTNTollBypass { get; set; }
        public string CallForwardingSimulRingUsageType { get; set; }
        public bool EnableVoicemailEscapeTimer { get; set; }
        public string PSTNVoicemailEscapeTimer { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
