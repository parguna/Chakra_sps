using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;


namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_DiscoveryProcess")]
    public class DiscoveryProcess : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }
        public long Endpointsystemid { get; set; }
        public bool Default { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public bool DisableDeltaDiscovery { get; set; }

        [ForeignKey("Endpointsystemid")]
        public virtual EndPoint Endpoint { get; set; }

        [DataMember]
        public virtual List<EndpointObjects> EndpointObjects { get; set; }

        public override string ToString()
        {
            return this.ProcessName.ToString();
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

            if (obj is DiscoveryProcess)
            {
                DiscoveryProcess other = (DiscoveryProcess)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Endpointsystemid.GetHashCode();
        }
    }
}
