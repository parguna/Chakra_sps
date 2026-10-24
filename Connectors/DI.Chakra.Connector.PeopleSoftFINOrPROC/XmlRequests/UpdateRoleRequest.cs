using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DI.Chakra.Connector.PeopleSoftFINOrPROC.XmlRequests
{

    [XmlRoot(ElementName = "Role")]
    public class Role
    {
        [XmlElement(ElementName = "RoleID")]
        public List<string> RoleID { get; set; }
        [XmlAttribute(AttributeName = "action")]
        public string Action { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "UpdateRoler")]
    public class UpdateRoler
    {
        [XmlElement(ElementName = "EmployeeID")]
        public string EmployeeID { get; set; }
        [XmlElement(ElementName = "ADID")]
        public string ADID { get; set; }
        [XmlElement(ElementName = "Role")]
        public List<Role> Role { get; set; }
    }

}
