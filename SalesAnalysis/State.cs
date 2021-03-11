
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SalesAnalysis
{
    public class State
    {

        [XmlElement("name")]
        public string StateName { get; set; }

        [XmlElement("state_code")]
        public string StateAbbreviation { get; set; }

        [XmlElement("id")]
        public int StateId { get; set; }

        [XmlElement(ElementName = "cities")]
        public List<City> Cities { get; set; }
    }
}
