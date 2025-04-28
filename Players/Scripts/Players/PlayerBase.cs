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
            controllerID = playerInit.ControllerID;
            networkID = playerInit.NetworkID;
            teamID = playerInit.TeamID;
            playerName = playerInit.PlayerName;
        }
        
        public void SetPlayerName(string newPlayerName)
        {
            playerName = newPlayerName;
            
            BEvents.PLAYERS_PlayerChangedName.Invoke(new(this));
        }
        
        public bool SetControllerID(EControllerID newControllerID)
        {
            if (ARE_ENUMS_EQUAL(ControllerID, newControllerID, true)
                || PlayerManager.Inst.CanChangeControllerID(playerID, newControllerID) == false)
                return false;
            
            controllerID = newControllerID;
            
            BEvents.PLAYERS_PlayerChangedControllerID.Invoke(new(this));
            
            return true;
        }

        public bool SetTeamID(ETeamID newTeamID)
        {
            if (ARE_ENUMS_EQUAL(newTeamID, teamID)
                || PlayerManager.Inst.CanJoinTeam(playerID, newTeamID) == false)
                return false;
            
            teamID = newTeamID;

            BEvents.PLAYERS_PlayerChangedTeam.Invoke(new(this));

            return true;
        }
        
        public bool JoinParty()
        {
            if (ARE_ENUMS_EQUAL(PlayerState, EPlayerState.IN_PARTY, true))
                return false;
            
            playerID = PlayerManager.Inst.GetNextPlayerIDAvailable();
            if (ARE_ENUMS_EQUAL(playerID, EPlayerID.NONE, true))
                return false;
            
            BEvents.PLAYERS_PlayerJoinedTheParty.Invoke(new(this));
            
            return true;
        }

        public bool LeaveParty()
        {
            if (ARE_ENUMS_NOT_EQUAL(PlayerState, EPlayerState.IN_PARTY, true))
                return false;
            
            playerID = EPlayerID.SPECTATOR;

            BEvents.PLAYERS_PlayerLeftTheParty.Invoke(new(this));

            return true;
        }

        public bool SetReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PlayerState, EPlayerState.IN_PARTY, true)
                 || ARE_EQUAL(isReady, true, true))
                return false;
            
            isReady = true;
            
            BEvents.PLAYERS_PlayerBecameReady.Invoke(new (this));
            
            return true;
        }
        
        public bool CancelReady()
        {
            if (ARE_ENUMS_NOT_EQUAL(PlayerState, EPlayerState.IN_PARTY, true)
                || ARE_EQUAL(isReady, false, true))
                return false;
            
            isReady = false;
            
            BEvents.PLAYERS_PlayerCanceledReady.Invoke(new (this));
            
            return true;
        }

        public bool SpawnPawn()
        {
            return PlayerManager.Inst.SpawnPawn(PlayerID);
        }
        
        public bool DestroyPawn()
        {
            return PlayerManager.Inst.DestroyPawn(PlayerID);
        }

        public void DestroyPlayer()
        {
            Destroy(gameObject);
        }

        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private EPlayerID playerID;
        [SerializeField] [ReadOnly] private EControllerID controllerID;
        [SerializeField] [ReadOnly] private ENetworkID networkID;
        [SerializeField] [ReadOnly] private ETeamID teamID;
        [SerializeField] [ReadOnly] private string playerName;
        [SerializeField] [ReadOnly] private bool hasJoinedParty;
        [SerializeField] [ReadOnly] private bool isReady;

        #endregion

        #region Variables


        public EPlayerID PlayerID => playerID;
        
        public EControllerID ControllerID => controllerID;
        
        public ENetworkID NetworkID => networkID;
        
        public ETeamID TeamID => teamID;
        
        public string PlayerName => playerName;
        
        public EPlayerState PlayerState
        {
            get
            {
                if (PlayerID == EPlayerID.NONE)
                {
                    return EPlayerState.NONE;
                }
                if (PlayerID == EPlayerID.SPECTATOR)
                {
                    return EPlayerState.IN_LOBBY;
                }
                {
                    return EPlayerState.IN_PARTY;
                }
            }
        }

        public bool IsReady => isReady;

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion


    }
}
