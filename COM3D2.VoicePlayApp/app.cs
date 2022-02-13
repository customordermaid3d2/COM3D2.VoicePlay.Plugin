using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace VoicePlayApp
{
    public partial class app : Form
    {
        internal static Task task;
        //private static Dictionary<string, string> dic = new Dictionary<string, string>();

        public app()
        {
            InitializeComponent();

            task = Task.Factory.StartNew(() => AcceptLoop());            
        }

        Socket sock;
        private static int mods=0,modr=0;
        private static string sub2_json;
        public static Dictionary<string, Dictionary<string, HashSet<string>>> listSub2;


        private void AcceptLoop()
        {
            while (true)
            {
                // (1) 소켓 객체 생성 (TCP 소켓)
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // (2) 서버에 연결
                var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);                

                do
                {
                    try
                    {                        
                        sock.Connect(ep);
                    }
                    catch (Exception)
                    {
                        NewMethod("Connect wait");
                        Thread.Sleep(1000);
                    }
                }
                while (!sock.Connected);

                NewMethod("Connect succ0");
                byte[] buff = new byte[8192];
                Dictionary<string, string> dic=null;

                while (sock.Connected)
                {
                    try
                    {                
                        // (4) 서버에서 데이타 수신
                        int n = sock.Receive(buff);

                        string data = Encoding.UTF8.GetString(buff, 0, n);
                        dic=JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                        foreach (var item in dic)
                        {
                            NewMethod("r: "+ item.Key + ","+ item.Value);
                            switch (item.Key)
                            {
                                case "sub2.json":
                                    sub2_json = item.Value;
                                    if (File.Exists(sub2_json)) listSub2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, HashSet<string>>>>(File.ReadAllText(sub2_json));
                                    NewMethod($"listSub2 {listSub2?.Count}");
                                    
                                    NewMethod();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        NewMethod($"{e.ToString()}");
                        modr = 0;
                        mods = 0;
                    }
                }

                NewMethod("Connect Close");
            }
        }

        private void NewMethod(string s)
        {
            this.Invoke(new Action(delegate ()
            {
                textBox2.AppendText(s + "\r\n");
            }));            
        }
        
        private void NewMethod()
        {
            this.Invoke(new Action(delegate ()
            {
                listBoxSet(1);
            }));            
        }

        private void listBoxSet(int i)
        {
            if (listSub2==null)
            {
                return;
            }
            if (i == 1)
            {
                listBox1.Items.Clear();
                foreach (var item2 in listSub2)
                {
                    listBox1.Items.Add(item2.Key);
                }
                if (listBox1.Items.Count > 0)
                    listBox1.SelectedIndex = 0;
            }
            if (i == 2)
            {
                listBox2.Items.Clear();
                if (listBox1.Items.Count>0)
                {
                    foreach (var item2 in listSub2[(string)listBox1.SelectedItem])
                    {
                        listBox2.Items.Add(item2.Key);
                    }
                    if (listBox2.Items.Count > 0)
                        listBox2.SelectedIndex = 0;
                }
            }
            if (i == 3)
            {
                listBox3.Items.Clear();
                if (listBox2.Items.Count>0)
                {
                    foreach (var item2 in listSub2[(string)listBox1.SelectedItem][(string)listBox2.SelectedItem])
                    {
                        listBox3.Items.Add(item2);
                    }
                    if (listBox3.Items.Count > 0)
                        listBox3.SelectedIndex = 0;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewMethod($"text1 {listBox1.SelectedIndex} {listBox1.SelectedItem}");
            listBoxSet(2);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewMethod($"text2 {listBox2.SelectedIndex} {listBox2.SelectedItem}");
            listBoxSet(3);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewMethod($"text3 {listBox3.SelectedIndex} {listBox3.SelectedItem}");


        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control && !e.Alt && !e.Shift)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["LogInfo"] = textBox1.Text;

                //var d = JsonConvert.SerializeObject(dic);
                //NewMethod("s: " + d);
                //byte[] buff = Encoding.UTF8.GetBytes(d);
                send(dic);

                textBox1.Clear();
            }
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox3.SelectedIndex<0)
            {
                return;
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["PlayDummyVoice"] = (string)listBox3.SelectedItem;
            send(dic);
        }

        private void send(Dictionary<string, string> dic)
        {            
            // (3) 서버에 데이타 전송
            string s = JsonConvert.SerializeObject(dic);
            sock.Send(Encoding.UTF8.GetBytes(s), SocketFlags.None);
            NewMethod("s: " + s);
        }
    }
}
