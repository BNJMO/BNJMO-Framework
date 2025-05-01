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

        /* Controller */
        public PlayerBase GetPlayer(EControllerID controllerID)
        {
            if (controllerID == EControllerID.NONE)
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
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BEvents.INPUT_ControllerConnected += BEvents_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected += BEvents_OnControllerDisconnected;
            BEvents.APP_SceneUpdated += BEvents_OnSceneUpdated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            BEvents.INPUT_ControllerConnected.Event -= BEvents_OnControllerConnected;
            BEvents.INPUT_ControllerDisconnected.Event -= BEvents_OnControllerDisconnected;
            BEvents.APP_SceneUpdated -= BEvents_OnSceneUpdated;
        }

        #endregion

        #region Events Callbacks
        
        private void BEvents_OnControllerConnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            if (IS_NOT_NULL(GetPlayer(controllerID), true))
                return;

            ESpectatorID spectatorID = GetNextFreeSpectatorID();
            SpawnPlayer(EPlayerID.NONE, spectatorID, controllerID);
        }

        private void BEvents_OnControllerDisconnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            DestroyPlayer(controllerID);
        }
        
        private void BEvents_OnSceneUpdated(BEHandle<SScene> beHandle)
        {
            FindPlayerSpawnPositionsInScene();
        }

        #endregion

        #region Others

        /* Spawn */
        protected virtual PlayerBase SpawnPlayer(EPlayerID playerID, ESpectatorID spectatorID,  
            EControllerID controllerID, ENetworkID networkID = ENetworkID.LOCAL)
        {
            if (playerID == EPlayerID.NONE
                && spectatorID == ESpectatorID.NONE)
            {
                LogConsoleError("Can't spawn a player! Both playerID and controllerID are None");
                return null;
            }
            
            if (spectatorID != ESpectatorID.NONE)
            {
                PlayerBase playerWithSameSpectatorID = GetPlayerInParty(playerID, false);
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
            
            PlayerBase spawnedPlayer = Instantiate(selectedPlayerPrefab, transform, true);
            spawnedPlayer.Init(new SPlayerInit
            {
                PlayerID = playerID,
                SpectatorID = spectatorID,
                ControllerID = controllerID,
                IsLocalPlayer = true,
                NetworkID = networkID,
                TeamID = ETeamID.NONE,
                PlayerName = "Player",
            });
            
            ConnectedPlayers.Add(spawnedPlayer);

            switch (spawnedPlayer.PartyState)
            {
                case EPlayerPartyState.IN_LOBBY:
                    PlayersInLobby.Add(spectatorID, spawnedPlayer);
                    break;
                
                case EPlayerPartyState.IN_PARTY:
                    PlayersInParty.Add(playerID, spawnedPlayer);
                    break;
            }
            
            BEvents.PLAYERS_PlayerConnected.Invoke(new BEHandle<PlayerBase>(spawnedPlayer));

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
            
            BEvents.PLAYERS_PlayerDisconnected.Invoke(new BEHandle<PlayerBase>(player));

            player.DestroyPlayer();
            return true;
        }

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

        #endregion
    }
}