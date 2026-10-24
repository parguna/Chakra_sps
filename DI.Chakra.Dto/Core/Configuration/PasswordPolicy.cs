using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PasswordPolicy : DtoBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Nullable<int> PasswordLength { get; set; }

        public Nullable<int> PasswordMinimumLength { get; set; }

        public Nullable<int> PasswordMaximumLength { get; set; }

        public Nullable<int> NoOfSpecialCharacters { get; set; }

        public bool SpecialCharNotAllowed { get; set; }

        public Nullable<int> NoOfLowerCaseAlphabets { get; set; }

        public Nullable<int> NoOfUpperCaseAlphabets { get; set; }

        public bool AlphabetsNotAllowed { get; set; }

        public Nullable<int> NoOfNumericCharacters { get; set; }

        public bool NumericCharacterNotAllowed { get; set; }

        public string NotAllowedCharacters { get; set; }

        public Nullable<int> NoOfDaysPasswordChangeAllowed { get; set; }

        public string NotAllowedKeywords { get; set; }

        public bool UserNameNotAllowedInPassword { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
