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
        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_Connected 
                = new ("PLAYERS_Connected");

        public static BEvent<BEventHandle<PlayerBase>> PLAYERS_Disconnected
                = new ("PLAYERS_Disconnected");

        /* Replicatable */
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_ControllerIDChanged
                = new ("PLAYERS_ControllerIDChanged");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_NameChanged
                = new ("PLAYERS_NameChanged");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PictureChanged
                = new ("PLAYERS_PictureChanged");

        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_PlayerIDChanged
                = new ("PLAYERS_PlayerIDChanged");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_SpectatorIDChanged
                = new ("PLAYERS_SpectatorIDChanged");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_TeamChanged
                = new ("PLAYERS_TeamChanged");

        public static BEvent<BEventHandle<SPlayerJoinedPartydArg>> PLAYERS_JoinedParty
                = new ("PLAYERS_JoinedParty");
        
        public static BEvent<BEventHandle<SPlayerLeftPartyArg>> PLAYERS_LeftParty
                = new ("PLAYERS_LeftParty");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_BecameReady
                = new ("PLAYERS_BecameReady");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> PLAYERS_CanceledReady
                = new ("PLAYERS_CanceledReady");

        /* Pawns */
        public static BEvent<BEventHandle<PawnBase>> PAWNS_Spawned
                = new ("PAWNS_Spawned");
        
        public static BEvent<BEventHandle<EPlayerID>> PAWNS_Destroyed
                = new ("PAWNS_Destroyed");
        
        #endregion

        #region Online
        public static BEvent<BEventHandle<SLobbyArg>> ONLINE_StartedLaunchingSession
            = new ("ONLINE_StartedLaunchingSession");
        
        public static BEvent<BEventHandle<SLobbyArg>> ONLINE_LaunchSessionSucceeded
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
         
        public static BEvent<BEventHandle<SPlayerIDMigrationArg>> ONLINE_MigratePlayerID
            = new ("ONLINE_MigratePlayerID");
        
        public static BEvent<BEventHandle> ONLINE_ConfirmPlayerIDMigration
            = new ("ONLINE_ConfirmPlayerIDMigration");
        
        public static BEvent<BEventHandle<SPlayerReplicationArg>> ONLINE_ReplicatePlayer
            = new ("ONLINE_ReplicatePlayer");
        
        #endregion

        #region Example
        public static BEvent<BEventHandle<int>> EXAMPLE_NumberIncremented 
            = new ("EXAMPLE_NumberIncremented");

        #endregion
        
        #region Replicated Events

        #endregion
        
        #region Authentication
        
        public static BEvent<BEventHandle<EAuthenticationServiceType>> AUTHENTICATION_Started
            = new ("AUTHENTICATION_Started");
        
        public static BEvent<BEventHandle<SAuthenticationArg>> AUTHENTICATION_Succeeded
            = new ("AUTHENTICATION_Succeeded"); 
        
        public static BEvent<BEventHandle<EAuthenticationFailureType>> AUTHENTICATION_Failed
            = new ("AUTHENTICATION_Failed");
        
        #endregion
    }
}
