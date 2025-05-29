using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class LobbyCodeText : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [SerializeField] private BText lobbyCodeText;

        #endregion

        #region Variables

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref lobbyCodeText);
        }

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();
            
            lobbyCodeText.SetText("");
        }
        

        protected override void Start()
        {
            base.Start();

            BMultiplayerManager_OnStateMachineHooks();
        }

        private void BMultiplayerManager_OnStateMachineHooks()
        {
            if (BOnlineManager.Inst == null
                || BOnlineManager.Inst.HandlerStateMachine == null) 
                return;
            
            var multiplayerManager = BOnlineManager.Inst;
            var stateMachine = multiplayerManager.HandlerStateMachine;
            stateMachine.Handler[EOnlineState.NotConnected].Exit += old =>
            {
                lobbyCodeText.SetText(multiplayerManager.LobbyCode);
            };
            stateMachine.Handler[EOnlineState.NotConnected].Enter += old =>
            {
                lobbyCodeText.SetText("");
            };
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
