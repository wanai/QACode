using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SalesAnalysis
{
    [XmlRoot(ElementName = "countries_states_cities")]
    public class CountriesStates
    {
        public CountriesStates()
        {
            List<Country> Countries = new List<Country>();
        }
        [XmlElement(ElementName = "country_state_city")]
        public List<Country> Countries { get; set; }

        public Country this[string name]
        {
            get { return Countries.FirstOrDefault(s => string.Equals(s.CountryName, name, StringComparison.OrdinalIgnoreCase)); }
        }
    }
}
