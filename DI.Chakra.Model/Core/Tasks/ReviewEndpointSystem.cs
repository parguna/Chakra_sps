using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Table("DI_M_ReviewEndpointSystems")]
    [Serializable]
    public class ReviewEndpointSystem : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long ReviewId { get; set; }
        public long ReviewEndpointId { get; set; }
        public long TaskId { get; set; }

        [XmlIgnore]
        [ForeignKey("ReviewId")]
        protected virtual AttestationReview AttestationReview { get; set; }

        [ForeignKey("ReviewEndpointId")]
        public virtual AttestationReviewEndpointSystem AttestationReviewEndpointSystem { get; set; }

        [ForeignKey("TaskId")]
        protected Task Task { get; set; }
    }
}
