using C1.Win.C1Chart;
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
using 工作工具库.File_Operation;
using 工作工具库.Protocol_Operation;
using 工作工具库.Window_Operation;

namespace 多设备放电检测测试仪
{
    
    public partial class Multi_Device_Form : Form
    {
       
        private  int[] DCCounts;
        
        private string Xml_Path = "Protocol.xml";
        private ChartDataArray arrHistory1;
        private ChartDataArray arrHistory2;
        private ChartDataArray arrHistory3;
        private ChartDataArray arrHistory4;
        private ChartDataArray arrHistory5;
        private byte[] ReadBytes1 = new byte[4000];
        private byte[] ReadBytes2 = new byte[4000];
        private byte[] ReadBytes3 = new byte[4000];
        private byte[] ReadBytes4 = new byte[4000];
        private byte[] ReadBytes5 = new byte[4000];
        private string[]Protocol_Group=new string[5];
        private string[] Protocol_TitleName_Group = new string[5];
        private Protocols protocols;
        private CartogramPaint cartogramPaint;
        private SendMessageTo16Bytes sendMessageTo16Bytes;
        private Protocols_GURD_XML protocols_GURD;
        private string Txt_Path = "WindowsName.txt";
        private FileCRUD fileCRUD = new FileCRUD();
        public Multi_Device_Form()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            cartogramPaint = new CartogramPaint();
            sendMessageTo16Bytes = new SendMessageTo16Bytes();
            protocols_GURD = new Protocols_GURD_XML();
            protocols = new Protocols();
           
            DCCounts = new int[5];
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
              
        SerialPort serialPort = sender as SerialPort;
            //serialPort.Read(ReadBytes, 0, serialPort.BytesToRead);
            //DCCounts[0] = (Convert.ToInt32(ReadBytes[11]) * 256 + Convert.ToInt32(ReadBytes[12])) * 3;


            if (serialPort==serialPort1)
            {
                serialPort.Read(ReadBytes1, 0, serialPort.BytesToRead);
                DCCounts[0] = (Convert.ToInt32(ReadBytes1[11]) * 256 + Convert.ToInt32(ReadBytes1[12])) * 3;
                Message(DisChargIng_Label_1, Partial_Max_Label_1,Partial_Average_Label_1,Total_Energy_Label_1,ReadBytes1);
            }
            else if(serialPort == serialPort2)
            {
                serialPort.Read(ReadBytes2, 0, serialPort.BytesToRead);
                DCCounts[1] = (Convert.ToInt32(ReadBytes2[11]) * 256 + Convert.ToInt32(ReadBytes2[12])) * 3;
                Message(DisChargIng_Label_2, Partial_Max_Label_2, Partial_Average_Label_2, Total_Energy_Label_2, ReadBytes2);
            }
            else if (serialPort == serialPort3)
            {
                serialPort.Read(ReadBytes3, 0, serialPort.BytesToRead);
                DCCounts[2] = (Convert.ToInt32(ReadBytes3[11]) * 256 + Convert.ToInt32(ReadBytes3[12])) * 3;
                Message(DisChargIng_Label_3, Partial_Max_Label_3, Partial_Average_Label_3, Total_Energy_Label_3, ReadBytes3);
            }
            else if (serialPort == serialPort4)
            {
                serialPort.Read(ReadBytes4, 0, serialPort.BytesToRead);
                DCCounts[3] = (Convert.ToInt32(ReadBytes4[11]) * 256 + Convert.ToInt32(ReadBytes4[12])) * 3;
                Message(DisChargIng_Label_4, Partial_Max_Label_4, Partial_Average_Label_4, Total_Energy_Label_4, ReadBytes4);
            }
            else if (serialPort == serialPort5)
            {
                serialPort.Read(ReadBytes5, 0, serialPort.BytesToRead);
                DCCounts[4] = (Convert.ToInt32(ReadBytes5[11]) * 256 + Convert.ToInt32(ReadBytes5[12])) * 3;
                Message(DisChargIng_Label_5, Partial_Max_Label_5, Partial_Average_Label_5, Total_Energy_Label_5, ReadBytes5);
            }
       
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
                        serialPort2.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;

