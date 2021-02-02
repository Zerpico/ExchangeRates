using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ConnectorCbr.Models
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
