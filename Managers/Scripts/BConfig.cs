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
        /* Scene */
        [FoldoutGroup("BConfig/Scene")]
        public int StartSceneBuildID;
        
        [FoldoutGroup("BConfig/Scene")]
        public SScene[] Scenes;
        

        /* App */
        [FoldoutGroup("BConfig/App")]
        public int TargetFramRate = 30;
        
        [FoldoutGroup("BConfig/App")]
        [InfoBox("-1 : Prevent screen dimming, -2 : Set the sleep timeout to whatever the user has specified in the system settings")]
        public int SleepTimeout = -1;
                
        [FoldoutGroup("BConfig/App")]
        public int VSyncCount = 0;
        
        
        /* Events */
        [FoldoutGroup("BConfig/Events")]
        public BEventDispatcherType EventDispatcherType;

        [FoldoutGroup("BConfig/Events")]
        public EBEHandleSerializationMethod bEHandleSerializationMethod = EBEHandleSerializationMethod.JSON_NEWTONSOFT; // TODO: Use   

        [FoldoutGroup("BConfig/Events")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public string UbiiBackendServerIP = "localhost";

        [FoldoutGroup("BConfig/Events")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public int UbiiBackendServerPort = 8101;
        

        /* Game */
        [FoldoutGroup("BConfig/Game")]
        public bool IsUseDebugGameMode = false;

        [FoldoutGroup("BConfig/Game")]
        public EGameMode DebugGameMode = EGameMode.NONE;
        
                
        /* Input */
        [FoldoutGroup("BConfig/Input")]
        public bool ConnectTouchController = false;

        [FoldoutGroup("BConfig/Input")]
        public bool ConnectAIControllers = false;

        [FoldoutGroup("BConfig/Input")]
        public bool TransformInpuAxisToCameraDirection = false;
        

        /* Player */
        [FoldoutGroup("BConfig/Player")]
        [Range(1, 16)]
        public int MaxNumberOfSpectators = 16;
                
        [FoldoutGroup("BConfig/Player")]
        [Range(1, 16)]
        public int MaxNumberOfPlayersInParty = 4;
        
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
        public PlayerPrefabTupple[] PlayerPrefabs;

        [Title("Pawn Prefabs")]
        [FoldoutGroup("BConfig/Player")] 
        public bool UseSamePrefabForAllPawns = true;
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == false")]
        public PawnBase PawnPrefab;   
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == true")]
        public PawnPrefabTupple[] PawnPrefabs;
                
        [FoldoutGroup("BConfig/Player")]
        [ReadOnly]
        [InfoBox("Must be set in realtime")]
        public Transform PawnSpawnParent;
        
        
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
        
        /* Log */
        [FormerlySerializedAs("IsLogEvents")]
        [Header("Events")]
        [FoldoutGroup("BConfig/Log")]
        public bool LogEvents = true;

        [FoldoutGroup("BConfig/Log")]
        public bool LogEventsNetworkID;
        
        [FoldoutGroup("BConfig/Log")]
        public bool LogEventsTimestamp; 
        
        [FoldoutGroup("BConfig/Log")]
        public bool LogEventsPing;

        [Header("Input")]
        [FoldoutGroup("BConfig/Log")]
        public bool LogInputButtonEvents;

        [FoldoutGroup("BConfig/Log")]
        public bool LogInputJoystickEvents;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