                case "Send_3":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[2], 8);
                        serialPort3.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_4":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[3], 8);
                        serialPort4.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_5":
                    {
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[4], 8);
                        serialPort5.Write(SendProtocol, 0, 8);
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
                        //cartogramPaint.ShiftRight();
                        paint_2d(ref arrHistory1, ref DCCounts, 0);
                    }
                 ;
                    break;
                case "2":
                    {
                        paint_2d(ref arrHistory2, ref DCCounts, 1);
                    }
                 ;
                    break;

                case "3":
                    {
                        paint_2d(ref arrHistory3, ref DCCounts, 2);
                    }
                 ;
                    break;
                case "4":
                    {
                        paint_2d(ref arrHistory4, ref DCCounts, 3);
                    }
                 ;
                    break;
                case "5":
                    {
                        paint_2d(ref arrHistory5, ref DCCounts, 4);
                    }
                 ;
                    break;



            }
            //paint_2d(ref arrHistory1, ref DCCounts, 4);
            timer.Start();
        }
        private void paint_2d(ref ChartDataArray arrhistory,ref int[]DCCounts ,int Count)
        {
              bool left2right = false;
            int hits = 200;
            if (left2right)
            {
                arrhistory.Length++;

                // Reach the max
                if (arrhistory.Length >= hits + 1)
                {
                    arrhistory.Length = 0;
                    arrhistory.Length++;
                }
                arrhistory[arrhistory.Length - 1] = new PointF(arrhistory.Length - 1, Convert.ToInt32(DCCounts[Count]));
            }
            else
            {
                if (arrhistory.Length <= hits + 1)
                    arrhistory.Length++;
                //ShiftRight(arrHistory);
                cartogramPaint.ShiftRight(ref arrhistory);

                arrhistory[0] = new PointF(0, Convert.ToInt32(DCCounts[Count]));
            }

            DCCounts[Count] = 0;//清空实时放电量数据
        }
        private void SerialPort_Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string Btn_Name = button.Name;
            string Btn_Text = button.Text;
           
            if(Btn_Text=="打开串口")
            {
                switch (Btn_Name)
                {
                    case "SerialPort1_Button":
                        {
                            SerialPort_OpenSeting(serialPort1, SerialPort1_ComboBox.Text.Trim());
                            Timer_Send_1.Enabled = true;
                            timer1.Enabled = true;
                            Timer_Send_1.Start();
                            //Timer_Send_2.Start();
                            //Timer_Send_3.Start();
                            //Timer_Send_4.Start();
                            //Timer_Send_5.Start();
                        }
                        ;
                        break;
                    case "SerialPort2_Button":
                        {
                            SerialPort_OpenSeting(serialPort2, SerialPort2_ComboBox.Text.Trim());
                            Timer_Send_2.Enabled = true;
                            timer2.Enabled = true;
                            Timer_Send_2.Start();
                        }
                        ;
                        break;

                    case "SerialPort3_Button":
                        {
                            SerialPort_OpenSeting(serialPort3, SerialPort3_ComboBox.Text.Trim());
                            Timer_Send_3.Enabled = true;
                            timer3.Enabled = true;
                            Timer_Send_3.Start();
                        }
                        ;
                        break;
                    case "SerialPort4_Button":
                        {
                            SerialPort_OpenSeting(serialPort4, SerialPort4_ComboBox.Text.Trim());
                            Timer_Send_4.Enabled = true;
                            timer4.Enabled = true;
                            Timer_Send_4.Start();
                        }
                        ;
                        break;
                    case "SerialPort5_Button":
                        {
                            SerialPort_OpenSeting(serialPort5, SerialPort5_ComboBox.Text.Trim());
                            Timer_Send_5.Enabled = true;
                            timer5.Enabled = true;
                            Timer_Send_5.Start();
                        }
                        ;
                        break;
                }

                SerialPort_Button_StatusChange(button);
            }
            else
            {
                //switch (Btn_Name)
                //{
                //    case "SerialPort1_Button":
                //        {
                          
                //            Timer_Send_1.Enabled = false;
                //            timer1.Enabled = false;
                //            serialPort1.Close();
                //            //Timer_Send_2.Start();
                //            //Timer_Send_3.Start();
                //            //Timer_Send_4.Start();
                //            //Timer_Send_5.Start();
                //        }
                //       ;
                //        break;
                //    case "SerialPort2_Button":
                //        {
                //            Timer_Send_2.Enabled = false;
                //            timer2.Enabled = false;
                //            serialPort2.Close();
                //        }
                //       ;
                //        break;

                //    case "SerialPort3_Button":
                //        {
                //            Timer_Send_3.Enabled = false;
                //            timer3.Enabled = false;
                //            serialPort3.Close();
                //        }
                //       ;
                //        break;
                //    case "SerialPort4_Button":
                //        {
                //            Timer_Send_4.Enabled = false;
                //            timer4.Enabled = false;
                //            serialPort4.Close();
                //        }
                //       ;
                //        break;
                //    case "SerialPort5_Button":
                //        {
                //            Timer_Send_5.Enabled = false;
                //            timer5.Enabled = false;
                //            serialPort5.Close();
                //        }
                //       ;
                //        break;
                //}
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
            try
            {
                serialPort.Open();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           


        }
        private void SetSave(SerialPort serialPort,int BaudRadte,Timer timer,int inteval)
        {
            serialPort.BaudRate = BaudRadte;
            timer.Interval = inteval;
            Equipment_Status_Change(Equipment1_BaudRate_Label, "串口波特率：" + Convert.ToString(BaudRadte));
            Equipment_Status_Change(Equipment1_Inteval_Label, "设备发送时间间隔："+Convert.ToString(inteval).Trim());


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
               
                protocols.Protocolist.Add(new ProtocolSingle("特高频", "01 03 00 1A 00 0A E4 0A"));
                protocols.Protocolist.Add(new ProtocolSingle("高频电流", "01 03 00 24 00 0A 85 C6"));
                protocols.Protocolist.Add(new ProtocolSingle("低压故障电弧", "01 03 00 2E 00 0A A5 C4"));
                protocols.Protocolist.Add(new ProtocolSingle("二合一", "01 03 00 38 00 0A 44 00"));
                protocols.Protocolist.Add(new ProtocolSingle("1号传感器", "01 03 00 10 00 0A C4 08"));
                protocols.Protocolist.Add(new ProtocolSingle("2号传感器", "01 03 00 1A 00 0A E4 0A"));
                protocols_GURD.Protocol_Save(protocols,Xml_Path);



            }
            if(File.Exists(Txt_Path))
            {
                this.Text=fileCRUD.Read(Txt_Path,Encoding.UTF8);
            }
            else
            {
                fileCRUD.Write(Txt_Path, "");
            }
            
                protocols = protocols_GURD.Load(Xml_Path);
                foreach(ProtocolSingle protocol in protocols.Protocolist)
            {
                Protocol_TitleName_ComboBox.Items.Add(protocol.ProtocolName);

            }
            cartogramPaint.CartogramInitialized(ref c1Chart1, ref arrHistory1);
            cartogramPaint.CartogramInitialized(ref c1Chart2, ref arrHistory2);
            cartogramPaint.CartogramInitialized(ref c1Chart3, ref arrHistory3);
            cartogramPaint.CartogramInitialized(ref c1Chart4, ref arrHistory4);
            cartogramPaint.CartogramInitialized(ref c1Chart5, ref arrHistory5);



        }

        private void Protocol_TitleName_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i=0;i<Protocol_TitleName_ComboBox.Items.Count;i++)
            {
                if(Protocol_TitleName_ComboBox.Text== protocols.Protocolist[i].ProtocolName)
                {
                    Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent;
                    break;
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
                        Equipment_Status_Change(Equipment1_ProtocolName_Label, "协议名："+Protocol_TitleName_Group[0]);
                        Equipment_Status_Change(Equipment1_ProtoclContent_Label, "协议内容：" + Protocol_Group[0]);

                    }
                   ;
                    break;
                case "设备2":
                    {
                        SetSave(serialPort2, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_2, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[1] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[1] = Protocol_TitleName_ComboBox.Text.Trim();
                        Equipment_Status_Change(Equipment2_ProtocolName_Label, "协议名：" + Protocol_TitleName_Group[1]);
                        Equipment_Status_Change(Equipment2_ProtoclContent_Label, "协议内容：" + Protocol_Group[1]);
                    }
                   ;
                    break;

                case "设备3":
                    {
                        SetSave(serialPort3, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_3, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[2] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[2] = Protocol_TitleName_ComboBox.Text.Trim();
                        Equipment_Status_Change(Equipment3_ProtocolName_Label, "协议名：" + Protocol_TitleName_Group[2]);
                        Equipment_Status_Change(Equipment3_ProtoclContent_Label, "协议内容：" + Protocol_Group[2]);
                    }
                   ;
                    break;
                case "设备4":
                    {
                        SetSave(serialPort4, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_4, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[3] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[3] = Protocol_TitleName_ComboBox.Text.Trim();
                        Equipment_Status_Change(Equipment4_ProtocolName_Label, "协议名：" + Protocol_TitleName_Group[3]);
                        Equipment_Status_Change(Equipment4_ProtoclContent_Label, "协议内容：" + Protocol_Group[3]);
                    }
                   ;
                    break;
                case "设备5":
                    {
                        SetSave(serialPort5, Convert.ToInt32(SerialPort_BaudRate_ComboBox.Text.Trim()), Timer_Send_5, Convert.ToInt32(Timer_Intevel_ComboBox.Text.Trim()));
                        Protocol_Group[4] = Protocol_Content_TextBox.Text.Trim();
                        Protocol_TitleName_Group[4] = Protocol_TitleName_ComboBox.Text.Trim();
                        Equipment_Status_Change(Equipment5_ProtocolName_Label, "协议名:" + Protocol_TitleName_Group[4]);
                        Equipment_Status_Change(Equipment5_ProtoclContent_Label, "协议内容:" + Protocol_Group[4]);
                    }
                   ;
                    break;



            }
            if(WIndowName_TextBox.Text.Trim()!=null)
            {
                fileCRUD.Write(Txt_Path, WIndowName_TextBox.Text.Trim());
                this.Text = WIndowName_TextBox.Text.Trim();
            }
           
            MessageBox.Show("保存成功");
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
            SerialPort_BaudRate_ComboBox.Text = BaudRateText.Trim();
            Timer_Intevel_ComboBox.Text =TimerIntevelText.Trim();
            Protocol_TitleName_ComboBox.Text =  ProtocolText;
            if(ProtocolText!=null)
            {
                Protocol_TitleName_ComboBox_SelectedIndexChanged(sender, e);
            }
        }
        private void Message(System.Windows.Forms.Label DisChargIng_Label, System.Windows.Forms.Label Partial_Max_Label, System.Windows.Forms.Label Partial_Average_Label, System.Windows.Forms.Label Total_Energy_Label, byte[] ReadBytes)
        {
            DisChargIng_Label.Text = ((Convert.ToInt32(ReadBytes[5]) * 256 + Convert.ToInt32(ReadBytes[6])) * 3).ToString();
            Partial_Max_Label.Text = ((Convert.ToInt32(ReadBytes[7]) * 256 + Convert.ToInt32(ReadBytes[8])) * 5).ToString() + " PC";
            Partial_Average_Label.Text = ((Convert.ToInt32(ReadBytes[9]) * 256 + Convert.ToInt32(ReadBytes[10])) * 5).ToString() + " PC";
            Total_Energy_Label.Text = ((Convert.ToInt32(ReadBytes[11]) * 256 + Convert.ToInt32(ReadBytes[12])) * 3).ToString();
        }
        private void Equipment_Status_Change(System.Windows.Forms.Label Equipment_Label,string Equipment_Text)

        {
            if(Equipment_Text!=null)
            {
                Equipment_Label.Text = Equipment_Text;

            }
        }

        private void Equipment5_ProtoclContent_Label_SizeChanged(object sender, EventArgs e)
        {
            
        }
    }
}
