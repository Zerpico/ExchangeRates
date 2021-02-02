using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectorCbr.Models
{
    public class ValutaNominal 
    {
        public string ValuteId { get; set; }
        public DateTime RecordDate { get; set; }
        public double Nominal { get; set; }
        public double Value { get; set; }       
    }
}
