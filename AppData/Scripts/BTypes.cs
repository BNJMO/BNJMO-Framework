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
        PRESISTENCE_STAGE,
        OTHER_STAGE, // (like in a Preview Scene)
    }
    #endregion

    #region AR
    public enum ARTrackingMode
    {
        NONE = 0,
        AR_FOUNDATION = 4,
    }

    public enum BTrackerType
    {
        NONE = 0,
        IMAGE = 1,
        OBJECT = 2,
    }

    public enum EPlayAreaType
    {
        NONE = 0,
        RECTANGLE = 1,

    }

    public enum EPlayAreaState
    {
        NOT_INITIALIZED = 0,
        SETTING_ANCHORS = 1,
        READY = 2,
    }

    #endregion

    #region States
    /// <summary>
    /// State of the whole application. 
    /// "IN" is used a hierarchy separator. A state can either be in MENU or in GAME.
    /// Note: Update the states classifications lists inside the class when more states are added to AppState!
    /// </summary>
    public enum EAppState
    {
        NONE = 0,

        /* Entry */
        IN_ENTRY = 100,

        /* Menu */
        IN_MENU_IN_MAIN = 200,

        /* Game */
        IN_GAME_IN_NOT_STARTED = 300,
        IN_GAME_IN_COUNTDOWN = 301,
        IN_GAME_IN_RUNNING = 302,
        IN_GAME_IN_PAUSED = 303,
        IN_GAME_IN_OVER = 304,
    }

    public enum EAppScene
    {
        NONE = 0,

        /* Entry */
        ENTRY = 100,

        /* Menu */
        MENU = 200,

        /* Game */
        GAME = 300,
    }

    /// <summary>
    /// State of the UI menu.
    /// "IN" is used here as a hierarchy separator. Generally second IN refers to a pop-up context on top of the original menu state.
    /// </summary>
    public enum EUIState
    {
        NONE = 0,

        /* Entry */
        IN_ENTRY = 100,

        /* Menu */
        IN_MENU_MAIN = 200,
        IN_MENU_EXAMPLE_TEXTS = 250,

        /* Game */
        IN_GAME_IN_NOT_STARTED = 300,
        IN_GAME_IN_COUNTDOWN = 301,
        IN_GAME_IN_RUNNING = 302,
        IN_GAME_IN_PAUSED = 303,
        IN_GAME_OVER = 304
    }
    #endregion

    #region Game
    public enum EGameMode
    {
        NONE = 0,
        SURVIVAL = 100,
        NORMAL = 101,
        TEST = 9000
    }
    #endregion

    #region Player

    public struct SPlayerInit
    {
        public EPlayerID PlayerID;
        public EControllerID ControllerID;
        public ENetworkID NetworkID;
        public ETeamID TeamID;
        public string PlayerName;
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
        SPECTATOR = 6969
    }

    public enum ETeamID
    {
        NONE = 0,
        TEAM_1 = 1,
        TEAM_2 = 2,
        TEAM_3 = 3,
        TEAM_4 = 4
    }

    public enum EPlayerState
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

    #region Input
    public enum EControllerID
    {
        NONE = 0,
        /* Touch */
        TOUCH = 1,
        /* AI */
        AI_1 = 2,
        AI_2 = 3,
        AI_3 = 4,
        AI_4 = 5,
        /* Devices */
        DEVICE_1 = 6,
        DEVICE_2 = 7,
        DEVICE_3 = 8,
        DEVICE_4 = 9,
        DEVICE_5 = 14,
        DEVICE_6 = 15,
        DEVICE_7 = 16,
        DEVICE_8 = 17,
        DEVICE_9 = 18,
        DEVICE_10 = 19,
        DEVICE_11 = 20,
        DEVICE_12 = 21,
        /* Network */
        NET_HOST = 50,
        NET_CLIENT_1 = 52,
        NET_CLIENT_2 = 53,
        NET_CLIENT_3 = 54,
        NET_CLIENT_4 = 55,
        NET_CLIENT_5 = 56,
        NET_CLIENT_6 = 57,
        NET_CLIENT_7 = 58,
        NET_CLIENT_8 = 59,
        NET_CLIENT_9 = 60,
        NET_CLIENT_10 = 61,
        NET_CLIENT_11 = 64,
        NET_CLIENT_12 = 62,
        NET_CLIENT_13 = 63,
    }

    public enum EControllerType
    {
        None = 0,
        Dualshock4 = 1,
        XboxOne = 2,
        MouseKeyboard = 3,
        TouchScreen = 4,
        AI = 5,
        Dualsense = 4,
        XboxSeries = 5,
        MiscController = 6,
        NetworkController = 7,
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
    public enum BEventDispatcherType
    {
        NONE,
        LOCAL,
        MIRROR,
        UBI_INTERACT
    }

    public enum EBEHandleSerializationMethod
    {
        JSON_NEWTONSOFT = 0,
        JSON_UNITY = 1,
    }

    public enum BEventReplicationType
    {
        LOCAL,          // The event invocation is not replicated and is only invoked on the same instance where it got called.
        TO_ALL,         // The event invocation is replicated to every instance on the network including the one where it got initially called.
        TO_ALL_OTHERS,  // The event invocation is replicated to every instance on the network expect the one where it got initially called.
        TO_TARGET       // The event invocation is only replicated on a specific targeted client (the designated client is specified as a parameter in the event invocation method).
    }

    public enum BEHandleType
    {
        NONE,
        GENERIC_0,
        GENERIC_1,
        GENERIC_2,
        GENERIC_3,
        GENERIC_4,
        GENERIC_5,
        STATE_UPDATE,
        TEST
    }

    public enum ENetworkID
    {
        NONE,
        LOCAL = 51,
        HOST = 50,
        CLIENT_1 = 22,
        CLIENT_2 = 23,
        CLIENT_3 = 24,
        CLIENT_4 = 25,
        CLIENT_5 = 26,
        CLIENT_6 = 27,
        CLIENT_7 = 28,
        CLIENT_8 = 29,
        CLIENT_9 = 30,
        CLIENT_10 = 31,
        CLIENT_11 = 34,
        CLIENT_12 = 32,
        CLIENT_13 = 33,
    }

    public enum ENetworkState
    {
        NOT_CONNECTED = 1,
        HOST = 2,
        CLIENT = 3,
    }
    
    #endregion
}