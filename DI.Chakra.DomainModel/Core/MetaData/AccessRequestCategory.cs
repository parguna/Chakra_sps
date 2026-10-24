using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DI.Chakra.DomainModel
{
    public class AccessRequestCategory : DomainModelBase<AccessRequestCategory, EntityModel.AccessRequestCategory> 
    {
        public string AccessRequestTypeDesc { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
