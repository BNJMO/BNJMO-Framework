using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BNJMO
{
    public static class BTypes
    {

    }

    #region General
    public struct STransform
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }

    /// <summary>
    /// type of the Stage (or Scene) where the object is currently.
    /// </summary>
    public enum EObjectStage
    {
        MAIN_STAGE,
        PREFAB_STAGE,
        PERSISTENCE_STAGE,
        OTHER_STAGE, // (like in a Preview Scene)
    }

    #endregion
    
    #region Online

    public enum EOnlineBackend
    {
        None = 0,
        Netcode,
    }
    
    public enum EOnlineState
    {
        None,
        NotConnected,
        InLobby,
        InOnlineSession,
    }

    public enum ELobbyType
    {
        None,
        QuickMatch,
        Private,
    }
    
    public enum EJoinOnlineSessionFailureType
    {
        None = 0,
        CreateLobby,
        JoinLobbyByQuickMatch,
        JoinLobbyByCode,
        CreateRelay,
        JoinRelay,
        StartOnlineSession,
        JoinLobbyTimeout,
        JoinOnlineSessionTimeout,
        NoConnection,
    }

    public enum ELeaveOnlineSessionReason
    {
        None = 0,
        LeaveFromUI,
        JoinOnlineSessionFailure,
        HostLeft,
        OtherClientLeft,
        GameEnded,
    }
    
    public enum ENetworkID
    {
        NONE = 0,
        LOCAL = 1001,
        HOST_1 = 1,
        CLIENT_2 = 2,
        CLIENT_3 = 3,
        CLIENT_4 = 4,
        CLIENT_5 = 5,
        CLIENT_6 = 6,
        CLIENT_7 = 7,
        CLIENT_8 = 8,
        CLIENT_9 = 9,
        CLIENT_10 = 10,
        CLIENT_11 = 11,
        CLIENT_12 = 12,
        CLIENT_13 = 13,
        CLIENT_14 = 14,
        CLIENT_15 = 15,
        CLIENT_16 = 16,
    }

    public enum EAuthority
    {
        NONE = 0,
        LOCAL = 1,
        HOST = 2,
        CLIENT = 3,
    }

    
    #endregion

    #region States
    [Serializable]
    public struct SScene
    {
        public string SceneName;
        public int SceneBuildID;
    }

    #endregion

    #region Player

    public struct SPlayerInit
    {
        public EPlayerID PlayerID;
        public ESpectatorID SpectatorID;
        public EControllerID ControllerID;
        public EControllerType ControllerType;
        public ENetworkID NetworkID;
        public ETeamID TeamID;
        public string PlayerName;
        public Sprite PlayerPicture;
    }

    [Serializable]
    public struct SPlayerReplicationArg
    {
        public ENetworkID NetworkID;
        public EControllerID OwnerControllerID;
        public EControllerType OwnerControllerType;
        public EPlayerID PlayerID;
        public ESpectatorID SpectatorID;
        public ETeamID TeamID;
        public string PlayerName;
        public string PlayerPictureBase64;
        
        public override string ToString()
        {
            return $"NetworkID: {NetworkID}, " +
                   $"OwnerControllerID: {OwnerControllerID}, " +
                   $"OwnerControllerType: {OwnerControllerType}, " +
                   $"PlayerID: {PlayerID}, " +
                   $"SpectatorID: {SpectatorID}, " +
                   $"TeamID: {TeamID}, " +
                   $"PlayerName: {PlayerName}";
        }
    }

    [Serializable]
    public struct SPlayerIDMigration
    {
        public EControllerID OwnerControllerID;
        public EControllerType OwnerControllerType;
        public EPlayerID ToPlayerID;
        public ESpectatorID ToSpectatorID;
        
        public override string ToString()
        {
            return $"OwnerControllerID: {OwnerControllerID}, " +
                   $"OwnerControllerType: {OwnerControllerType}, " +
                   $"ToPlayerID: {ToPlayerID}, " +
                   $"ToSpectatorID: {ToSpectatorID}";
        }
    }
    
    public struct SPawnInit
    {
        public PlayerBase Player;
        public Vector3 Position;
        public Quaternion Rotation;
    }
    
    public enum EPlayerID
    {
        NONE = 0,
        PLAYER_1 = 1,
        PLAYER_2 = 2,
        PLAYER_3 = 3,
        PLAYER_4 = 4,
        PLAYER_5 = 5,
        PLAYER_6 = 6,
        PLAYER_7 = 7,
        PLAYER_8 = 8,
        PLAYER_9 = 9,
        PLAYER_10 = 10,
        PLAYER_11 = 11,
        PLAYER_12 = 12,
        PLAYER_13 = 13,
        PLAYER_14 = 14,
        PLAYER_15 = 15,
        PLAYER_16 = 16,
    }

    public enum ESpectatorID
    {
        NONE = 0,
        SPECTATOR_1 = 1,
        SPECTATOR_2 = 2,
        SPECTATOR_3 = 3,
        SPECTATOR_4 = 4,
        SPECTATOR_5 = 5,
        SPECTATOR_6 = 6,
        SPECTATOR_7 = 7,
        SPECTATOR_8 = 8,
        SPECTATOR_9 = 9,
        SPECTATOR_10 = 10,
        SPECTATOR_11 = 11,
        SPECTATOR_12 = 12,
        SPECTATOR_13 = 13,
        SPECTATOR_14 = 14,
        SPECTATOR_15 = 15,
        SPECTATOR_16 = 16,
    }

    public enum ETeamID
    {
        NONE = 0,
        TEAM_1 = 1,
        TEAM_2 = 2,
        TEAM_3 = 3,
        TEAM_4 = 4,
        TEAM_5 = 5,
        TEAM_6 = 6,
        TEAM_7 = 7,
        TEAM_8 = 8,
        TEAM_9 = 9,
        TEAM_10 = 10,
        TEAM_11 = 11,
        TEAM_12 = 12,
        TEAM_13 = 13,
        TEAM_14 = 14,
        TEAM_15 = 15,
        TEAM_16 = 16,
    }

    public enum EPlayerPartyState
    {
        NONE = 0,
        IN_LOBBY = 1,
        IN_PARTY = 2,
    }
    
    [Serializable]
    public struct PlayerPrefabTupple
    {
        public EPlayerID PlayerID; 
        public PlayerBase Prefab;
    }
    
    [Serializable]
    public struct PawnPrefabTupple
    {
        public EPlayerID PlayerID; 
        public PawnBase Prefab;
    }
    
    #endregion
    
    #region Authentication
    
    public enum EAuthenticationServiceType
    {
        None = 0,
        Anonymous,
        UnityPlayerAccount,
        UsernameAndPassword,
        Apple,
        AppleGameCenter,
        Google,
        GooglePlayGames,
        Facebook,
        Steam,
        Oculus,
        OpenIDConnect,
        CodeLinking,
        CustomID,
    }

    public struct SAuthenticationArg
    {
        public  EAuthenticationServiceType AuthenticationServiceType;
        public string PlayerID;
        public string AccessToken;
    }

    public enum EAuthenticationFailureType
    {
        None = 0,
        NotConnected,
        SignInWithServiceFailed,
    }
    
    #endregion

    #region Input
    public enum EControllerID
    {
        NONE = 0,
        /* Touch */
        TOUCH_1 = 201,
        TOUCH_2 = 202,
        TOUCH_3 = 203,
        TOUCH_4 = 204,
        /* AI */
        AI_1 = 1,
        AI_2 = 2,
        AI_3 = 3,
        AI_4 = 4,
        AI_5 = 5,
        AI_6 = 6,
        AI_7 = 7,
        AI_8 = 8,
        AI_9 = 9,
        AI_10 = 10,
        AI_11 = 11,
        AI_12 = 12,
        AI_13 = 13,
        AI_14 = 14,
        AI_15 = 15,
        AI_16 = 16,
        /* Devices */
        DEVICE_1 = 101,
        DEVICE_2 = 102,
        DEVICE_3 = 103,
        DEVICE_4 = 104,
        DEVICE_5 = 105,
        DEVICE_6 = 106,
        DEVICE_7 = 107,
        DEVICE_8 = 108,
        DEVICE_9 = 109,
        DEVICE_10 = 110,
        DEVICE_11 = 111,
        DEVICE_12 = 112,
        DEVICE_13 = 113,
        DEVICE_14 = 114,
        DEVICE_15 = 115,
        DEVICE_16 = 116,
        /* Network */
        REMOTE_1 = 1001,
        REMOTE_2 = 1002,
        REMOTE_3 = 1003,
        REMOTE_4 = 1004,
        REMOTE_5 = 1005,
        REMOTE_6 = 1006,
        REMOTE_7 = 1007,
        REMOTE_8 = 1008,
        REMOTE_9 = 1009,
        REMOTE_10 = 1010,
        REMOTE_11 = 1011,
        REMOTE_12 = 1012,
        REMOTE_13 = 1013,
        REMOTE_14 = 1014,
        REMOTE_15 = 1015,
        REMOTE_16 = 1016,
    }

    public enum EControllerType
    {
        None = 0,
        Dualshock4 = 1,
        XboxOne = 2,
        MouseKeyboard = 3,
        TouchScreen = 4,
        AI = 5,
        Dualsense = 6,
        XboxSeries = 7,
        MiscController = 8,
        NetworkController = 9,
        Switch = 10,
    }

    public enum EInputButton
    {
        NONE = 0,

        /* Operations */
        CONFIRM = 1,
        CANCEL = 2,
        OPTIONS = 3,

        /* Actions */ 
        SOUTH = 10,
        WEST = 11,
        EAST = 12,
        NORTH = 13,
        SHOULDER_L = 14,
        SHOULDER_R = 15,
        TRIGGER_L = 16,
        TRIGGER_R = 17,
        JOYSTICK_L = 18,
        JOYSTICK_R = 19,

        /* Directions */
        UP = 30,
        DOWN = 31,
        LEFT = 32,
        RIGHT = 33
    }

    public enum EInputAxis
    {
        NONE = 0,
        MOVEMENT = 1,
        ROTATION = 2,
        TRIGGER_AXIS_L = 3,
        TRIGGER_AXIS_R = 4,
    }

    public enum ESnappingMode
    {
        FIXED = 1,
        FLOATING = 2,
        DYNAMIC = 3
    }

    public enum EJoystickAxisRestriction
    {
        NONE,
        BOTH,
        HORIZONTAL,
        VERTICAL
    }

    public enum EJoystickState
    {
        NONE,
        IDLE,
        SELECTED_CAN_TRIGGER_BUTTON,
        SELECTED_CANNOT_TRIGGER_BUTTON
    }
    #endregion

    #region UI
    public enum EButtonDirection
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    #endregion

    #region BEvents

    public enum EBEHandleSerializationMethod
    {
        JSON_NEWTONSOFT = 0,
        JSON_UNITY = 1,
    }

    public enum BEventBroadcastType
    {
        LOCAL,          // The event invocation is not replicated and is only invoked on the same instance where it got called.
        TO_ALL,         // The event invocation is replicated to every instance on the network including the one where it got initially called.
        TO_ALL_OTHERS,  // The event invocation is replicated to every instance on the network expect the one where it got initially called.
        TO_TARGET       // The event invocation is only replicated on a specific targeted client (the designated client is specified as a parameter in the event invocation method).
    }

    #endregion
}