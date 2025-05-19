using System;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BEvents
    {
        public static Dictionary<string, AbstractBEvent> AllReplicatedBEvents { get; } = new();

        #region Scene
        public static BEvent<BEventHandle<SScene>> APP_SceneWillChange
            = new ("APP_SceneWillChange");

        public static BEvent<BEventHandle<SScene>> APP_SceneUpdated
            = new ("APP_SceneUpdated");

        #endregion

        #region UI
        public static BEvent<BEventHandle<BFrame>> UI_FocusedFrameUpdated
            = new ("UI_FocusedFrameUpdated");

        public static BEvent<BEventHandle<BMenu, BMenu>> UI_HighlightedBMenuUpdated
            = new ("UI_HighlightedBMenuUpdated");

        /* Buttons */
        public static BEvent<BEventHandle<BButton>> UI_ButtonHighlighted
            = new ("UI_ButtonHighlighted");
                
        public static BEvent<BEventHandle<BButton>> UI_ButtonPressed
            = new ("UI_ButtonPressed");

        public static BEvent<BEventHandle<BButton>> UI_ButtonUnhighlighted
            = new ("UI_ButtonUnhighlighted");

        public static BEvent<BEventHandle<BButton, bool>> UI_ButtonReleased
            = new ("UI_ButtonReleased");


        #endregion

        #region Input
        public static BEvent<BEventHandle<EControllerID>> INPUT_ControllerConnected
            = new ("INPUT_ControllerConnected");

        public static BEvent<BEventHandle<EControllerID>> INPUT_ControllerDisconnected
            = new ("INPUT_ControllerDisconnected");

        public static BEvent<BEventHandle<EControllerID, EInputButton>> INPUT_ButtonPressed
            = new ("INPUT_ButtonPressed");

        public static BEvent<BEventHandle<EControllerID, EInputButton>> INPUT_ButtonReleased
            = new ("INPUT_ButtonReleased");

        public static BEvent<BEventHandle<EControllerID, EInputAxis, float, float>> INPUT_AxisUpdated
            = new ("INPUT_AxisUpdated");


        #endregion

        #region Players
        /* Players */
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerConnected 
                = new ("PLAYERS_PlayerConnected");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerDisconnected
                = new ("PLAYERS_PlayerDisconnected");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerChangedControllerID
                = new ("PLAYERS_PlayerChangedControllerID");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerChangedName
                = new ("PLAYERS_PlayerChangedName");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerChangedTeam
                = new ("PLAYERS_PlayerChangedTeam");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerJoinedTheParty
                = new ("PLAYERS_PlayerJoinedTheParty");
        
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerLeftTheParty
                = new ("PLAYERS_PlayerLeftTheParty");
        
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerBecameReady
                = new ("PLAYERS_PlayerReadinessChanged");
        
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerCanceledReady
                = new ("PLAYERS_PlayerCanceledReady");

        /* Pawns */
        public static BEvent<BEventHandle<PawnBase>> PAWNS_PawnSpawned
                = new ("PAWNS_PawnSpawned");
        
        public static BEvent<BEventHandle<PawnBase>> PAWNS_PawnDestroyed
                = new ("PAWNS_PawnDestroyed");
        
        #endregion

        #region Multiplayer
        public static BEvent<BEventHandle<ELobbyType>> MULTIPLAYER_StartedLaunchingMultiplayer
            = new ("MULTIPLAYER_StartedLaunchingMultiplayer");
        
        public static BEvent<BEventHandle> MULTIPLAYER_LaunchMultiplayerSucceeded
            = new ("MULTIPLAYER_LaunchMultiplayerSucceeded");
  
        public static BEvent<BEventHandle<EJoinMultiplayerFailureType>> MULTIPLAYER_LaunchMultiplayerFailed
            = new ("MULTIPLAYER_LaunchMultiplayerFailed");
  
        public static BEvent<BEventHandle<ELeaveMultiplayerReason>> MULTIPLAYER_ShutdownMultiplayer
            = new ("MULTIPLAYER_ShutdownMultiplayer");
  
        public static BEvent<BEventHandle<ENetworkID>> MULTIPLAYER_RemotePlayerJoined
            = new ("MULTIPLAYER_RemotePlayerJoined");

        public static BEvent<BEventHandle<ENetworkID>> MULTIPLAYER_RemotePlayerLeft
            = new ("MULTIPLAYER_RemotePlayerLeft");
        
        #endregion

        #region Test
        public static BEvent<BEventHandle<int>> TEST_CounterIncrement
            = new BEvent<BEventHandle<int>>("TEST_CounterIncrement");

        public static BEvent<BEventHandle<float>> TEST_FloatTest
            = new BEvent<BEventHandle<float>>("TEST_FloatTest");

        public static BEvent<BEventHandle<Vector3>> TEST_Vector3Test
            = new BEvent<BEventHandle<Vector3>>("TEST_Vector3Test");

        public static BEvent<BEventHandle<Vector3>> TEST_ImagePosition
            = new BEvent<BEventHandle<Vector3>>("TEST_ImagePosition");

        public static BEvent<BEventHandle<Vector3>> TEST_TrackerMoved
            = new BEvent<BEventHandle<Vector3>>("TEST_TrackerMoved");

        #endregion
        
        #region Replicated Events

        #endregion
    }
}
