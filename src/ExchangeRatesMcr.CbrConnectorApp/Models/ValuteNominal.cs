using System;
using System.Xml.Serialization;

namespace ExchangeRatesMcr.CbrConnectorApp.Models
{
    [Serializable]
    public class ValutaNominal
    {
        [XmlElement]
        public string ValuteId { get; set; }
        [XmlElement]
        public DateTime RecordDate { get; set; }
        [XmlElement]
        public double Nominal { get; set; }
        [XmlElement]
        public double Value { get; set; }
    }
}
