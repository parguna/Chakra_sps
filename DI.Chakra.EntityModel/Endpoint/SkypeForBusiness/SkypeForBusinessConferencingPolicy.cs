using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_ConferencingPolicy")]
    public class SkypeForBusinessConferencingPolicy : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Identity { get; set; }
        public bool AllowIPAudio { get; set; }
        public bool AllowIPVideo { get; set; }
        public bool AllowMultiView { get; set; }
        public string Description { get; set; }
        public bool AllowParticipantControl { get; set; }
        public bool AllowAnnotations { get; set; }
        public bool DisablePowerPointAnnotations { get; set; }
        public bool AllowUserToScheduleMeetingsWithAppSharing { get; set; }
        public string ApplicationSharingMode { get; set; }
        public bool AllowNonEnterpriseVoiceUsersToDialOut { get; set; }
        public bool AllowAnonymousUsersToDialOut { get; set; }
        public bool AllowAnonymousParticipantsInMeetings { get; set; }
        public bool AllowFederatedParticipantJoinAsSameEnterprise { get; set; }
        public bool AllowExternalUsersToSaveContent { get; set; }
        public bool AllowExternalUserControl { get; set; }
        public bool AllowExternalUsersToRecordMeeting { get; set; }
        public bool AllowPolls { get; set; }
        public bool AllowSharedNotes { get; set; }
        public bool AllowQandA { get; set; }
        public bool AllowOfficeContent { get; set; }
        public bool EnableDialInConferencing { get; set; }
        public string EnableAppDesktopSharing { get; set; }
        public bool AllowConferenceRecording { get; set; }
        public bool EnableP2PRecording { get; set; }
        public bool EnableFileTransfer { get; set; }
        public bool EnableP2PFileTransfer { get; set; }
        public bool EnableP2PVideo { get; set; }
        public bool AllowLargeMeetings { get; set; }
        public bool EnableOnlineMeetingPromptForLyncResources { get; set; }
        public bool EnableDataCollaboration { get; set; }
        public string MaxVideoConferenceResolution { get; set; }
        public string MaxMeetingSize { get; set; }
        public string AudioBitRateKb { get; set; }
        public string VideoBitRateKb { get; set; }
        public string AppSharingBitRateKb { get; set; }
        public string FileTransferBitRateKb { get; set; }
        public string TotalReceiveVideoBitRateKb { get; set; }
        public bool EnableMultiViewJoin { get; set; }
        public bool EnableReliableConferenceDeletion { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
