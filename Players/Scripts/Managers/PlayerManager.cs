using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{

    public class PlayerManager : AbstractSingletonManager<PlayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Controller */
        public PlayerBase GetPlayer(EControllerID controllerID)
        {
            PlayerBase player = null;
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (playerItr.ControllerID == controllerID)
                {
                    player = playerItr;
                    break;
                }
            }
            IS_NOT_NULL(player);
            return player;
        }
       
        public bool CanChangeControllerID(EPlayerID playerID, EControllerID controllerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true)
                || IS_NULL(ActivePlayers[playerID], true))
                return false;
            
            PlayerBase player = ActivePlayers[playerID];
            if (ARE_ENUMS_EQUAL(playerID, player.PlayerID, true))
                return false;
            
            // Check first if any of the active players already has the controllerID assigned 
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (IS_NULL(playerItr, true))
                    continue;

                if (playerItr.ControllerID == controllerID)
                    return false;
            }
            
            return true;
        }

        /* Player */
        public PlayerBase GetPlayer(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true)
                || IS_NULL(ActivePlayers[playerID], true))
                return null;
            
            return ActivePlayers[playerID];;
        }
        
        public bool CanJoinTeam(EPlayerID playerID, ETeamID teamID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true)
                || IS_NULL(ActivePlayers[playerID], true))
                return false;
            
            if (teamID == ETeamID.NONE)
                return true;
            
            int numberOfPlayersInTeam = GetAllPlayersInTeam(teamID).Length;
            int maxNumberOfPlayersInTeam = BManager.Inst.Config.MaxPlayersInTeam;
            
            if (IS_GREATER_OR_EQUAL(numberOfPlayersInTeam, maxNumberOfPlayersInTeam, true))
                return false;
            
            return true;
        }
        
        public PlayerBase[] GetAllPlayersInTeam(ETeamID teamID)
        {
            List<PlayerBase> players = new();
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (playerItr.TeamID == teamID)
                {
                    players.Add(playerItr);
                    break;
                }
            }
            return players.ToArray();
        }
                
        public EPlayerID GetNextPlayerIDAvailable()
        {
            int maxPlayerID = 0;
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                maxPlayerID = Mathf.Max(maxPlayerID, (int) playerItr.PlayerID);
            }
            
            int maxPlayersInParty = BManager.Inst.Config.MaxPlayersInParty;
            if (IS_GREATER(maxPlayerID, maxPlayersInParty, true))
                return EPlayerID.NONE;
            
            return (EPlayerID) (maxPlayerID + 1);
        }

        /* Party */
        public PlayerBase[] GetAllPlayersInTheLobby()
        {
            List<PlayerBase> players = new();
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (IS_NULL(playerItr, true))
                    continue;
                
                if (playerItr.PlayerState == EPlayerState.IN_LOBBY)
                {
                    players.Add(playerItr);
                    break;
                }
            }
            return players.ToArray();
        }
        
        public List<PlayerBase> GetAllPlayersInTheParty()
        {
            List<PlayerBase> result = new();
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (IS_NULL(playerItr, true))
                    continue;

                if (playerItr.PlayerState == EPlayerState.IN_PARTY)
                {
                    result.Add(playerItr);
                }
            }
            return result;
        }
   
        public bool AreAllPlayersReady()
        {
            foreach (PlayerBase playerItr in ActivePlayers.Values)
            {
                if (IS_NULL(playerItr, true))
                    continue;
                
                if (playerItr.PlayerState == EPlayerState.IN_PARTY
                    && playerItr.IsReady == false)
                    return false;
            }
            return true;
        }
        
        /* Pawn */
        public PawnBase SpawnPawn(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true)
                || IS_KEY_CONTAINED(ActivePawns, playerID, true))
                return null;

            PlayerBase player = ActivePlayers[playerID];
            if (IS_NULL(player, true))
                return null;

            PawnBase selectedPawnPrefab;
            if (BManager.Inst.Config.UseSamePrefabForAllPawns)
            {
                selectedPawnPrefab = pawnPrefab;
            }
            else
            {
                if (IS_KEY_NOT_CONTAINED(pawnPrefabsMap, playerID, true))
                    return null;
                
                selectedPawnPrefab = pawnPrefabsMap[playerID];
            }
            
            if (IS_KEY_NOT_CONTAINED(PlayersSpawnPositions, playerID, true))
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

        public bool DestroyPawn(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true)
                || IS_KEY_NOT_CONTAINED(ActivePawns, playerID, true))
                return false;
            
            PawnBase pawn = ActivePawns[playerID];
            pawn.DestroyPawn();
            ActivePawns.Remove(playerID);
            
            BEvents.PAWNS_PawnDestroyed.Invoke(new(pawn));
            
            return true;
        }
        
        public PawnBase RespawnPawn(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true))
                return null;

            if (ActivePawns.ContainsKey(playerID))
            {
                DestroyPawn(playerID);
            }
            
            return SpawnPawn(playerID);
        }

        public void SpawnAllPawnsFromPlayersInParty()
        {
            foreach (PlayerBase player in ActivePlayers.Values)
            {
                if (player.PlayerState != EPlayerState.IN_PARTY)
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

        /// <summary> Added whenever a player has spawned. Removed when he leaves. </summary>
        public Dictionary<EPlayerID, PlayerBase> ActivePlayers { get; } = new();

        /// <summary> Added whenever a pawn has spawned. Removed when he gets destroyed. </summary>
        public Dictionary<EPlayerID, PawnBase> ActivePawns { get; } = new();

        private PlayerBase playerPrefab;
        private Dictionary<EPlayerID, PlayerBase> playerPrefabsMap { get; } = new();
        private PawnBase pawnPrefab;
        private Dictionary<EPlayerID, PawnBase> pawnPrefabsMap { get; } = new();
        
        #endregion

        #region Life Cycle
  
        protected override void OnEnable()
        {
            base.OnEnable();

            /* Input events */
            BEvents.INPUT_ControllerConnected += On_INPUT_ControllerConnected;
            BEvents.INPUT_ControllerDisconnected += On_INPUT_ControllerDisconnected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Input events
            BEvents.INPUT_ControllerConnected.Event -= On_INPUT_ControllerConnected;
            BEvents.INPUT_ControllerDisconnected.Event -= On_INPUT_ControllerDisconnected;
        }

        protected override void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
            base.OnNewSceneReinitialize(newScene, lastScene);

            FindPlayerSpawnPositionsInScene();
        }
        #endregion

        #region Events Callbacks
        
        private void On_INPUT_ControllerConnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            if (IS_NOT_NULL(GetPlayer(controllerID)))
                return; 
            
            SpawnPlayer(EPlayerID.SPECTATOR, controllerID);
        }

        private void On_INPUT_ControllerDisconnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;
            PlayerBase player = GetPlayer(controllerID);
            if (IS_NULL(player, true))
                return;
            
            DestroyPlayer(player.PlayerID);
        }

        #endregion

        #region Initialization

        
        #endregion

        #region Others

        /* Spawn */
        protected virtual PlayerBase SpawnPlayer(EPlayerID playerID, EControllerID controllerID, ENetworkID networkID = ENetworkID.LOCAL)
        {
            if (IS_KEY_CONTAINED(ActivePlayers, playerID, true))
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
            
            PlayerBase spawnedPlayer = Instantiate(selectedPlayerPrefab, transform, true);
            spawnedPlayer.gameObject.name = playerID.ToString();
            spawnedPlayer.Init(new SPlayerInit
            {
                PlayerID = playerID,
                ControllerID = controllerID,
                NetworkID = networkID,
                TeamID = ETeamID.NONE,
                PlayerName = playerID.ToString(),
            });
            ActivePlayers.Add(playerID, spawnedPlayer);
            
            BEvents.PLAYERS_PlayerConnected.Invoke(new BEHandle<PlayerBase>(spawnedPlayer));

            return spawnedPlayer;
        }
        
        protected virtual bool DestroyPlayer(EPlayerID playerID)
        {
            if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID, true))
                return false;

            if (ActivePawns.ContainsKey(playerID))
            {
                DestroyPawn(playerID);
            }

            PlayerBase player = ActivePlayers[playerID];
            ActivePlayers.Remove(playerID);
            
            BEvents.PLAYERS_PlayerDisconnected.Invoke(new BEHandle<PlayerBase>(player));

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