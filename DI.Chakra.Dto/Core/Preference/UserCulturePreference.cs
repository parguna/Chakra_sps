using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Dto
{
    public class UserCulturePreference :DtoBase
    {
        public int UserId { get; set; }

        public string CultureName { get; set; }

        public string DateFormat { get; set; }

        public string TimeFormat { get; set; }

        public string LongDateFormat { get; set; }

        public string LongTimeFormat { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string LandingPageMenuName { get; set; }
    }
}
