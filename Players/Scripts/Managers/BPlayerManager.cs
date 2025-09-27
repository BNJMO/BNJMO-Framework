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
        public PlayerBase GetPlayerFromControllerID(EControllerID controllerID, bool logWarnings = true)
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
            foreach (PlayerBase playerItr in ConnectedPlayers)
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

        public PlayerBase[] GetAllSpectators()
        {
            return ConnectedPlayers
                .Where(playerItr => playerItr.SpectatorID != ESpectatorID.NONE)
                .ToArray();
        }
        
        public ESpectatorID[] GetAllConnectedSpectatorIDs()
        {
            return ConnectedPlayers
                .Where(playerItr => playerItr.SpectatorID != ESpectatorID.NONE)
                .Select(playerItr => playerItr.SpectatorID)
                .ToArray();
        }
        
        public PlayerBase[] GetAllActivePlayers()
        {
           return ConnectedPlayers
               .Where(playerItr => playerItr.PlayerID != EPlayerID.NONE)
               .ToArray();
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
            if (allActivePlayers.Length < 1)
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
            if (IS_KEY_CONTAINED(ActivePawnMap, playerID, logWarnings))
                return null;
            
            PlayerBase player = GetActivePlayer(playerID);
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
            
            ActivePawnMap.Add(playerID, spawnedPawn);
            
            BEvents.PAWNS_Spawned.Invoke(new(spawnedPawn));
            
            return spawnedPawn;
        }

        public bool DestroyPawn(EPlayerID playerID, bool logWarnings = true)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePawnMap, playerID, logWarnings))
                return false;
            
            PawnBase pawn = ActivePawnMap[playerID];
            pawn.DestroyPawn();
            ActivePawnMap.Remove(playerID);
            
            BEvents.PAWNS_Destroyed.Invoke(new(playerID));
            
            return true;
        }
        
        public PawnBase RespawnPawn(EPlayerID playerID, bool logWarnings = true)
        {
            if (ActivePawnMap.ContainsKey(playerID))
            {
                DestroyPawn(playerID, logWarnings);
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

        public PawnBase GetPawn(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePawnMap, playerID, true))
                return null;
            
            return ActivePawnMap[playerID];
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

        /// <summary> Added whenever a pawn has spawned. Removed when he gets destroyed. </summary>
        public Dictionary<EPlayerID, PawnBase> ActivePawnMap { get; } = new();

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
        private void BEvents_APP_OnSceneUpdated(BEventHandle<SScene> bEventHandle)
        {
            FindPlayerSpawnPositionsInScene();
        }

        /* Input */
        private void BEvents_INPUT_OnControllerConnected(BEventHandle<EControllerID, EControllerType> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            if (IS_NOT_NULL(GetPlayerFromControllerID(controllerID), true))
                return;

            if (BUtils.IsControllerIDRemote(controllerID))
                return;

            ESpectatorID spectatorID = GetNextFreeSpectatorID();
            EControllerType controllerType = eventHandle.Arg2;
            SpawnPlayer(EPlayerID.NONE, spectatorID, controllerID, controllerType);
        }

        private void BEvents_INPUT_OnControllerDisconnected(BEventHandle<EControllerID, EControllerType> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            DestroyPlayer(controllerID);
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
            ENetworkID leftNetworkID = handle.Arg1;
            if (leftNetworkID == BOnlineManager.Inst.LocalNetworkID)
                return;
            
            for (int i = ConnectedPlayers.Count - 1; i >= 0; i--)
            {
                var playerItr = ConnectedPlayers[i];
                if (playerItr.NetworkID != leftNetworkID)
                    continue;

                DestroyPlayer(playerItr.ControllerID);
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

            
            PlayerBase player = GetPlayerFromControllerID(controllerID);
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
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleError("Can't spawn a player! Both playerID and controllerID are None");
                return null;
            }
            
            if (spectatorID != ESpectatorID.NONE)
            {
                PlayerBase playerWithSameSpectatorID = GetSpectator(spectatorID, false);
                if (IS_NOT_NULL(playerWithSameSpectatorID, true))
                    return null;
            }
            
            if (playerID != EPlayerID.NONE)
            {
                PlayerBase playerWithSamePlayerID = GetActivePlayer(playerID, false);
                if (IS_NOT_NULL(playerWithSamePlayerID, true))
                    return null;
            }
            
            PlayerBase playerWithSameControllerID = GetPlayerFromControllerID(controllerID);
            if (IS_NOT_NULL(playerWithSameControllerID, true))
                return null;

            if (networkID == ENetworkID.LOCAL)
            {
                networkID = BOnlineManager.Inst.LocalNetworkID;
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

        protected virtual bool DestroyPlayer(EControllerID controllerID)
        {
            PlayerBase player = GetPlayerFromControllerID(controllerID);
            if (IS_NULL(player, true))
                return false;

            if (ActivePawnMap.ContainsKey(player.PlayerID))
            {
                DestroyPawn(player.PlayerID);
            }
            
            ConnectedPlayers.Remove(player);
            
            BEvents.PLAYERS_Disconnected.Invoke(new (player));

            player.DestroyPlayer();
            return true;
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

        #endregion
    }
}