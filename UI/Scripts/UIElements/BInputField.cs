using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;

namespace BNJMO
{
    public class BInputField : BUIElement
    {
        #region Public Events

        public event Action<BInputField, string> TextUpdated;
        public event Action<BInputField, string> TextSubmitted;

        #endregion

        #region Public Methods

        public void SetInputText(string newText)
        {
            if (inputFieldTMP)
            {
                inputFieldTMP.text = newText;
            }

            ApplyTextToBText(newText);
        }

        public string GetInputText() 
        {
            return inputFieldTMP ? inputFieldTMP.text : "";
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
            ApplyPlaceholderText(newText);
        }

        public void UpdateLocalizedPlaceholder(params object[] values)
        {
            if (localizedPlaceholder == null)
                return;

            localizedPlaceholder.Arguments = values;
            localizedPlaceholder.RefreshString();
        }

        #endregion

        #region Inspector Variables

        [BoxGroup("BInputField", centerLabel: true)]
        [SerializeField, TextArea, HideIf("@useLocalizationForPlaceholder")]
        private string placeHolderText = "Placeholder...";

        [SerializeField] private bool useLocalizationForPlaceholder = false;

        [SerializeField, ShowIf("useLocalizationForPlaceholder")]
        private LocalizedString localizedPlaceholder;

        [FoldoutGroup("BInputField/Colors"), Header("Placeholder")]
        [SerializeField] private Color defaultPlaceHolderTextColor = new(0f, 0f, 0f, .75f);
        [SerializeField] private Color defaultInputTextColor = new(0f, 0f, 0f, 1f);
        [SerializeField] private Color validInputTextColor = new(0f, .85f, 0f, 1f);
        [SerializeField] private Color invalidInputTextColor = new(.85f, 0f, 0f, 1f);

        [Header("References")]
        [SerializeField] private BImage backgroundImage;
        [SerializeField] private TMP_InputField inputFieldTMP;
        [SerializeField, FormerlySerializedAs("placeholderBTextReference")]
        private BText placeholderBText;
        [SerializeField, FormerlySerializedAs("inputBTextReference")]
        private BText inputBText;

        [SerializeField, BoxGroup("BInputField")]
        private bool enableSelectionCaret = true;

        [SerializeField, BoxGroup("BInputField"), InfoBox("Added in play mode")]
        private BSelectionCaret selectionCaret;

        #endregion

        #region Variables

        public bool IsInputTextValid { get; private set; }

        private bool IsCurrentLanguageArabic =>
            LocalizationSettings.SelectedLocale != null &&
            LocalizationSettings.SelectedLocale.Identifier.Code.StartsWith("ar");

        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate())
                return;

            base.OnValidate();

            objectNamePrefix = "IF_";
            SetComponentIfNull(ref inputFieldTMP);
            SetComponentInChildrenIfNull(ref backgroundImage);

            if (useLocalizationForPlaceholder
                && localizedPlaceholder != null
                && localizedPlaceholder.IsEmpty == false)
            {
                placeHolderText = localizedPlaceholder.GetLocalizedString();
            }

            ApplyPlaceholderText(placeHolderText);

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
                inputFieldTMP.onValueChanged.AddListener(InputField_OnValueChanged);
                inputFieldTMP.onSubmit.AddListener(InputField_OnSubmit);
            }

            if (useLocalizationForPlaceholder 
                && localizedPlaceholder != null)
            {
                localizedPlaceholder.StringChanged += OnLocalizedPlaceholderChanged;
                localizedPlaceholder.RefreshString();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (inputFieldTMP)
            {
                inputFieldTMP.onValueChanged.RemoveListener(InputField_OnValueChanged);
                inputFieldTMP.onSubmit.RemoveListener(InputField_OnSubmit);
            }

            if (useLocalizationForPlaceholder 
                && localizedPlaceholder != null)
            {
                localizedPlaceholder.StringChanged -= OnLocalizedPlaceholderChanged;
            }
        }

        protected override void Start()
        {
            base.Start();

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

        private void InputField_OnValueChanged(string rawInput)
        {
            ApplyTextToBText(rawInput);
            InvokeEventIfBound(TextUpdated, this, rawInput);
        }

        private void InputField_OnSubmit(string submitted)
        {
            InvokeEventIfBound(TextSubmitted, this, submitted);
        }

        private void OnLocalizedPlaceholderChanged(string value)
        {
            ApplyPlaceholderText(value);
        }

        #endregion

        #region Private Methods

        private void ApplyTextToBText(string value)
        {
            if (!inputBText 
                || inputFieldTMP == null)
                return;

            if (IsCurrentLanguageArabic 
                && BUtils.StringIsOnlyArabic(value))
            {
                Wait(0.01f, () =>
                {
                    UpdateInputField(inputBText.FormatRTL(value));
                });
            }
            else
            {
                inputBText.SetText(value);
            }
        }

        private void UpdateInputField(string formatted)
        {
            inputFieldTMP.onValueChanged.RemoveListener(InputField_OnValueChanged);

            inputFieldTMP.text = formatted;
            inputBText.SetText(formatted);

            inputFieldTMP.caretPosition = formatted.Length;
            inputFieldTMP.selectionAnchorPosition = formatted.Length;
            inputFieldTMP.selectionFocusPosition = formatted.Length;

            inputFieldTMP.onValueChanged.AddListener(InputField_OnValueChanged);
        }

        private void ApplyPlaceholderText(string value)
        {
            if (!placeholderBText) 
                return;

            if (IsCurrentLanguageArabic)
            {
                placeholderBText.SetText(placeholderBText.FormatRTL(value));
            }
            else
            {
                placeholderBText.SetText(value);
            }
        }

        #endregion
    }
}
