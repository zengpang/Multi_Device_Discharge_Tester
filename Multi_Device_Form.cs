using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 多设备放电检测测试仪.Protocol;
using 多设备放电检测测试仪.Protocols_Operation;
using 工作工具库.Cartogram;
using 工作工具库.Protocol_Operation;

namespace 多设备放电检测测试仪
{
    
    public partial class Multi_Device_Form : Form
    {
        private byte[] ReadBytes = new byte[4000];
        private  int DCCount = 0;
        private string Xml_Path = "Protocol.xml";
         
        private string[]Protocol_Group=new string[5];
        private string[] Protocol_TitleName_Group = new string[5];
        private Protocols protocols;
        private CartogramPaint cartogramPaint;
        private SendMessageTo16Bytes sendMessageTo16Bytes;
        private Protocols_GURD_XML protocols_GURD;

        public Multi_Device_Form()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            cartogramPaint = new CartogramPaint();
            sendMessageTo16Bytes = new SendMessageTo16Bytes();
            protocols_GURD = new Protocols_GURD_XML();
            protocols = new Protocols();

        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = sender as SerialPort;
            serialPort.Read(ReadBytes,0, serialPort.BytesToRead);

            DCCount = (Convert.ToInt32(ReadBytes[11]) * 256 + Convert.ToInt32(ReadBytes[12])) * 3;
        }

