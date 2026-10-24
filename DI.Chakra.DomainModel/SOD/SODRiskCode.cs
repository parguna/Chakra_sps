using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SODRiskCode : DomainModelBase<SODRiskCode, EntityModel.SODRiskCode>
    {
        public string RiskCode { get; set; }

        public string RiskDescription { get; set; }

        public Nullable<long> RiskLevelId { get; set; }

        public int Status { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> RiskScoreId { get; set; }

        public  SODRiskLevel sodrisklevel { get; set; }

        public  SODRiskScore sodriskscore { get; set; }

        public  User user { get; set; }

        public  List<SODMitigation> Mitigations { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static List<SODRiskCode> GetAllByIncludeUser()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODRiskCode>(dataAccessTransaction)
                    .GetAll( riskcd => riskcd.sodrisklevel, 
                    riskcd => riskcd.user, 
                    riskcd => riskcd.Mitigations, 
                    riskcd => riskcd.sodriskscore).ToList());
            }
        }


        public static SODRiskCode GetByRiskCode(string riskCode)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODRiskCode>(tranaction)
                    .Get(riskcd => riskcd.RiskCode == riskCode, 
                    riskcd => riskcd.sodrisklevel, 
                    riskcd => riskcd.user, 
                    riskcd => riskcd.Mitigations, 
                    riskcd => riskcd.sodriskscore).FirstOrDefault());
            }
        }

        public static bool DoesExists(string riskCode)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODRiskCode>(dataAccessTransaction)
                    .Any(riskcode => riskcode.RiskCode == riskCode);
            }
        }

        public static Dictionary<long, string> GetBySearchString(string searchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RiskCodeRepository(dataAccessTransaction).GetBySearchString(CheckNull.AsteriskToString(searchString));
            }
        }

        public static bool HasRiskLevel(long riskLevelId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODRiskCode>(dataAccessTransaction).
                    Any(code => code.RiskLevelId == riskLevelId);
            }
        }

        public static bool HasRiskScore(long riskScoreId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.SODRiskCode>(dataAccessTransaction).
                    Any(code => code.RiskScoreId == riskScoreId);
            }
        }

        public static bool IsExists(SODRiskCode  rskCode)
        {
            bool isExists = false;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<SODRiskCode> rskCodes = SODRiskCode.GetAll();
                foreach(SODRiskCode code in rskCodes)
                {
                    if ((code.RiskLevelId == rskCode.RiskLevelId && code.RiskScoreId == rskCode.RiskScoreId && code.Status == rskCode.Status) && (code.Id!=rskCode.Id))
                    {
                        isExists = true;
                        break;
                    }

                }
            }
            return isExists;
        }
            


    }
}
