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
        [XmlElement("ProtocolContent")]
        public string ProtocolContent { get; set; }
        public ProtocolSingle()
        {

        }
        public ProtocolSingle(string ProtocolName,string ProtocolContent)
        {
            this.ProtocolName = ProtocolName;
            this.ProtocolContent = ProtocolContent;

        }
        
        

    }
}
