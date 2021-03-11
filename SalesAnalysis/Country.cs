using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SalesAnalysis
{
    public class Country
    {
        [XmlElement("name")]
        public string CountryName { get; set; }

        [XmlElement("iso3")]
        public string CountryAbbreviation { get; set; }

        [XmlElement("id")]
        public int CountryId { get; set; }

        [XmlElement(ElementName ="states")]
        public List<State> States { get; set; }
    }
}
