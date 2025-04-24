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

        [BoxGroup("Mother Of Managers", centerLabel: true)]
        /* Scene */
        [FoldoutGroup("Mother Of Managers/Scene")]
        public EAppScene StartScene = EAppScene.NONE;

        /* App */
        [FoldoutGroup("Mother Of Managers/App")]
        public int TargetFramRate = 30;
        
        /* Events (Networking) */
        [FoldoutGroup("Mother Of Managers/Events (Networking)")]
        public BEventDispatcherType EventDispatcherType;

        [FoldoutGroup("Mother Of Managers/Events (Networking)")]
        public EBEHandleSerializationMethod bEHandleSerializationMethod = EBEHandleSerializationMethod.JSON_NEWTONSOFT; // TODO: Use   

        [FoldoutGroup("Mother Of Managers/Events (Networking)")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public string UbiiBackendServerIP = "localhost";

        [FoldoutGroup("Mother Of Managers/Events (Networking)")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public int UbiiBackendServerPort = 8101;

        /* AR */
        [FoldoutGroup("Mother Of Managers/AR")]
        public ARTrackingMode ARTrackingMode = ARTrackingMode.NONE;

        [FoldoutGroup("Mother Of Managers/AR")]
        public EPlayAreaType PlayAreaType = EPlayAreaType.NONE;

        /* Input */
        [FoldoutGroup("Mother Of Managers/Input")]
        public bool ConnectTouchController = false;

        [FoldoutGroup("Mother Of Managers/Input")]
        public bool ConnectAIControllers = false;

        [FoldoutGroup("Mother Of Managers/Input")]
        public bool TransformInpuAxisToCameraDirection = false;

        /* Game */
        [FoldoutGroup("Mother Of Managers/Game")]
        public bool IsUseDebugGameMode = false;

        [FoldoutGroup("Mother Of Managers/Game")]
        public EGameMode DebugGameMode = EGameMode.NONE;
        
        /* Player */
        [FormerlySerializedAs("PlayerSpawnPositionPrefab")] [FoldoutGroup("Mother Of Managers/Player")]
        public AbstractPawnSpawnPosition PawnSpawnPositionPrefab;
        
        [FormerlySerializedAs("PlayersPrefabObjects")] [FormerlySerializedAs("PlayersPrefab")] [FoldoutGroup("Mother Of Managers/Player")]
        public PawnPrefabObject[] PawnPrefabObjects;
        
        [FoldoutGroup("Mother Of Managers/Player")]
        public bool IsSpawnGhostPlayerPositionsIfNotFound = false;

        [FoldoutGroup("Mother Of Managers/Player")]
        public bool SpawnPlayersUnderSameTransformAsSpawnPositions = false;
        
        /* AI */
        [FoldoutGroup("Mother Of Managers/AI")]
        public int MaximumNumberOfAIToSpawn = 0;

        /* Debug */
        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool IsDebugLogEvents = true;

        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool IsDebugEventsNetworkID = false;

        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool LogMissingDebugTexts = true;

        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool DebugButtonEvents = false;

        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool DebugJoystickEvents = false;

        [FoldoutGroup("Mother Of Managers/Debug")]
        public bool DebugUIButtonsEvents = false;

        [FoldoutGroup("Mother Of Managers/Debug")]
        [DisableIf("@this.EventDispatcherType != BEventDispatcherType.UBI_INTERACT")]
        public bool DebugUbiiTopicPublish = false;

        [FoldoutGroup("Mother Of Managers/Debug")]
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
