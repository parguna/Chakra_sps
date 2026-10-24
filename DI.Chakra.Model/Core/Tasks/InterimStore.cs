using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{

    [Table("DI_M_InterimStore")]
    [Serializable]
    public class InterimStore : IAggregateRoot
    {
        public InterimStore()
        {
        }

        [Key]
        public long Id { get; set; }
        public InterimStoreItemType InterimStoreItemType { get; set; }
        public long? TaskId { get; set; }
        public long? UserId { get; set; }
        public string XmlData { get; set; }
        public bool IsSaved { get; set; }
        public bool IsActive { get; set; }


    }

}
