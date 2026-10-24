using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Connector.ESV
{

    [XmlRoot(ElementName = "Roles")]
    public class Role
    {
        [XmlElement(ElementName = "RoleCode")]
        public List<string> RoleCode { get; set; }
        [XmlAttribute(AttributeName = "actiontype")]
        public string Actiontype { get; set; }
    }

    [XmlRoot(ElementName = "Root")]
    public class Root
    {
        [XmlElement(ElementName = "Roles")]
        public List<Role> Roles { get; set; }
    }
   
}

