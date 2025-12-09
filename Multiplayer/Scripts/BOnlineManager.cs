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
        public void CreatePrivateLobby(string gameMode = "", string lobbyOptions = "")
        {
            if (IS_NULL(OnlineHandler, true))
                return;
            
            OnlineHandler.CreatePrivateLobby(gameMode, lobbyOptions);
        }

        public void JoinPrivateLobby(string lobbyCode, string gameMode = "")
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.JoinPrivateLobby(lobbyCode, gameMode);
        }

        public void QuickMatch(string gameMode = "", string lobbyOptions = "")
        {
            if (IS_NULL(OnlineHandler, true))
                return;

            OnlineHandler.QuickMatch(gameMode, lobbyOptions);
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

        public EAuthority Authority => OnlineHandler == null ? EAuthority.LOCAL : OnlineHandler.Authority;

        public string LobbyCode => OnlineHandler == null ? "" : OnlineHandler.LobbyCode;

        public ELobbyType LobbyType => IS_NULL(OnlineHandler, true) ? ELobbyType.None : OnlineHandler.LobbyType;

        public ENetworkID LocalNetworkID => OnlineHandler ==null ? ENetworkID.NONE : OnlineHandler.LocalNetworkID;

        public ENetworkID[] ConnectedClients
        {
            get
            {
                if (OnlineHandler == null
                    || OnlineHandler.StateMachine.CurrentState != EOnlineState.InOnlineSession)
                    return Array.Empty<ENetworkID>();

                return OnlineHandler.ConnectedClientListeners.Keys.ToArray();
            }
        }
        
        public StateMachine<EOnlineState> HandlerStateMachine => OnlineHandler == null ? null : OnlineHandler.StateMachine;

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
                
                case EOnlineBackend.None:
                    OnlineHandler = gameObject.AddComponent<NoneOnlineHandler>();
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
