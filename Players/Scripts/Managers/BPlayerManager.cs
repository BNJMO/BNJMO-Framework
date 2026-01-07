using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNJMO
{

    public class BPlayerManager : AbstractSingletonManager<BPlayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* ControllerID */
        public PlayerBase GetPlayerWithControllerID(EControllerID controllerID, bool logWarnings = true)
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
       
        public bool IsAnyPlayerWithControllerID(EControllerID controllerID, bool logWarnings = true)
        {
            return ConnectedPlayers.Any(playerItr => playerItr.ControllerID == controllerID);
        }

        /* NetworkID */
        public List<PlayerBase> GetPlayersWithNetworkID(ENetworkID networkID, bool logWarnings = true)
        {
            return ConnectedPlayers.Where(playerItr => playerItr.NetworkID == networkID).ToList();
        }
        
        /* Team */
        public bool CanJoinTeam(ETeamID teamID, bool logWarnings = true)
        {
            int numberOfPlayersInTeam = GetAllPlayersInTeam(teamID).Count;
            int maxNumberOfPlayersInTeam = BManager.Inst.Config.MaxNumberOfPlayersInTeam;
            
            if (IS_GREATER_OR_EQUAL(numberOfPlayersInTeam, maxNumberOfPlayersInTeam, logWarnings))
                return false;
            
            return true;
        }
        
        public List<PlayerBase> GetAllPlayersInTeam(ETeamID teamID)
        {
            return ConnectedPlayers.Where(playerItr => playerItr.TeamID == teamID).ToList();
        }

        /* Party */
        public PlayerBase GetSpectator(ESpectatorID spectatorID, bool logWarnings = true)
        {
            return ConnectedPlayers
                .FirstOrDefault(playerItr => playerItr.SpectatorID != ESpectatorID.NONE 
                                             && playerItr.SpectatorID == spectatorID);
        }
       
        public PlayerBase GetActivePlayer(EPlayerID playerID, bool logWarnings = true)
        {
            return ConnectedPlayers
                .FirstOrDefault(playerItr => playerItr.PlayerID != EPlayerID.NONE 
                                             && playerItr.PlayerID == playerID);
        }

        public List<PlayerBase> GetAllSpectators()
        {
            return ConnectedPlayers
                .Where(playerItr => playerItr.SpectatorID != ESpectatorID.NONE)
                .ToList();
        }
        
        public ESpectatorID[] GetAllConnectedSpectatorIDs()
        {
            return ConnectedPlayers
                .Where(playerItr => playerItr.SpectatorID != ESpectatorID.NONE)
                .Select(playerItr => playerItr.SpectatorID)
                .ToArray();
        }
        
        public List<PlayerBase> GetAllActivePlayers()
        {
           return ConnectedPlayers
               .Where(playerItr => playerItr.PlayerID != EPlayerID.NONE)
               .ToList();
        }
        
        public EPlayerID[] GetAllConnectedPlayerIDs()
        {
            return ConnectedPlayers
                .Where(playerItr => playerItr.PlayerID != EPlayerID.NONE)
                .Select(playerItr => playerItr.PlayerID)
                .ToArray();
        }
        
        public ESpectatorID GetNextFreeSpectatorID()
        {
            HashSet<ESpectatorID> spectatorIDSet = new(GetAllConnectedSpectatorIDs());
            for (int i = 1; i <= BManager.Inst.Config.MaxNumberOfSpectators; i++)
            {
                var spectatorIDItr = (ESpectatorID)i;
                if (spectatorIDSet.Contains(spectatorIDItr) == false)
                {
                    return spectatorIDItr;
                }
            }
            return ESpectatorID.NONE;
        }

        public bool IsSpectatorIDAvailable(ESpectatorID spectatorID)
        {
            return ConnectedPlayers.All(playerItr => playerItr.SpectatorID != spectatorID);
        }

        public EPlayerID GetNextFreePlayerID()
        {
            HashSet<EPlayerID> playerIDSet = new(GetAllConnectedPlayerIDs());
            for (int i = 1; i <= BManager.Inst.Config.MaxNumberOfActivePlayers; i++)
            {
                var playerIDItr = (EPlayerID)i;
                if (playerIDSet.Contains(playerIDItr) == false)
                {
                    return playerIDItr;
                }
            }
            return EPlayerID.NONE;
        }
        
        public bool IsPlayerIDAvailable(EPlayerID playerID)
        {
            return ConnectedPlayers.All(playerItr => playerItr.PlayerID != playerID);
        }
        
        public bool AreAllPlayersReady(bool logWarnings = true)
        {
            var allActivePlayers = GetAllActivePlayers();
            if (allActivePlayers.Count < 1)
                return false;
            
            foreach (PlayerBase playerItr in allActivePlayers)
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
            if (GetActivePawn(playerID))
                return null;
            
            PlayerBase player = GetActivePlayer(playerID);
            if (IS_NULL(player, logWarnings))
                return null;

            PawnBase selectedPawnPrefab = GetPawnPrefab(playerID);
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
            
            BEvents.PAWNS_Spawned.Invoke(new(spawnedPawn));
            
            return spawnedPawn;
        }

        public PawnBase RespawnPawn(EPlayerID playerID, bool logWarnings = true)
        {
            var activePawn = GetActivePawn(playerID);
            if (activePawn)
            {
                activePawn.DestroyPawn();
            }
            
            return SpawnPawn(playerID, logWarnings);
        }

        public void SpawnAllPawnsFromActivePlayers(bool logWarnings = true)
        {
            foreach (PlayerBase player in GetAllActivePlayers())
            {
                if (IS_NULL(player, logWarnings))
                    continue;
                
                SpawnPawn(player.PlayerID);
            }
        }

        public void DestroyAllActivePawns(bool logWarnings = true)
        {
            foreach (var playerItr in GetAllActivePlayers())
            {
                if (IS_NOT_VALID(playerItr, true))
                    continue;
                
                var pawnItr = playerItr.ActivePawn;
                if (!pawnItr)
                    continue;
                
                pawnItr.DestroyPawn();
            }
        }

        public PawnBase GetActivePawn(EPlayerID playerID)
        {
            if (IS_NONE(playerID, true))
                return null;
            
            return ConnectedPlayers
                .FirstOrDefault(playerItr => playerItr.PlayerID == playerID)
                ?.ActivePawn;
        }

        public List<PawnBase> GetAllActivePawns()
        {
            List<PawnBase> list = new();
            foreach (var playerItr in GetAllActivePlayers())
            {
                if (IS_NOT_VALID(playerItr, true))
                    continue;
                
                var pawnItr = playerItr.ActivePawn;
                if (!pawnItr)
                    continue;
                
                list.Add(pawnItr);
            }

            return list;
        }

        #endregion

        #region Inspector Variables

        
        #endregion

        #region Variables
        /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
        public Dictionary<EPlayerID, PawnSpawnPositionBase> PlayersSpawnPositions { get; } = new();

        /// <summary> Added whenever a player is connected. Removed when he disconnects. </summary>
        public List<PlayerBase> ConnectedPlayers { get; } = new();

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();

            FindPlayerSpawnPositionsInScene();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BEvents.APP_SceneUpdated += BEvents_APP_OnSceneUpdated;
            BEvents.PLAYERS_Disconnected += BEvents_PLAYERS_OnDisconnected;
            BEvents.INPUT_ControllerConnected += BEvents_INPUT_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected += BEvents_INPUT_OnControllerDisconnected;
            BEvents.ONLINE_LaunchSessionSucceeded += BEvents_ONLINE_OnLaunchSessionSucceeded;
            BEvents.ONLINE_ShutdownSession += BEvents_ONLINE_OnShutdownSession;
            BEvents.ONLINE_ClientLeft += BEvents_ONLINE_OnClientLeft;
            BEvents.ONLINE_RequestReplicatePlayer += BEvents_ONLINE_OnRequestReplicatePlayer;
            BEvents.ONLINE_MigratePlayerID += BEvents_ONLINE_OnMigratePlayerIDs;
            BEvents.ONLINE_ConfirmPlayerIDMigration += BEvents_ONLINE_OnConfirmPlayerIDsMigration;
            BEvents.ONLINE_ReplicatePlayer += BEvents_ONLINE_OnReplicatePlayer;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            BEvents.APP_SceneUpdated -= BEvents_APP_OnSceneUpdated;
            BEvents.PLAYERS_Disconnected -= BEvents_PLAYERS_OnDisconnected;
            BEvents.INPUT_ControllerConnected -= BEvents_INPUT_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected -= BEvents_INPUT_OnControllerDisconnected;
            BEvents.ONLINE_LaunchSessionSucceeded -= BEvents_ONLINE_OnLaunchSessionSucceeded;
            BEvents.ONLINE_ShutdownSession -= BEvents_ONLINE_OnShutdownSession;
            BEvents.ONLINE_ClientLeft -= BEvents_ONLINE_OnClientLeft;
            BEvents.ONLINE_RequestReplicatePlayer -= BEvents_ONLINE_OnRequestReplicatePlayer;
            BEvents.ONLINE_MigratePlayerID -= BEvents_ONLINE_OnMigratePlayerIDs;
            BEvents.ONLINE_ConfirmPlayerIDMigration -= BEvents_ONLINE_OnConfirmPlayerIDsMigration;
            BEvents.ONLINE_ReplicatePlayer -= BEvents_ONLINE_OnReplicatePlayer;
        }

        #endregion

        #region Events Callbacks
                
        /* Scene */
        private void BEvents_APP_OnSceneUpdated(BEventHandle<SScene> handle)
        {
            FindPlayerSpawnPositionsInScene();
        }

        /* Players */
        private void BEvents_PLAYERS_OnDisconnected(BEventHandle<PlayerBase> handle)
        {
            var disconnectedPlayer = handle.Arg1;
            if (IS_NULL(disconnectedPlayer, true))
                return;

            if (IS_VALUE_CONTAINED(ConnectedPlayers, disconnectedPlayer))
            {
                ConnectedPlayers.Remove(disconnectedPlayer);
            }

            BInputManager.Inst.DisconnectController(disconnectedPlayer.ControllerID);
        }

        /* Input */
        private void BEvents_INPUT_OnControllerConnected(BEventHandle<EControllerID, EControllerType> handle)
        {
            var controllerID = handle.Arg1;
            if (IS_NOT_NULL(GetPlayerWithControllerID(controllerID), true))
                return;

            if (BUtils.IsControllerIDRemote(controllerID))
                return;

            ESpectatorID spectatorID = GetNextFreeSpectatorID();
            EControllerType controllerType = handle.Arg2;
            SpawnPlayer(EPlayerID.NONE, spectatorID, controllerID, controllerType);
        }

        private void BEvents_INPUT_OnControllerDisconnected(BEventHandle<EControllerID, EControllerType> handle)
        {
            EControllerID controllerID = handle.Arg1;

            var player = GetPlayerWithControllerID(controllerID);
            if (player)
            {
                player.DestroyPlayer();
            }
        }

        /* Multiplayer */
        private void BEvents_ONLINE_OnLaunchSessionSucceeded(BEventHandle<SLobbyArg> handle)
        {
            foreach (var playerItr in ConnectedPlayers)
            {
                if (playerItr.NetworkID != ENetworkID.LOCAL)
                    continue;

                ENetworkID localNetworkID = BOnlineManager.Inst.LocalNetworkID;
                playerItr.SetNetworkID(localNetworkID);

                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(playerItr);

                switch (BOnlineManager.Inst.Authority)
                {
                    case EAuthority.CLIENT:
                        BEvents.ONLINE_RequestReplicatePlayer.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_TARGET, true, ENetworkID.HOST_1);
                        break;
                }
            }
        }

        private void BEvents_ONLINE_OnShutdownSession(BEventHandle<ELeaveOnlineSessionReason, ENetworkID> handle)
        {
            ENetworkID oldLocalNetworkID = handle.Arg2;
            for (int i = ConnectedPlayers.Count - 1; i >= 0; i--)
            {
                var playerItr = ConnectedPlayers[i];
                if (oldLocalNetworkID == ENetworkID.LOCAL)
                    continue;
                
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

        private void BEvents_ONLINE_OnClientLeft(BEventHandle<ENetworkID> handle)
        {
            ENetworkID clientLeftNetworkID = handle.Arg1;
            if (clientLeftNetworkID == BOnlineManager.Inst.LocalNetworkID)
                return;

            var associatedPlayers = GetPlayersWithNetworkID(clientLeftNetworkID);
            foreach (var playerItr in associatedPlayers)
            {
                if (playerItr == null)
                    continue;
                
                playerItr.DestroyPlayer();
            }
        }

        private void BEvents_ONLINE_OnRequestReplicatePlayer(BEventHandle<SPlayerReplicationArg> handle)
        {
            if (ARE_EQUAL(handle.InvokingNetworkID, ENetworkID.HOST_1, true))
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
            Sprite playerPicture = BUtils.DecodeSprite(playerReplicationArg.PlayerPictureBase64);
            ENetworkID networkID = handle.InvokingNetworkID;
            EControllerID controllerID = BInputManager.Inst.ConnectNextRemoteController();
            EControllerType controllerType = playerReplicationArg.OwnerControllerType;
            SpawnPlayer(newPlayerID, newSpectatorID, controllerID, controllerType, networkID, teamID, playerName, playerPicture);

            // Response to Host
            SPlayerIDMigrationArg playerIDMigrationArg = new()
            {
                OwnerControllerID = playerReplicationArg.OwnerControllerID,
                OwnerControllerType = playerReplicationArg.OwnerControllerType,
                ToPlayerID = newPlayerID,
                ToSpectatorID = newSpectatorID,
            };
            BEvents.ONLINE_MigratePlayerID.Invoke(new (playerIDMigrationArg), BEventBroadcastType.TO_TARGET, true, networkID);
        }

        private void BEvents_ONLINE_OnMigratePlayerIDs(BEventHandle<SPlayerIDMigrationArg> handle)
        {
            if (handle.InvokingNetworkID != ENetworkID.HOST_1)
                return;

            SPlayerIDMigrationArg playerIDMigrationArg = handle.Arg1;
            EControllerID controllerID = playerIDMigrationArg.OwnerControllerID;

            
            PlayerBase player = GetPlayerWithControllerID(controllerID);
            if (IS_NULL(player, true))
                return;
            
            ESpectatorID newSpectatorID = playerIDMigrationArg.ToSpectatorID;
            player.SetSpectatorID(newSpectatorID);
            
            EPlayerID newPlayerID = playerIDMigrationArg.ToPlayerID;
            player.SetPlayerID(newPlayerID);
            
            BEvents.ONLINE_ConfirmPlayerIDMigration.Invoke(new(), BEventBroadcastType.TO_TARGET, true, ENetworkID.HOST_1);
        }
        
        private void BEvents_ONLINE_OnConfirmPlayerIDsMigration(BEventHandle handle)
        {
            ENetworkID migratedPlayerNetworkID = handle.InvokingNetworkID;
            
            if (ARE_NOT_EQUAL(BOnlineManager.Inst.Authority, EAuthority.HOST, true)
                || ARE_EQUAL(migratedPlayerNetworkID, ENetworkID.HOST_1, true))
                return;

            foreach (var playerItr in ConnectedPlayers)
            {
                if (playerItr == null
                    || playerItr.NetworkID == migratedPlayerNetworkID)
                    continue;

                SPlayerReplicationArg playerReplicationArgItr = BUtils.CreatePlayerReplicationArgFromPlayer(playerItr);
                BEvents.ONLINE_ReplicatePlayer.Invoke(new (playerReplicationArgItr), BEventBroadcastType.TO_TARGET, true, migratedPlayerNetworkID);
            }
        }

        private void BEvents_ONLINE_OnReplicatePlayer(BEventHandle<SPlayerReplicationArg> handle)
        {
            SPlayerReplicationArg playerReplicationArg = handle.Arg1;
            
            ENetworkID networkID = playerReplicationArg.NetworkID;
            if (IS_NONE(networkID, true)
                || networkID == BOnlineManager.Inst.LocalNetworkID)
                return;
            
            EPlayerID playerID = playerReplicationArg.PlayerID;
            ESpectatorID spectatorID = playerReplicationArg.SpectatorID;
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleWarning("Both PlayerID and SpectatorID of replicated player are NONE!");
                return;
            }

            PlayerBase spectatorWithSameID = GetSpectator(spectatorID, false);
            PlayerBase playerWithSameID = GetActivePlayer(playerID, false);
            if (spectatorWithSameID != null
                && playerWithSameID != null)
            {
                if (spectatorWithSameID != null)
                {
                    LogConsoleWarning($"A player with the same playerID already exists: {playerID}");
                }
                else if (playerWithSameID != null)
                {
                    LogConsoleWarning($"A player with the same spectatorID already exists: {spectatorID}");
                }
                return;
            }

            ETeamID teamID = playerReplicationArg.TeamID;
            string playerName = playerReplicationArg.PlayerName;
            Sprite playerPicture = BUtils.DecodeSprite(playerReplicationArg.PlayerPictureBase64);
            
            EControllerID controllerID = BInputManager.Inst.ConnectNextRemoteController();
            if (IS_NONE(controllerID, true))
                return;

            EControllerType controllerType = playerReplicationArg.OwnerControllerType;
            SpawnPlayer(playerID, spectatorID, controllerID, controllerType, networkID, teamID, playerName, playerPicture);
        }

        #endregion

        #region Others

        /* Spawn */
        protected virtual PlayerBase SpawnPlayer(EPlayerID playerID, ESpectatorID spectatorID,  
            EControllerID controllerID, EControllerType controllerType, ENetworkID networkID = ENetworkID.LOCAL, ETeamID teamID = ETeamID.NONE, 
            string playerName = "Player", Sprite playerPicture = null)
        {
            // Check PlayerID and SpectatorID for NONE
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleError("Can't spawn a player! Both playerID and controllerID are None");
                return null;
            }
            
            // Check for player with same SpectatorID
            if (spectatorID != ESpectatorID.NONE)
            {
                PlayerBase playerWithSameSpectatorID = GetSpectator(spectatorID, false);
                if (IS_NOT_NULL(playerWithSameSpectatorID, true))
                    return null;
            }

            // Check for player with same PlayerID
            if (playerID != EPlayerID.NONE)
            {
                PlayerBase playerWithSamePlayerID = GetActivePlayer(playerID, false);
                if (IS_NOT_NULL(playerWithSamePlayerID, true))
                    return null;
            }
            
            // Check if ControllerID is connected
            if (IS_NOT_TRUE(BInputManager.Inst.IsControllerConnected(controllerID), true))
                return null;
            
            // Check for player with same ControllerID
            PlayerBase playerWithSameControllerID = GetPlayerWithControllerID(controllerID);
            if (IS_NOT_NULL(playerWithSameControllerID, true))
                return null;

            // Set local NetworkID
            if (networkID == ENetworkID.LOCAL)
            {
                networkID = BOnlineManager.Inst.LocalNetworkID;
            }

            // Fetch player prefab
            PlayerBase selectedPlayerPrefab = GetPlayerPrefab(playerID);
            if (IS_NULL(selectedPlayerPrefab, true))
                return null;
            
            // Instantiate player prefab
            PlayerBase spawnedPlayer = Instantiate(selectedPlayerPrefab, transform, true);
            spawnedPlayer.Init(new SPlayerInitArg
            {
                PlayerID = playerID,
                SpectatorID = spectatorID,
                ControllerID = controllerID,
                ControllerType = controllerType,
                NetworkID = networkID,
                TeamID = teamID,
                PlayerName = playerName,
                PlayerPicture = playerPicture,
            });
            ConnectedPlayers.Add(spawnedPlayer);
            BEvents.PLAYERS_Connected.Invoke(new (spawnedPlayer));

            // Replicate spawned player
            var replicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(spawnedPlayer);
            switch (BOnlineManager.Inst.Authority)
            {
                case EAuthority.HOST when networkID == BOnlineManager.Inst.LocalNetworkID:
                    BEvents.ONLINE_ReplicatePlayer.Invoke(new (replicationArg), BEventBroadcastType.TO_ALL_OTHERS);
                    break;
                
                case EAuthority.CLIENT when networkID == BOnlineManager.Inst.LocalNetworkID:
                    BEvents.ONLINE_RequestReplicatePlayer.Invoke(new (replicationArg), BEventBroadcastType.TO_TARGET, true, ENetworkID.HOST_1);
                    break;
            }
            
            return spawnedPlayer;
        }
        
        /* Prefabs */
        private PlayerBase GetPlayerPrefab(EPlayerID playerID)
        {
            PlayerBase playerPrefab;
            if (BConfig.Inst.UseSamePrefabForAllPawns)
            {
                playerPrefab = BConfig.Inst.PlayerPrefab;
            }
            else
            {
                var playerPrefabs =  BConfig.Inst.PlayerPrefabs.ToList();
                playerPrefab = playerPrefabs.FirstOrDefault(pair => pair.PlayerID == playerID).Prefab;

                if (IS_NULL(playerPrefab))
                {
                    playerPrefab = BConfig.Inst.PlayerPrefab;
                }
            }

            IS_NOT_NULL(playerPrefab);
            return playerPrefab;
        }
        
        private PawnBase GetPawnPrefab(EPlayerID playerID)
        {
            PawnBase PawnPrefab;
            if (BConfig.Inst.UseSamePrefabForAllPawns)
            {
                PawnPrefab = BConfig.Inst.PawnPrefab;
            }
            else
            {
                var PawnPrefabs =  BConfig.Inst.PawnPrefabs.ToList();
                PawnPrefab = PawnPrefabs.FirstOrDefault(pair => pair.PlayerID == playerID).Prefab;

                if (!PawnPrefab)
                {
                    PawnPrefab = BConfig.Inst.PawnPrefab;
                }
            }

            IS_NOT_NULL(PawnPrefab);
            return PawnPrefab;
        }

        /* Initialization */
        private void FindPlayerSpawnPositionsInScene()
        {
            PlayersSpawnPositions.Clear();

            // Try to find already placed player spawn positions in the scene
            PawnSpawnPositionBase[] spawnPositions = FindObjectsOfType<PawnSpawnPositionBase>();
            foreach (PawnSpawnPositionBase spawnPosition in spawnPositions)
            {
                if (IS_KEY_NOT_CONTAINED(PlayersSpawnPositions, spawnPosition.PlayerID))
                {
                    PlayersSpawnPositions.Add(spawnPosition.PlayerID, spawnPosition);
                }
            }
        }

        #endregion
    }
}