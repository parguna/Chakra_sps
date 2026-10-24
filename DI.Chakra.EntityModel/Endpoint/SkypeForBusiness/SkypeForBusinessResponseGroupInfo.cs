using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_ResponseGroupInfo")]
    public class SkypeForBusinessResponseGroupInfo : EntityBase, ICreatable, IUpdatable
    {
        public SkypeForBusinessResponseGroupInfo()
        {
            SkypeForBusinessUserInfo = new List<SkypeForBusinessUserInfo>();
        }
        public string OwnerPool { get; set; }
        //Will be Teams CallQueue Agents
        public string AgentsByUri { get; set; }
        //Teams CallQueue Name
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParticipationPolicy { get; set; }
        public string AgentAlertTime { get; set; }
        public string RoutingMethod { get; set; }
        //Will be used for Team DistributionGroupIdentity
        public string DistributionGroupAddress { get; set; }
        [Required]
        public string Identity { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string DistributionGroupName { get; set; }
        public List<SkypeForBusinessUserInfo> SkypeForBusinessUserInfo { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
        public long EndPointSystemId { get; set; }
    }
}
