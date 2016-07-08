using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace evenotify_v2.models
{
    public class MailList
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

            private eveapiResultRowset rowsetField;

            /// <remarks/>
            public eveapiResultRowset rowset
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
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class eveapiResultRowset
        {

            private eveapiResultRowsetRow[] rowField;

            private string nameField;

            private string keyField;

            private string columnsField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("row")]
            public eveapiResultRowsetRow[] row
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
        public partial class eveapiResultRowsetRow
        {

            private uint messageIDField;

            private uint senderIDField;

            private string senderNameField;

            private string sentDateField;

            private string titleField;

            private string toCorpOrAllianceIDField;

            private string toCharacterIDsField;

            private string toListIDField;

            private ushort senderTypeIDField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint messageID
            {
                get
                {
                    return this.messageIDField;
                }
                set
                {
                    this.messageIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint senderID
            {
                get
                {
                    return this.senderIDField;
                }
                set
                {
                    this.senderIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string senderName
            {
                get
                {
                    return this.senderNameField;
                }
                set
                {
                    this.senderNameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string sentDate
            {
                get
                {
                    return this.sentDateField;
                }
                set
                {
                    this.sentDateField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string title
            {
                get
                {
                    return this.titleField;
                }
                set
                {
                    this.titleField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string toCorpOrAllianceID
            {
                get
                {
                    return this.toCorpOrAllianceIDField;
                }
                set
                {
                    this.toCorpOrAllianceIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string toCharacterIDs
            {
                get
                {
                    return this.toCharacterIDsField;
                }
                set
                {
                    this.toCharacterIDsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string toListID
            {
                get
                {
                    return this.toListIDField;
                }
                set
                {
                    this.toListIDField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ushort senderTypeID
            {
                get
                {
                    return this.senderTypeIDField;
                }
                set
                {
                    this.senderTypeIDField = value;
                }
            }
        }

    }
}