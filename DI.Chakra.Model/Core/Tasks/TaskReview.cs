using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_TaskReviews")]
    [Serializable]
    public class TaskReview : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public long TaskId { get; set; }
        public long ReviewId { get; set; }

        [ForeignKey("ReviewId")]
        public virtual AttestationReview AttestationReview { get; set; }
    }
}