        private void timer_Send_Tick(object sender, EventArgs e)
        {
            Timer timer = sender as Timer;
            timer.Stop();
            string Timer_Tag = timer.Tag as string;
            switch (Timer_Tag)
            {
                case "Send_1":
                    {
                         byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[0],8);
                        serialPort1.Write(SendProtocol,0,8);
                    }
                ;
                    break;
                case "Send_2":
                    {
                         byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[1], 8);
                        serialPort1.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;

                case "Send_3":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[2], 8);
                        serialPort1.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_4":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[3], 8);
                        serialPort1.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_5":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[4], 8);
                        serialPort1.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;



            }

            timer.Start();
        }
        private void timer_paint_Tick(object sender, EventArgs e)
        {
            Timer timer = sender as Timer;
            string Timer_Tag = timer.Tag as string;
            timer.Stop();
            switch (Timer_Tag)
            {
                case "1":
                    {
                        cartogramPaint.RealtimeCharge(Convert.ToInt32(DCCount), ref c1Chart1);
                    }
                 ;
                    break;
                case "2":
                    {
                        cartogramPaint.RealtimeCharge(Convert.ToInt32(DCCount), ref c1Chart2);
                    }
                 ;
                    break;

                case "3":
                    {
                        cartogramPaint.RealtimeCharge(Convert.ToInt32(DCCount), ref c1Chart3);
                    }
                 ;
                    break;
                case "4":
                    {
                        cartogramPaint.RealtimeCharge(Convert.ToInt32(DCCount), ref c1Chart4);
                    }
                 ;
                    break;
                case "5":
                    {
                        cartogramPaint.RealtimeCharge(Convert.ToInt32(DCCount), ref c1Chart5);
                    }
                 ;
                    break;



            }
            
            timer.Start();
        }

        private void SerialPort_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string Btn_Name = button.Name;
            SerialPort_Button_StatusChange(button);
            switch (Btn_Name)
            {
                case "SerialPort1_Button":
                    {
                        SerialPort_OpenSeting(serialPort1, SerialPort1_ComboBox.Text.Trim());
                    }
                    ;
                    break;
                case "SerialPort2_Button":
                    {
                        SerialPort_OpenSeting(serialPort2, SerialPort2_ComboBox.Text.Trim());
                    }
                    ;
                    break;

                case "SerialPort3_Button":
                    {
                        SerialPort_OpenSeting(serialPort3, SerialPort3_ComboBox.Text.Trim());
                    }
                    ;
                    break;
                case "SerialPort4_Button":
                    {
                        SerialPort_OpenSeting(serialPort4, SerialPort4_ComboBox.Text.Trim());
                    }
                    ;
                    break;
                case "SerialPort5_Button":
                    {
                        SerialPort_OpenSeting(serialPort5, SerialPort5_ComboBox.Text.Trim());
                    }
                    ;
                    break;



            }
        }
        private void SerialPort_Button_StatusChange( Button btn)
        {
            btn.BackColor = Color.Red;
            btn.Text = "关闭串口";
        }
        private void SerialPort_OpenSeting(SerialPort serialPort,string text)
        {
            serialPort.PortName = text;
            serialPort.Open();

        }
        private void SetSave(SerialPort serialPort,int BaudRadte,Timer timer,int inteval)
        {
            serialPort.BaudRate = BaudRadte;
            timer.Interval = inteval;

        }
        private void ComboBox_StartSeting(ComboBox comboBox)
        {
            foreach(string ComNameItem in SerialPort.GetPortNames())
            {
                comboBox.Items.Add(ComNameItem);

            }
        }

        private void Multi_Device_Form_Load(object sender, EventArgs e)
        {
            ComboBox_StartSeting(SerialPort1_ComboBox);
            ComboBox_StartSeting(SerialPort2_ComboBox);
            ComboBox_StartSeting(SerialPort3_ComboBox);
            ComboBox_StartSeting(SerialPort4_ComboBox);
            ComboBox_StartSeting(SerialPort5_ComboBox);
            if(!File.Exists(Xml_Path))
            {
                
                Protocols protocols = new Protocols();
                protocols.Protocolist.Add(new ProtocolSingle("红外线", "ee e1 01 55 ff fc fd ff"));
                protocols_GURD.Protocol_Save(protocols,Xml_Path);



            }
         
                protocols = protocols_GURD.Load(Xml_Path);
                foreach(ProtocolSingle protocol in protocols.Protocolist)
            {
                Protocol_TitleName_ComboBox.Items.Add(protocol.ProtocolName);

            }
       

        }

        private void Protocol_TitleName_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i=0;i<Protocol_TitleName_ComboBox.Items.Count;i++)
            {
                if(Protocol_TitleName_ComboBox.Items[i].ToString()== protocols.Protocolist[i].ProtocolName)
                {
                    Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent;
                }
            }
        }

        private void SaveSet_Button_Click(object sender, EventArgs e)
        {
            
            switch (Equipment_ComboBox.Text.Trim())
            {
                case "设备1":
                    {

                        SetSave(serialPort1, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_1, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[0]= Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[0] = Protocol_TitleName_ComboBox.Text.Trim();

                    }
                   ;
                    break;
                case "设备2":
                    {
                        SetSave(serialPort2, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_2, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[1] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[1] = Protocol_TitleName_ComboBox.Text.Trim();
                    }
                   ;
                    break;

                case "设备3":
                    {
                        SetSave(serialPort3, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_3, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[2] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[2] = Protocol_TitleName_ComboBox.Text.Trim();
                    }
                   ;
                    break;
                case "设备4":
                    {
                        SetSave(serialPort4, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_4, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[3] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[3] = Protocol_TitleName_ComboBox.Text.Trim();
                    }
                   ;
                    break;
                case "设备5":
                    {
                        SetSave(serialPort5, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_5, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[4] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[4] = Protocol_TitleName_ComboBox.Text.Trim();
                    }
                   ;
                    break;



            }
        }

        private void Equipment_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Protocol_Content_TextBox.Clear();
            string BaudRateText = "";
            string TimerIntevelText = "";
            string ProtocolText = "";


            switch (Equipment_ComboBox.Text.Trim())
            {
                case "设备1":
                    {
                        BaudRateText = Convert.ToString( serialPort1.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_1.Interval);
                        ProtocolText = Protocol_TitleName_Group[0];



                    }
                  ;
                    break;
                case "设备2":
                    {
                        BaudRateText = Convert.ToString(serialPort2.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_2.Interval);
                        ProtocolText = Protocol_TitleName_Group[1];
                    }
                  ;
                    break;

                case "设备3":
                    {
                        BaudRateText = Convert.ToString(serialPort3.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_3.Interval);
                        ProtocolText = Protocol_TitleName_Group[2];
                    }
                  ;
                    break;
                case "设备4":
                    {
                        BaudRateText = Convert.ToString(serialPort4.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_4.Interval);
                        ProtocolText = Protocol_TitleName_Group[3];
                    }
                  ;
                    break;
                case "设备5":
                    {
                        BaudRateText = Convert.ToString(serialPort5.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_5.Interval);
                        ProtocolText = Protocol_TitleName_Group[4];
                    }
                  ;
                    break;



            }
            SerialPort_BaudRate_ComboBox.Text = "  " + BaudRateText.Trim();
            Timer_Intevel_ComboBox.Text = " " + TimerIntevelText.Trim();
            Protocol_TitleName_ComboBox.Text = " " + ProtocolText;
            if(ProtocolText!=null)
            {
                Protocol_TitleName_ComboBox_SelectedIndexChanged(sender, e);
            }
        }
    }
}
