using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BMultiplayerManager : AbstractSingletonManager<BMultiplayerManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

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

        #endregion

        #region Inspector Variables


        #endregion

        #region Variables

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

        public bool IsLocalPlayerHost
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return false;

                return MultiplayerHandler.IsLocalPlayerHost;
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

        public StateMachine<EMultiplayerState> HandlerStateMachine
        {
            get
            {
                if (IS_NULL(MultiplayerHandler, true))
                    return null;

                return MultiplayerHandler.StateMachine;
            }
        }
        
        public AbstractMultiplayerHandler MultiplayerHandler { get; private set; }

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();

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
