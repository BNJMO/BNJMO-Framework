using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNJMO
{
    public class BMultiplayerManager : AbstractSingletonManager<BMultiplayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Match making */
        public void CreatePrivateLobby()
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;
            
            MultiplayerHandler.CreatePrivateLobby();
        }

        public void JoinPrivateLobby(string lobbyCode)
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;

            MultiplayerHandler.JoinPrivateLobby(lobbyCode);
        }

        public void QuickMatch()
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;

            MultiplayerHandler.QuickMatch();
        }

        public void SetLobbyLock(bool isLocked)
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;

            MultiplayerHandler.SetLobbyLock(isLocked);
        }

        public void ShutdownLobbyAndMultiplayer(ELeaveMultiplayerReason leaveReason)
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;

            MultiplayerHandler.ShutdownLobbyAndMultiplayer(leaveReason);
        }

        /* Event Replication */
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID)
        {
            if (IS_NULL(MultiplayerHandler, true))
                return;
            
            MultiplayerHandler.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
        }
        
        #endregion

        #region Inspector Variables


        #endregion

        #region Variables
        
        public AbstractMultiplayerHandler MultiplayerHandler { get; private set; }

        public EAuthority Authority 
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return EAuthority.LOCAL;

                return MultiplayerHandler.Authority;
            }
        }

        public string LobbyCode
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return "";

                return MultiplayerHandler.LobbyCode;
            }
        }

        public ELobbyType LobbyType
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return ELobbyType.None;

                return MultiplayerHandler.LobbyType;
            }
        }

        public ENetworkID LocalNetworkID
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return ENetworkID.NONE;

                return MultiplayerHandler.LocalNetworkID;
            }
        }

        public ENetworkID[] ConnectedPlayers
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true)
                    || ARE_NOT_EQUAL(MultiplayerHandler.StateMachine.CurrentState, EMultiplayerState.InParty, true))
                    return Array.Empty<ENetworkID>();

                return MultiplayerHandler.ConnectedPlayerListeners.Keys.ToArray();
            }
        }
        
        public StateMachine<EMultiplayerState> HandlerStateMachine
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return null;

                return MultiplayerHandler.StateMachine;
            }
        }
        
        public string LocalIPAddress => BUtils.GetLocalIPAddress();

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            switch (BManager.Inst.Config.MultiplayerBackend)
            {
                case EMultiplayerBackend.Netcode:
                    MultiplayerHandler = gameObject.AddComponent<NetcodeMultiplayerHandler>();
                    break;
            }
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
