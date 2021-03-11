

using System.Xml.Serialization;

namespace SalesAnalysis
{
    public class City
    {
        [XmlElement("name")]
        public string Name { get; set; }
        
        [XmlElement("id")]
        public int CityId { get; set; }    
    }
}
