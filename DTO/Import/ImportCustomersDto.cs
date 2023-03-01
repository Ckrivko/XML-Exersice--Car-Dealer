using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("Customer")]
    public class ImportCustomersDto
    {

        // <name>Zada Attwood</name>
        // <birthDate>1982-10-01T00:00:00</birthDate>
        //<isYoungDriver>true</isYoungDriver>

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool IsYoungDriver { get; set; }

    }
}
