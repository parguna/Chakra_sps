using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;


namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class PersonnelArea : DomainModelBase<PersonnelArea, EntityModel.PersonnelArea>
    {
        public string Code { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static PersonnelArea GetByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PersonnelArea>(dataAccessTransaction).Get(a => a.Code == code).FirstOrDefault());
            }
        }


    }
}
