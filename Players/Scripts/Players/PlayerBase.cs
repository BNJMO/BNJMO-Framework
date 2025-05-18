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
            networkID = playerInit.NetworkID;
            teamID = playerInit.TeamID;
            playerName = playerInit.PlayerName;
            UpdateObjectNameOnPartyStateChange(false);
        }
        
        public void SetPlayerName(string newPlayerName, bool invokeBEvent = true)
        {
            playerName = newPlayerName;
            name = playerName;

            if (invokeBEvent)
            {
                BEvents.PLAYERS_PlayerChangedName.Invoke(new(this));
            }
        }
        
        public bool SetControllerID(EControllerID newControllerID)
        {
            if (ARE_ENUMS_EQUAL(ControllerID, newControllerID, true)
                || BPlayerManager.Inst.IsControllerIDAvailable(newControllerID) == false)
                return false;
            
            controllerID = newControllerID;
            
            BEvents.PLAYERS_PlayerChangedControllerID.Invoke(new(this));
            
            return true;
        }

        public bool SetTeamID(ETeamID newTeamID)
        {
            if (ARE_ENUMS_EQUAL(newTeamID, teamID, true)
                || ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                || BPlayerManager.Inst.CanJoinTeam(newTeamID) == false)
                return false;
            
            teamID = newTeamID;

            BEvents.PLAYERS_PlayerChangedTeam.Invoke(new(this));

            return true;
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
            
            BEvents.PLAYERS_PlayerJoinedTheParty.Invoke(new(this));

            UpdateObjectNameOnPartyStateChange();
            
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
            
            BEvents.PLAYERS_PlayerLeftTheParty.Invoke(new(this));

            UpdateObjectNameOnPartyStateChange();
            
            return true;
        }

        public bool SetReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                 || ARE_EQUAL(isReady, true, true))
                return false;
            
            isReady = true;
            
            BEvents.PLAYERS_PlayerBecameReady.Invoke(new (this));
            
            return true;
        }
        
        public bool CancelReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.IN_PARTY, true)
                || ARE_EQUAL(isReady, false, true))
                return false;
            
            isReady = false;
            
            BEvents.PLAYERS_PlayerCanceledReady.Invoke(new (this));
            
            return true;
        }

        public bool SpawnPawn()
        {
            return BPlayerManager.Inst.SpawnPawn(PlayerID);
        }
        
        public bool DestroyPawn()
        {
            return BPlayerManager.Inst.DestroyPawn(PlayerID);
        }

        public void DestroyPlayer()
        {
            Destroy(gameObject);
        }

        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private EPlayerID playerID;
        [SerializeField] [ReadOnly] private ESpectatorID spectatorID;
        [SerializeField] [ReadOnly] private EControllerID controllerID;
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
        
        public ENetworkID NetworkID => networkID;

        public bool IsLocalPlayer
        {
            get
            {
                if (networkID == ENetworkID.LOCAL)
                    return true;

                if (BMultiplayerManager.Inst
                    && BMultiplayerManager.Inst.MultiplayerHandler)
                {
                    return BMultiplayerManager.Inst.MultiplayerHandler.LocalNetworkID == networkID;
                }

                return true;
            }
        }
        
        public bool IsAI => BUtils.IsControllerIDAI(ControllerID);

        public bool IsRemote => BUtils.IsControllerIDRemote(ControllerID); 

        public ETeamID TeamID => teamID;
        
        public string PlayerName => playerName;
        
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

        protected virtual void UpdateObjectNameOnPartyStateChange(bool invokeBEvent = true)
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
