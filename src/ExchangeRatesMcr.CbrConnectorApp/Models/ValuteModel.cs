using System;
using System.Xml.Serialization;

namespace ExchangeRatesMcr.CbrConnectorApp.Models
{
    [Serializable]
    public class ValutaModel
    {
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public string EngName { get; set; }
        [XmlElement]
        public double Nominal { get; set; }
        [XmlElement]
        public string ParentCode { get; set; }
    }
}
