#region NAMESPACE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#endregion

namespace DI.Chakra.Model
{
    [TableInfo("DI_Oracle11gUM_UserInfoHistory", "Oracle11gUMUserInfoHistory")]
    [Table("DI_Oracle11gUM_UserInfoHistory")]
    public class OracleUMUserInfoHistory:IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public int UserId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string UserName { get; set; }
        public int OraUserId { get; set; }
        public string AccountStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LockAt { get; set; }
        public DateTime? ExpiryAt { get; set; }
        public string DefaultTableSpace { get; set; }
        public string Profile { get; set; }
        public string InitialRSRCConsumerGroup { get; set; }
        public string AuthenticationType { get; set; }
       // public int DIUserId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }




        /// <summary>
        /// To create the object
        /// </summary>
        /// <returns></returns>
        public static OracleUMUserInfoHistory CreateOracleUMUserInfoHistory()
        {
            OracleUMUserInfoHistory oraUserInfoHistory = new OracleUMUserInfoHistory();
            return oraUserInfoHistory;
        }


        public override string ToString()
        {
            return this.UserName;
        }

        /// <summary>
        /// Checks if the given instance is equals to the current one.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if they equals, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is OracleUMUserInfoHistory)
            {
                OracleUMUserInfoHistory other = (OracleUMUserInfoHistory)obj;
                return this.UserId.Equals(other.UserId);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>
        /// <returns>The category hash code.</returns>
        public override int GetHashCode()
        {
            return this.UserId.GetHashCode() ^ this.UserName.GetHashCode();
        }


    }
}
