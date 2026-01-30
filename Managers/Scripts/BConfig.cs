using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine.Serialization;

namespace BNJMO
{
    [CreateAssetMenu(fileName = "BConfig", menuName = "BNJMO/Create BConfig")]
    public class BConfig : ScriptableObject
    {
        #region Public Events


        #endregion

        #region Public Methods

        public virtual void InitConfig()
        {
            Inst = this;
        }

        public static C GetInst<C>() where C : BConfig
        {
            return Inst as C;
        }
        
        public static string GetValueFromProjectConfigFile(string key)
        {
            TextAsset configFile = Resources.Load<TextAsset>("ProjectConfig"); 
            string[] lines = configFile.text.Split('\n');
            foreach (string rawLineItr in lines)
            {
                string lineItr = rawLineItr.Trim();

                // Skip empty lines and comment lines
                if (string.IsNullOrEmpty(lineItr) || lineItr.StartsWith("#"))
                    continue;

                // Remove comments from end of line
                int commentIndex = lineItr.IndexOf('#');
                if (commentIndex >= 0)
                    lineItr = lineItr.Substring(0, commentIndex).Trim();

                // Split by '=' to get key and value
                int separatorIndex = lineItr.IndexOf('=');
                if (separatorIndex < 0)
                    continue; // malformed line

                string keyItr = lineItr.Substring(0, separatorIndex).Trim();
                if (keyItr != key)
                    continue;
                
                return lineItr.Substring(separatorIndex + 1).Trim();
            }

            return "";
        }

        #endregion

        #region Inspector Variables

        [BoxGroup("BConfig", centerLabel: true)]
        
        /* App */
        [FoldoutGroup("BConfig/App")]
        public int TargetFramRate = 30;
        
        [FoldoutGroup("BConfig/App")]
        [InfoBox("-1 : Prevent screen dimming, -2 : Set the sleep timeout to whatever the user has specified in the system settings")]
        public int SleepTimeout = -1;
                
        [FoldoutGroup("BConfig/App")]
        public int VSyncCount = 0;     
        
        [FoldoutGroup("BConfig/App")]
        public Texture2D CustomCursorTexture; 
        
        [FoldoutGroup("BConfig/App"), HideIf("@CustomCursorTexture == null")]
        public Vector2 CustomCursorHotspot = Vector2.zero;   
        
        [FoldoutGroup("BConfig/App"), HideIf("@CustomCursorTexture == null")]
        public CursorMode  CursorMode = CursorMode.Auto;
        
        /* Authentication */
        [FoldoutGroup("BConfig/Authentication")] 
        public bool SignInAnounymousOnStart = false;
        
        /* BEvents */
        [FoldoutGroup("BConfig/BEvents")]
        public EBEHandleSerializationMethod bEHandleSerializationMethod = EBEHandleSerializationMethod.JSON_NEWTONSOFT;
        
        /* Debug Logs */
        [Header("BEvents")]
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BEvents")]
        public bool LogBEvents = true;

        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BEvents NetworkID"), DisableIf("@this.LogBEvents == false")]
        public bool LogBEventsNetworkID;
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BEvents Broadcast Type"), DisableIf("@this.LogBEvents == false")]
        public bool LogBEventsBroadcastType;
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BEvents Time Stamp"), DisableIf("@this.LogBEvents == false")]
        public bool LogBEventsTimestamp; 
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BEvents Ping"), DisableIf("@this.LogBEvents == false")]
        public bool LogBEventsPing;
        
        [Header("UI")]
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log UI BEvents")]
        public bool LogUIBEvents;
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BMenu Highlight BEvents"), DisableIf("@this.LogUIBEvents == false")]
        public bool LogBMenuHighlightBEvents;
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log BButton BEvents"), DisableIf("@this.LogUIBEvents == false")]
        public bool LogBButtonBEvents;
        
        [Header("Input")]
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log Input BEvents")]
        public bool LogInputBEvents;
        
        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log Input Button BEvents"), DisableIf("@this.LogInputBEvents == false")]
        public bool LogInputButtonBEvents;

        [FoldoutGroup("BConfig/Debug Logs"), LabelText("Log Input Joystick BEvents"), DisableIf("@this.LogInputBEvents == false")]
        public bool LogInputJoystickBEvents;
        
        /* Input */
        [FormerlySerializedAs("ConnectTouchController")] [FoldoutGroup("BConfig/Input")]
        public bool ConnectTouchControllerInLateStart;

        [FoldoutGroup("BConfig/Input")]
        public int NumberOfAIControllersToSpawnInLateStart = 0;

        [FoldoutGroup("BConfig/Input")]
        public bool TransformInpuAxisToCameraDirection;
        
        /* Online */
        [FoldoutGroup("BConfig/Online")] 
        public EOnlineBackend onlineBackend = EOnlineBackend.Netcode;
        
        [FoldoutGroup("BConfig/Online")] 
        public bool AutomaticallyLaunchOnlineSession = true;
        
        [FoldoutGroup("BConfig/Online")] 
        public int NumberOfPlayersInLobbyToStartOnlineSession = 1;     
        
        [FoldoutGroup("BConfig/Online")] 
        public float LobbyUpdateTimeSeconds = 2.0f;    
        
        [FoldoutGroup("BConfig/Online")] 
        public float JoinOnlineSessionTimeout = 30.0f;
  
        [FoldoutGroup("BConfig/Online")] 
        [InfoBox("Only possible when still in Lobby state and Party not started yet (i.e. the relay hasn't been created yet")]
        public bool HandoffLobbyWhenHostDisconnects = true;
        
        [Header("Netcode")]
        [FoldoutGroup("BConfig/Online")] 
        public NetworkManager NetcodeNetworkManagerPrefab;
        
        /* Players */
        [FoldoutGroup("BConfig/Player")]
        [Range(1, 16)]
        public int MaxNumberOfSpectators = 16;
                
        [FormerlySerializedAs("MaxNumberOfPlayersInParty")]
        [FoldoutGroup("BConfig/Player")]
        [Range(1, 16)]
        public int MaxNumberOfActivePlayers = 4;
        
        [FoldoutGroup("BConfig/Player")]
        [Range(0, 16)]
        public int MaxNumberOfTeams = 4;
        
        [FoldoutGroup("BConfig/Player")]
        [Range(1, 16)]
        public int MaxNumberOfPlayersInTeam = 4;
        
        [Title("Player Prefabs")]
        [FoldoutGroup("BConfig/Player")] 
        public bool MatchPlayerNameToPartyState = true;        

        [FoldoutGroup("BConfig/Player")] 
        public bool UseSamePrefabForAllPlayers = true;
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPlayers == false")]
        public PlayerBase PlayerPrefab;     
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPlayers == true")]
        public PlayerPrefabPair[] PlayerPrefabs;

        [Title("Pawn Prefabs")]
        [FoldoutGroup("BConfig/Player")] 
        public bool UseSamePrefabForAllPawns = true;
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == false")]
        public PawnBase PawnPrefab;   
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == true")]
        public PawnPrefabPair[] PawnPrefabs;
                
        [FoldoutGroup("BConfig/Player")]
        [ReadOnly]
        [InfoBox("Must be set in realtime")]
        public Transform PawnSpawnParent;
        
        /* Scene */
        [FoldoutGroup("BConfig/Scene")]
        public SScene[] Scenes;
        
        #endregion

        #region Variables

        public static BConfig Inst;

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
