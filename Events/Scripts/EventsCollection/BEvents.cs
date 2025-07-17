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
        public static BEvent<BEventHandle<EControllerID, EControllerType>> INPUT_ControllerConnected
            = new ("INPUT_ControllerConnected");

        public static BEvent<BEventHandle<EControllerID, EControllerType>> INPUT_ControllerDisconnected
            = new ("INPUT_ControllerDisconnected");

        public static BEvent<BEventHandle<EControllerID, EControllerType, EInputButton>> INPUT_ButtonPressed
            = new ("INPUT_ButtonPressed");

        public static BEvent<BEventHandle<EControllerID, EControllerType, EInputButton>> INPUT_ButtonReleased
            = new ("INPUT_ButtonReleased");

        public static BEvent<BEventHandle<EControllerID, EControllerType, EInputAxis, float, float>> INPUT_AxisUpdated
            = new ("INPUT_AxisUpdated");


        #endregion

        #region Players
        /* Players */
        /* Local */
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerConnected 
                = new ("PLAYERS_PlayerConnected");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_PlayerDisconnected
                = new ("PLAYERS_PlayerDisconnected");

        /* Replicatable */
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerChangedControllerID
                = new ("PLAYERS_PlayerChangedControllerID");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerChangedName
                = new ("PLAYERS_PlayerChangedName");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerChangedPicture
                = new ("PLAYERS_PlayerChangedPicture");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerChangedTeam
                = new ("PLAYERS_PlayerChangedTeam");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerJoinedTheParty
                = new ("PLAYERS_PlayerJoinedTheParty");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerLeftTheParty
                = new ("PLAYERS_PlayerLeftTheParty");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerBecameReady
                = new ("PLAYERS_PlayerReadinessChanged");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerCanceledReady
                = new ("PLAYERS_PlayerCanceledReady");

        /* Pawns */
        public static BEvent<BEventHandle<EPlayerID>> PAWNS_PawnSpawned
                = new ("PAWNS_PawnSpawned");
        
        public static BEvent<BEventHandle<EPlayerID>> PAWNS_PawnDestroyed
                = new ("PAWNS_PawnDestroyed");
        
        #endregion

        #region Online
        public static BEvent<BEventHandle<ELobbyType>> ONLINE_StartedLaunchingSession
            = new ("ONLINE_StartedLaunchingSession");
        
        public static BEvent<BEventHandle> ONLINE_LaunchSessionSucceeded
            = new ("ONLINE_LaunchSessionSucceeded");
  
        public static BEvent<BEventHandle<EJoinOnlineSessionFailureType>> ONLINE_LaunchSessionFailed
            = new ("ONLINE_LaunchSessionFailed");
  
        /// <summary>
        /// When the online session is shut down (both lobby and relay closed)
        /// <param name="Arg1">Reason of shutdown</param>
        /// <param name="Arg2">OldLocalNetworkID (local NetworkID from previous online session)</param>
        /// </summary>
        public static BEvent<BEventHandle<ELeaveOnlineSessionReason, ENetworkID>> ONLINE_ShutdownSession
            = new ("ONLINE_ShutdownSession");
  
        public static BEvent<BEventHandle<ENetworkID>> ONLINE_ClientJoined
            = new ("ONLINE_ClientJoined");

        public static BEvent<BEventHandle<ENetworkID>> ONLINE_ClientLeft
            = new ("ONLINE_ClientLeft");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> ONLINE_RequestReplicatePlayer
            = new ("ONLINE_RequestReplicatePlayer");
         
        public static BEvent<BEventHandle<SPlayerIDMigration>> ONLINE_MigratePlayerIDs
            = new ("ONLINE_MigratePlayerIDs");
        
        public static BEvent<BEventHandle> ONLINE_ConfirmPlayerIDsMigration
            = new ("ONLINE_ConfirmPlayerIDsMigration");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> ONLINE_ReplicatePlayer
            = new ("ONLINE_ReplicatePlayer");
        
        #endregion

        #region Example
        public static BEvent<BEventHandle<int>> EXAMPLE_NumberIncremented 
            = new ("EXAMPLE_NumberIncremented");

        #endregion
        
        #region Replicated Events

        #endregion
    }
}
