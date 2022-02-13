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

namespace VoicePlayAppTest
{
    public partial class test : Form
    {
        internal static Task task;
        //private static Dictionary<string, string> dic = new Dictionary<string, string>();

        public test()
        {
            InitializeComponent();

            task = Task.Factory.StartNew(() => AcceptLoop());
        }

        private void AcceptLoop()
        {
            // (1) 소켓 객체 생성 (TCP 소켓)
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // (2) 포트에 바인드
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000);
            sock.Bind(ep);

            // (3) 포트 Listening 시작
            sock.Listen(10);
            
            while (true)
            {
                // (4) 연결을 받아들여 새 소켓 생성 (하나의 연결만 받아들임)
                Socket clientSock = sock.Accept();

                NewMethod("AcceptTcpClient succ");

                byte[] buff = new byte[8192];
                Dictionary<string, string> dic = null;

                while (clientSock.Connected)
                {
                    try
                    {
                        // (5) 소켓 수신
                        int n = clientSock.Receive(buff);

                        string data = Encoding.UTF8.GetString(buff, 0, n);
                        dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                        foreach (var item in dic)
                        {
                            NewMethod("r: " + item.Key + "," + item.Value);
                        }

                        var d = JsonConvert.SerializeObject(dic);

                        NewMethod("s: " + d);

                        buff = Encoding.UTF8.GetBytes(d);

                        // (6) 소켓 송신
                        clientSock.Send(buff,SocketFlags.None);  // echo
                    }
                    catch (Exception e)
                    {
                        NewMethod($"{e.ToString()}");
                    }
                }
                clientSock.Close();

                Thread.Sleep(1000);
            }

            sock.Close();
        }

        private void NewMethod(string s)
        {
            this.Invoke(new Action(delegate ()
            {
                textBox2.AppendText(s + "\r\n");
            }));
        }

    }
}
