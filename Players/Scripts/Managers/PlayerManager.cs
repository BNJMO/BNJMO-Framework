using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    [Serializable]
    public struct PlayerPrefab
    {
        public EPlayerID PlayerID; 
        public AbstractPawn Prefab;
    }

    public class PlayerManager : AbstractSingletonManager<PlayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Controller */
        public EPlayerID GetAssignedPlayerID(EControllerID controllerID)
        {
            if (IS_KEY_CONTAINED(ControllersMap, controllerID))
            {
                return ControllersMap[controllerID];
            }
            return EPlayerID.NONE;
        }

        public EControllerID GetAssignedControllerID(EPlayerID playerID)
        {
            if (IS_KEY_CONTAINED(PartyStatusMap, playerID))
            {
                return PartyStatusMap[playerID].ControllerID;
            }
            return EControllerID.NONE;
        }
       
        // TODO
        public void ChangePlayerController(EPlayerID playerID, EControllerID controllerID)
        {
        }

        /* Player */
        public List<EPlayerID> GetJoinedPlayers()
        {
            List<EPlayerID> result = new List<EPlayerID>();
            foreach (KeyValuePair<EPlayerID, PlayerJoinStatus> pair in PartyStatusMap)
            {
                PlayerJoinStatus playerJoinStatus = pair.Value;
                if (playerJoinStatus.HasJoined == true)
                {
                    result.Add(pair.Key);
                }
            }
            return result;
        }
        
        public string GetPlayerName(EPlayerID playerID)
        {
            if (IS_KEY_CONTAINED(PartyStatusMap, playerID))
            {
                return PartyStatusMap[playerID].PlayerName;
            }

            return "";
        }        
        
        public void SetPlayerName(EPlayerID playerID, string playerName)
        {
            if (IS_KEY_CONTAINED(PartyStatusMap, playerID))
            {
                PartyStatusMap[playerID].PlayerName = playerName;
            }
        }

        public EPlayerID[] GetPlayersInTeam(ETeamID inTeamID)
        {
            return Teams[inTeamID].ToArray();
        }

        public bool IsPlayerAlive(EPlayerID playerID)
        {
            return ActivePlayers.ContainsKey(playerID);
        }

        /* Party */
        public bool HasPlayerJoined(EPlayerID playerID)
        {
            foreach (KeyValuePair<EPlayerID, PlayerJoinStatus> pair in PartyStatusMap)
            {
                PlayerJoinStatus playerJoinStatus = pair.Value;
                if ((playerID == pair.Key)
                    && (playerJoinStatus.HasJoined == true))
                {
                    return true;
                }
            }
            return false;
        }

        public bool JoinPlayer(EPlayerID playerID, EControllerID controllerID, string playerName = "")
        {
            if (IS_KEY_CONTAINED(PartyStatusMap, playerID)
                && IS_KEY_CONTAINED(ControllersMap, controllerID)
                && IS_NOT_TRUE(PartyStatusMap[playerID].HasJoined))
            {
                PartyStatusMap[playerID].ControllerID = controllerID;
                PartyStatusMap[playerID].HasJoined = true;

                // Update assigned PlayerID in the controller map (instead of Spectator)
                ControllersMap[controllerID] = playerID;

                BEvents.PLAYERS_PlayerJoined.Invoke(new BEHandle<EPlayerID, EControllerID, string>
                    (playerID, controllerID, playerName));

                return true;
            }
            
            LogConsoleWarning($"Cannot join new player with controller {controllerID}");
            return false;
        }

        public EPlayerID JoinPlayer(EControllerID controllerID, string playerName = "")
        {
            EPlayerID playerID = GetNextNotJoinedPlayerID();

            if (IS_NOT_NONE(playerID)
                && IS_KEY_CONTAINED(PartyStatusMap, playerID)
                && IS_KEY_CONTAINED(ControllersMap, controllerID)
                && IS_NOT_TRUE(PartyStatusMap[playerID].HasJoined))
            {
                if (ControllersMap[controllerID] != EPlayerID.SPECTATOR)
                {
                    LogConsoleWarning($"Trying to join a new player with {controllerID} but it is already joined as {ControllersMap[controllerID]}");
                    return EPlayerID.NONE;
                }
                
                PartyStatusMap[playerID].ControllerID = controllerID;
                PartyStatusMap[playerID].HasJoined = true;

                // Update assigned PlayerID in the controller map (instead of Spectator)
                ControllersMap[controllerID] = playerID;

                BEvents.PLAYERS_PlayerJoined.Invoke(new BEHandle<EPlayerID, EControllerID, string>
                    (playerID, controllerID, playerName));
                
                return playerID;
            }

            LogConsoleWarning($"Cannot join new player with controller {controllerID}");
            return EPlayerID.NONE;
        }

        public bool LeavePlayer(EPlayerID playerID)
        {
            if ((IS_KEY_CONTAINED(PartyStatusMap, playerID))
            && (IS_TRUE(PartyStatusMap[playerID].HasJoined)))
            {
                // Destroy Player
                if (ActivePlayers.ContainsKey(playerID))
                {
                    DestroyPawn(playerID);
                }

                // Reset controller to Spectator
                EControllerID controllerID = PartyStatusMap[playerID].ControllerID;
                if (IS_KEY_CONTAINED(ControllersMap, controllerID))
                {
                    ControllersMap[controllerID] = EPlayerID.SPECTATOR;
                }

                // Reset entry of playerID in party status 
                PartyStatusMap[playerID] = new PlayerJoinStatus(EControllerID.NONE, "");

                // Trigger global event
                BEvents.PLAYERS_PlayerLeft.Invoke(new BEHandle<EPlayerID, EControllerID>(playerID, controllerID));

                // Are the rest of the joined players ready?
                CheckIfAllPlayersAreReady();

                return true;
            }
            
            LogConsoleWarning($"Cannot leave player {playerID} because it had not joined the party");
            return false;
        }

        public bool SetPlayerReady(EPlayerID playerID)
        {
            LogConsole(playerID + " ready");
            if ((IS_KEY_CONTAINED(PartyStatusMap, playerID))
            && (IS_TRUE(PartyStatusMap[playerID].HasJoined))
            && (IS_NOT_TRUE(PartyStatusMap[playerID].IsReady)))
            {
                PartyStatusMap[playerID].IsReady = true;

                BEvents.PLAYERS_PlayerReady.Invoke(new BEHandle<EPlayerID>(playerID));

                CheckIfAllPlayersAreReady();
                
                return true;
            }
            
            LogConsoleWarning($"Cannot set player ready for player {playerID}");
            return false;
        }

        public bool CancelPlayerReady(EPlayerID playerID)
        {
            if ((IS_KEY_CONTAINED(PartyStatusMap, playerID))
            && (IS_TRUE(PartyStatusMap[playerID].HasJoined))
            && (IS_TRUE(PartyStatusMap[playerID].IsReady)))
            {
                PartyStatusMap[playerID].IsReady = false;

                BEvents.PLAYERS_PlayerCanceledReady.Invoke(new BEHandle<EPlayerID>(playerID));
                
                return true;
            }

            LogConsoleWarning($"Cannot cancel player ready for player {playerID}");
            return false;
        }

        public bool AreAllPlayersReady()
        {
            bool partyIsEmpty = true;
            bool areAllReady = true;
            foreach (PlayerJoinStatus playerJoinStatus in PartyStatusMap.Values)
            {
                if (playerJoinStatus.HasJoined)
                {
                    partyIsEmpty = false;
                    
                    if (!playerJoinStatus.IsReady)
                    {
                        areAllReady = false;
                        break;
                    }
                }
   
            }
            return areAllReady && !partyIsEmpty;
        }
        
        /* Pawn */
        public AbstractPawn SpawnPawn(EPlayerID playerID)
        {
            AbstractPawn spawnedPawn = null;

            if (IS_TRUE(HasPlayerJoined(playerID))
                && IS_KEY_NOT_CONTAINED(ActivePlayers, playerID)
                && IS_KEY_CONTAINED(playersPrefabsMap, playerID))
            {
                AbstractPawn pawnPrefab = playersPrefabsMap[playerID];

                Vector3 playerPosition = Vector3.zero;
                Quaternion playerRotation = Quaternion.identity;
                AbstractPawnSpawnPosition pawnSpawnPosition = null;
                if (PlayersSpawnPositions.ContainsKey(playerID))
                {
                    pawnSpawnPosition = PlayersSpawnPositions[playerID];
                    playerPosition = pawnSpawnPosition.Position;
                    playerRotation = pawnSpawnPosition.Rotation;
                }
                else
                {
                    LogConsoleWarning("No spawn position defined for : " + playerID + ". Spawning at root");
                }

                spawnedPawn = Instantiate(pawnPrefab, playerPosition, playerRotation);
                spawnedPawn.PlayerID = playerID;
                spawnedPawn.TeamID = PlayersTeam[playerID];

                ActivePlayers.Add(playerID, spawnedPawn);

                // Place player under parent of SpawnPosition
                if (pawnSpawnPosition
                    && BManager.Instance.Config.SpawnPlayersUnderSameTransformAsSpawnPositions == true
                    && pawnSpawnPosition.transform.parent)
                {
                    spawnedPawn.transform.parent = pawnSpawnPosition.transform.parent;
                }

                BEvents.PLAYERS_PlayerSpawned.Invoke(new BEHandle<EPlayerID, IPlayer>(playerID, spawnedPawn));
            }
            return spawnedPawn;
        }

        public void SpawnAllJoinedPlayers()
        {
            LogConsole("Spawning remaining AI players");
            // Fill empty slots with AI
            int emptySlots = 4 - GetJoinedPlayers().Count;
            int remainingAI = BManager.Instance.Config.MaximumNumberOfAIToSpawn;
            while ((emptySlots > 0) && (remainingAI > 0))
            {
                EControllerID aIControllerID = BUtils.GetAIControllerIDFrom(BManager.Instance.Config.MaximumNumberOfAIToSpawn - remainingAI + 1);
                JoinNextAIPlayer(aIControllerID);
                emptySlots--;
                remainingAI--;
            }

            foreach (EPlayerID playerID in GetJoinedPlayers())
            {
                if (IS_KEY_NOT_CONTAINED(ActivePlayers, playerID))
                {
                    SpawnPawn(playerID);
                }
            }
        }

        public bool DestroyPawn(EPlayerID playerID)
        {
            if (IS_KEY_CONTAINED(ActivePlayers, playerID))
            {
                AbstractPawn pawn = ActivePlayers[playerID];
                if ((IS_KEY_CONTAINED(PlayersDeathPositions, playerID))
                    && (IS_KEY_CONTAINED(PlayersDeathRotations, playerID)))
                {
                    PlayersDeathPositions[playerID] = pawn.Position;
                    PlayersDeathRotations[playerID] = pawn.Rotation;
                }

                ActivePlayers.Remove(playerID);
                BEvents.PLAYERS_PlayerDied.Invoke(new BEHandle<EPlayerID, IPlayer>(playerID, pawn));
                pawn.DestroyPawn();

                return true;
            }
            
            LogConsoleWarning($"Cannot destroy player {playerID}");
            return false;
        }

        #endregion

        #region Inspector Variables

        
        #endregion

        #region Variables
        /// <summary> Positions in the scene (or around PlayerManager if not found) where the players will be spawned. </summary>
        public Dictionary<EPlayerID, AbstractPawnSpawnPosition> PlayersSpawnPositions { get; } = new Dictionary<EPlayerID, AbstractPawnSpawnPosition>();

        /// <summary> Added whenever a player has spawned. Removed when he dies. </summary>
        public Dictionary<EPlayerID, AbstractPawn> ActivePlayers { get; } = new Dictionary<EPlayerID, AbstractPawn>();

        /// <summary> All assigned players for every team. </summary>
        public Dictionary<ETeamID, List<EPlayerID>> Teams { get; } = new Dictionary<ETeamID, List<EPlayerID>>();

        /// <summary> Assigned team of every player </summary>
        public Dictionary<EPlayerID, ETeamID> PlayersTeam { get; } = new Dictionary<EPlayerID, ETeamID>();
        
        /// <summary> The join status of every possible player id (even of not assigned yet) </summary>
        public Dictionary<EPlayerID, PlayerJoinStatus> PartyStatusMap { get; } = new Dictionary<EPlayerID, PlayerJoinStatus>();
        public Dictionary<EControllerID, EPlayerID> ControllersMap { get; } = new Dictionary<EControllerID, EPlayerID>();
        // Still useful?
        public Dictionary<EPlayerID, Vector3> PlayersDeathPositions { get; } = new Dictionary<EPlayerID, Vector3>();
        public Dictionary<EPlayerID, Quaternion> PlayersDeathRotations { get; } = new Dictionary<EPlayerID, Quaternion>();
        
        private Dictionary<EPlayerID, AbstractPawn> playersPrefabsMap { get; } = new Dictionary<EPlayerID, AbstractPawn>();
        
        #endregion

        #region Life Cycle
        protected override void Awake()
        {
            base.Awake();

            LoadPlayerResources();
            ReinitializeDictionaries();
            ReinitializeControllersMap();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            // Input events
            BEvents.INPUT_ControllerConnected.Event += On_INPUT_ControllerConnected;
            BEvents.INPUT_ControllerDisconnected.Event += On_INPUT_ControllerDisconnected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Input events
            BEvents.INPUT_ControllerConnected.Event -= On_INPUT_ControllerConnected;
            BEvents.INPUT_ControllerDisconnected.Event -= On_INPUT_ControllerDisconnected;
        }

        protected override void Update()
        {
            base.Update();

            UpdatePartyDebugText();
        }

        protected override void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
            base.OnNewSceneReinitialize(newScene, lastScene);

            FindPlayerSpawnPositionsInScene();

            ReinitializeDictionaries();
        }
        #endregion

        #region Events Callbacks


        #endregion

        #region Party Join
        
        private EPlayerID GetNextNotJoinedPlayerID()
        {
            foreach (var pair in PartyStatusMap)
            {
                EPlayerID playerID = pair.Key;
                PlayerJoinStatus playerJoinStatus = pair.Value;

                if (playerJoinStatus.HasJoined == false)
                {
                    return playerID;
                }
            }
            return EPlayerID.NONE;
        }

        private EPlayerID JoinNextAIPlayer(EControllerID controllerID)
        {
            foreach (var pair in PartyStatusMap)
            {
                EPlayerID playerID = pair.Key;
                PlayerJoinStatus playerJoinStatus = pair.Value;

                if (playerJoinStatus.HasJoined == false)
                {
                    JoinPlayer(playerID, controllerID);
                    return playerID;
                }
            }
            return EPlayerID.NONE;
        }

        private void CheckIfAllPlayersAreReady()
        {
            bool areAllReady = false;
            foreach (PlayerJoinStatus playerJoinStatus in PartyStatusMap.Values)
            {
                if (playerJoinStatus.HasJoined == true)
                {
                    areAllReady = true;
                    if (playerJoinStatus.IsReady == false)
                    {
                        areAllReady = false;
                        break;
                    }
                }
            }
            if (areAllReady == true)
            {
                BEvents.PLAYERS_AllPlayersReady.Invoke(new BEHandle());
            }
        }
        
        #endregion

        #region Events Callbacks
        
        private void On_INPUT_ControllerConnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;

            // Add connected controller as a spectator
            if (IS_KEY_NOT_CONTAINED(ControllersMap, controllerID))
            {
                ControllersMap.Add(controllerID, EPlayerID.SPECTATOR);

                BEvents.PLAYERS_PlayerJoined.Invoke(new BEHandle<EPlayerID, EControllerID, string>
                    (EPlayerID.SPECTATOR, controllerID, ""));
            }
        }

        private void On_INPUT_ControllerDisconnected(BEHandle<EControllerID> eventHandle)
        {
            EControllerID controllerID = eventHandle.Arg1;

            if (IS_KEY_CONTAINED(ControllersMap, controllerID))
            {
                EPlayerID playerID = ControllersMap[controllerID];

                // Was a joined controller?
                if (playerID != EPlayerID.SPECTATOR
                    && IS_KEY_CONTAINED(PartyStatusMap, playerID))
                {
                    PartyStatusMap[playerID].Flush();
                }

                // Destroy Player
                if (ActivePlayers.ContainsKey(playerID))
                {
                    DestroyPawn(playerID);
                }

                ControllersMap.Remove(controllerID);

                BEvents.PLAYERS_PlayerLeft.Invoke(new BEHandle<EPlayerID, EControllerID>(playerID, controllerID));

            }
        }

        #endregion

        #region Initialization
        
        private void ReinitializeDictionaries()
        {
            ActivePlayers.Clear();

            Teams.Clear();
            Teams[ETeamID.TEAM_1] = new List<EPlayerID>();
            Teams[ETeamID.TEAM_2] = new List<EPlayerID>();
            Teams[ETeamID.TEAM_3] = new List<EPlayerID>();
            Teams[ETeamID.TEAM_4] = new List<EPlayerID>();

            PartyStatusMap.Clear();
            PlayersTeam.Clear();
            PlayersDeathPositions.Clear();
            PlayersDeathRotations.Clear();

            // Adding all players (Besides Spectator and None)
            foreach (EPlayerID playerID in Enum.GetValues(typeof(EPlayerID)))
            {
                if ((playerID != EPlayerID.NONE)
                    && (playerID != EPlayerID.SPECTATOR))
                {
                    PlayersTeam.Add(playerID, ETeamID.NONE);
                    PlayersDeathPositions.Add(playerID, Vector3.zero);
                    PlayersDeathRotations.Add(playerID, Quaternion.identity);
                    PartyStatusMap.Add(playerID, new PlayerJoinStatus(EControllerID.NONE, ""));
                }
            }
        }

        private void LoadPlayerResources()
        {
            playersPrefabsMap.Clear();

            foreach (PawnPrefabObject playerPrefabObject in BManager.Instance.Config.PawnPrefabObjects)
            {
                if ((IS_NULL(playerPrefabObject, true)
                    && IS_NOT_VALID(playerPrefabObject.pawnPrefab, true))
                        || IS_NONE(playerPrefabObject.PlayerID, true))
                {
                    continue;
                }
                
                AbstractPawn pawnPrefab = playerPrefabObject.pawnPrefab;
                if (IS_KEY_NOT_CONTAINED(playersPrefabsMap, playerPrefabObject.PlayerID)
                    && IS_NOT_NULL(pawnPrefab))
                {
                    playersPrefabsMap.Add(playerPrefabObject.PlayerID, pawnPrefab);
                }
            }

            IS_NOT_NULL(BManager.Instance.Config.PawnSpawnPositionPrefab);
        }

        private void FindPlayerSpawnPositionsInScene()
        {
            PlayersSpawnPositions.Clear();

            // Try to find already placed player spawn positions in the scene
            AbstractPawnSpawnPosition[] spawnPositions = FindObjectsOfType<AbstractPawnSpawnPosition>();
            foreach (AbstractPawnSpawnPosition spawnPosition in spawnPositions)
            {
                if (IS_KEY_NOT_CONTAINED(PlayersSpawnPositions, spawnPosition.PayerID))
                {
                    PlayersSpawnPositions.Add(spawnPosition.PayerID, spawnPosition);
                }
            }

            // Determine spawn positions relative to this transform if no PlayerSpawnPosition found in scene
            if (BManager.Instance.Config.IsSpawnGhostPlayerPositionsIfNotFound == true)
            {
                int angle;
                for (int i = 1; i < 5; i++)
                {
                    angle = 90 * i;
                    EPlayerID playerID = BUtils.GetPlayerIDFrom(i);
                    if (PlayersSpawnPositions.ContainsKey(playerID) == false)
                    {
                        AbstractPawnSpawnPosition spawnGhost = Instantiate(BManager.Instance.Config.PawnSpawnPositionPrefab);
                        spawnGhost.PayerID = playerID;
                        spawnGhost.Position = transform.position + Vector3.forward * 3.0f + Vector3.left * 3.0f;
                        spawnGhost.transform.RotateAround(transform.position, Vector3.up, angle);
                        spawnGhost.Rotation = transform.rotation;
                        PlayersSpawnPositions.Add(playerID, spawnGhost);
                    }
                }
            }
        }

        private void ReinitializeControllersMap()
        {
            List<EControllerID> keys = new List<EControllerID>();
            foreach (EControllerID key in ControllersMap.Keys)
            {
                keys.Add(key);

            }
            foreach (EControllerID controllerID in keys)
            {
                ControllersMap[controllerID] = EPlayerID.SPECTATOR;
            }
        }
        
        #endregion

        #region Debug
        
        private void UpdatePartyDebugText()
        {
            string playerStatusLog = "Party join status : \n";
            foreach (var pair in PartyStatusMap)
            {
                playerStatusLog += pair.Key + " : " + pair.Value.ControllerID + " - joined : " + pair.Value.HasJoined + " | is ready : " + pair.Value.IsReady + "\n";
            }
            LogCanvas(BConsts.DEBUGTEXT_JoinedPlayers, playerStatusLog);
        }
        
        #endregion

        #region Others

        private void AssignPlayerToTeam(EPlayerID playerID, ETeamID teamID)
        {
            // First remove from old team
            ETeamID oldTeamID = PlayersTeam[playerID];
            if (oldTeamID != ETeamID.NONE)
            {
                Teams[oldTeamID].Remove(playerID);
            }

            Teams[teamID].Add(playerID);
            PlayersTeam[playerID] = teamID;
        }

        private void RemovePlayerFromTeam(EPlayerID playerID, ETeamID teamID)
        {
            // TODO: Implement removing player from team
        }

        #endregion

    }
}