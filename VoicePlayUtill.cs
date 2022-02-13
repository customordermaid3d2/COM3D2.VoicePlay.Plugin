using BepInEx.Configuration;
using BepInEx.Logging;
using CM3D2.Toolkit.Guest4168Branch.Arc;
using CM3D2.Toolkit.Guest4168Branch.Arc.Entry;
using MaidStatus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace COM3D2.VoicePlay.Plugin
{
    public static class VoicePlayUtill
    {
        public static bool isLoaded = false;
        public static bool isLoading = false;
        internal static ManualLogSource log;
        internal static ConfigFile config;
        internal static Task task;
        public static string jsonPath;

        public static Regex regex = new Regex(@"^\d+[a-zA-Z]*$");
        public static Regex regex2 = new Regex(@"^voice_.*$");

        /// <summary>
        /// arc
        /// </summary>
        public static HashSet<string> listArc;
        /// <summary>
        /// arc , ogg
        /// </summary>
        //public static Dictionary<string, HashSet<string>> listArcSub;
        /// <summary>
        /// arc . replaceText.ToLower() , ogg
        /// </summary>
        //public static Dictionary<string, Dictionary<string, HashSet<string>>> listArcSub2;
        /// <summary>
        /// arc . replaceText.ToLower() , sub , ogg
        /// </summary>
        //public static Dictionary<string, Dictionary<string, Dictionary<string, HashSet<string>>>> listArcSub3;

        /// <summary>
        /// id , replaceText.ToLower()
        /// </summary>
        //public static Dictionary<int, string> listId;

        /// <summary>
        /// ogg
        /// </summary>
        //public static HashSet<string> listAll;
        /// <summary>
        /// replaceText.ToLower() , ogg
        /// </summary>
        //public static Dictionary<string, HashSet<string>> listSub;
        /// <summary>
        /// replaceText.ToLower() , sub , ogg
        /// </summary>
        public static Dictionary<string, Dictionary<string, HashSet<string>>> listSub2;

        internal static string[] rep;
        internal static string[] sub;
        internal static string[] ogg;

        internal static void JSONLoad()
        {
            //NewMethod("id", ref listId);
            NewMethod("arc", ref listArc);
            //NewMethod("arcSub", ref listArcSub);
            //NewMethod("arcSub2", ref listArcSub2);
            //NewMethod("arcSub3", ref listArcSub3);
            //NewMethod("all", ref listAll);
            //NewMethod("sub", ref listSub);
            NewMethod("sub2", ref listSub2);
            /*
            listId = new Dictionary<int, string>();
            listAll = new HashSet<string>();
            listSub = new Dictionary<string, HashSet<string>>();
            listSub2 = new Dictionary<string, Dictionary<string, HashSet<string>>>();
            */
        }

        private static void NewMethod<T>(string s, ref T t) where T : new()
        {
            if (File.Exists(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-{s}.json")) t = JsonConvert.DeserializeObject<T>(File.ReadAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-{s}.json"));
            else t = new T();
        }

        internal static void JSONSave()
        {
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-id.json", JsonConvert.SerializeObject(listId, Formatting.Indented)); // 자동 들여쓰기
            File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-arc.json", JsonConvert.SerializeObject(listArc, Formatting.Indented)); // 자동 들여쓰기
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-arcSub.json", JsonConvert.SerializeObject(listArcSub, Formatting.Indented)); // 자동 들여쓰기
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-arcSub2.json", JsonConvert.SerializeObject(listArcSub2, Formatting.Indented)); // 자동 들여쓰기
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-arcSub3.json", JsonConvert.SerializeObject(listArcSub3, Formatting.Indented)); // 자동 들여쓰기
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-all.json", JsonConvert.SerializeObject(listAll, Formatting.Indented)); // 자동 들여쓰기
            //File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-sub.json", JsonConvert.SerializeObject(listSub, Formatting.Indented)); // 자동 들여쓰기
            File.WriteAllText(jsonPath + $@"\{PluginInfo.PLUGIN_GUID}-sub2.json", JsonConvert.SerializeObject(listSub2, Formatting.Indented)); // 자동 들여쓰기
        }



        internal static void Awake(BepInEx.Logging.ManualLogSource logger, BepInEx.Configuration.ConfigFile Config)
        {
            log = logger;
            config = Config;
            // GameUty.FileSystem.IsExistentFile(list[k])
            // GameUty.FileSystemOld.IsExistentFile(list[k])

            jsonPath = Path.GetDirectoryName(config.ConfigFilePath);
        }

        internal static void Start()
        {
            // var tokenSource2 = new CancellationTokenSource();
            // CancellationToken ct = tokenSource2.Token;

            //task = Task.Factory.StartNew(() => init(), tokenSource2.Token);
            task = Task.Factory.StartNew(() => init());

            //init();
        }

        private static void init()
        {
            if (isLoading)
            {
                return;
            }
            isLoading = true;

            Stopwatch stopwatch = new Stopwatch(); //객체 선언
            stopwatch.Start(); // 시간측정 시작

            log.LogInfo($"init start {stopwatch.ElapsedMilliseconds}");
            

            try
            {
                log.LogInfo($"JSONLoad");
                JSONLoad();
                /*
                listId = new Dictionary<int, string>();
                listAll = new HashSet<string>();
                listSub = new Dictionary<string, HashSet<string>>();
                listSub2 = new Dictionary<string, Dictionary<string, HashSet<string>>>();
                */

                //log.LogInfo($"GetAllDatas");
                // foreach (var item in Personal.GetAllDatas(false))// Awake 단계에서 사용 불가
                // {
                //     if (listId.ContainsKey(item.id))
                //     {
                //         continue;
                //     }
                //     var r = item.replaceText.ToLower();
                //     listId.Add(item.id, r);
                //     listSub.Add(r, new HashSet<string>());
                //     listSub2.Add(r, new Dictionary<string, HashSet<string>>());
                // }

                log.LogInfo($"GetFiles");
                //var listArc = Directory.GetFiles(UTY.gameDataPath, "voice_*.arc", SearchOption.AllDirectories).ToList();
                //var listArcTmp = new HashSet<string>(Directory.GetFiles(UTY.gameDataPath, "voice_*.arc", SearchOption.AllDirectories).ToList().ConvertAll(s => s.ToLower()));

                var listArcTmp = GameUty.loadArchiveList.Where(s => regex2.IsMatch(s));

                var listArcDel = listArc.Except(listArcTmp);

                foreach (var item in listArcDel)
                {

                }

                int k = 0, c1 = 0, c2 = 0;

                foreach (string fl in listArcTmp)
                {
                    if (listArc.Contains(fl))
                    {
                        log.LogInfo($"foreach listArcTmp {fl} skip");
                        continue;
                    }
                    log.LogInfo($"foreach listArcTmp {fl} run");

                    listArc.Add(fl);
                    //listArcSub[fl] = new HashSet<string>();
                    //listArcSub2[fl] = new Dictionary<string, HashSet<string>>();
                    //listArcSub3[fl] = new Dictionary<string, Dictionary<string, HashSet<string>>>();

                    ArcFileSystem fileSystem = new ArcFileSystem();
                    fileSystem.LoadArc(UTY.gameDataPath + $@"\{fl}.arc");

                    foreach (ArcFileEntry f in fileSystem.Files.Values)
                    {
                        //log.LogInfo($"foreach ArcFileEntry {f.Name}");
                        //log.LogInfo($"f.Name , {f.Name} ");

                        //listAll.Add(f.Name);
                        //listArcSub[fl].Add(f.Name);

                        var fname = f.Name.Split('_', '.');

                        //log.LogInfo($"f.Name , {n.Length } , {f.Name} , {String.Join(",", n)}");
                        //log.LogInfo($"foreach ArcFileEntry1 {f.Name}");
                        //if (!listId.ContainsValue(fname[0]))
                        if (!listSub2.ContainsKey(fname[0]))
                        {
                            //listId[fname[0].GetHashCode()] = fname[0];
                            //listSub[fname[0]] = new HashSet<string>();
                            listSub2[fname[0]] = new Dictionary<string, HashSet<string>>();

                        }
                        //log.LogInfo($"foreach ArcFileEntry2 {f.Name}");
                        //if (!listArcSub2[fl].ContainsKey(fname[0]))
                        //{
                        //    listArcSub2[fl][fname[0]] = new HashSet<string>();
                        //    listArcSub3[fl][fname[0]] = new Dictionary<string, HashSet<string>>();
                        //}
                        //listArcSub2[fl][fname[0]].Add(f.Name);

                        for (int i = fname.Length - 2; i > 0; i--)
                        {
                            //if (int.TryParse(n[i], out v))
                            if (!regex.IsMatch(fname[i])) continue;
                            //{
                            //listSub[fname[0]].Add(f.Name);
                            // h1_1234.ogg
                            // h1_1234_ex.ogg
                            // h1_ex_1234.ogg
                            switch (i)
                            {
                                case 3:
                                    k = 1;
                                    break;
                                case 2:
                                    k = 1;
                                    break;
                                case 1:
                                    if (fname.Length == 3) k = 0;
                                    else k = 2;
                                    break;
                                default:
                                    k = 0;
                                    break;
                            }
                            if (!listSub2[fname[0]].ContainsKey(fname[k]))
                            {
                                listSub2[fname[0]][fname[k]] = new HashSet<string>();
                            }
                            listSub2[fname[0]][fname[k]].Add(f.Name);
                            c1++;
                            //if (!listArcSub3[fl][fname[0]].ContainsKey(fname[k]))
                            //{
                            //    listArcSub3[fl][fname[0]][fname[k]] = new HashSet<string>();
                            //}
                            //listArcSub3[fl][fname[0]][fname[k]].Add(f.Name);
                            //log.LogInfo($"f.Name , {fname.Length } , {i}  , {k} , {f.Name} , {String.Join(",", fname)}");
                            break;
                            //}
                        }
                    }

                    log.LogInfo($"fl , {fl} , {stopwatch.ElapsedMilliseconds}");
                }

                /*
                //foreach (var item in listSub)
                //{
                //    log.LogInfo($"listSub , {item.Key} , {item.Value.Count}");
                //}
                */
                foreach (var item in listSub2)
                {
                    //log.LogInfo($"listSub2 , {item.Key} , {item.Value.Count}");
                    foreach (var item2 in item.Value)
                    {
                        //log.LogInfo($"listSub2 , {item.Key} , {item2.Key} , {item2.Value.Count}");
                        c2 += item2.Value.Count;
                    }
                }
                /*
                foreach (var item in listArcSub)
                {
                    log.LogInfo($"listArcSub , {item.Key} , {item.Value.Count}");
                }
                foreach (var item in listArcSub2)
                {
                    log.LogInfo($"listArcSub2 , {item.Key} , {item.Value.Count}");
                    foreach (var item2 in item.Value)
                    {
                        log.LogInfo($"listArcSub2 , {item.Key} , {item2.Key} , {item2.Value.Count}");
                    }
                }
                foreach (var item in listArcSub3)
                {
                    log.LogInfo($"listArcSub3 , {item.Key} , {item.Value.Count}");
                    foreach (var item2 in item.Value)
                    {
                        log.LogInfo($"listArcSub3 , {item2.Key} , {item2.Key} , {item2.Value.Count}");
                        foreach (var item3 in item2.Value)
                        {
                            log.LogInfo($"listArcSub3 , {item.Key} , {item2.Key} ,{item3.Key} , {item3.Value.Count}");
                        }
                    }
                }
                */
                log.LogInfo($"c , {c1} , {c2}");
                //log.LogInfo($"listId , {listId.Count}");
                log.LogInfo($"listArc , {listArc.Count}");
                //log.LogInfo($"listArcSub , {listArcSub.Count}");
                //log.LogInfo($"listArcSub2 , {listArcSub2.Count}");
                //log.LogInfo($"listArcSub3 , {listArcSub3.Count}");
                //log.LogInfo($"listAll , {listAll.Count}");
                //log.LogInfo($"listSub , {listSub.Count}");
                log.LogInfo($"listSub2 , {listSub2.Count}");

                rep = listSub2.Keys.ToArray();
                NewMethod1(0);
                NewMethod1(0, 0);

                JSONSave();
                VoicePlayServer.configSend();

                isLoaded = true;

            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }


            isLoading = false;

            log.LogInfo($"init end {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop(); //시간측정 끝
        }


        internal static void NewMethod1(int i)
        {
            sub = listSub2[rep[i]].Keys.ToArray();
        }
        
        internal static void NewMethod1(int i,int j)
        {
            ogg = listSub2[rep[i]][sub[j]].ToArray();
        }

        internal static void oggSet(int k)
        {
            //maid.AudioMan.LoadPlay(f_strFileName, fadeTime, false, isLoop);
            GameMain.Instance.SoundMgr.PlayDummyVoice(ogg[k]);            
        }

        internal static void OnApplicationQuit()
        {
            task?.Dispose();
        }
    }
}