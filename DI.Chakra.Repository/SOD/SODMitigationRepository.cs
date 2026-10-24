using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODMitigationRepository : RepositoryBase<SODMitigation>
    {
        public SODMitigationRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<object> GetReportforMitigation()
        {
            return (CurrentContext.SODMitigations.Include("RiskCodes").ToList()
                   .Select(emp => new
                   {
                       MitigationCode = emp.MitigationCode,
                       MitigationDescription = emp.MitigationDescription,
                       Document = emp.DocumentName,
                       DocumentDate = emp.DocumentUploadedDate,
                       RiskCode = emp.RiskCodes.Select(m => m.RiskCode)
                   })).ToList<object>();
        }
        public override long Insert(SODMitigation mitigation)
        {
            if (mitigation.RiskCodes.Count > 0)
            {
                var selectedRiskCodes = mitigation.RiskCodes.ToList();
                mitigation.RiskCodes.Clear();
                foreach (var sodriskcode in selectedRiskCodes)
                {
                    SODRiskCode riskCode = this.CurrentContext.SODRiskCodes.Find(sodriskcode.Id);
                    if (riskCode != null)
                        mitigation.RiskCodes.Add(riskCode);
                }
            }
            return base.Insert(mitigation);

        }

        public override void Update(SODMitigation mitigation)
        {
            SODMitigation dbMitigation = this.GetById(mitigation.Id, r => r.RiskCodes);
            dbMitigation.RiskCodes.Clear();
            foreach (var sodRiskCode in mitigation.RiskCodes)
            {
                SODRiskCode riskCode = this.CurrentContext.SODRiskCodes.Find(sodRiskCode.Id);
                if (riskCode != null)
                    dbMitigation.RiskCodes.Add(riskCode);
            }

            base.Update(mitigation);
        }
    }
}
