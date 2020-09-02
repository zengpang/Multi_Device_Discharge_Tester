using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace 多设备放电检测测试仪.Protocol
{
    [XmlRoot("ProtocolRoot")]
    public  class Protocols
    {
        [XmlArray("Protocols"), XmlArrayItem("Protocol")]
        public List<ProtocolSingle> Protocolist { get; set; } = new List<ProtocolSingle>();
    }
}
