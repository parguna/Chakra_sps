using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DI.Chakra.Model
{
    [Table("DI_M_TabMenu")]
    [Serializable]
    public class TabMenu : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string UniqueName { get; set; }
        public string MenuName { get; set; }
        public long? ParentTabMenuId { get; set; }
        public long? PageId { get; set; }
        [ForeignKey("PageId")]
        public WebPage Page { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public override string ToString()
        {
            return MenuName.ToString();
        }


    }
}
