using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace 多设备放电检测测试仪.Protocol
{
    public class ProtocolSingle
    {
        [XmlElement("ProtocolName")]
        public string ProtocolName { get; set; }
        [XmlElement("ProtocolContent_1")]

        public string ProtocolContent_1 { get; set; }
        [XmlElement("ProtocolContent_2")]
        public string ProtocolContent_2 { get; set; }
        [XmlElement("ProtocolContent_3")]
        public string ProtocolContent_3 { get; set; }
        [XmlElement("ProtocolContent_4")]
        public string ProtocolContent_4 { get; set; }
        [XmlElement("ProtocolContent_5")]
        public string ProtocolContent_5 { get; set; }

        public ProtocolSingle()
        {

        }
        public ProtocolSingle(string ProtocolName,string ProtocolContent_1, string ProtocolContent_2, string ProtocolContent_3, string ProtocolContent_4, string ProtocolContent_5)
        {
            this.ProtocolName = ProtocolName;
            this.ProtocolContent_1 = ProtocolContent_1;
            this.ProtocolContent_2 = ProtocolContent_2;
            this.ProtocolContent_3 = ProtocolContent_3;
            this.ProtocolContent_4 = ProtocolContent_4;
            this.ProtocolContent_5 = ProtocolContent_5;

        }
        
        

    }
}
