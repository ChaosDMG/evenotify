using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace evenotify_v2.models
{
    public class apiCheck
    {

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class eveapi
        {

            private string currentTimeField;

            private eveapiResult resultField;

            private string cachedUntilField;

            private byte versionField;

            /// <remarks/>
            public string currentTime
            {
                get
                {
                    return this.currentTimeField;
                }
                set
                {
                    this.currentTimeField = value;
                }
            }

            /// <remarks/>
            public eveapiResult result
            {
                get
                {
                    return this.resultField;
                }
                set
                {
                    this.resultField = value;
                }
            }

            /// <remarks/>
            public string cachedUntil
            {
                get
                {
                    return this.cachedUntilField;
                }
                set
                {
                    this.cachedUntilField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte version
            {
                get
                {
                    return this.versionField;
                }
                set
                {
                    this.versionField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class eveapiResult
        {

            private eveapiResultKey keyField;

            /// <remarks/>
            public eveapiResultKey key
            {
                get
                {
                    return this.keyField;
                }
                set
                {
                    this.keyField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class eveapiResultKey
        {

            private eveapiResultKeyRowset rowsetField;

            private uint accessMaskField;

            private string typeField;

            private string expiresField;

            /// <remarks/>
            public eveapiResultKeyRowset rowset
            {
                get
                {
                    return this.rowsetField;
                }
                set
                {
                    this.rowsetField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint accessMask
            {
                get
                {
                    return this.accessMaskField;
                }
                set
                {
                    this.accessMaskField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string type
            {
                get
                {
                    return this.typeField;
                }
                set
                {
                    this.typeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string expires
            {
                get
                {
                    return this.expiresField;
                }
                set
                {
                    this.expiresField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class eveapiResultKeyRowset
        {

            private eveapiResultKeyRowsetRow[] rowField;

            private string nameField;

            private string keyField;

            private string columnsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("row")]
            public eveapiResultKeyRowsetRow[] row
            {
                get
                {
                    return this.rowField;
                }
                set
                {
                    this.rowField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string key
            {
                get
                {
                    return this.keyField;
                }
                set
                {
                    this.keyField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string columns
            {
                get
                {
                    return this.columnsField;
                }
                set
                {
                    this.columnsField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class eveapiResultKeyRowsetRow
        {

            private uint characterIDField;

            private string characterNameField;

            private uint corporationIDField;

            private string corporationNameField;

            private byte allianceIDField;

            private string allianceNameField;

            private byte factionIDField;

            private string factionNameField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint characterID
            {
                get
                {
                    return this.characterIDField;
                }
                set
                {
                    this.characterIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string characterName
            {
                get
                {
                    return this.characterNameField;
                }
                set
                {
                    this.characterNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint corporationID
            {
                get
                {
                    return this.corporationIDField;
                }
                set
                {
                    this.corporationIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string corporationName
            {
                get
                {
                    return this.corporationNameField;
                }
                set
                {
                    this.corporationNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte allianceID
            {
                get
                {
                    return this.allianceIDField;
                }
                set
                {
                    this.allianceIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string allianceName
            {
                get
                {
                    return this.allianceNameField;
                }
                set
                {
                    this.allianceNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte factionID
            {
                get
                {
                    return this.factionIDField;
                }
                set
                {
                    this.factionIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string factionName
            {
                get
                {
                    return this.factionNameField;
                }
                set
                {
                    this.factionNameField = value;
                }
            }
        }



    }
}