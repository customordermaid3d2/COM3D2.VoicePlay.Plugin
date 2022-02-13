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
using Newtonsoft.Json;

namespace COM3D2.VoicePlay.Plugin
{
    internal class VoicePlayServer
    {                      
        private static ManualLogSource log;
        private static ConfigFile config;
        
        internal static Task task;
        private static Socket sock;
        internal static Socket clientSock;
        private static bool isQuit;
        //private static int mods = 0, modr = 0;

        private static Dictionary<string, string> dic;
        private static Dictionary<string, string> mod;

        internal static void Awake(ManualLogSource logger, ConfigFile Config)
        {
            log = logger;
            config = Config;

            // (1) 소켓 객체 생성 (TCP 소켓)
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            // (2) 포트에 바인드            
            sock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7000));

            // (3) 포트 Listening 시작
            sock.Listen(10);

            task = Task.Factory.StartNew(() => Accept());
        }


        internal static void Start()
        {
            log.LogInfo("Start Server");   
        }


        internal static void configSend()
        {
            log.LogInfo("configSend");
            Dictionary<string, string> d = new Dictionary<string, string>();
            d["sub2.json"] = VoicePlayUtill.jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-sub2.json";
            VoicePlayServer.send(d);
        }

        private static void Accept()
        {
            log.LogInfo("VoicePlayServer.Accept");

            while (!isQuit)
            {
                // (4) 연결을 받아들여 새 소켓 생성 (하나의 연결만 받아들임)
                clientSock = sock.Accept();

                configSend();

                log.LogInfo("AcceptTcpClient succ");

                byte[] buff = new byte[8192];

                while (clientSock.Connected && !isQuit)
                {
                    try
                    {
                        // (5) 소켓 수신
                        int n = clientSock.Receive(buff);

                        string data = Encoding.UTF8.GetString(buff, 0, n);
                        dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                        foreach (var item in dic)
                        {
                            log.LogInfo("r: " + item.Key + "," + item.Value);
                            switch (item.Key)
                            {
                                case "LogInfo":
                                    log.LogInfo($"{item.Value}");
                                    break;
                                case "PlayDummyVoice":
                                    GameMain.Instance.SoundMgr.PlayDummyVoice(item.Value);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.LogInfo($"{e.Message}");
                        //modr = 0;
                        //mods = 0;
                    }
                }
                clientSock?.Close();

                Thread.Sleep(1000);
            }            
            log.LogInfo($"isQuit {isQuit}");
        }

        internal static void send(Dictionary<string, string> d)
        {
            //log.LogInfo("so: " + d.Key + "," + d.Value);
            var dic = JsonConvert.SerializeObject(d);

            log.LogInfo("sd: " + dic);

            byte[] buff = Encoding.UTF8.GetBytes(dic);

            clientSock?.Send(buff, SocketFlags.None);


        }

        internal static void OnApplicationQuit()
        {
            isQuit = true;
            log.LogInfo("VoicePlayServer.OnApplicationQuit");
            sock?.Close();
            clientSock?.Close();
        }
    }
}
