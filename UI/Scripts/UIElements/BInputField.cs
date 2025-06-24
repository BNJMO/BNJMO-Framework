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
        
        public event Action<BInputField, string> TextSelected;
        public event Action<BInputField, string> TextUpdated;
        public event Action<BInputField, string> TextEditEnded;
        public event Action<BInputField, string> TextSubmitted;

        #endregion

        #region Public Methods

        public void SetInputText(string newText)
        {
            if (inputFieldTMP)
            {
                inputFieldTMP.text = newText;
            }
            if (inputBText)
            {
                inputBText.SetText(newText);
            }
        }

        public string GetInputText()
        {
            if (inputFieldTMP)
            {
                return inputFieldTMP.text;
            }

            return "";
        }

        public void SetInputTextColor(Color newColor)
        {
            defaultInputTextColor = newColor;
            
            if (inputBText)
            {
                inputBText.SetColor(newColor);
            }
        }

        public void SetInputTextValid(bool isValid)
        {
            IsInputTextValid = isValid;
            if (inputBText)
            {
                Color color = IsInputTextValid ? validInputTextColor : invalidInputTextColor;
                inputBText.SetColor(color);
            }
        }

        public void ResetInputTextColor()
        {
            if (inputBText)
            {
                inputBText.SetColor(defaultInputTextColor);
            }
        }

        public void SetPlaceholderText(string newText)
        {
            if (placeholderBText)
            {
                placeholderBText.SetText(newText);
            }
        }

        #endregion

        #region Inspector Variables
        
        [BoxGroup("BInputField", centerLabel: true)] 
        
        [SerializeField] [BoxGroup("BInputField")] [TextArea] 
        private string placeHolderText = "Placeholder...";


        [FoldoutGroup("BInputField/Colors")]
        [Header("Placeholder")]
        [SerializeField]
        [FoldoutGroup("BInputField/Colors")]
        private Color defaultPlaceHolderTextColor = new (0.0f, 0.0f, 0.0f, 0.75f);

        [SerializeField] [FoldoutGroup("BInputField/Colors")] 
        private Color defaultInputTextColor = new (0.0f, 0.0f, 0.0f, 1.0f);
        
        [SerializeField] [FoldoutGroup("BInputField/Colors")] 
        private Color validInputTextColor = new (0.0f, 0.85f, 0.0f, 1.0f);
        
        [SerializeField] [FoldoutGroup("BInputField/Colors")] 
        private Color invalidInputTextColor = new (0.85f, 0.0f, 0.0f, 1.0f);
        
        [Header("References")]
        [SerializeField] [BoxGroup("BInputField")] 
        private BImage backgroundImage;
        
        [SerializeField] [BoxGroup("BInputField")] 
        private TMP_InputField inputFieldTMP;

        [SerializeField] [FormerlySerializedAs("placeholderBTextReference")] [BoxGroup("BInputField")] 
        private BText placeholderBText;

        [SerializeField] [FormerlySerializedAs("inputBTextReference")]  [BoxGroup("BInputField")] 
        private BText inputBText;

        [SerializeField] [BoxGroup("BInputField")]
        private bool enableSelectionCaret = true;
        
        [SerializeField] [BoxGroup("BInputField")] [InfoBox("Added in play mode")]
        private BSelectionCaret selectionCaret;

        #endregion

        #region Variables
        
        public bool IsInputTextValid { get; private set; }
        
        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            objectNamePrefix = "IF_";

            base.OnValidate();

            SetComponentIfNull(ref inputFieldTMP);
            SetComponentInChildrenIfNull(ref backgroundImage);
            
            SetPlaceholderText(placeHolderText);

            if (placeholderBText)
            {
                placeholderBText.SetColor(defaultPlaceHolderTextColor);
            }
            if (inputBText)
            {
                inputBText.SetColor(defaultInputTextColor);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (inputFieldTMP)
            {
                inputFieldTMP.onSelect.AddListener(InputField_OnSelect);
                inputFieldTMP.onTextSelection.AddListener(InputField_OnTextSelection);
                inputFieldTMP.onValueChanged.AddListener(InputField_OnValueChanged);
                inputFieldTMP.onEndEdit.AddListener(InputField_OnEndEdit);
                inputFieldTMP.onSubmit.AddListener(InputField_OnSubmit);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (inputFieldTMP)
            {
                inputFieldTMP.onSelect.RemoveListener(InputField_OnSelect);
                inputFieldTMP.onTextSelection.RemoveListener(InputField_OnTextSelection);
                inputFieldTMP.onValueChanged.RemoveListener(InputField_OnValueChanged);
                inputFieldTMP.onEndEdit.RemoveListener(InputField_OnEndEdit);
                inputFieldTMP.onSubmit.RemoveListener(InputField_OnSubmit);
            }
        }

        protected override void Start()
        {
            base.Start();

            // Add BSelectionCaraet to automatically instantiated TMP_SelectionCaret
            var tmpSelectionCaret = GetComponentInChildren<TMP_SelectionCaret>();
            if (tmpSelectionCaret
                && tmpSelectionCaret.GetComponent<BSelectionCaret>() == null)
            {
                selectionCaret = tmpSelectionCaret.gameObject.AddComponent<BSelectionCaret>();
                if (enableSelectionCaret == false)
                {
                    selectionCaret.DisableUI();
                }
            }
        }
        

        #endregion

        #region Events Callbacks

        private void InputField_OnSelect(string newString)
        {
            InvokeEventIfBound(TextSelected, this, newString);
        }

        private void InputField_OnTextSelection(string newString, int int1, int int2)
        {
            InvokeEventIfBound(TextSelected, this, newString);
        }
        
        private void InputField_OnValueChanged(string newString)
        {
            InvokeEventIfBound(TextUpdated, this, newString);
        }     
        
        private void InputField_OnSubmit(string newString)
        {
            InvokeEventIfBound(TextSubmitted, this, newString);
            inputFieldTMP.OnDeselect(null);
        }
        
        private void InputField_OnEndEdit(string newString)
        {
            InvokeEventIfBound(TextEditEnded, this, newString);
        }
  
        #endregion

        #region Private Methods

  

        #endregion
    }
}
