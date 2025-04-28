using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace BNJMO
{
    [CreateAssetMenu(fileName = "BConfig", menuName = "BNJMO/Create BConfig")]
    public class BConfig : ScriptableObject
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [BoxGroup("BConfig", centerLabel: true)]
        /* Scene */
        [FoldoutGroup("BConfig/Scene")]
        public EAppScene StartScene = EAppScene.NONE;

        /* App */
        [FoldoutGroup("BConfig/App")]
        public int TargetFramRate = 30;
        
        /* Events (Networking) */
        [FoldoutGroup("BConfig/Events (Networking)")]
        public BEventDispatcherType EventDispatcherType;

        [FoldoutGroup("BConfig/Events (Networking)")]
        public EBEHandleSerializationMethod bEHandleSerializationMethod = EBEHandleSerializationMethod.JSON_NEWTONSOFT; // TODO: Use   

        [FoldoutGroup("BConfig/Events (Networking)")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public string UbiiBackendServerIP = "localhost";

        [FoldoutGroup("BConfig/Events (Networking)")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public int UbiiBackendServerPort = 8101;

        /* AR */
        [FoldoutGroup("BConfig/AR")]
        public ARTrackingMode ARTrackingMode = ARTrackingMode.NONE;

        [FoldoutGroup("BConfig/AR")]
        public EPlayAreaType PlayAreaType = EPlayAreaType.NONE;

        /* Input */
        [FoldoutGroup("BConfig/Input")]
        public bool ConnectTouchController = false;

        [FoldoutGroup("BConfig/Input")]
        public bool ConnectAIControllers = false;

        [FoldoutGroup("BConfig/Input")]
        public bool TransformInpuAxisToCameraDirection = false;

        /* Game */
        [FoldoutGroup("BConfig/Game")]
        public bool IsUseDebugGameMode = false;

        [FoldoutGroup("BConfig/Game")]
        public EGameMode DebugGameMode = EGameMode.NONE;
        
        /* Player */ 
        [FoldoutGroup("BConfig/Player")]
        public int MaxPlayersInTeam = 4;
        
        [FoldoutGroup("BConfig/Player")]
        public int MaxPlayersInParty = 4;
        
        [FoldoutGroup("BConfig/Player")]
        public Transform PawnSpawnParent;

        [FoldoutGroup("BConfig/Player")] 
        public bool UseSamePrefabForAllPlayers = true;
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPlayers == false")]
        public PlayerBase PlayerPrefab;     
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPlayers == true")]
        public PlayerPrefabTupple[] PlayerPrefabs;

        [FoldoutGroup("BConfig/Player")] 
        public bool UseSamePrefabForAllPawns = true;
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == false")]
        public PawnBase PawnPrefab;   
        
        [FoldoutGroup("BConfig/Player")] [DisableIf("@this.UseSamePrefabForAllPawns == true")]
        public PawnPrefabTupple[] PawnPrefabs;
        
        
        /* AI */
        [FoldoutGroup("BConfig/AI")]
        public int MaximumNumberOfAIToSpawn = 0;

        /* Debug */
        [FoldoutGroup("BConfig/Debug")]
        public bool IsDebugLogEvents = true;

        [FoldoutGroup("BConfig/Debug")]
        public bool IsDebugEventsNetworkID = false;

        [FoldoutGroup("BConfig/Debug")]
        public bool LogMissingDebugTexts = true;

        [FoldoutGroup("BConfig/Debug")]
        public bool DebugButtonEvents = false;

        [FoldoutGroup("BConfig/Debug")]
        public bool DebugJoystickEvents = false;

        [FoldoutGroup("BConfig/Debug")]
        public bool DebugUIButtonsEvents = false;

        [FoldoutGroup("BConfig/Debug")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public bool DebugUbiiTopicPublish = false;

        [FoldoutGroup("BConfig/Debug")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public bool DebugUbiiTopicRecieve = false;


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
