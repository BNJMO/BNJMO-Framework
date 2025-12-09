using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace BNJMO
{
    public class NoneOnlineHandler : AbstractOnlineHandler
    {
        #region Public Events

        #endregion

        #region Public Methods
        
        /* Lobby and Matching making */
        public override void CreatePrivateLobby(string gameMode = "", string lobbyOptions = "")
        {
            LogConsoleWarning("Cannot create Private Lobby because Online Backend is None");
        }

        public override void JoinPrivateLobby(string lobbyCode, string gameMode = "")
        {
            LogConsoleWarning("Cannot join Private Lobby because Online Backend is None");
        }

        public override void QuickMatch(string gameMode = "", string lobbyOptions = "")
        {
            LogConsoleWarning("Cannot join Quick Match because Online Backend is None");
        }

        public override void SetLobbyLock(bool isLocked)
        {
            LogConsoleWarning("Cannot Lock Lobby because Online Backend is None");
        }
        
        public override async void ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason leaveReason)
        {
            LogConsoleWarning("Cannot Shutdown Lobby and Multiplayer because Online Backend is None");
        }
     
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables

        public override string LobbyCode
        {
            get => "";
            protected set { }
        }

        public override ELobbyType LobbyType
        {
            get => ELobbyType.None;
            protected set { }
        }

        public override EAuthority Authority 
        {
            get => EAuthority.LOCAL;
            protected set { }
        }

        public override IClientListener LocalClientListener { get; protected set; }

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();
            
            StateMachine.DebugStateChange = false;
            StateMachine.UpdateState(EOnlineState.NotConnected);
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others
        

        #endregion
    }
}
