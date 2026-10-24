using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DI.Chakra.DomainModel
{
    public class PasswordPolicy : DomainModelBase<PasswordPolicy, EntityModel.PasswordPolicy>
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

        public static bool DoesExists(string name)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.PasswordPolicy>(dataAccessTransaction).Any(x=>x.Name == name);
            }
        }
        public static PasswordPolicy GetByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new PasswordPolicyRepository(dataAccessTransaction).GetByEndpointSystemTypeId(endpointSystemTypeId));
            }
        }

        public static PasswordPolicy GetByEndpointSystemId(long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new PasswordPolicyRepository(dataAccessTransaction).GetByEndpointSystemId(endpointSystemId));
            }
        }

        public static PasswordPolicy GetByEndpointSystemTypeId(long endpointSystemTypeId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new PasswordPolicyRepository(dataAccessTransaction).GetByEndpointSystemTypeId(endpointSystemTypeId, endpointSystemId));
            }
        }

        public static string GetIACMLastPasswordChangeByEndpointUser(long endPointSystemId, string userName)
        {            
            string PwdLastModifiedDate = string.Empty;
            
            long endPointSystemTypeId = EndPoint.GetById(endPointSystemId).EndpointSystemTypeId.Value;
            EndpointType endPointSystemTypeName = EndPointSystemType.GetById(endPointSystemTypeId).EndpointSystemTypeName;

            DateTime? IACMLastPasswordChange = null; // DateTime.Now;
            if (endPointSystemTypeName == EndpointType.ActiveDirectory)
            {
                ADSUserInfo userInfo = ADSUserInfo.GetByEndPointSystemId(userName, endPointSystemId);
                if (userInfo != null)
                    IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
            }
            else if (endPointSystemTypeName == EndpointType.SAP)
            {
                SAPUserinfo userInfo = SAPUserinfo.GetByUserLogonName(userName, endPointSystemId);
                if (userInfo != null)
                    IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
            }
            else if (endPointSystemTypeName == EndpointType.SAPEnterprisePortal)
            {
                SAPEPUserInfo userInfo = SAPEPUserInfo.GetByUserName(userName, endPointSystemId);
                if (userInfo != null)
                    IACMLastPasswordChange = userInfo.IACMLastPasswordChange;
            }

            if (IACMLastPasswordChange == null)
                PwdLastModifiedDate = null;
            else
                PwdLastModifiedDate = IACMLastPasswordChange.Value.ToString();

            return PwdLastModifiedDate;
        }

        public static long GetPasswordPolicyIdByName(string policyName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var passwordPolicy = GetByName(policyName);
                if (passwordPolicy != null)
                {
                    return passwordPolicy.Id;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static PasswordPolicy GetByName(string policyName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PasswordPolicy>(dataAccessTransaction).
                                                                        Get(pp => pp.Name == policyName).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetPasswordPolicies(string displayNameSearchString)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new PasswordPolicyRepository(dataAccessTransaction).GetPasswordPolicies(CheckNull.AsteriskToString(displayNameSearchString));
            }
        }

        public static string ConstructGeneralUserPassword()
        {
            PasswordPolicy passwordPolicy;            
            passwordPolicy = PasswordPolicy.GetByEndpointSystemId((long)Infrastructure.Enums.EndpointType.ActiveDirectory);
            if (passwordPolicy == null)
                passwordPolicy = PasswordPolicy.GetById(1);

            if (passwordPolicy != null)
            {
                string passwd = PasswordPolicy.ConstructGeneralUserPassword
                   (passwordPolicy.PasswordLength.HasValue ? passwordPolicy.PasswordLength.Value : 8,
                    passwordPolicy.NoOfSpecialCharacters.HasValue ? passwordPolicy.NoOfSpecialCharacters.Value : 0,
                    passwordPolicy.NoOfLowerCaseAlphabets.HasValue ? passwordPolicy.NoOfLowerCaseAlphabets.Value : 0,
                    passwordPolicy.NoOfUpperCaseAlphabets.HasValue ? passwordPolicy.NoOfUpperCaseAlphabets.Value : 0,
                    passwordPolicy.NoOfNumericCharacters.HasValue ? passwordPolicy.NoOfNumericCharacters.Value : 0, passwordPolicy.NotAllowedKeywords);
                return passwd;
            }
            else
                return "@bcd1234";
        }

        public static string ConstructGeneralUserPassword(int PasswordLength, int NoOfSpecialCharacter, int NoOfLowercaseChar,
                                                                                        int NoOfUppercaseChar, int NoOfDigitChar, string NotAllowedChars)
        {
            string passwd = string.Empty;
            StringBuilder passwdSB = new StringBuilder();

            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(NotAllowedChars);

            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;
            char[] excludedChars = alphaPart.ToArray();
            int[] excludedDigits = numberPart.ToArray().Select(n => Convert.ToInt32(n.ToString())).ToArray();
            //string[] spchar = new string[] { "-", "!", "$", "@", "#", ".", ",", "^" };
            string[] spchar = new string[] { "#", "%", "&", "!" };
            int specialCharRandIndx;
            int startIndx = 0;
            int endIndx = 0;

            int pswdChrLength = (PasswordLength - NoOfSpecialCharacter - NoOfDigitChar);

            pswdChrLength = pswdChrLength > 0 ? pswdChrLength : 8;

            string pswdStr = GetRandomString(pswdChrLength, excludedChars);

            endIndx = NoOfLowercaseChar;
            if (endIndx > 0 && pswdStr.Length >= endIndx)
            {
                passwdSB.Append(pswdStr.Substring(startIndx, endIndx).ToLower());
                startIndx = endIndx;
            }

            endIndx += NoOfUppercaseChar;
            if (NoOfUppercaseChar > 0 && pswdStr.Length >= endIndx)
            {
                passwdSB.Append(pswdStr.Substring(startIndx, endIndx - startIndx).ToUpper());
                startIndx = endIndx;
            }
            passwdSB.Append(pswdStr.Substring(startIndx, pswdChrLength - startIndx));

            passwd = passwdSB.ToString();

            int digit;
            int rndmIndx;
            for (int i = 0; i < NoOfDigitChar; i++)
            {
                bool exclude = true;
                while (exclude)
                {
                    digit = GetSecureRandomInt(0, 9);
                    if (!excludedDigits.Contains(digit))
                    {
                        rndmIndx = GetSecureRandomInt(0, pswdChrLength - 1);
                        passwd = passwd.Insert(rndmIndx, digit.ToString());
                        pswdChrLength++;
                        exclude = false;
                    }
                }

            }

            for (int i = 0; i < NoOfSpecialCharacter && i < spchar.Length; i++)
            {
                specialCharRandIndx = GetSecureRandomInt(0, spchar.Length - 1);
                rndmIndx = GetSecureRandomInt(0, pswdChrLength - 1);
                passwd = passwd.Insert(rndmIndx, spchar[specialCharRandIndx]);
            }
            //some system in sap don't accept lower case letters as password
            if (NoOfLowercaseChar == 0)
            {
                passwd = passwd.ToUpper();
            }
            return passwd;
        }

        private static string GetRandomString(int size, char[] excludedChars)
        {
            // A=65, Z=90, a=97, z=122
            StringBuilder sb = new StringBuilder();
            char tmpCh;
            for (int tmpInt = 0; tmpInt < size; tmpInt++)
            {
                bool exclude = true;
                while (exclude)
                {
                    tmpCh = Convert.ToChar(GetSecureRandomInt(97, 122));
                    if (!excludedChars.Contains(tmpCh))
                    {
                        sb.Append(tmpCh);
                        exclude = false;
                    }
                }
            }
            return sb.ToString();
        }

        private static int GetSecureRandomInt(int minValue, int maxValueExclusive)
        {
            if (maxValueExclusive <= minValue)
                return minValue;

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                uint value = BitConverter.ToUInt32(bytes, 0);
                return minValue + (int)(value % (uint)(maxValueExclusive - minValue));
            }
        }
        public static bool IsPolicyEndpointExists(long id)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new PasswordPolicyRepository(dataAccessTransaction).IsPolicyEndpointExists(id);
            }
        }
        
    }
}
