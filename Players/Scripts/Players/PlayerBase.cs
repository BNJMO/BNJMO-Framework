using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    public class PlayerBase : BBehaviour, IPlayer
    {
        #region Public Events


        #endregion

        #region Public Methods
        
        public void Init(SPlayerInit playerInit)
        {
            playerID = playerInit.PlayerID;
            spectatorID = playerInit.SpectatorID;
            controllerID = playerInit.ControllerID;
            controllerType = playerInit.ControllerType;
            networkID = playerInit.NetworkID;
            teamID = playerInit.TeamID;
            playerName = playerInit.PlayerName;
            PlayerPicture = playerInit.PlayerPicture;
            UpdateObjectNameToPartyState(false);
        }

        public bool SetPlayerID(EPlayerID newPlayerID)
        {
            if (PlayerID == newPlayerID)
                return false;

            playerID = newPlayerID;
            UpdateObjectNameToPartyState();

            return true;
        }
        
        public bool SetSpectatorID(ESpectatorID newSpectatorID)
        {
            if (SpectatorID == newSpectatorID)
                return false;

            spectatorID = newSpectatorID;
            UpdateObjectNameToPartyState();

            return true;
        }

        public bool SetControllerID(EControllerID newControllerID, EControllerType newControllerType)
        {
            if (ControllerID == newControllerID
                || BPlayerManager.Inst.IsControllerIDAvailable(newControllerID) == false)
                return false;
            
            controllerID = newControllerID;
            controllerType = newControllerType;

            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerChangedControllerID.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            
            return true;
        }

        public bool SetTeamID(ETeamID newTeamID)
        {
            if (newTeamID == teamID
                || ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                || BPlayerManager.Inst.CanJoinTeam(newTeamID) == false)
                return false;
            
            teamID = newTeamID;

            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerChangedTeam.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);

            return true;
        }

        public bool SetNetworkID(ENetworkID newNetworkID)
        {
            if (NetworkID == newNetworkID)
                return false;
            
            networkID = newNetworkID;

            return true;
        }
               
        public void SetPlayerName(string newPlayerName, bool invokeBEvent = true)
        {
            playerName = newPlayerName;
            name = playerName;

            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_PlayerChangedName.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }
        }

        public void SetPlayerPicture(Sprite newPlayerPicture)
        {
            PlayerPicture = newPlayerPicture;

            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerChangedPicture.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
        }
        
        public bool JoinParty()
        {
            if (ARE_ENUMS_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true))
                return false;
            
            EPlayerID newPlayerID = BPlayerManager.Inst.JoinParty(this);
            if (ARE_ENUMS_EQUAL(newPlayerID, EPlayerID.NONE, true))
                return false;

            playerID = newPlayerID;
            spectatorID = ESpectatorID.NONE;
            
            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerJoinedTheParty.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);

            UpdateObjectNameToPartyState();
            
            return true;
        }

        public bool LeaveParty()
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true))
                return false;

            ESpectatorID newSpectatorID = BPlayerManager.Inst.LeaveParty(this);
            if (ARE_ENUMS_EQUAL(newSpectatorID, ESpectatorID.NONE, true))
                return false;
            
            playerID = EPlayerID.NONE;
            spectatorID = newSpectatorID;
            
            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerLeftTheParty.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);

            UpdateObjectNameToPartyState();
            
            return true;
        }

        public bool SetReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                 || ARE_EQUAL(isReady, true, true))
                return false;
            
            isReady = true;
            
            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerBecameReady.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_ALL);
            
            return true;
        }
        
        public bool CancelReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                || ARE_EQUAL(isReady, false, true))
                return false;
            
            isReady = false;
            
            SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
            BEvents.PLAYERS_PlayerCanceledReady.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_ALL);
            
            return true;
        }

        public bool SpawnPawn()
        {
            return BPlayerManager.Inst.SpawnPawn(PlayerID);
        }
        
        public bool DestroyPawn(bool logWarnings = false)
        {
            return BPlayerManager.Inst.DestroyPawn(PlayerID, logWarnings);
        }

        public void DestroyPlayer()
        {
            DestroyPawn(false);
            Destroy(gameObject);
        }

        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private EPlayerID playerID;
        [SerializeField] [ReadOnly] private ESpectatorID spectatorID;
        [SerializeField] [ReadOnly] private EControllerID controllerID;
        [SerializeField] [ReadOnly] private EControllerType controllerType;
        [SerializeField] [ReadOnly] private ENetworkID networkID;
        [SerializeField] [ReadOnly] private bool isLocalPlayer;
        [SerializeField] [ReadOnly] private ETeamID teamID;
        [SerializeField] [ReadOnly] private string playerName;
        [SerializeField] [ReadOnly] private bool isReady;

        #endregion

        #region Variables


        public EPlayerID PlayerID => playerID;

        public ESpectatorID SpectatorID => spectatorID;

        public EControllerID ControllerID => controllerID;
        
        public EControllerType ControllerType => controllerType;
        
        public ENetworkID NetworkID => networkID;

        public bool IsLocalPlayer
        {
            get
            {
                if (networkID == ENetworkID.LOCAL)
                    return true;

                if (BOnlineManager.Inst
                    && BOnlineManager.Inst.OnlineHandler)
                {
                    return BOnlineManager.Inst.OnlineHandler.LocalNetworkID == networkID;
                }

                return true;
            }
        }
        
        public bool IsAI => BUtils.IsControllerIDAI(ControllerID);

        public bool IsRemote => BUtils.IsControllerIDRemote(ControllerID); 

        public ETeamID TeamID => teamID;
        
        public string PlayerName => playerName;
        
        public Sprite PlayerPicture { get; protected set; }

        public EPlayerPartyState PartyState
        {
            get
            {
                if (PlayerID == EPlayerID.NONE
                    && SpectatorID != ESpectatorID.NONE)
                {
                    return EPlayerPartyState.IN_LOBBY;
                }
                
                if (PlayerID != EPlayerID.NONE
                         && SpectatorID == ESpectatorID.NONE)
                {
                    return EPlayerPartyState.IN_PARTY;
                }
  
                return EPlayerPartyState.NONE;
            }
        }

        public bool IsReady => isReady;

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        protected virtual void UpdateObjectNameToPartyState(bool invokeBEvent = true)
        {
            if (BManager.Inst.Config.MatchPlayerNameToPartyState == false)
                return;
            
            switch (PartyState)
            {
                case EPlayerPartyState.IN_LOBBY:
                    SetPlayerName(spectatorID.ToString(), invokeBEvent); 
                    break;
                
                case EPlayerPartyState.IN_PARTY:
                    SetPlayerName(playerID.ToString(), invokeBEvent);
                    break;
            }
        }

        #endregion


    }
}
