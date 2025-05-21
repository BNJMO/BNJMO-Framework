using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{

    public class BPlayerManager : AbstractSingletonManager<BPlayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* ControllerID */
        public PlayerBase GetPlayer(EControllerID controllerID, bool logWarnings = true)
        {
            if (IS_NONE(controllerID, logWarnings))
                return null;
            
            PlayerBase player = null;
            foreach (PlayerBase playerItr in ConnectedPlayers)
            {
                if (playerItr.ControllerID == controllerID)
                {
                    player = playerItr;
                    break;
                }
            }
            return player;
        }
       
        public bool IsControllerIDAvailable(EControllerID controllerID, bool logWarnings = true)
        {
            foreach (PlayerBase playerItr in ConnectedPlayers)
            {
                if (IS_NULL(playerItr, logWarnings))
                    continue;

                if (playerItr.ControllerID == controllerID)
                    return false;
            }
            return true;
        }

        /* NetworkID */
        public PlayerBase[] GetAllPlayersFromNetworkID(ENetworkID networkID, bool logWarnings = true)
        {
            if (IS_NONE(networkID, logWarnings))
                return null;
            
            List<PlayerBase> players =new();
            foreach (PlayerBase playerItr in ConnectedPlayers)
            {
                if (playerItr.NetworkID == networkID)
                {
                    players.Add(playerItr);
                    break;
                }
            }
            return players.ToArray();
        }
        
        /* Team */
        public bool CanJoinTeam(ETeamID teamID, bool logWarnings = true)
        {
            int numberOfPlayersInTeam = GetAllPlayersInTeam(teamID).Length;
            int maxNumberOfPlayersInTeam = BManager.Inst.Config.MaxNumberOfPlayersInTeam;
            
            if (IS_GREATER_OR_EQUAL(numberOfPlayersInTeam, maxNumberOfPlayersInTeam, logWarnings))
                return false;
            
            return true;
        }
        
        public PlayerBase[] GetAllPlayersInTeam(ETeamID teamID)
        {
            List<PlayerBase> players = new();
            foreach (PlayerBase playerItr in PlayersInParty.Values)
            {
                if (playerItr.TeamID == teamID)
                {
                    players.Add(playerItr);
                    break;
                }
            }
            return players.ToArray();
        }

        /* Party */
        public EPlayerID JoinParty(PlayerBase player, bool logWarnings = true)
        {
            if (IS_KEY_CONTAINED(PlayersInParty, player.PlayerID, true))
                return EPlayerID.NONE;
            
            EPlayerID playerID = GetNextFreePlayerID();
            if (ARE_ENUMS_EQUAL(playerID, EPlayerID.NONE, true))
                return EPlayerID.NONE;

            if (IS_KEY_CONTAINED(PlayersInLobby, player.SpectatorID))
            {
                PlayersInLobby.Remove(player.SpectatorID);
            }
            
            PlayersInParty.Add(playerID, player);
            return playerID;
        }

        public ESpectatorID LeaveParty(PlayerBase player, bool logWarnings = true)
        {
            if (IS_KEY_NOT_CONTAINED(PlayersInParty, player.PlayerID, logWarnings))
                return ESpectatorID.NONE;
            
            PlayersInParty.Remove(player.PlayerID);
            
            ESpectatorID spectatorID = GetNextFreeSpectatorID();
            if (ARE_ENUMS_EQUAL(spectatorID, ESpectatorID.NONE, true))
            {
                LogConsoleError($"Disconnecting player {player} because no free spectatorID found");
                DestroyPlayer(player.ControllerID);
                return ESpectatorID.NONE;
            }
            
            PlayersInLobby.Add(spectatorID, player);
            
            return spectatorID;
        }
        
        public PlayerBase GetPlayerInLobby(ESpectatorID spectatorID, bool logWarnings = true)
        {
            if (IS_KEY_NOT_CONTAINED(PlayersInLobby, spectatorID, logWarnings))
                return null;
            
            return PlayersInLobby[spectatorID];
        }
       
        public PlayerBase GetPlayerInParty(EPlayerID playerID, bool logWarnings = true)
        {
            if (IS_KEY_NOT_CONTAINED(PlayersInParty, playerID, logWarnings))
                return null;
            
            return PlayersInParty[playerID];
        }

        public PlayerBase[] GetAllPlayersInLobby()
        {
            return PlayersInLobby.Values.ToArray();
        }
        
        public PlayerBase[] GetAllPlayersInTheParty()
        {
            return PlayersInParty.Values.ToArray();
        }
   
        public bool AreAllPlayersReady(bool logWarnings = true)
        {
            if (PlayersInParty.Count == 0)
                return false;
            
            foreach (PlayerBase playerItr in PlayersInParty.Values)
            {
                if (IS_NULL(playerItr, logWarnings))
                    continue;
                
                if (playerItr.IsReady == false)
                    return false;
            }
            return true;
        }
        
        /* Pawn */
        public PawnBase SpawnPawn(EPlayerID playerID, bool logWarnings = true)
        {
            if (IS_KEY_CONTAINED(ActivePawns, playerID, logWarnings))
                return null;
            
            PlayerBase player = GetPlayerInParty(playerID);
            if (IS_NULL(player, logWarnings))
                return null;

            PawnBase selectedPawnPrefab;
            if (BManager.Inst.Config.UseSamePrefabForAllPawns)
            {
                selectedPawnPrefab = pawnPrefab;
            }
            else
            {
                if (IS_KEY_NOT_CONTAINED(pawnPrefabsMap, playerID, logWarnings))
                    return null;
                
                selectedPawnPrefab = pawnPrefabsMap[playerID];
            }
            if (IS_NULL(selectedPawnPrefab, logWarnings))
                return null;
            
            if (IS_KEY_NOT_CONTAINED(PlayersSpawnPositions, playerID, logWarnings))
                return null;

            PawnSpawnPositionBase pawnSpawnPosition = PlayersSpawnPositions[playerID];
            Transform pawnSpawnParent = BManager.Inst.Config.PawnSpawnParent;
            PawnBase spawnedPawn = Instantiate(selectedPawnPrefab, pawnSpawnParent, true);
            spawnedPawn.Init(new()
            {
                Player = player,
                Position = pawnSpawnPosition.Position,
                Rotation = pawnSpawnPosition.Rotation,
            }); 
            
            ActivePawns.Add(playerID, spawnedPawn);
            
            BEvents.PAWNS_PawnSpawned.Invoke(new(spawnedPawn));
            
            return spawnedPawn;
        }

        public bool DestroyPawn(EPlayerID playerID, bool logWarnings = true)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePawns, playerID, logWarnings))
                return false;
            
            PawnBase pawn = ActivePawns[playerID];
            pawn.DestroyPawn();
            ActivePawns.Remove(playerID);
            
            BEvents.PAWNS_PawnDestroyed.Invoke(new(pawn));
            
            return true;
        }
        
        public PawnBase RespawnPawn(EPlayerID playerID, bool logWarnings = true)
        {
            if (ActivePawns.ContainsKey(playerID))
            {
                DestroyPawn(playerID, logWarnings);
            }
            
            return SpawnPawn(playerID, logWarnings);
        }

        public void SpawnAllPawnsFromPlayersInParty(bool logWarnings = true)
        {
            foreach (PlayerBase player in PlayersInParty.Values)
            {
                if (IS_NULL(player, logWarnings))
                    continue;
                
                SpawnPawn(player.PlayerID);
            }
        }

        /* Others */
        public void ReloadPrefabs()
        {
            InitializePrefabs();
        }
        
        #endregion

        #region Inspector Variables

        
        #endregion

        #region Variables
        /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
        public Dictionary<EPlayerID, PawnSpawnPositionBase> PlayersSpawnPositions { get; } = new();

        /// <summary> Added whenever a player is connected. Removed when he disconnects. </summary>
        public List<PlayerBase> ConnectedPlayers { get; } = new();
        
        /// <summary> Map with all the players in the lobby. </summary>
        public Dictionary<ESpectatorID, PlayerBase> PlayersInLobby { get; } = new();

        /// <summary> Map with all the players in the party. </summary>
        public Dictionary<EPlayerID, PlayerBase> PlayersInParty { get; } = new();

        /// <summary> Added whenever a pawn has spawned. Removed when he gets destroyed. </summary>
        public Dictionary<EPlayerID, PawnBase> ActivePawns { get; } = new();

        private PlayerBase playerPrefab;
        private Dictionary<EPlayerID, PlayerBase> playerPrefabsMap { get; } = new();
        private PawnBase pawnPrefab;
        private Dictionary<EPlayerID, PawnBase> pawnPrefabsMap { get; } = new();
        
        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();

            InitializePrefabs();
            FindPlayerSpawnPositionsInScene();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BEvents.APP_SceneUpdated += BEvents_APP_OnSceneUpdated;
            BEvents.INPUT_ControllerConnected += BEvents_INPUT_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected += BEvents_INPUT_OnControllerDisconnected;
            BEvents.MULTIPLAYER_LaunchMultiplayerSucceeded += BEvents_MULTIPLAYER_OnLaunchMultiplayerSucceeded;
            BEvents.MULTIPLAYER_ShutdownMultiplayer += BEvents_MULTIPLAYER_OnShutdownMultiplayer;
            BEvents.MULTIPLAYER_ClientJoined += BEvents_MULTIPLAYER_OnClientJoined;
            BEvents.MULTIPLAYER_ClientLeft += BEvents_MULTIPLAYER_OnClientLeft;
            BEvents.MULTIPLAYER_RequestReplicatePlayer += BEvents_MULTIPLAYER_RequestReplicatePlayer;
            BEvents.MULTIPLAYER_MigratePlayerIDs += BEvents_MULTIPLAYER_OnMigratePlayerIDs;
            BEvents.MULTIPLAYER_ReplicatePlayer += BEvents_MULTIPLAYER_OnReplicatePlayer;
            BEvents.MULTIPLAYER_ReplicateAllPlayers += BEvents_MULTIPLAYER_OnReplicateAllPlayers;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            BEvents.APP_SceneUpdated -= BEvents_APP_OnSceneUpdated;
            BEvents.INPUT_ControllerConnected -= BEvents_INPUT_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected -= BEvents_INPUT_OnControllerDisconnected;
            BEvents.MULTIPLAYER_LaunchMultiplayerSucceeded -= BEvents_MULTIPLAYER_OnLaunchMultiplayerSucceeded;
            BEvents.MULTIPLAYER_ShutdownMultiplayer -= BEvents_MULTIPLAYER_OnShutdownMultiplayer;
            BEvents.MULTIPLAYER_ClientJoined -= BEvents_MULTIPLAYER_OnClientJoined;
            BEvents.MULTIPLAYER_ClientLeft -= BEvents_MULTIPLAYER_OnClientLeft;
            BEvents.MULTIPLAYER_RequestReplicatePlayer -= BEvents_MULTIPLAYER_RequestReplicatePlayer;
            BEvents.MULTIPLAYER_MigratePlayerIDs -= BEvents_MULTIPLAYER_OnMigratePlayerIDs;
            BEvents.MULTIPLAYER_ReplicatePlayer -= BEvents_MULTIPLAYER_OnReplicatePlayer;
            BEvents.MULTIPLAYER_ReplicateAllPlayers += BEvents_MULTIPLAYER_OnReplicateAllPlayers;
        }

        #endregion

        #region Events Callbacks
                
        /* Scene */
        private void BEvents_APP_OnSceneUpdated(BEventHandle<SScene> bEventHandle)
        {
            FindPlayerSpawnPositionsInScene();
        }

        /* Input */
        private void BEvents_INPUT_OnControllerConnected(BEventHandle<EControllerID, EControllerType> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            if (IS_NOT_NULL(GetPlayer(controllerID), true))
                return;

            if (BUtils.IsControllerIDRemote(controllerID))
                return;

            ESpectatorID spectatorID = GetNextFreeSpectatorID();
            SpawnPlayer(EPlayerID.NONE, spectatorID, controllerID);
        }

        private void BEvents_INPUT_OnControllerDisconnected(BEventHandle<EControllerID, EControllerType> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            DestroyPlayer(controllerID);
        }

        /* Multiplayer */
        private void BEvents_MULTIPLAYER_OnLaunchMultiplayerSucceeded(BEventHandle handle)
        {
            foreach (var playerItr in ConnectedPlayers)
            {
                if (playerItr.NetworkID != ENetworkID.LOCAL)
                    continue;

                ENetworkID localNetworkID = BMultiplayerManager.Inst.LocalNetworkID;
                playerItr.SetNetworkID(localNetworkID);

                SPlayerReplicationArg playerReplicationArg = CreatePlayerReplicationArg(playerItr);

                switch (BMultiplayerManager.Inst.Authority)
                {
                    case EAuthority.CLIENT:
                        BEvents.MULTIPLAYER_RequestReplicatePlayer.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_TARGET, true, ENetworkID.HOST_1);
                        break;
                    
                    // case EAuthority.HOST:
                    //     BEvents.MULTIPLAYER_ReplicatePlayer.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_ALL_OTHERS);
                    //     break;
                }
            }
        }

        private void BEvents_MULTIPLAYER_OnShutdownMultiplayer(BEventHandle<ELeaveMultiplayerReason, ENetworkID> handle)
        {
            ENetworkID oldLocalNetworkID = handle.Arg2;
            for (int i = ConnectedPlayers.Count - 1; i >= 0; i--)
            {
                var playerItr = ConnectedPlayers[i];
                if (playerItr.NetworkID == oldLocalNetworkID)
                {
                    playerItr.SetNetworkID(ENetworkID.LOCAL);
                }
                else
                {
                    BInputManager.Inst.DisconnectController(playerItr.ControllerID);
                }
            }
        }
        
        private void BEvents_MULTIPLAYER_OnClientJoined(BEventHandle<ENetworkID> handle)
        {
            ENetworkID newNetworkID = handle.Arg1;
            
            if (BMultiplayerManager.Inst.Authority != EAuthority.HOST
                || newNetworkID == BMultiplayerManager.Inst.LocalNetworkID)
                return;

            List<SPlayerReplicationArg> playerReplicationArgs = new();
            foreach (var playerItr in ConnectedPlayers)
            {
                if (playerItr == null)
                    continue;

                LogConsoleYellow($"Replicating : {playerItr.PlayerName}");
                SPlayerReplicationArg playerReplicationArgItr = CreatePlayerReplicationArg(playerItr);
                BEvents.MULTIPLAYER_ReplicatePlayer.Invoke(new (playerReplicationArgItr), BEventBroadcastType.TO_TARGET, true, newNetworkID);
                
                playerReplicationArgs.Add(playerReplicationArgItr);
            }
            
            // BEvents.MULTIPLAYER_ReplicateAllPlayers.Invoke(new (playerReplicationArgs.ToArray()), BEventBroadcastType.TO_ALL_OTHERS);
        }

        private void BEvents_MULTIPLAYER_OnClientLeft(BEventHandle<ENetworkID> handle)
        {
            ENetworkID leftNetworkID = handle.Arg1;
            if (leftNetworkID == BMultiplayerManager.Inst.LocalNetworkID)
                return;
            
            for (int i = ConnectedPlayers.Count - 1; i >= 0; i--)
            {
                var playerItr = ConnectedPlayers[i];
                if (playerItr.NetworkID != leftNetworkID)
                    continue;

                DestroyPlayer(playerItr.ControllerID);
            }
        }

        private void BEvents_MULTIPLAYER_RequestReplicatePlayer(BEventHandle<SPlayerReplicationArg> handle)
        {
            if (handle.InvokingNetworkID == ENetworkID.HOST_1)
                return;
            
            SPlayerReplicationArg playerReplicationArg = handle.Arg1;

            EPlayerID newPlayerID = EPlayerID.NONE;
            if (playerReplicationArg.PlayerID != EPlayerID.NONE)
            {
                newPlayerID = GetNextFreePlayerID();
            }
            
            ESpectatorID newSpectatorID = ESpectatorID.NONE;
            if (playerReplicationArg.SpectatorID != ESpectatorID.NONE)
            {
                newSpectatorID = GetNextFreeSpectatorID();
            }
            
            ETeamID teamID = playerReplicationArg.TeamID;
            string playerName = playerReplicationArg.PlayerName;
            ENetworkID networkID = handle.InvokingNetworkID;
            EControllerID controllerID = BInputManager.Inst.ConnectNextRemoteController();
            SpawnPlayer(newPlayerID, newSpectatorID, controllerID, networkID, teamID, playerName);

            // Response to Host
            SPlayerIDMigration playerIDMigration = new()
            {
                LocalControllerID = playerReplicationArg.LocalControllerID,
                ToPlayerID = newPlayerID,
                ToSpectatorID = newSpectatorID,
            };
            BEvents.MULTIPLAYER_MigratePlayerIDs.Invoke(new (playerIDMigration), BEventBroadcastType.TO_TARGET, true, networkID);
        }

        private void BEvents_MULTIPLAYER_OnMigratePlayerIDs(BEventHandle<SPlayerIDMigration> handle)
        {
            if (handle.InvokingNetworkID != BMultiplayerManager.Inst.LocalNetworkID)
                return;

            SPlayerIDMigration playerIDMigration = handle.Arg1;
            EControllerID controllerID = playerIDMigration.LocalControllerID;

            
            PlayerBase player = GetPlayer(controllerID);
            if (IS_NULL(player, true))
                return;
            
            EPlayerID newPlayerID = playerIDMigration.ToPlayerID;
            ESpectatorID newSpectatorID = playerIDMigration.ToSpectatorID;
            player.SetPlayerID(newPlayerID);
            player.SetSpectatorID(newSpectatorID);
        }

        private void BEvents_MULTIPLAYER_OnReplicatePlayer(BEventHandle<SPlayerReplicationArg> handle)
        {
            SPlayerReplicationArg playerReplicationArg = handle.Arg1;
            
            ENetworkID networkID = playerReplicationArg.NetworkID;
            if (IS_NONE(networkID, true)
                || networkID == BMultiplayerManager.Inst.LocalNetworkID)
                return;
            
            EPlayerID playerID = playerReplicationArg.PlayerID;
            ESpectatorID spectatorID = playerReplicationArg.SpectatorID;
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleWarning("Both PlayerID and SpectatorID of replicated player are NONE!");
            }

            ETeamID teamID = playerReplicationArg.TeamID;
            string playerName = playerReplicationArg.PlayerName;
            
            EControllerID controllerID = BInputManager.Inst.ConnectNextRemoteController();
            if (IS_NONE(controllerID, true))
                return;
            
            SpawnPlayer(playerID, spectatorID, controllerID, networkID, teamID, playerName);
        }

        private void BEvents_MULTIPLAYER_OnReplicateAllPlayers(BEventHandle<SPlayerReplicationArg[]> handle)
        {
            SPlayerReplicationArg[] playerReplicationArgs = handle.Arg1;
            foreach (var playerReplicationArgItr in playerReplicationArgs)
            {
                
            }
        }

        #endregion

        #region Others

        /* Spawn */
        protected virtual PlayerBase SpawnPlayer(EPlayerID playerID, ESpectatorID spectatorID,  
            EControllerID controllerID, ENetworkID networkID = ENetworkID.LOCAL, ETeamID teamID = ETeamID.NONE, 
            string playerName = "Player")
        {
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleError("Can't spawn a player! Both playerID and controllerID are None");
                return null;
            }
            
            if (spectatorID != ESpectatorID.NONE)
            {
                PlayerBase playerWithSameSpectatorID = GetPlayerInLobby(spectatorID, false);
                if (IS_NOT_NULL(playerWithSameSpectatorID, true))
                    return null;
            }
            
            if (playerID != EPlayerID.NONE)
            {
                PlayerBase playerWithSamePlayerID = GetPlayerInParty(playerID, false);
                if (IS_NOT_NULL(playerWithSamePlayerID, true))
                    return null;
            }
            
            PlayerBase playerWithSameControllerID = GetPlayer(controllerID);
            if (IS_NOT_NULL(playerWithSameControllerID, true))
                return null;

            if (networkID == ENetworkID.LOCAL)
            {
                networkID = BMultiplayerManager.Inst.LocalNetworkID;
            }

            // Fetch player prefab
            PlayerBase selectedPlayerPrefab;
            if (BManager.Inst.Config.UseSamePrefabForAllPlayers)
            {
                selectedPlayerPrefab = playerPrefab;
            }
            else
            {
                if (IS_KEY_NOT_CONTAINED(playerPrefabsMap, playerID, true))
                    return null;
                
                selectedPlayerPrefab = playerPrefabsMap[playerID];
            }
            if (IS_NULL(selectedPlayerPrefab, true))
                return null;
            
            // Instantiate player prefab
            PlayerBase spawnedPlayer = Instantiate(selectedPlayerPrefab, transform, true);
            spawnedPlayer.Init(new SPlayerInit
            {
                PlayerID = playerID,
                SpectatorID = spectatorID,
                ControllerID = controllerID,
                NetworkID = networkID,
                TeamID = teamID,
                PlayerName = playerName,
            });
            
            ConnectedPlayers.Add(spawnedPlayer);

            // Update party state
            switch (spawnedPlayer.PartyState)
            {
                case EPlayerPartyState.IN_LOBBY:
                    PlayersInLobby.Add(spectatorID, spawnedPlayer);
                    break;
                
                case EPlayerPartyState.IN_PARTY:
                    PlayersInParty.Add(playerID, spawnedPlayer);
                    break;
            }
            
            BEvents.PLAYERS_PlayerConnected.Invoke(new BEventHandle<PlayerBase>(spawnedPlayer));

            // Replicate spawned player
            var replicationArg = CreatePlayerReplicationArg(spawnedPlayer);
            switch (BMultiplayerManager.Inst.Authority)
            {
                case EAuthority.HOST when networkID == BMultiplayerManager.Inst.LocalNetworkID:
                    BEvents.MULTIPLAYER_ReplicatePlayer.Invoke(new (replicationArg), BEventBroadcastType.TO_ALL_OTHERS);
                    break;
                
                case EAuthority.CLIENT when networkID == BMultiplayerManager.Inst.LocalNetworkID:
                    BEvents.MULTIPLAYER_RequestReplicatePlayer.Invoke(new (replicationArg), BEventBroadcastType.TO_TARGET, true, ENetworkID.HOST_1);
                    break;
            }
            
            return spawnedPlayer;
        }

        protected virtual bool DestroyPlayer(EControllerID controllerID)
        {
            PlayerBase player = GetPlayer(controllerID);
            if (IS_NULL(player, true))
                return false;

            if (ActivePawns.ContainsKey(player.PlayerID))
            {
                DestroyPawn(player.PlayerID);
            }

            if (PlayersInLobby.ContainsKey(player.SpectatorID))
            {
                PlayersInLobby.Remove(player.SpectatorID);
            }
            
            if (PlayersInParty.ContainsKey(player.PlayerID))
            {
                PlayersInParty.Remove(player.PlayerID);
            }
            
            ConnectedPlayers.Remove(player);
            
            BEvents.PLAYERS_PlayerDisconnected.Invoke(new BEventHandle<PlayerBase>(player));

            player.DestroyPlayer();
            return true;
        }

        /* IDs */
        private ESpectatorID GetNextFreeSpectatorID()
        {
            ESpectatorID spectatorID = ESpectatorID.NONE;
            for (int i = 1; i <= BManager.Inst.Config.MaxNumberOfSpectators; i++)
            {
                ESpectatorID spectatorIDItr = (ESpectatorID) i;
                if (PlayersInLobby.ContainsKey(spectatorIDItr) == false)
                {
                    spectatorID = spectatorIDItr;
                    break;
                }
            }
            return spectatorID;
        }

        private EPlayerID GetNextFreePlayerID()
        {
            EPlayerID playerID = EPlayerID.NONE;
            for (int i = 1; i <= BManager.Inst.Config.MaxNumberOfPlayersInParty; i++)
            {
                EPlayerID playerIDItr = (EPlayerID) i;
                if (PlayersInParty.ContainsKey(playerIDItr) == false)
                {
                    playerID = playerIDItr;
                    break;
                }
            }
            return playerID;
        }

        /* Initialization */
        private void InitializePrefabs()
        {
            playerPrefab = BManager.Inst.Config.PlayerPrefab;
            
            playerPrefabsMap.Clear();
            foreach (PlayerPrefabTupple playerPrefabItr in BManager.Inst.Config.PlayerPrefabs)
            {
                playerPrefabsMap.Add(playerPrefabItr.PlayerID, playerPrefabItr.Prefab);
            }
            
            pawnPrefab = BManager.Inst.Config.PawnPrefab;
            
            foreach (PawnPrefabTupple pawnPrefabItr in BManager.Inst.Config.PawnPrefabs)
            {
                pawnPrefabsMap.Add(pawnPrefabItr.PlayerID, pawnPrefabItr.Prefab);
            }
        }

        private void FindPlayerSpawnPositionsInScene()
        {
            PlayersSpawnPositions.Clear();

            // Try to find already placed player spawn positions in the scene
            PawnSpawnPositionBase[] spawnPositions = FindObjectsOfType<PawnSpawnPositionBase>();
            foreach (PawnSpawnPositionBase spawnPosition in spawnPositions)
            {
                if (IS_KEY_NOT_CONTAINED(PlayersSpawnPositions, spawnPosition.PayerID))
                {
                    PlayersSpawnPositions.Add(spawnPosition.PayerID, spawnPosition);
                }
            }
        }

        /* Replication */
        private static SPlayerReplicationArg CreatePlayerReplicationArg(PlayerBase fromPlayer)
        {
            SPlayerReplicationArg replicationArg = new()
            {
                NetworkID = fromPlayer.NetworkID,
                LocalControllerID = fromPlayer.ControllerID,
                PlayerID = fromPlayer.PlayerID,
                SpectatorID = fromPlayer.SpectatorID,
                TeamID = fromPlayer.TeamID,
                PlayerName = fromPlayer.PlayerName,
            };
            return replicationArg;
        }


        #endregion
    }
}