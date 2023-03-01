using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("Sale")]
    public  class ImportSalesDto
    {
        //<carId>329</carId>
        //<customerId>26</customerId>
        //<discount>40</discount>

        [XmlElement("carId")]
        public int CarId { get; set; }
        
        [XmlElement("customerId")]
        public int CustomerId { get; set; }
      
        [XmlElement("discount")]
        public decimal Discount { get; set; }


    }
}
