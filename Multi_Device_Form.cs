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
        private ChartDataArray arrHistory6;
        private ChartDataArray arrHistory7;
        private byte[] ReadBytes1 = new byte[4000];
        private byte[] ReadBytes2 = new byte[4000];
        private byte[] ReadBytes3 = new byte[4000];
        private byte[] ReadBytes4 = new byte[4000];
        private byte[] ReadBytes5 = new byte[4000];
        private int PortLength = 45;
        private Dictionary<string, SerialPort> serualPort_Dic = new Dictionary<string, SerialPort>(); 
        private string[]Protocol_Group=new string[5];
        private string[] Protocol_TitleName_Group = new string[5];
        private Protocols protocols;
        private CartogramPaint cartogramPaint;
        private SendMessageTo16Bytes sendMessageTo16Bytes;
        private Protocols_GURD_XML protocols_GURD;
        private string Txt_Path = "WindowsName.txt";
        private int Channel = 1;
        private FileCRUD fileCRUD = new FileCRUD();
        public Multi_Device_Form()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            cartogramPaint = new CartogramPaint();
            sendMessageTo16Bytes = new SendMessageTo16Bytes();
            protocols_GURD = new Protocols_GURD_XML();
            protocols = new Protocols();
           
            DCCounts = new int[7];
        }

        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
              
        SerialPort serialPort = sender as SerialPort;
            //serialPort.Read(ReadBytes, 0, serialPort.BytesToRead);
            //DCCounts[0] = (Convert.ToInt32(ReadBytes[11]) * 256 + Convert.ToInt32(ReadBytes[12])) * 3;

            
            if (serialPort==serialPort1)
            {
                serialPort.Read(ReadBytes1, 0, serialPort.BytesToRead);
                byte[] tmp = CRC16(ReadBytes1.Take(PortLength - 2).ToArray(), PortLength - 2);
                if(!(tmp[0] == ReadBytes1[PortLength - 2] && tmp[1] == ReadBytes1[PortLength - 1]))
                {
                    return;
                }
                DCCounts[0] = (Convert.ToInt32(ReadBytes1[11]) * 256 + Convert.ToInt32(ReadBytes1[12]))*3 ;
                DCCounts[5]= (Convert.ToInt32(ReadBytes1[31]) * 256 + Convert.ToInt32(ReadBytes1[32]))*3 ;
                Message(DisChargIng_Label_1, Partial_Max_Label_1,Partial_Average_Label_1,Total_Energy_Label_1,ReadBytes1);
                Message(DisChargIng_Label_6, Partial_Max_Label_6, Partial_Average_Label_6, Total_Energy_Label_6, ReadBytes1);

            }
            else if(serialPort == serialPort2)
            {
                serialPort.Read(ReadBytes2, 0, serialPort.BytesToRead);
                byte[] tmp = CRC16(ReadBytes1.Take(PortLength - 2).ToArray(), PortLength - 2);
                if (!(tmp[0] == ReadBytes1[PortLength - 2] && tmp[1] == ReadBytes1[PortLength - 1]))
                {
                    return;
                }
                DCCounts[1] = (Convert.ToInt32(ReadBytes2[11]) * 256 + Convert.ToInt32(ReadBytes2[12])) ;
                DCCounts[6] = (Convert.ToInt32(ReadBytes2[31]) * 256 + Convert.ToInt32(ReadBytes2[32])) ;
                Message(DisChargIng_Label_2, Partial_Max_Label_2, Partial_Average_Label_2, Total_Energy_Label_2, ReadBytes2);
                Message(DisChargIng_Label_7, Partial_Max_Label_7, Partial_Average_Label_7, Total_Energy_Label_7, ReadBytes2);
            }
            else if (serialPort == serialPort3)
            {
                serialPort.Read(ReadBytes3, 0, serialPort.BytesToRead);
                DCCounts[2] = (Convert.ToInt32(ReadBytes3[11]) * 256 + Convert.ToInt32(ReadBytes3[12])) ;
                Message(DisChargIng_Label_3, Partial_Max_Label_3, Partial_Average_Label_3, Total_Energy_Label_3, ReadBytes3);
            }
            else if (serialPort == serialPort4)
            {
                serialPort.Read(ReadBytes4, 0, serialPort.BytesToRead);
                DCCounts[3] = (Convert.ToInt32(ReadBytes4[11]) * 256 + Convert.ToInt32(ReadBytes4[12])) ;
                Message(DisChargIng_Label_4, Partial_Max_Label_4, Partial_Average_Label_4, Total_Energy_Label_4, ReadBytes4);
            }
            else if (serialPort == serialPort5)
            {
                serialPort.Read(ReadBytes5, 0, serialPort.BytesToRead);
                DCCounts[4] = (Convert.ToInt32(ReadBytes5[11]) * 256 + Convert.ToInt32(ReadBytes5[12])) ;
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
                        if (Protocol_Group[0] == null)
                        {
                            MessageBox.Show("发送协议不能为空");
                            SerialPort_Button_Click(SerialPort1_Button, e);
                            return;
                        }
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[0],8);
                        serialPort1.Write(SendProtocol,0,8);
                    }
                ;
                    break;
                case "Send_2":
                    {
                        if (Protocol_Group[1] == null)
                        {
                            MessageBox.Show("协议不能为空");
                            SerialPort_Button_Click(SerialPort2_Button, e);
                            return;
                        }
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[1], 8);
                        serialPort2.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;

                case "Send_3":
                    {
                        if (Protocol_Group[2] == null)
                        {
                            MessageBox.Show("协议不能为空");
                            SerialPort_Button_Click(SerialPort3_Button, e);
                            return;
                        }
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[2], 8);
                        serialPort3.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_4":
                    {
                        if (Protocol_Group[3] == null)
                        {
                            MessageBox.Show("协议不能为空");
                            SerialPort_Button_Click(SerialPort4_Button, e);
                            return;
                        }
                        byte[] SendProtocol = sendMessageTo16Bytes.StringTo16Bytes(Protocol_Group[3], 8);
                        serialPort4.Write(SendProtocol, 0, 8);
                    }
                ;
                    break;
                case "Send_5":
                    {
                        if (Protocol_Group[4] == null)
                        {
                            MessageBox.Show("协议不能为空");
                            SerialPort_Button_Click(SerialPort5_Button, e);
                            return;
                        }
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
                        paint_2d(ref arrHistory6, ref DCCounts, 5);
                    }
                 ;
                    break;
                case "2":
                    {
                        paint_2d(ref arrHistory2, ref DCCounts, 1);
                        paint_2d(ref arrHistory7, ref DCCounts, 6);
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
            try
            {
                if (Btn_Text == "打开串口")
                {
                    switch (Btn_Name)
                    {
                        case "SerialPort1_Button":
                            {

                                if (SerialPort1_ComboBox.Text.Trim() == "")
                                {
                                    MessageBox.Show("端口不能为空");
                                    return;
                                }

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
                                if (SerialPort2_ComboBox.Text.Trim() == "")
                                {
                                    MessageBox.Show("端口不能为空");
                                    return;
                                }

                                SerialPort_OpenSeting(serialPort2, SerialPort2_ComboBox.Text.Trim());
                                Timer_Send_2.Enabled = true;
                                timer2.Enabled = true;
                                Timer_Send_2.Start();




                            }
                            ;
                            break;

                        case "SerialPort3_Button":
                            {
                                if (SerialPort3_ComboBox.Text.Trim() == "")
                                {
                                    MessageBox.Show("端口不能为空");
                                    return;
                                }
                                SerialPort_OpenSeting(serialPort3, SerialPort3_ComboBox.Text.Trim());
                                Timer_Send_3.Enabled = true;
                                timer3.Enabled = true;
                                Timer_Send_3.Start();
                            }
                            ;
                            break;
                        case "SerialPort4_Button":
                            {
                                if (SerialPort4_ComboBox.Text.Trim() == "")
                                {
                                    MessageBox.Show("端口不能为空");
                                    return;
                                }
                                SerialPort_OpenSeting(serialPort4, SerialPort4_ComboBox.Text.Trim());
                                Timer_Send_4.Enabled = true;
                                timer4.Enabled = true;
                                Timer_Send_4.Start();
                            }
                            ;
                            break;
                        case "SerialPort5_Button":
                            {
                                if (SerialPort5_ComboBox.Text.Trim() == "")
                                {
                                    MessageBox.Show("端口不能为空");
                                    return;
                                }
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
                else if (Btn_Text == "关闭串口")
                {
                    switch (Btn_Name)
                    {
                        case "SerialPort1_Button":
                            {

                                Timer_Send_1.Enabled = false;
                                timer1.Enabled = false;
                                Timer_Send_1.Stop();
                                serialPort1.Dispose();
                                serialPort1.Close();
                                //Timer_Send_2.Start();
                                //Timer_Send_3.Start();
                                //Timer_Send_4.Start();
                                //Timer_Send_5.Start();
                            }
                               ;
                            break;
                        case "SerialPort2_Button":
                            {

                                Timer_Send_2.Enabled = false;
                                timer2.Enabled = false;
                                Timer_Send_2.Stop();
                                serialPort2.Dispose();
                                serialPort2.Close();
                            }
                               ;
                            break;

                        case "SerialPort3_Button":
                            {

                                Timer_Send_3.Enabled = false;
                                timer3.Enabled = false;
                                Timer_Send_3.Stop();
                                serialPort3.Dispose();
                                serialPort3.Close();
                            }
                               ;
                            break;
                        case "SerialPort4_Button":
                            {

                                Timer_Send_4.Enabled = false;
                                timer4.Enabled = false;
                                Timer_Send_4.Stop();
                                serialPort4.Dispose();
                                serialPort4.Close();
                            }
                               ;
                            break;
                        case "SerialPort5_Button":
                            {

                                Timer_Send_5.Enabled = false;
                                timer5.Enabled = false;
                                Timer_Send_5.Stop();
                                serialPort5.Dispose();
                                serialPort5.Close();
                            }
                               ;
                            break;
                    }

                    SerialPort_Button_StatusChange(button);
                }
            }
            catch
            {
                MessageBox.Show("端口被占用");
            }
           
         
        }
        private void SerialPort_Button_StatusChange( Button btn)
        {
            if(btn.Text=="打开串口")
            {
                btn.BackColor = Color.Red;
                btn.Text = "关闭串口";
            }
            else if(btn.Text== "关闭串口")
            {
                btn.BackColor = Color.White;
                btn.Text = "打开串口";

            }
           
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
               
                protocols.Protocolist.Add(new ProtocolSingle("超声波", "01 03 00 10 00 0A C4 08", "02 03 00 10 00 0A C4 3B", "03 03 00 10 00 0A C5 EA", "04 03 00 10 00 0A C4 5D", "05 03 00 10 00 0A C5 8C"));
                protocols.Protocolist.Add(new ProtocolSingle("特高频", "01 03 00 1A 00 0A E4 0A", "02 03 00 1A 00 0A E4 39", "03 03 00 1A 00 0A E5 E8", "04 03 00 1A 00 0A E4 5F", "05 03 00 1A 00 0A E5 8E"));
                protocols.Protocolist.Add(new ProtocolSingle("高频电流", "01 03 00 24 00 0A 85 C6", "02 03 00 24 00 0A 85 F5", "03 03 00 24 00 0A 84 24", "04 03 00 24 00 0A 85 93", "05 03 00 24 00 0A 84 42"));
                protocols.Protocolist.Add(new ProtocolSingle("低压故障电弧", "01 03 00 2E 00 0A A5 C4", "02 03 00 2E 00 0A A5 F7", "03 03 00 2E 00 0A A4 26", "04 03 00 2E 00 0A A5 91", "05 03 00 2E 00 0A A4 40"));
                protocols.Protocolist.Add(new ProtocolSingle("二合一", "01 03 00 10 00 0F 04 0B", "02 03 00 10 00 0F 04 38", "03 03 00 10 00 0F 05 E9", "04 03 00 10 00 0F 04 5E", "05 03 00 10 00 0F 05 8F"));
                protocols.Protocolist.Add(new ProtocolSingle("基站局放检测", "01 03 00 10 00 14 44 00", "02 03 00 10 00 14 44 33", "03 03 00 10 00 0F 05 E9", "04 03 00 10 00 0F 04 5E", "05 03 00 10 00 0F 05 8F"));
                //protocols.Protocolist.Add(new ProtocolSingle("1号传感器", "01 03 00 10 00 0A C4 08","","","",""));
                //protocols.Protocolist.Add(new ProtocolSingle("2号传感器", "01 03 00 1A 00 0A E4 0A","","","",""));
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
            cartogramPaint.CartogramInitialized(ref c1Chart6, ref arrHistory6);
            cartogramPaint.CartogramInitialized(ref c1Chart7, ref arrHistory7);


        }

        private void Protocol_TitleName_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
         
            switch(Channel)
            {
                case 1:
                    {
                        for (int i = 0; i < Protocol_TitleName_ComboBox.Items.Count; i++)
                        {
                            if (Protocol_TitleName_ComboBox.Text == protocols.Protocolist[i].ProtocolName)
                            {
                                Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent_1;
                                break;
                            }
                        }
                    };
                    break;
                case 2:
                    {
                        for (int i = 0; i < Protocol_TitleName_ComboBox.Items.Count; i++)
                        {
                            if (Protocol_TitleName_ComboBox.Text == protocols.Protocolist[i].ProtocolName)
                            {
                                Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent_2;
                                break;
                            }
                        }
                    };
                    break;
                case 3:
                    {
                        for (int i = 0; i < Protocol_TitleName_ComboBox.Items.Count; i++)
                        {
                            if (Protocol_TitleName_ComboBox.Text == protocols.Protocolist[i].ProtocolName)
                            {
                                Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent_3;
                                break;
                            }
                        }
                    };
                    break;
                case 4:
                    {
                        for (int i = 0; i < Protocol_TitleName_ComboBox.Items.Count; i++)
                        {
                            if (Protocol_TitleName_ComboBox.Text == protocols.Protocolist[i].ProtocolName)
                            {
                                Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent_4;
                                break;
                            }
                        }
                    };
                    break;
                case 5:
                    {
                        for (int i = 0; i < Protocol_TitleName_ComboBox.Items.Count; i++)
                        {
                            if (Protocol_TitleName_ComboBox.Text == protocols.Protocolist[i].ProtocolName)
                            {
                                Protocol_Content_TextBox.Text = protocols.Protocolist[i].ProtocolContent_5;
                                break;
                            }
                        }
                    };
                    break;
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
                        Equipment_Status_Change(Equipment1_BaudRate_Label, "串口波特率：" + Convert.ToString(SerialPort_BaudRate_ComboBox.Text.Trim()));
                        Equipment_Status_Change(Equipment1_Inteval_Label, "设备发送时间间隔：" + Convert.ToString(Timer_Intevel_ComboBox.Text.Trim()));
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
                        Equipment_Status_Change(Equipment2_BaudRate_Label, "串口波特率：" + Convert.ToString(SerialPort_BaudRate_ComboBox.Text.Trim()));
                        Equipment_Status_Change(Equipment2_Inteval_Label, "设备发送时间间隔：" + Convert.ToString(Timer_Intevel_ComboBox.Text.Trim()));
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
                        Equipment_Status_Change(Equipment3_BaudRate_Label, "串口波特率：" + Convert.ToString(SerialPort_BaudRate_ComboBox.Text.Trim()));
                        Equipment_Status_Change(Equipment3_Inteval_Label, "设备发送时间间隔：" + Convert.ToString(Timer_Intevel_ComboBox.Text.Trim()));
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
                        Equipment_Status_Change(Equipment4_BaudRate_Label, "串口波特率：" + Convert.ToString(SerialPort_BaudRate_ComboBox.Text.Trim()));
                        Equipment_Status_Change(Equipment4_Inteval_Label, "设备发送时间间隔：" + Convert.ToString(Timer_Intevel_ComboBox.Text.Trim()));
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
                        Equipment_Status_Change(Equipment5_BaudRate_Label, "串口波特率：" + Convert.ToString(SerialPort_BaudRate_ComboBox.Text.Trim()));
                        Equipment_Status_Change(Equipment5_Inteval_Label, "设备发送时间间隔：" + Convert.ToString(Timer_Intevel_ComboBox.Text.Trim()));
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
                        Channel = 1;


                    }
                  ;
                    break;
                case "设备2":
                    {
                        BaudRateText = Convert.ToString(serialPort2.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_2.Interval);
                        ProtocolText = Protocol_TitleName_Group[1];
                        Channel = 2;
                    }
                  ;
                    break;

                case "设备3":
                    {
                        BaudRateText = Convert.ToString(serialPort3.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_3.Interval);
                        ProtocolText = Protocol_TitleName_Group[2];
                        Channel = 3;
                    }
                  ;
                    break;
                case "设备4":
                    {
                        BaudRateText = Convert.ToString(serialPort4.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_4.Interval);
                        ProtocolText = Protocol_TitleName_Group[3];
                        Channel = 4;
                    }
                  ;
                    break;
                case "设备5":
                    {
                        BaudRateText = Convert.ToString(serialPort5.BaudRate);
                        TimerIntevelText = Convert.ToString(Timer_Send_5.Interval);
                        ProtocolText = Protocol_TitleName_Group[4];
                        Channel = 5;
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
            DisChargIng_Label.Text = ((Convert.ToInt32(ReadBytes[5]) * 256 + Convert.ToInt32(ReadBytes[6])) ).ToString();
            Partial_Max_Label.Text = ((Convert.ToInt32(ReadBytes[7]) * 256 + Convert.ToInt32(ReadBytes[8])) ).ToString() + " PC";
            Partial_Average_Label.Text = ((Convert.ToInt32(ReadBytes[9]) * 256 + Convert.ToInt32(ReadBytes[10])) ).ToString() + " PC";
            Total_Energy_Label.Text = ((Convert.ToInt32(ReadBytes[11]) * 256 + Convert.ToInt32(ReadBytes[12])) ).ToString();
        }
        private void Message_2(System.Windows.Forms.Label DisChargIng_Label, System.Windows.Forms.Label Partial_Max_Label, System.Windows.Forms.Label Partial_Average_Label, System.Windows.Forms.Label Total_Energy_Label, byte[] ReadBytes)
        {
            DisChargIng_Label.Text = ((Convert.ToInt32(ReadBytes[25]) * 256 + Convert.ToInt32(ReadBytes[26]))).ToString();
            Partial_Max_Label.Text = ((Convert.ToInt32(ReadBytes[27]) * 256 + Convert.ToInt32(ReadBytes[28]))).ToString() + " PC";
            Partial_Average_Label.Text = ((Convert.ToInt32(ReadBytes[29]) * 256 + Convert.ToInt32(ReadBytes[30]))).ToString() + " PC";
            Total_Energy_Label.Text = ((Convert.ToInt32(ReadBytes[31]) * 256 + Convert.ToInt32(ReadBytes[32]))).ToString();
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
        #region CRC
        /// <summary>
        /// CRC 高位校验码 checkCRCLow
        /// </summary>
        private static readonly byte[] checkCRCHigh =
            {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };//CRC表,将事先计算好的CRC保存在数组中，以此节省一部分计算时间

        /// <summary>
        /// CRC 低位校验码 checkCRCLow
        /// </summary>
        private static readonly byte[] checkCRCLow =
            {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
            0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
            0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
            0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
            0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
            0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
            0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
            0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
            0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
            0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
            0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
            0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
            0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
            0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
            0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
            0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
            0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
            0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };//CRC表
        /// <summary>
        /// CRC 校验
        /// </summary>
        /// <param name="data"> 校验的字节数组 </param>
        /// <param name="length"> 校验的数组长度 </param>
        /// <returns> 该字节数组的奇偶校验字节 </returns>
        public static byte[] CRC16(byte[] data, int arrayLength) //CRC 校验
        {
            byte CRCHigh = 0xFF; // 重置 CRC 高位校验码
            byte CRCLow = 0xFF; // 重置 CRC 低位校验码
            byte index;
            int i = 0;
            while (arrayLength-- > 0)
            {
                index = (byte)(CRCHigh ^ data[i++]);
                CRCHigh = (byte)(CRCLow ^ checkCRCHigh[index]);
                CRCLow = checkCRCLow[index];
            }
            // int CRC1 = int.Parse(ReturnData[0].ToString());
            // int CRC2 = int.Parse(ReturnData[1].ToString()); 
            // string s1 = CRC1.ToString("x");
            // string s2 = CRC2.ToString("x");
            // byte[] Bytedata = {byte.Parse(s1), byte.Parse(s2)};
            return new byte[] { CRCHigh, CRCLow };
        }
        #endregion
        #region 协议
        /// <summary>
        /// 协议类
        /// </summary> 
        
        
       
    
        #endregion
    }
}
