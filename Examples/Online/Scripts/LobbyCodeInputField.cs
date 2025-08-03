using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BNJMO
{
    public class LobbyCodeInputField : BBehaviour
    {
        #region Public Events

        public event Action SubmittedInvalidCode;
        public event Action<bool> LobbyCodeValidityChanged;

        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables
        
        [SerializeField]
        private BInputField inputField;

        #endregion

        #region Variables

        public string LobbyCode { get; private set; }

        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref inputField);
        }

        protected override void Awake()
        {
            base.Awake();
            
            inputField.BUIElementShown += InputField_OnBUIElementShown;
            inputField.TextSelected += InputField_OnTextSelected;
            inputField.TextEditEnded += InputField_OnTextEditEnded;
            inputField.TextSubmitted += InputField_OnTextSubmitted;
            inputField.TextUpdated += InputField_OnTextUpdated;
        }


        #endregion

        #region Events Callbacks
        
        private void InputField_OnBUIElementShown(BUIElement buiElement)
        {
            ResetLobbyCode(inputField);
        }

        private void InputField_OnTextSelected(BInputField inputField, string text)
        {
            ResetLobbyCode(inputField);
        }

        private void InputField_OnTextEditEnded(BInputField inputField, string text)
        {
            UpdateLobbyCode(inputField, text);
        }
        
        private void InputField_OnTextSubmitted(BInputField inputField, string text)
        {
            UpdateLobbyCode(inputField, text);
        }
        
        private void InputField_OnTextUpdated(BInputField inputField, string text)
        {
            UpdateLobbyCode(inputField, text);
        }


        #endregion

        #region Others
        
        private void ResetLobbyCode(BInputField inputField)
        {
            LobbyCode = "";
            inputField.SetInputText("");
            inputField.ResetInputTextColor();
        }

        private void UpdateLobbyCode(BInputField inputField, string text)
        {
            if (LobbyCodeText.IsValidLobbyCode(text))
            {
                LobbyCode = text;
                string formattedText = LobbyCodeText.FormatLobbyCode(text);
                inputField.SetInputText(formattedText);
                inputField.SetInputTextValid(true);
                LobbyCodeValidityChanged?.Invoke(true);
            }
            else
            {
                LobbyCode = "";
                inputField.SetInputTextValid(false);
                SubmittedInvalidCode?.Invoke();
                LobbyCodeValidityChanged?.Invoke(false);
            }
        }

        #endregion
    }
}
