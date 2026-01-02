using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Contains pre-defines constant members.
    /// </summary>
    public static class BConsts
    {
        /* App State*/
        public const float TIME_BEFORE_SCENE_CHANGE = 0.3f;

        /* Resources paths */
        public const string PATH_SoundObject = "SoundObject";
        public const string PATH_MirrorNetworkManager = "MirrorNetworkManager";
        /* AR */
        public const string PATH_AR_WorldRootBAnchor = "WorldRootBAnchor";
        public const string PATH_AR_PlayAreaBAnchor = "PlayAreaBAnchor";
        public const string PATH_AR_TestBAnchor = "TestBAnchor";
        public const string PATH_AR_PlayArea_Rectangle = "PlayArea_Rectangle";
        public const string PATH_AR_PlayAreaPlane = "PlayAreaPlane";

        /* UI paths */
        public const string PATH_BFrame = "F_BFrame";
        public const string PATH_BMenu = "M_BMenu";
        public const string PATH_BButton = "B_BButton";
        public const string PATH_BContainer = "C_BContainer";
        public const string PATH_BCarousel = "C_BCarousel";
        public const string PATH_BImage = "I_BImage";
        public const string PATH_BRawImage = "I_BRawImage";
        public const string PATH_BSpriteRenderer = "I_BSpriteRenderer";
        public const string PATH_B3DText = "T_B3DText";
        public const string PATH_BText = "T_BText";
        public const string PATH_InputField = "IF_InputField";
        public const string PATH_BScrollView = "SV_BScrollView";
        public const string PATH_BScrollViewNested = "SV_BScrollViewNested";

        /* Multiplayer */
        public const string PATH_NetcodeNetworkManager = "NetcodeNetworkManager";

        /* DropZone */
        public const string PATH_DZ_DropZonePlane = "DropZonePlane";
        public const string PATH_DZ_DroneBall = "BallDrone";
        public const string PATH_DZ_PlayAreaSeparation = "PlayAreaSeparation";

        /* Debug Texts*/
        public const string DEBUGTEXT_JoinedPlayers = "JoinedPlayers";
        public const string DEBUGTEXT_TestGameMode = "TestGameMode";
        public const string DEBUGTEXT_ConnectedControllers = "ConnectedControllers";
        public const string DEBUGTEXT_NetControllerInputSource = "NetControllerInputSource";
        public const string DEBUGTEXT_MirrorPlayers = "MirrorPlayers";
        public const string DEBUGTEXT_NetworkState = "NetworkState";
        
        /* Input */
        public const float JOYSTICK_DEAD_ZONE = 0.3f;
        public const float THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT = 0.07f;
        public const float THRESHOLD_JOYSTICK_DISTANCE_ROTATION = 0.05f;

        public static ENetworkID[] NETWORK_CLIENTS = new ENetworkID[]
        {
        ENetworkID.CLIENT_2,
        ENetworkID.CLIENT_3,
        ENetworkID.CLIENT_4,
        ENetworkID.CLIENT_5,
        ENetworkID.CLIENT_6,
        ENetworkID.CLIENT_7,
        ENetworkID.CLIENT_8,
        ENetworkID.CLIENT_9,
        ENetworkID.CLIENT_10,
        ENetworkID.CLIENT_11,
        ENetworkID.CLIENT_12,
        ENetworkID.CLIENT_13,
        ENetworkID.CLIENT_14,
        ENetworkID.CLIENT_15,
        ENetworkID.CLIENT_16,
        };

        public static EControllerID[] DEVICE_CONTROLLERS = new EControllerID[]
        {
        EControllerID.DEVICE_1,
        EControllerID.DEVICE_2,
        EControllerID.DEVICE_3,
        EControllerID.DEVICE_4,
        EControllerID.DEVICE_5,
        EControllerID.DEVICE_6,
        EControllerID.DEVICE_7,
        EControllerID.DEVICE_8,
        EControllerID.DEVICE_9,
        EControllerID.DEVICE_10,
        EControllerID.DEVICE_11,
        EControllerID.DEVICE_12,
        EControllerID.DEVICE_13,
        EControllerID.DEVICE_14,
        EControllerID.DEVICE_15,
        EControllerID.DEVICE_16,
        };

        public static EControllerID[] AI_CONTROLLERS = new EControllerID[]
        {
        EControllerID.AI_1,
        EControllerID.AI_2,
        EControllerID.AI_3,
        EControllerID.AI_4,
        EControllerID.AI_5,
        EControllerID.AI_6,
        EControllerID.AI_7,
        EControllerID.AI_8,
        EControllerID.AI_9,
        EControllerID.AI_10,
        EControllerID.AI_11,
        EControllerID.AI_12,
        EControllerID.AI_13,
        EControllerID.AI_14,
        EControllerID.AI_15,
        EControllerID.AI_16,
        };

        public static EControllerID[] TOUCH_CONTROLLERS = new EControllerID[]
        {
            EControllerID.TOUCH_1,
            EControllerID.TOUCH_2,
            EControllerID.TOUCH_3,
            EControllerID.TOUCH_4,
        };
        
        public static EControllerID[] REMOTE_CONTROLLERS = new EControllerID[]
        {
            EControllerID.REMOTE_1,
            EControllerID.REMOTE_2,
            EControllerID.REMOTE_3,
            EControllerID.REMOTE_4,
            EControllerID.REMOTE_5,
            EControllerID.REMOTE_6,
            EControllerID.REMOTE_7,
            EControllerID.REMOTE_8,
            EControllerID.REMOTE_9,
            EControllerID.REMOTE_10,
            EControllerID.REMOTE_11,
            EControllerID.REMOTE_12,
            EControllerID.REMOTE_13,
            EControllerID.REMOTE_14,
            EControllerID.REMOTE_15,
            EControllerID.REMOTE_16,
        };
        
        public const string NONE = "None";
        public const string SEPARATOR = "<color=white>|</color>";
        public const string COMMA = "<color=white>,</color>";

    }
}