using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCMServiceIntance
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://xmlns.rwsentosa.com/Enterprise/Tools/schemas/RWS_IDS_OFF_USER_RQST.V1")]
    public partial class RWS_ID_MSG1_HDRMsgDataRecord_TypeShape
    {

        private string rWS_SYSTEMIDField;

        private string eMPLIDField;

        private string oPRIDField;

        private string eMAILIDField;

        private string rWS_TRANSACTION_IDField;

        private string classField;

        public RWS_ID_MSG1_HDRMsgDataRecord_TypeShape()
        {
            this.classField = "R";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 0)]
        public string RWS_SYSTEMID
        {
            get
            {
                return this.rWS_SYSTEMIDField;
            }
            set
            {
                this.rWS_SYSTEMIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        public string EMPLID
        {
            get
            {
                return this.eMPLIDField;
            }
            set
            {
                this.eMPLIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        public string OPRID
        {
            get
            {
                return this.oPRIDField;
            }
            set
            {
                this.oPRIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        public string EMAILID
        {
            get
            {
                return this.eMAILIDField;
            }
            set
            {
                this.eMAILIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        public string RWS_TRANSACTION_ID
        {
            get
            {
                return this.rWS_TRANSACTION_IDField;
            }
            set
            {
                this.rWS_TRANSACTION_IDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }
    }

    ///// <remarks/>
    //[System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    //[System.SerializableAttribute()]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(TypeName = "RWS_ID_MSG1_HDRMsgDataRecord_TypeShape", Namespace = "http://xmlns.rwsentosa.com/Enterprise/Tools/schemas/RWS_IDS_OFF_USER_RESP.V1")]
    //public partial class RWS_ID_MSG1_HDRMsgDataRecord_TypeShape1
    //{

    //    private string rWS_SYSTEMIDField;

    //    private string rWS_TRANSACTION_IDField;

    //    private string rWS_TRANS_STATUSField;

    //    private string eRRORMSGField;

    //    private string classField;

    //    public RWS_ID_MSG1_HDRMsgDataRecord_TypeShape1()
    //    {
    //        this.classField = "R";
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", Order = 0)]
    //    public string RWS_SYSTEMID
    //    {
    //        get
    //        {
    //            return this.rWS_SYSTEMIDField;
    //        }
    //        set
    //        {
    //            this.rWS_SYSTEMIDField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
    //    public string RWS_TRANSACTION_ID
    //    {
    //        get
    //        {
    //            return this.rWS_TRANSACTION_IDField;
    //        }
    //        set
    //        {
    //            this.rWS_TRANSACTION_IDField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
    //    public string RWS_TRANS_STATUS
    //    {
    //        get
    //        {
    //            return this.rWS_TRANS_STATUSField;
    //        }
    //        set
    //        {
    //            this.rWS_TRANS_STATUSField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
    //    public string ERRORMSG
    //    {
    //        get
    //        {
    //            return this.eRRORMSGField;
    //        }
    //        set
    //        {
    //            this.eRRORMSGField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlAttributeAttribute()]
    //    public string @class
    //    {
    //        get
    //        {
    //            return this.classField;
    //        }
    //        set
    //        {
    //            this.classField = value;
    //        }
    //    }
    //}

}
