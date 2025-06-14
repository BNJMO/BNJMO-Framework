using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class OnlineStatusText : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables
        
        [SerializeField] private BText matchTypeText;
        [SerializeField] private BText multiplayerStatusText;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();
            
            MultiplayerManager_OnStateMachineHooks();
            BEvents.ONLINE_StartedLaunchingSession += BEvents_ONLINE_OnStartedLaunchingSession;
            BEvents.ONLINE_LaunchSessionSucceeded += BEvents_ONLINE_OnLaunchSessionSucceeded;
        }

        #endregion

        #region Events Callbacks

        private void MultiplayerManager_OnStateMachineHooks()
        {
            if (BOnlineManager.Inst == null
                || BOnlineManager.Inst.HandlerStateMachine == null) 
                return;
            
            var multiplayerManager = BOnlineManager.Inst;
            var stateMachine = multiplayerManager.HandlerStateMachine;
            stateMachine.Handler[EOnlineState.NotConnected].Enter += old =>
            {
                matchTypeText.SetText("- Local -");
                multiplayerStatusText.SetText("Connecting...");
            };
            
            stateMachine.Handler[EOnlineState.InLobby].Enter += old =>
            {
                switch (multiplayerManager.LobbyType)
                {
                    case ELobbyType.QuickMatch:
                        matchTypeText.SetText("- Quick match -");
                        break;
                    case ELobbyType.Private:
                        matchTypeText.SetText("- Private match -");
                        break;
                }

                switch (multiplayerManager.Authority)
                {
                    case EAuthority.HOST:
                        multiplayerStatusText.SetText($"Created lobby");
                        break;
                    case EAuthority.CLIENT:
                        multiplayerStatusText.SetText($"Joined lobby");
                        break;
                }
            };
            
            stateMachine.Handler[EOnlineState.InOnlineSession].Enter += old =>
            {
                switch (multiplayerManager.LobbyType)
                {
                    case ELobbyType.QuickMatch:
                        matchTypeText.SetText("- Quick match -");
                        break;
                    case ELobbyType.Private:
                        matchTypeText.SetText("- Private match -");
                        break;
                }
                
                multiplayerStatusText.SetText($"In party. \nNetworkID : {multiplayerManager.LocalNetworkID}");
            };
        }

        private void BEvents_ONLINE_OnStartedLaunchingSession(BEventHandle<ELobbyType> handle)
        {
            if (BOnlineManager.Inst == null)
                return;
            
            var multiplayerManager = BOnlineManager.Inst;
            switch (multiplayerManager.LobbyType)
            {
                case ELobbyType.QuickMatch:
                    matchTypeText.SetText("- Quick match -");
                    break;
                case ELobbyType.Private:
                    matchTypeText.SetText("- Private match -");
                    break;
                case ELobbyType.None:
                    matchTypeText.SetText("");
                    break;
            }
            
            multiplayerStatusText.SetText($"Started launching multiplayer.");
        }

        private void BEvents_ONLINE_OnLaunchSessionSucceeded(BEventHandle handle)
        {
            var multiplayerManager = BOnlineManager.Inst;

            switch (multiplayerManager.LobbyType)
            {
                case ELobbyType.QuickMatch:
                    matchTypeText.SetText("- Quick match -");
                    break;
                case ELobbyType.Private:
                    matchTypeText.SetText("- Private match -");
                    break;
            }
                
            multiplayerStatusText.SetText($"In party. \nNetworkID : {multiplayerManager.LocalNetworkID}");
        }

        #endregion

        #region Others


        #endregion
    }
}
