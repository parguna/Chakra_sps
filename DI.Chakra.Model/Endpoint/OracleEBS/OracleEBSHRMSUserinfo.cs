#region NAMESPACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

#endregion

namespace DI.Chakra.Model
{
    [TableInfo("DI_OEBS_HRMSUserInfo", "ORACLE HRMS User Info")]
    [Table("DI_OEBS_HRMSUserInfo")]
    public class OracleEBSHRMSUserInfo : IAggregateRoot
    {
        [Key] 
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public int? DIUserId { get; set; }
        public string UserName { get; set; }

        public string PERSONID { get; set; }
        public DateTime? EFFECTIVE_START_DATE { get; set; }
        public DateTime? EFFECTIVE_END_DATE { get; set; }
        public string BUSINESS_GROUP_ID { get; set; }
        public string PERSON_TYPE_ID { get; set; }
        public DateTime? START_DATE { get; set; }
        //public string APPLICANT_NUMBER { get; set; }
        //public decimal COMMENT_ID { get; set; }
        //public string CURRENT_APPLICANT_FLAG { get; set; }
        //public string CURRENT_EMP_OR_APL_FLAG { get; set; }
        //public string CURRENT_EMPLOYEE_FLAG { get; set; }
        //public DateTime DATE_EMPLOTEE_DATA_VERIFIED { get; set; }
        public DateTime? DATE_OF_BIRTH { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        //public string EMPLOYEE_NUMBER { get; set; }
        //public string EXPENSE_CHECK_SEND_TO_ADDRESS { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string FULL_NAME { get; set; }
        public string KNOWN_AS { get; set; }
        public string MARTIAL_STATUS { get; set; }
        //public string MIDDLE_NAMES { get; set; }
        public string NATIONALITY { get; set; }
        public string NATIONAL_IDENTIFIER { get; set; }
        public string PREVIOUS_LAST_NAME { get; set; }
        //public string REGISTERED_DISABLED_FLAG { get; set; }
        public string SEX { get; set; }
        public string TITLE { get; set; }
        //public decimal VENDOR_ID { get; set; }
        public string WORK_TELEPHONE { get; set; }
        //public decimal REQUEST_ID { get; set; }
        //public int PROGRAM_APPLICANT_ID { get; set; }
        //public decimal PROGRAM_ID { get; set; }
        //public DateTime PROGRAM_UPDATE_DATE { get; set; }
        //public string ATTRIBUTE_CATEGORY { get; set; }
        //public string ATTRIBUTE1 { get; set; }
        //public string ATTRIBUTE2 { get; set; }
        //public string ATTRIBUTE3 { get; set; }
        //public string ATTRIBUTE4 { get; set; }
        //public string ATTRIBUTE5 { get; set; }
        //public string ATTRIBUTE6 { get; set; }
        //public string ATTRIBUTE7 { get; set; }
        //public string ATTRIBUTE8 { get; set; }
        //public string ATTRIBUTE9 { get; set; }
        //public string ATTRIBUTE10 { get; set; }
        //public string ATTRIBUTE11 { get; set; }
        //public string ATTRIBUTE12 { get; set; }
        //public string ATTRIBUTE13 { get; set; }
        //public string ATTRIBUTE14 { get; set; }
        //public string ATTRIBUTE15 { get; set; }
        //public string ATTRIBUTE16 { get; set; }
        //public string ATTRIBUTE17 { get; set; }
        //public string ATTRIBUTE18 { get; set; }
        //public string ATTRIBUTE19 { get; set; }
        //public string ATTRIBUTE20 { get; set; }
        //public string ATTRIBUTE21 { get; set; }
        //public string ATTRIBUTE22 { get; set; }
        //public string ATTRIBUTE23 { get; set; }
        //public string ATTRIBUTE24 { get; set; }
        //public string ATTRIBUTE25 { get; set; }
        //public string ATTRIBUTE26 { get; set; }
        //public string ATTRIBUTE27 { get; set; }
        //public string ATTRIBUTE28 { get; set; }
        //public string ATTRIBUTE29 { get; set; }
        //public string ATTRIBUTE30 { get; set; }
        //public DateTime LAST_UPDATE_DATE { get; set; }
        //public int LAST_UPDATED_BY { get; set; }
        //public int LAST_UPDATED_LOGIN { get; set; }
        //public int CREATED_BY { get; set; }
        //public DateTime CREATION_DATE { get; set; }
        //public string PER_INFORMATION_CATEGORY { get; set; }
        //public string PER_INFORMATION1 { get; set; }
        //public string PER_INFORMATION2 { get; set; }
        //public string PER_INFORMATION3 { get; set; }
        //public string PER_INFORMATION4 { get; set; }
        //public string PER_INFORMATION5 { get; set; }
        //public string PER_INFORMATION6 { get; set; }
        //public string PER_INFORMATION7 { get; set; }
        //public string PER_INFORMATION8 { get; set; }
        //public string PER_INFORMATION9 { get; set; }
        //public string PER_INFORMATION10 { get; set; }
        //public string PER_INFORMATION11 { get; set; }
        //public string PER_INFORMATION12 { get; set; }
        //public string PER_INFORMATION13 { get; set; }
        //public string PER_INFORMATION14 { get; set; }
        //public string PER_INFORMATION15 { get; set; }
        //public string PER_INFORMATION16 { get; set; }
        //public string PER_INFORMATION17 { get; set; }
        //public string PER_INFORMATION18 { get; set; }
        //public string PER_INFORMATION19 { get; set; }
        //public string PER_INFORMATION20 { get; set; }
        //public DateTime BACKGROUND_DATE_CHECK_STATUS { get; set; }
        //public string BLOOD_TYPE { get; set; }
        //public string CORRESPONDANCE_LANGUAGE { get; set; }
        //public string FAST_PATH_EMPLOYEE { get; set; }
        //public int FTE_CAPACITY { get; set; }
        //public DateTime HOLD_APPLICANT_DATE_UNTIL { get; set; }
        //public string HONORS { get; set; }
        //public string INTERNAL_LOCATION { get; set; }
        //public string LAST_MEDICAL_TEST_BY { get; set; }
        //public DateTime LAST_MEDICAL_TEST_DATE { get; set; }
        //public string MAILSTOP { get; set; }
        //public string OFFICE_NUMBER { get; set; }
        //public string ON_MILITARY_SERVICE { get; set; }
        //public string ORDER_NAME { get; set; }
        //public string PER_NAME_ADJUNCT { get; set; }
        //public DateTime PROJECTED_START_DATE { get; set; }
        //public string REHIRE_AUTHORIZOR { get; set; }
        //public string REHIRE_RECOMMENDATION { get; set; }
        //public string RESUME_EXISTS { get; set; }
        //public DateTime RESUME_LAST_UPDATED { get; set; }
        //public string SECOND_PASSPORT_EXISTS { get; set; }
        //public string STUDENT_STATUS { get; set; }
        //public string SUFFIX { get; set; }
        //public string WORK_SCHEDULE { get; set; }
        //public string PER_INFORMATION21 { get; set; }
        //public string PER_INFORMATION22 { get; set; }
        //public string PER_INFORMATION23 { get; set; }
        //public string PER_INFORMATION24 { get; set; }
        //public string PER_INFORMATION25 { get; set; }
        //public string PER_INFORMATION26 { get; set; }
        //public string PER_INFORMATION27 { get; set; }
        //public string PER_INFORMATION28 { get; set; }
        //public string PER_INFORMATION29 { get; set; }
        //public string PER_INFORMATION30 { get; set; }
        //public int OBJECT_VERSION_NUMBER { get; set; }
        //public DateTime DATE_OF_DEATH { get; set; }
        //public string REHIRE_REASON { get; set; }
        //public string COORD_BEN_MED_PLN_NO { get; set; }
        //public string COORD_BEN_NO_CVG_FLAG { get; set; }
        //public DateTime DPDNT_ADOPTION_DATE { get; set; }
        //public string DPDNT_VLNTRY_SVCE_FLAG { get; set; }
        //public DateTime RECEIPT_OF_DEATH_CERT_DATE { get; set; }
        //public string USES_TOBACCO_FLAG { get; set; }
        //public int BENFIT_GROUP_ID { get; set; }
        public DateTime? ORGINAL_DATE_OF_HIRE { get; set; }
        //public string TOWN_OF_BIRTH { get; set; }
        //public string REGION_OF_BIRTH { get; set; }
        //public string COUNTRY_OF_BIRTH { get; set; }
        //public string GOBAL_PERSON_ID { get; set; }
        //public string COORD_BEN_MED_PL_NAME { get; set; }
        //public string COORD_BEN_MED_INSR_CRR_NAME { get; set; }
        //public string COORD_BEN_MED_INSR_CRR_IDENT { get; set; }
        //public string COORD_BEN_MED_EXT_ER { get; set; }
        //public DateTime COORD_BEN_MED_CVG_STRT_DT { get; set; }
        //public DateTime COORD_BEN_MED_CVG_END_DT { get; set; }
        //public decimal PARTY_ID { get; set; }
        //public string NPW_NUMBER { get; set; }
        //public string CURRENT_NPW_FLAG { get; set; }
        //public string GLOBAL_NAME { get; set; }
        //public string LOCAL_NAME { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int Version { get; set; }
        
        public bool IsLocked { get; set; }
        //public LockScenario LockScenario { get; set; }
        public string AccountPassword { get; set; }


        ///OU 
        public string OrganisationName { get; set; }
        public int OrganisationId { get; set; }

        //public EndPoint EndPointSystem { get; set; }

    }

}
