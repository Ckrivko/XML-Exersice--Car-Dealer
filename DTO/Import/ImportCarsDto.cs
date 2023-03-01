using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("Car")]
    public class ImportCarsDto
    {
                
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistanceance { get; set; }

        [XmlArray("parts")]
        public HashSet<ImportCarPartsDto> Parts { get; set; } = new HashSet<ImportCarPartsDto>();

    }
}
