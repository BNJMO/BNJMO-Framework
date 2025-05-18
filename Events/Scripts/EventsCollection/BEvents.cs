using System;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BEvents : AbstractBEventsCollection
    {
        #region Replicated Events
        private Dictionary<string, AbstractBEvent> allReplicatedBEvents = new Dictionary<string, AbstractBEvent>()
        {
            /* Test */
            { "TEST_CounterIncrement", TEST_CounterIncrement },
            { "TEST_FloatTest", TEST_FloatTest },
            { "TEST_Vector3Test", TEST_Vector3Test },
            { "TEST_ImagePosition", TEST_ImagePosition },
            { "TEST_TrackerMoved", TEST_TrackerMoved },

            /* Network */
            { "NETWORK_PlayerJoined", NETWORK_PlayerJoined },
            { "NETWORK_PlayerLeft", NETWORK_PlayerLeft },
            { "NETWORK_PlayerNameUpdated", NETWORK_PlayerNameUpdated },
            { "NETWORK_Ping", NETWORK_Ping },
        };

        public override Dictionary<string, AbstractBEvent> GetAllReplicatedBEvents()
        {
            return allReplicatedBEvents;
        }
        #endregion

        #region Test
        public static BEvent<BEHandle<int>> TEST_CounterIncrement
                = new BEvent<BEHandle<int>>("TEST_CounterIncrement");

        public static BEvent<BEHandle<float>> TEST_FloatTest
                = new BEvent<BEHandle<float>>("TEST_FloatTest");

        public static BEvent<BEHandle<Vector3>> TEST_Vector3Test
                = new BEvent<BEHandle<Vector3>>("TEST_Vector3Test");

        public static BEvent<BEHandle<Vector3>> TEST_ImagePosition
                = new BEvent<BEHandle<Vector3>>("TEST_ImagePosition");

        public static BEvent<BEHandle<Vector3>> TEST_TrackerMoved
                = new BEvent<BEHandle<Vector3>>("TEST_TrackerMoved");


        #endregion

        #region Scene
        public static BEvent<BEHandle<SScene>> APP_SceneWillChange
            = new BEvent<BEHandle<SScene>>("APP_SceneWillChange");

        public static BEvent<BEHandle<SScene>> APP_SceneUpdated
            = new BEvent<BEHandle<SScene>>("APP_SceneUpdated");

        #endregion
     
        #region Game
        public event Action<AbstractGameMode> GAME_GameStarted;
        public void Invoke_GAME_GameStarted(AbstractGameMode gameMode)
        {
            if (GAME_GameStarted != null)
            {
                GAME_GameStarted.Invoke(gameMode);
            }
            DebugLog("Game started : " + gameMode.GameModeType);
        }


        public event Action<AbstractGameMode> GAME_GamePaused;
        public void Invoke_GAME_GamePaused(AbstractGameMode gameMode)
        {
            if (GAME_GamePaused != null)
            {
                GAME_GamePaused.Invoke(gameMode);
            }
            DebugLog("Game paused : " + gameMode.GameModeType);
        }

        public event Action<AbstractGameMode> GAME_GameUnPaused;
        public void Invoke_GAME_GameUnPaused(AbstractGameMode gameMode)
        {
            if (GAME_GameUnPaused != null)
            {
                GAME_GameUnPaused.Invoke(gameMode);
            }
            DebugLog("Game unpaused : " + gameMode.GameModeType);
        }

        public event Action<AbstractGameMode, bool> GAME_GameEnded;
        public void Invoke_GAME_GameEnded(AbstractGameMode gameMode, bool wasAborted = false)
        {
            if (GAME_GameEnded != null)
            {
                GAME_GameEnded.Invoke(gameMode, wasAborted);
            }
            DebugLog("Game ended : " + gameMode.GameModeType + ". Aborted : " + wasAborted.ToString());
        }

        public event Action<AbstractPlayerStats, EGameMode> GAME_PlayerStatsUpdated;
        public void Invoke_GAME_PlayerStatsUpdated(AbstractPlayerStats updatedPlayerStats, EGameMode fromGameMode)
        {
            if (GAME_PlayerStatsUpdated != null)
            {
                GAME_PlayerStatsUpdated.Invoke(updatedPlayerStats, fromGameMode);
            }
            // DebugLog("Player stats " + updatedPlayerStats.ToString() + " updated for " + fromGameMode);
        }

        public event Action<AbstractGameMode> GAME_SetPlayersScores;
        public void Invoke_GAME_SetPlayersScores(AbstractGameMode gameMode)
        {
            if (GAME_SetPlayersScores != null)
            {
                GAME_SetPlayersScores.Invoke(gameMode);
            }
            DebugLog("Set Players Scores");
        }

        public event Action<int> GAME_GameTimeUpdated;
        public void Invoke_GAME_GameTimeUpdated(int time)
        {
            if (GAME_GameTimeUpdated != null)
            {
                GAME_GameTimeUpdated.Invoke(time);
            }
            // DebugLog("Time has been Updated to : " + time);
        }

        #endregion

        #region Players
        /* Players */
        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerConnected 
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerConnected");

        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerDisconnected
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerDisconnected");

        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerChangedControllerID
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerChangedControllerID");

        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerChangedName
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerChangedName");

        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerChangedTeam
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerChangedTeam");

        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerJoinedTheParty
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerJoinedTheParty");
        
        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerLeftTheParty
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerLeftTheParty");
        
        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerBecameReady
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerReadinessChanged");
        
        public static BEvent<BEHandle<PlayerBase>> PLAYERS_PlayerCanceledReady
                = new BEvent<BEHandle<PlayerBase>>("PLAYERS_PlayerCanceledReady");

        /* Pawns */
        public static BEvent<BEHandle<PawnBase>> PAWNS_PawnSpawned
                = new BEvent<BEHandle<PawnBase>>("PAWNS_PawnSpawned");
        
        public static BEvent<BEHandle<PawnBase>> PAWNS_PawnDestroyed
                = new BEvent<BEHandle<PawnBase>>("PAWNS_PawnDestroyed");
        

        #endregion

        #region UI
        //public BEvent<StateBEHandle<EUIState>> UI_UIStateUpdated 
        // = new BEvent<StateBEHandle<EUIState>>("UI_UIStateUpdated");

        public static BEvent<BEHandle<BFrame>> UI_FocusedFrameUpdated
               = new BEvent<BEHandle<BFrame>>("UI_FocusedFrameUpdated");

        public static BEvent<BEHandle<BMenu, BMenu>> UI_HighlightedBMenuUpdated
               = new BEvent<BEHandle<BMenu, BMenu>>("UI_HighlightedBMenuUpdated");

        /* Buttons */
        public static BEvent<BEHandle<BButton>> UI_ButtonHighlighted
               = new BEvent<BEHandle<BButton>>("UI_ButtonHighlighted");
                
        public static BEvent<BEHandle<BButton>> UI_ButtonPressed
               = new BEvent<BEHandle<BButton>>("UI_ButtonPressed");

        public static BEvent<BEHandle<BButton>> UI_ButtonUnhighlighted
               = new BEvent<BEHandle<BButton>>("UI_ButtonUnhighlighted");

        public static BEvent<BEHandle<BButton, bool>> UI_ButtonReleased
               = new BEvent<BEHandle<BButton, bool>>("UI_ButtonReleased");


        #endregion

        #region Input
        public static BEvent<BEHandle<EControllerID>> INPUT_ControllerConnected
            = new BEvent<BEHandle<EControllerID>>("INPUT_ControllerConnected");

        public static BEvent<BEHandle<EControllerID>> INPUT_ControllerDisconnected
            = new BEvent<BEHandle<EControllerID>>("INPUT_ControllerDisconnected");

        public static BEvent<BEHandle<EControllerID, EInputButton>> INPUT_ButtonPressed
            = new BEvent<BEHandle<EControllerID, EInputButton>>("INPUT_ButtonPressed");

        public static BEvent<BEHandle<EControllerID, EInputButton>> INPUT_ButtonReleased
            = new BEvent<BEHandle<EControllerID, EInputButton>>("INPUT_ButtonReleased");

        public static BEvent<BEHandle<EControllerID, EInputAxis, float, float>> INPUT_AxisUpdated
            = new BEvent<BEHandle<EControllerID, EInputAxis, float, float>>("INPUT_AxisUpdated");


        #endregion

        #region Multiplayer
        public static BEvent<BEHandle> MULTIPLAYER_JoinMultiplayerSucceeded
            = new ("MULTIPLAYER_JoinMultiplayerSucceeded");
  
        public static BEvent<BEHandle<EJoinMultiplayerFailureType>> MULTIPLAYER_JoinMultiplayerFailed
            = new ("MULTIPLAYER_JoinMultiplayerFailed");
  
        public static BEvent<BEHandle<ELeaveMultiplayerReason>> MULTIPLAYER_LeftMultiplayer
            = new ("MULTIPLAYER_LeftMultiplayer");
  
        public static BEvent<BEHandle<ENetworkID>> MULTIPLAYER_RemotePlayerJoined
            = new ("MULTIPLAYER_RemotePlayerJoined");

        public static BEvent<BEHandle<ENetworkID>> MULTIPLAYER_RemotePlayerLeft
            = new ("MULTIPLAYER_RemotePlayerLeft");



        
        
        #endregion
        
        #region Network
        public static BEvent<BEHandle<AbstractBEventDispatcher>> NETWORK_NewBEventDispatcherSet
        = new BEvent<BEHandle<AbstractBEventDispatcher>>("NETWORK_NewBEventDispatcherSet");

        public static BEvent<BEHandle<ENetworkState>> NETWORK_NetworkStateUpdated
                = new BEvent<BEHandle<ENetworkState>>("NETWORK_NetworkStateUpdated");

        public static BEvent<BEHandle> NETWORK_StartedHost 
                = new BEvent<BEHandle>("NETWORK_StartedHost");

        public static BEvent<BEHandle<ENetworkID>> NETWORK_ConnectedToHost
                = new BEvent<BEHandle<ENetworkID>>("NETWORK_ConnectedToHost");

        public static BEvent<BEHandle> NETWORK_ConnectionStoped
                = new BEvent<BEHandle>("NETWORK_ConnectionStoped");

        public static BEvent<BEHandle<ENetworkID>> NETWORK_NewNetworkIDConnected 
                = new BEvent<BEHandle<ENetworkID>>("NETWORK_NewNetworkIDConnected");

        public static BEvent<BEHandle<ENetworkID>> NETWORK_NetworkIDDisconnected
                = new BEvent<BEHandle<ENetworkID>>("NETWORK_NetworkIDDisconnected");

        public static BEvent<BEHandle<string[]>> NETWORK_DiscoveredHostsUpdated
                = new BEvent<BEHandle<string[]>>("NETWORK_DiscoveredHostsUpdated");

        // Used from PlayerNetworkManager to replicate joining connected players 
        public static BEvent<BEHandle<EPlayerID, EControllerID, string>> NETWORK_PlayerJoined
                = new BEvent<BEHandle<EPlayerID, EControllerID, string>>("NETWORK_PlayerJoined");

        // Used from PlayerNetworkManager to replicate joining connected players 
        public static BEvent<BEHandle<EPlayerID, EControllerID>> NETWORK_PlayerLeft
                = new BEvent<BEHandle<EPlayerID, EControllerID>>("NETWORK_PlayerLeft");
 
        public static BEvent<BEHandle<EPlayerID, string>> NETWORK_PlayerNameUpdated
                = new BEvent<BEHandle<EPlayerID, string>>("NETWORK_PlayerNameUpdated");

        public static BEvent<BEHandle<ENetworkID, int>> NETWORK_Ping
                = new BEvent<BEHandle<ENetworkID, int>>("NETWORK_Ping");

        #endregion

        // TODO: Depricated
        private void DebugLog(string messageLog)
        {
            if (BManager.Inst.Config.IsDebugLogEvents == true)
            {
                Debug.Log("<color=green>[EVENT (old)]</color> " + messageLog);
            }
        }


    }
}
