using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class NovaMatrixParmeter
    {
        public NovaMatrixType novaMatrixType { get; set; }
        public string Company { get; set; }
        public string Section { get; set; }
        public string Department { get; set; }
    }

    public class NovaMatrixResponse
    {
        public NovaMatrixResponse()
        {
            this.NovaMatrixResponseDetails = new List<NovaMatrixResponseDetail>();
        }
        public List<NovaMatrixResponseDetail> NovaMatrixResponseDetails { get; set; }
    }
    public class NovaMatrixResponseDetail
    {
        public NovaMatrixResponseDetail()
        {
            FlinsTCode = new List<string>();
            ADGroup = new List<string>();
            FlinsBiTCode = new List<string>();
            IsExclusiveForUserAccount = false;
        }

        public int index { get; set; }
        public NovaMatrixType novaMatrixType { get; set; }
        public string Company { get; set; }
        public string Section { get; set; }
        public string Department { get; set; }

        public bool IsExclusiveForUserAccount { get; set; }

        public string Position { get; set; }
        public List<string> FlinsBiTCode { get; set; }
        public List<string> ADGroup { get; set; }
        public List<string> FlinsTCode { get; set; }
        public string JobDescription { get; set; }
        public override string ToString()
        {
            var matrix = UtilityMethods.GetEnumAttribute<Description>(novaMatrixType);
            return $"Matrix Type :{matrix},Position:{Position}, Company:{Company},Section:{Section},Deparment:{Department},IsExclusiveForUserAccount:{IsExclusiveForUserAccount}";
        }

    }

    
    public class SAPNovaTCodes   
    {
        public SAPNovaTCodes(string tCode)
        {
            TCode = tCode;
            CompositeRoles = new List<string>();
            IsCompositeRole = false;
        }
        public bool IsCompositeRole { get; set; }
        public string TCode { get; set; }
        public List<string> CompositeRoles { get; set; }
    }
}
