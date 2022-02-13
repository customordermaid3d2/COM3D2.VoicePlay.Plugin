using BepInEx;
using COM3D2.LillyUtill;
using COM3D2API;
using System;
using UnityEngine;

namespace COM3D2.VoicePlay.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class VoicePlay : BaseUnityPlugin
    {

        MyWindowRect myWindowRect;
        int windowId;
        private Vector2 scrollPosition=new Vector2();
        private int rep;
        private int sub;
        private int ogg;

        public void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo("Awake");
            myWindowRect = new MyWindowRect(Config, PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME,"VP");
            windowId = new System.Random().Next();
            scrollPosition = new Vector2();

            VoicePlayUtill.Awake(Logger,Config);
            VoicePlayServer.Awake(Logger,Config);
        }

        public void OnEnable()
        {
            Logger.LogInfo("OnEnable");
        }

        public void Start()
        {
            Logger.LogInfo("Start");

            // 이건 기어메뉴 아이콘
            SystemShortcutAPI.AddButton(
                PluginInfo.PLUGIN_NAME 
                , new Action(delegate ()
                { // 기어메뉴 아이콘 클릭시 작동할 기능
                    myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                })
                , PluginInfo.PLUGIN_NAME  // 표시될 툴팁 내용                               
            , MyUtill.ExtractResource(Resource1.icon));// 표시될 아이콘

            VoicePlayUtill.Start();
            VoicePlayServer.Start();
        }

        /*
        public void OnGUI()
        {
            try
            {
                if (!myWindowRect.IsGUIOn)
                    return;

                //GUI.skin.window = ;

                //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
                // 별도 창을 띄우고 WindowFunction 를 실행함. 이건 스킨 설정 부분인데 따로 공부할것
                myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, PluginInfo.PLUGIN_NAME);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e.ToString());
            }

        }

        public virtual void WindowFunction(int id)
        {
            #region head
            
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬

            // 라벨 추가
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUI.changed = false;

            GUILayout.EndHorizontal();// 가로 정렬 끝

            #endregion
            
            if (!myWindowRect.IsOpen || !VoicePlayUtill.isLoaded)
            {

            }
            else
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

                #region 여기에 내용 작성
                
                GUILayout.Label("id");
                rep =GUILayout.SelectionGrid(rep, VoicePlayUtill.rep,3);
                if (GUI.changed)
                {
                    VoicePlayUtill.NewMethod1(rep);
                    GUI.changed = false;
                }
                
                GUILayout.Label("sub");
                sub = GUILayout.SelectionGrid(sub, VoicePlayUtill.sub, 3);
                if (GUI.changed)
                {
                    VoicePlayUtill.NewMethod1(rep,sub);
                    GUI.changed = false;
                }

                GUILayout.Label("sub");
                ogg = GUILayout.SelectionGrid(ogg, VoicePlayUtill.ogg, 2);
                if (GUI.changed)
                {
                    VoicePlayUtill.oggSet(ogg);
                    GUI.changed = false;
                }

                #endregion

                GUILayout.EndScrollView();
            }

            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }
        */

        public void OnApplicationQuit()
        {
            Logger.LogInfo("OnApplicationQuit ");
            VoicePlayUtill.OnApplicationQuit();
        }

        public void OnDisable()
        {
            Logger.LogInfo("OnDisable");
        }

    }
}
