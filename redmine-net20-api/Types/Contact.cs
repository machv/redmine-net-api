using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("easy_contact")]
    public class Contact : Identifiable<Issue>, IXmlSerializable, IEquatable<Contact>
    {
        [XmlElement("firstname")]
        public string FirstName { get; set; }
        [XmlElement("lastname")]
        public string LastName { get; set; }

        [XmlElement("easy_contact_type")]
        public IdentifiableName ContactType { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public IList<IssueCustomField> CustomFields { get; set; }

        public bool Equals(Contact other)
        {
            if (other == null) return false;

            return (Id == other.Id && LastName == other.LastName);
        }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "custom_fields": CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case "easy_contact_type":
                        IdentifiableName contactType = new IdentifiableName();

                        contactType.Id = Convert.ToInt32(reader.GetAttribute("id"));
                        //contactType.Name = reader.GetAttribute("name");
                        reader.Read();
                        if (reader.Name == "name")
                        {
                            contactType.Name = reader.ReadElementContentAsString();
                            reader.Read();
                        }

                        ContactType = contactType;
                        break;

                    case "lastname": LastName = reader.ReadElementContentAsString(); break;

                    case "firstname": FirstName = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(ContactType, "easy_contact_type");
            writer.WriteElementString("firstname", FirstName);
            writer.WriteElementString("lastname", LastName);

            if (CustomFields != null)
            {
                writer.WriteStartElement("custom_fields");
                writer.WriteAttributeString("type", "array");
                foreach (var cf in CustomFields)
                {
                    new XmlSerializer(cf.GetType()).Serialize(writer, cf);
                }
                writer.WriteEndElement();
            }
        }
    }
}
