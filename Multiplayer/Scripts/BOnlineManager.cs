using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BNJMO
{
    public class BOnlineManager : AbstractSingletonManager<BOnlineManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Match making */
        public void CreatePrivateLobby()
        {
            if (IS_NULL(OnlineHandler, true))
                return;
            
            OnlineHandler.CreatePrivateLobby();
        }

        public void JoinPrivateLobby(string lobbyCode)
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.JoinPrivateLobby(lobbyCode);
        }

        public void QuickMatch()
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.QuickMatch();
        }

        public void SetLobbyLock(bool isLocked)
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.SetLobbyLock(isLocked);
        }

        public void ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason leaveReason)
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.ShutdownLobbyAndMultiplayer(leaveReason);
        }

        /* Event Replication */
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID)
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            eventHandle.InvokingNetworkID = LocalNetworkID;
            OnlineHandler.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
        }
        
        #endregion

        #region Inspector Variables


        #endregion

        #region Variables
        
        public AbstractOnlineHandler OnlineHandler { get; private set; }

        public EAuthority Authority 
        {
            get
            {
                if (IS_NULL(OnlineHandler, true))
                    return EAuthority.LOCAL;

                return OnlineHandler.Authority;
            }
        }

        public string LobbyCode
        {
            get
            {
                if (IS_NULL(OnlineHandler, true))
                    return "";

                return OnlineHandler.LobbyCode;
            }
        }

        public ELobbyType LobbyType
        {
            get
            {
                if (IS_NULL(OnlineHandler, true))
                    return ELobbyType.None;

                return OnlineHandler.LobbyType;
            }
        }

        public ENetworkID LocalNetworkID
        {
            get
            {
                if (IS_NULL(OnlineHandler, true))
                    return ENetworkID.NONE;

                return OnlineHandler.LocalNetworkID;
            }
        }

        public ENetworkID[] ConnectedClients
        {
            get
            {
                if (IS_NULL(OnlineHandler, true)
                    || ARE_NOT_EQUAL(OnlineHandler.StateMachine.CurrentState, EOnlineState.InOnlineSession, true))
                    return Array.Empty<ENetworkID>();

                return OnlineHandler.ConnectedClientListeners.Keys.ToArray();
            }
        }
        
        public StateMachine<EOnlineState> HandlerStateMachine
        {
            get
            {
                if (IS_NULL(OnlineHandler, true))
                    return null;

                return OnlineHandler.StateMachine;
            }
        }
        
        public string LocalIPAddress => BUtils.GetLocalIPAddress();

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            switch (BManager.Inst.Config.onlineBackend)
            {
                case EOnlineBackend.Netcode:
                    OnlineHandler = gameObject.AddComponent<NetcodeOnlineHandler>();
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
