using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Table("DI_M_PasswordPolicy")]
    public class PasswordPolicy : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
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
