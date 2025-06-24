using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BNJMO
{
    public class LobbyCodeText : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        public static string FormatLobbyCode(string text)
        {
            if (text.Length < 6)
                return text;

            string formattedString = text.Substring(0, 3) + " " + text.Substring(3, 3);
            formattedString = formattedString.ToUpper();
            return formattedString;
        }

        public static bool IsValidLobbyCode(string text)
        {
            bool isCorrectLength = text.Length == 6;
            bool isCorrectInput = Regex.IsMatch(text, @"^[A-Za-z0-9]+$");
            return isCorrectLength && isCorrectInput;
        }
        
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
                lobbyCodeText.SetText(FormatLobbyCode(multiplayerManager.LobbyCode));
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
