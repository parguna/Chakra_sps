using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODMitigation : DomainModelBase<SODMitigation, EntityModel.SODMitigation>
    {
        public string MitigationCode { get; set; }

        public string MitigationDescription { get; set; }

        public string MitigationSteps { get; set; }

        public Nullable<long> ApproverUserId { get; set; }

        public Nullable<long> ApproverGroupId { get; set; }

        public Nullable<long> MonitorUserId { get; set; }

        public Nullable<long> MonitorGroupId { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string DocumentName { get; set; }

        public string DocumentUploadedPath { get; set; }

        public Nullable<DateTime> DocumentUploadedDate { get; set; }

        public List<SODRiskCode> RiskCodes { get; set; }

        public User Approver { get; set; }

        public User Reveiwer { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static SODMitigation GetByIdIncludeSodRiskCode(long id)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODMitigation>(tranaction)
                    .GetById(id,
                    riskmit=>riskmit.RiskCodes));
            }
        }

        public static bool HasRiskCode(long riskCodeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODMitigation>(dataAccessTransaction)
                    .Any(miti => miti.RiskCodes.Any(code =>code.Id==riskCodeId));
            }
        }

        public static bool DoesExists(string mitiCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODMitigation>(dataAccessTransaction)
                    .Any(miti => miti.MitigationCode == mitiCode);
            }
        }

        //public override void Save()
        //{
        //    bool isNew = this.IsNew;

        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        base.Save(transaction);

        //        if (!isNew && this.RiskCodes != null)
        //        {
        //            List<SODRiskCode> dbRiskCodes = ORMap(new RepositoryBase<EntityModel.SODMitigation>(transaction)
        //                .Get(g => g.Id == this.Id, g => g.RiskCodes).Single()).RiskCodes;

        //            List<SODRiskCode> removedRiskCodes = dbRiskCodes.Except(this.RiskCodes, new IdBaseEqualityComparer<SODRiskCode>()).ToList();

        //            removedRiskCodes.ForEach(prd => prd.Delete(transaction));

        //            this.RiskCodes.ForEach(prd => prd.Save(transaction));

        //        }

        //        transaction.Commit();
        //    }
        //}

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new SODMitigationRepository(transaction).Insert(ORUnMap());
                    //base.Save(transaction);
                }
                else
                {
                    new SODMitigationRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }
    }
}
