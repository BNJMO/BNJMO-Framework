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
        
        public virtual void Init(SPlayerInitArg playerInitArg)
        {
            playerID = playerInitArg.PlayerID;
            spectatorID = playerInitArg.SpectatorID;
            controllerID = playerInitArg.ControllerID;
            controllerType = playerInitArg.ControllerType;
            networkID = playerInitArg.NetworkID;
            teamID = playerInitArg.TeamID;
            playerName = playerInitArg.PlayerName;
            PlayerPicture = playerInitArg.PlayerPicture;
            UpdateObjectNameToPartyState(false);
        }

        public virtual bool SetPlayerID(EPlayerID newPlayerID)
        {
            if (PlayerID == newPlayerID)
                return false;

            if (PartyState == EPlayerPartyState.SPECTATOR)
            {
                JoinPartyAtPlayerID(newPlayerID);
            }
            else
            {
                playerID = newPlayerID;
                var playerReplicationArg =  BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_PlayerIDChanged.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            UpdateObjectNameToPartyState();

            return true;
        }
        
        public virtual bool SetSpectatorID(ESpectatorID newSpectatorID)
        {
            if (SpectatorID == newSpectatorID)
                return false;

            if (PartyState == EPlayerPartyState.ACTIVE_PLAYER)
            {
                LeavePartyAtSpectatorID(newSpectatorID);
            }
            else
            {
                spectatorID = newSpectatorID;
                var playerReplicationArg =  BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_SpectatorIDChanged.Invoke(new (playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            UpdateObjectNameToPartyState();

            return true;
        }

        public virtual bool SetControllerID(EControllerID newControllerID, EControllerType newControllerType, bool invokeBEvent = true)
        {
            if (ControllerID == newControllerID
                || BPlayerManager.Inst.IsAnyPlayerWithControllerID(newControllerID))
                return false;
            
            controllerID = newControllerID;
            controllerType = newControllerType;

            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_ControllerIDChanged.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            return true;
        }

        public virtual bool SetTeamID(ETeamID newTeamID, bool invokeBEvent = true)
        {
            if (newTeamID == teamID
                || ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.ACTIVE_PLAYER, true)
                || BPlayerManager.Inst.CanJoinTeam(newTeamID) == false)
                return false;
            
            teamID = newTeamID;
            
            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_TeamChanged.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            return true;
        }

        public virtual bool SetNetworkID(ENetworkID newNetworkID)
        {
            if (NetworkID == newNetworkID)
                return false;
            
            networkID = newNetworkID;

            return true;
        }
               
        public virtual void SetPlayerName(string newPlayerName, bool invokeBEvent = true)
        {
            playerName = newPlayerName;
            name = playerName;

            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_NameChanged.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }
        }

        public virtual void SetPlayerPicture(Sprite newPlayerPicture, bool invokeBEvent = true)
        {
            PlayerPicture = newPlayerPicture;

            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_PictureChanged.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }
        }
  
        public virtual bool JoinPartyAtPlayerID(EPlayerID atPlayerID, bool invokeBEvent = true)
        {
            if (ARE_ENUMS_EQUAL(PartyState, EPlayerPartyState.ACTIVE_PLAYER, true)
                || IS_NONE(atPlayerID, true))
                return false;

            if (IS_NOT_TRUE(BPlayerManager.Inst.IsPlayerIDAvailable(atPlayerID), true))
                return false;

            ESpectatorID oldSpectatorID = spectatorID;
            playerID = atPlayerID;
            spectatorID = ESpectatorID.NONE;
            
            if (invokeBEvent)
            {
                SPlayerJoinedPartyArg playerJoinedPartyArg = new()
                {
                    ControllerID = controllerID,
                    NewPlayerID = playerID,
                    OldSpectatorID = oldSpectatorID,
                };
                BEvents.PLAYERS_JoinedParty.Invoke(new(playerJoinedPartyArg), BEventBroadcastType.TO_ALL);
            }

            UpdateObjectNameToPartyState();
            
            return true;
        }
        
        public virtual EPlayerID JoinParty(bool invokeBEvent = true)
        {
            EPlayerID newPlayerID = BPlayerManager.Inst.GetNextFreePlayerID();
            bool joinPartySuccessful = JoinPartyAtPlayerID(newPlayerID, invokeBEvent);
            return joinPartySuccessful ? newPlayerID : EPlayerID.NONE;
        }

        public virtual bool LeavePartyAtSpectatorID(ESpectatorID atSpectatorID, bool invokeBEvent = true)
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.ACTIVE_PLAYER, true)
                || IS_NONE(atSpectatorID, true))
                return false;
            
            if (IS_NOT_TRUE(BPlayerManager.Inst.IsSpectatorIDAvailable(atSpectatorID), true))
                return false;

            EPlayerID oldPlayerID = playerID;
            playerID = EPlayerID.NONE;
            spectatorID = atSpectatorID;

            if (invokeBEvent)
            {
                SPlayerLeftPartyArg playerLeftPartyArg = new()
                {
                    NewSpectatorID = spectatorID,
                    OldPlayerID = oldPlayerID,
                };
                BEvents.PLAYERS_LeftParty.Invoke(new(playerLeftPartyArg), BEventBroadcastType.TO_ALL);
            }

            UpdateObjectNameToPartyState();
            
            return true;
        }

        public virtual ESpectatorID LeaveParty(bool invokeBEvent = true)
        {
            ESpectatorID newSpectatorID = BPlayerManager.Inst.GetNextFreeSpectatorID();
            var leavePartySuccessful = LeavePartyAtSpectatorID(newSpectatorID);
            return leavePartySuccessful ? newSpectatorID : ESpectatorID.NONE;
        }

        public virtual bool SetReady(bool invokeBEvent = true)
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.ACTIVE_PLAYER, true)
                 || ARE_EQUAL(isReady, true, true))
                return false;
            
            isReady = true;
            
            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_BecameReady.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            return true;
        }
        
        public virtual bool CancelReady(bool invokeBEvent = true)
        {
            if (ARE_ENUMS_NOT_EQUAL(PartyState, EPlayerPartyState.ACTIVE_PLAYER, true)
                || ARE_EQUAL(isReady, false, true))
                return false;
            
            isReady = false;

            if (invokeBEvent)
            {
                SPlayerReplicationArg playerReplicationArg = BUtils.CreatePlayerReplicationArgFromPlayer(this);
                BEvents.PLAYERS_CanceledReady.Invoke(new(playerReplicationArg), BEventBroadcastType.TO_ALL);
            }

            return true;
        }

        public virtual bool SpawnPawn()
        {
            return BPlayerManager.Inst.SpawnPawn(PlayerID);
        }
        
        public virtual bool DestroyPawn(bool logWarnings = false)
        {
            return BPlayerManager.Inst.DestroyPawn(PlayerID, logWarnings);
        }

        public virtual void DestroyPlayer()
        {
            BEvents.PLAYERS_Disconnected.Invoke(new (this));
            DestroyPawn(false);
            Destroy(gameObject);
        }

        #endregion

        #region Inspector Variables

        [Title("BNJMO")]
        
        [SerializeField, ReadOnly] 
        private EPlayerID playerID;
        
        [SerializeField, ReadOnly] 
        private ESpectatorID spectatorID;
        
        [SerializeField, ReadOnly] 
        private EControllerID controllerID;
        
        [SerializeField, ReadOnly] 
        private EControllerType controllerType;
        
        [SerializeField, ReadOnly] 
        private ENetworkID networkID;
        
        [SerializeField, ReadOnly] 
        private bool isLocalPlayer;
        
        [SerializeField, ReadOnly] 
        private ETeamID teamID;
        
        [SerializeField, ReadOnly] 
        private string playerName;
        
        [SerializeField, ReadOnly] 
        private bool isReady;

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
                    return EPlayerPartyState.SPECTATOR;
                }
                
                if (PlayerID != EPlayerID.NONE
                         && SpectatorID == ESpectatorID.NONE)
                {
                    return EPlayerPartyState.ACTIVE_PLAYER;
                }
  
                return EPlayerPartyState.NONE;
            }
        }

        public bool IsReady => isReady;

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();
            
            BEvents.PLAYERS_JoinedParty += BEvents_PLAYERS_OnJoinedTheParty;
            BEvents.PLAYERS_LeftParty += BEvents_PLAYERS_OnLeftParty;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            BEvents.PLAYERS_JoinedParty -= BEvents_PLAYERS_OnJoinedTheParty;
            BEvents.PLAYERS_LeftParty -= BEvents_PLAYERS_OnLeftParty;
        }

        #endregion

        #region Events Callbacks

        private void BEvents_PLAYERS_OnJoinedTheParty(BEventHandle<SPlayerJoinedPartyArg> handle)
        {
            if (handle.InvokingNetworkID == BOnlineManager.Inst.LocalNetworkID
                || handle.Arg1.OldSpectatorID != SpectatorID)
                return;

            spectatorID = ESpectatorID.NONE;
            playerID = handle.Arg1.NewPlayerID;
        }

        private void BEvents_PLAYERS_OnLeftParty(BEventHandle<SPlayerLeftPartyArg> handle)
        {
            if (handle.InvokingNetworkID == BOnlineManager.Inst.LocalNetworkID
                || handle.Arg1.OldPlayerID != PlayerID)
                return;

            spectatorID = handle.Arg1.NewSpectatorID;
            playerID = EPlayerID.NONE;
        }

        #endregion

        #region Others

        protected virtual void UpdateObjectNameToPartyState(bool invokeBEvent = true)
        {
            if (BManager.Inst.Config.MatchPlayerNameToPartyState == false)
                return;
            
            switch (PartyState)
            {
                case EPlayerPartyState.SPECTATOR:
                    SetPlayerName(spectatorID.ToString(), invokeBEvent); 
                    break;
                
                case EPlayerPartyState.ACTIVE_PLAYER:
                    SetPlayerName(playerID.ToString(), invokeBEvent);
                    break;
            }
        }

        #endregion


    }
}
