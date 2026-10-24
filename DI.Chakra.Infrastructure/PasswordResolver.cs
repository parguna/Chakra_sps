using System;
using System.Security.Cryptography;
using System.Text;

namespace DI.Chakra.Infrastructure
{
    public class PasswordResolver
    {

        public string ConstructGeneralUserPassword(int PasswordLength, int NoOfSpecialCharacter, int NoOfLowercaseChar,
                                                                                        int NoOfUppercaseChar, int NoOfDigitChar)
        {
            string passwd = string.Empty;
            StringBuilder passwdSB = new StringBuilder();
            //string[] spchar = new string[] { "-", "!", "$", "@", "#", ".", ",", "^" };
            string[] spchar = new string[] { "$", "@", "#" };
            int specialCharRandIndx;
            int startIndx = 0;
            int endIndx = 0;

            int pswdChrLength = (PasswordLength - NoOfSpecialCharacter - NoOfDigitChar);

            pswdChrLength = pswdChrLength > 0 ? pswdChrLength : 8;

            string pswdStr = GetRandomString(pswdChrLength);

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
                digit = GetSecureRandomInt(0, 9);
                rndmIndx = GetSecureRandomInt(0, pswdChrLength - 1);
                passwd = passwd.Insert(rndmIndx, digit.ToString());
                pswdChrLength++;
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

        private string GetRandomString(int size)
        {
            // A=65, Z=90, a=97, z=122
            const string valid = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder sb = new StringBuilder();
            while (0 < size--)
            {
                sb.Append(valid[GetSecureRandomInt(0, valid.Length)]);
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
    }
}


