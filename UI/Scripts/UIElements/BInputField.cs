using System;
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

            if (inputBText)
            {
                if (IsCurrentLanguageArabic && IsOnlyArabic(newText))
                {
                    inputBText.SetRTLText(newText);
                }
                else
                {
                    inputBText.SetText(newText);
                }
            }
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
            if (placeholderBText)
            {
                if (IsCurrentLanguageArabic && IsOnlyArabic(newText))
                {
                    placeholderBText.SetRTLText(newText);
                }
                else
                {
                    placeholderBText.SetText(newText);
                }
            }
        }

        public void UpdateLocalizedPlaceholder(params object[] values)
        {
            if (localizedPlaceholder != null)
            {
                localizedPlaceholder.Arguments = values;
                localizedPlaceholder.RefreshString();
            }
        }

        #endregion

        #region Inspector Variables

        [BoxGroup("BInputField", centerLabel: true)]
        [SerializeField, TextArea]
        private string placeHolderText = "Placeholder...";

        [SerializeField]
        private bool useLocalizationForPlaceholder = false;

        [SerializeField, ShowIf("useLocalizationForPlaceholder")]
        private LocalizedString localizedPlaceholder;

        [FoldoutGroup("BInputField/Colors"), Header("Placeholder")]
        [SerializeField]
        private Color defaultPlaceHolderTextColor = new(0f, 0f, 0f, 0.75f);

        [SerializeField]
        private Color defaultInputTextColor = new(0f, 0f, 0f, 1f);

        [SerializeField]
        private Color validInputTextColor = new(0f, 0.85f, 0f, 1f);

        [SerializeField]
        private Color invalidInputTextColor = new(0.85f, 0f, 0f, 1f);

        [Header("References")]
        [SerializeField]
        private BImage backgroundImage;

        [SerializeField]
        private TMP_InputField inputFieldTMP;

        [SerializeField, FormerlySerializedAs("placeholderBTextReference")]
        private BText placeholderBText;

        [SerializeField, FormerlySerializedAs("inputBTextReference")]
        private BText inputBText;

        [SerializeField]
        private bool enableSelectionCaret = true;

        [SerializeField, InfoBox("Added in play mode")]
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
            if (!CanValidate()) return;
            base.OnValidate();

            objectNamePrefix = "IF_";

            SetComponentIfNull(ref inputFieldTMP);
            SetComponentInChildrenIfNull(ref backgroundImage);

            if (useLocalizationForPlaceholder && localizedPlaceholder != null && !localizedPlaceholder.IsEmpty)
            {
                placeHolderText = localizedPlaceholder.GetLocalizedString();
            }

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
                inputFieldTMP.onValueChanged.AddListener(InputField_OnValueChanged);
                inputFieldTMP.onSubmit.AddListener(InputField_OnSubmit);
            }

            if (useLocalizationForPlaceholder && localizedPlaceholder != null)
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

            if (useLocalizationForPlaceholder && localizedPlaceholder != null)
            {
                localizedPlaceholder.StringChanged -= OnLocalizedPlaceholderChanged;
            }
        }

        protected override void Start()
        {
            base.Start();

            var tmpSelectionCaret = GetComponentInChildren<TMP_SelectionCaret>();
            if (tmpSelectionCaret && tmpSelectionCaret.GetComponent<BSelectionCaret>() == null)
            {
                selectionCaret = tmpSelectionCaret.gameObject.AddComponent<BSelectionCaret>();
                if (!enableSelectionCaret)
                {
                    selectionCaret.DisableUI();
                }
            }
        }

        #endregion

        #region Events Callbacks

        private void InputField_OnValueChanged(string newString)
        {
            if (inputBText)
            {
                if (IsCurrentLanguageArabic && IsOnlyArabic(newString))
                {
                    inputBText.SetRTLText(newString);
                }
                else
                {
                    inputBText.SetText(newString);
                }
            }

            InvokeEventIfBound(TextUpdated, this, newString);
        }

        private void InputField_OnSubmit(string newString)
        {
            InvokeEventIfBound(TextSubmitted, this, newString);
        }

        private void OnLocalizedPlaceholderChanged(string value)
        {
            if (placeholderBText)
            {
                if (IsCurrentLanguageArabic && IsOnlyArabic(value))
                {
                    placeholderBText.SetRTLText(value);
                }
                else
                {
                    placeholderBText.SetText(value);
                }
            }
        }

        #endregion

        #region Private Methods

        private bool IsOnlyArabic(string s)
        {
            return !string.IsNullOrEmpty(s) && Regex.IsMatch(s, @"^[\p{IsArabic}]+$");
        }

        #endregion
    }
}
