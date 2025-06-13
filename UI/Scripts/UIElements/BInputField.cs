using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace BNJMO
{
    public class BInputField : BUIElement
    {
        #region Public Events
        
        public event Action<BInputField, string> TextUpdated;

        #endregion

        #region Public Methods

        public void SetInputText(string newText)
        {
            if (inputFieldTMPReference)
            {
                inputFieldTMPReference.text = newText;
            }
        }

        public string GetInputText()
        {
            if (inputFieldTMPReference)
            {
                return inputFieldTMPReference.text;
            }

            return "";
        }

        public void SetPlaceholderText(string newText)
        {
            if (placeholderBTextReference)
            {
                placeholderBTextReference.SetText(newText);
            }
        }

        #endregion

        #region Inspector Variables
        
        [BoxGroup("BInputField", centerLabel: true)] [SerializeField] 
        private string placeHolderText = "Placeholder...";
        
        [Header("References")]
        [BoxGroup("BInputField")] [SerializeField] 
        private TMP_InputField inputFieldTMPReference;

        [FormerlySerializedAs("placeholderBText")] [BoxGroup("BInputField")] [SerializeField]
        private BText placeholderBTextReference;

        [FormerlySerializedAs("inputBText")] [BoxGroup("BInputField")] [SerializeField]
        private BText inputBTextReference;

        #endregion

        #region Private Variables
        
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            objectNamePrefix = "IF_";

            base.OnValidate();

            SetComponentIfNull(ref inputFieldTMPReference);
            
            SetPlaceholderText(placeHolderText);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (inputFieldTMPReference)
            {
                inputFieldTMPReference.onValueChanged.AddListener(OnInputFieldTMPChanged);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (inputFieldTMPReference)
            {
                inputFieldTMPReference.onValueChanged.RemoveListener(OnInputFieldTMPChanged);
            }
        }

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks
        private void OnInputFieldTMPChanged(string newString)
        {
            InvokeEventIfBound(TextUpdated, this, newString);
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
