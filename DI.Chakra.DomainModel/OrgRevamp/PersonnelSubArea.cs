using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;



namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class PersonnelSubArea : DomainModelBase<PersonnelSubArea, EntityModel.PersonnelSubArea>
    {

        public long PersonnelAreaId { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static PersonnelSubArea GetByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(a => a.Code == code).FirstOrDefault());
            }
        }

        public static PersonnelSubArea GetByCodeAndPersonnelAreaId(string code, long personnelAreaId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PersonnelSubArea>(dataAccessTransaction).Get(a => a.Code == code && a.PersonnelAreaId == personnelAreaId).FirstOrDefault());
            }
        }

    }
}
