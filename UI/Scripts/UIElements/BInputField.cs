using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        //public void SetPlaceholderText(string newText)
        //{
        //    if (inputFieldTMP)
        //    {
        //        inputFieldTMP.text = newText;
        //    }
        //}

        #endregion

        #region Inspector Variables
        [SerializeField]
        private TMP_InputField inputFieldTMPReference;
                
        //[SerializeField]
        //[HideInInspector]
        //private TMP_InputField inputFieldTMP;

        #endregion

        #region Private Variables
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            objectNamePrefix = "IF_";

            base.OnValidate();

            SetComponentIfNull(ref inputFieldTMPReference);
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
        protected override void OnUIShown()
        {
            base.OnUIShown();

            // TODO: Disable components instead
            if (inputFieldTMPReference)
            {
                inputFieldTMPReference.gameObject.SetActive(true);
            }
        }

        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (inputFieldTMPReference)
            {
                inputFieldTMPReference.gameObject.SetActive(false);
            }
        }


        #endregion
    }
}
