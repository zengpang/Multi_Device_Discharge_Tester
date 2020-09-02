using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using 多设备放电检测测试仪.Protocol;

namespace 多设备放电检测测试仪.Protocols_Operation
{
    public class Protocols_GURD_XML
    {
        public void Protocol_Save(Protocols protocolList, string Protocol_Path)
        {
            using (StreamWriter ProtocolstreamWriter = new StreamWriter(Protocol_Path))
            {
                using (StringWriter ProtocolXmlContent = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Protocols));
                    serializer.Serialize(ProtocolXmlContent, protocolList);
                    ProtocolstreamWriter.Write(ProtocolXmlContent);

                }


            }

        }

        public Protocols Load(string Protocol_Path)
        {
            Protocols protocolList = new Protocols();
            using (StreamReader ProtocolXmlRead = new StreamReader(Protocol_Path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Protocols));
                protocolList = serializer.Deserialize(ProtocolXmlRead) as Protocols;

            }
            return protocolList;
        }
        public Protocols Add(string Protocol_TitleName, string Protocol_Content, Protocols protocolList)
        {
            protocolList.Protocolist.Add(new ProtocolSingle(Protocol_TitleName, Protocol_Content));
            return protocolList;
        }
    }
}
