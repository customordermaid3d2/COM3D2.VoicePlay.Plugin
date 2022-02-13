using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace COM3D2.VoicePlay.Plugin
{
    internal class VoicePlayServer
    {                      
        private static ManualLogSource log;
        private static ConfigFile config;
        
        internal static Task task;

        internal static void Awake(ManualLogSource logger, ConfigFile Config)
        {
            log = logger;
            config = Config;

        }

        internal static void Start()
        {
            log.LogInfo("Start Server");
            task = Task.Factory.StartNew(() => AcceptLoop());
        }

        private static Dictionary<string,string> dic = new Dictionary<string,string>();

        private static void AcceptLoop()
        {
            // TcpListener 생성자에 붙는 매개변수는 
            // 첫번째는 IP를 두번째는 port 번호입니다.
            TcpListener server = new TcpListener(IPAddress.Any, 9999);

            // 서버를 시작합니다.
            server.Start();

            // 클라이언트 객체를 만들어 9999에 연결한 client를 받아옵니다
            // 받아올때까지 서버는 대기합니다.
            TcpClient tcpClient = server.AcceptTcpClient();
            NetworkStream networkStream = tcpClient.GetStream(); 
            MemoryStream memoryStream = new MemoryStream(); 
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            
            log.LogInfo("AcceptLoop AcceptTcpClient");
            log.LogInfo($"{tcpClient.ToString()}");
            log.LogInfo($"{tcpClient.Client.LocalEndPoint.ToString()}");
            log.LogInfo($"{tcpClient.Client.RemoteEndPoint.ToString()}");

            dic["test"] = "test1";

            //byte[] byteData;
            while (true)
            {
                binaryFormatter.Serialize(memoryStream, dic);

                byte[] length = BitConverter.GetBytes(memoryStream.Length); 
                byte[] byteArray = memoryStream.ToArray();

                networkStream.Write(length, 0, 4); 
                networkStream.Write(byteArray, 0, byteArray.Length);



                byte[] lengthArray = new byte[4]; 
                networkStream.Read(lengthArray, 0, lengthArray.Length); 
                int length = BitConverter.ToInt32(lengthArray, 0); 
                byte[] byteArray = new byte[length]; 
                networkStream.Read(byteArray, 0, byteArray.Length); 
                MemoryStream memoryStream = new MemoryStream(byteArray); 
                memoryStream.Position = 0; object obj = binaryFormatter.Deserialize(memoryStream); 
                List<Data> datas = (List<Data>)obj;


                // Socket은 byte[] 형식으로 데이터를 주고받으므로 byte[]형 변수를 선언합니다.
                //byteData = new byte[1024];
                // client가 write한 정보를 읽어옵니다.
                // 아래의 작업 이후에 byteData에는 읽어온 데이터가 들어갑니다.
                //tcpClient.GetStream().Read(byteData, 0, byteData.Length);

                // 출력을 위해 string형으로 바꿔줍니다.
                //string strData = Encoding.Default.GetString(byteData);
                //
                // byteData의 크기는 1024인데 스트림에서 읽어온 데이터가 1024보다 작은경우
                // 공백이 출력되니 비어있는 문자열을 제거합니다.
                //int endPoint = strData.IndexOf('\0');
                //string parsedMessage = strData.Substring(0, endPoint + 1);

                // 파싱된 데이터를 출력해주고 while루프를 돕니다.
                //Console.WriteLine(parsedMessage);
            }

            memoryStream.Close(); 
            networkStream.Close(); 
            tcpClient.Close();


        }

    }
}
