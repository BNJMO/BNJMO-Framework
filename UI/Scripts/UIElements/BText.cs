using UnityEngine;
using UnityEngine.UI;
using BNJMO;
using TMPro;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class BText : BUIElement
{
    #region Public Events
    public event Action<string> BTextUpdated;
    #endregion

    #region Public Methods

    public void SetText(string newText, bool isArabicRTL = false)
    {
        if (writeTextUppercase)
        {
            newText = newText.ToUpper();
        }

        if (isArabicRTL)
        {
            newText = GetRTLFormatedText(newText);
        }

        if (textUI)
        {
            textUI.text = newText;
        }
        if (textMesh)
        {
            textMesh.text = newText;
        }
        if (textMeshPro)
        {
            textMeshPro.text = newText;
        }

        InvokeEventIfBound(BTextUpdated, newText);
    }

    public void SetColor(Color newColor)
    {
        color = newColor;

        if (textUI)
        {
            textUI.color = newColor;
        }
        if (textMesh)
        {
            textMesh.color = newColor;
        }
        if (textMeshPro)
        {
            textMeshPro.color = newColor;
        }
    }

    public void SetOpacity(float alpha)
    {
        Color newColor = new Color(color.r, color.g, color.b, alpha);
        SetColor(newColor);
    }

    public void FormatTextRTL()
    {
        SetText(GetRTLFormatedText(text));
    }

    public void UpdateLocalizedVariables(params object[] values)
    {
        localizedString.Arguments = values;
        localizedString.RefreshString();
    }

    #endregion

    #region Inspector Values
    [BoxGroup("BText", centerLabel: true)]

    [SerializeField]
    [BoxGroup("BText")] 
    [HideIf("@this.useLocalization == true")]
    [TextArea(4, 8)]
    private string text = "BText";

    [BoxGroup("BText")]
    [SerializeField]
    private bool useLocalization = false;

    [ShowIf("useLocalization")]
    [SerializeField]
    private LocalizedString localizedString;

    [SerializeField]
    [BoxGroup("BText")] 
    private TMP_FontAsset defaultFontAsset;

    [BoxGroup("BText")] 
    [SerializeField] 
    private bool writeTextUppercase = false;

    [BoxGroup("BText")] 
    [SerializeField] 
    private bool overrideUINameFromText = false;

    [BoxGroup("BText")] 
    [SerializeField] 
    private Color color = Color.white;

    [Button("Format Text To RTL")]
    private void Button_FormatTextToRTL()
    {
        FormatTextRTL();
    }
    #endregion

    #region Variables

    public LocalizedString LocalizedString => localizedString;

    public bool WriteTextUppercase { get { return writeTextUppercase; } set { writeTextUppercase = value; } }

    public string Text { get { return text; } }
    
    public Color TextColor { get { return color; } }
    
    public float TextOpacity { get { return color.a; } }

    public TMP_Text TextMeshPro { get { return textMeshPro; } }

    [SerializeField]
    [HideInInspector]
    private Text textUI;

    [SerializeField]
    [HideInInspector]
    private TextMesh textMesh;

    [SerializeField]
    [HideInInspector]
    private TMP_Text textMeshPro;

    [SerializeField]
    [HideInInspector]
    private TextMeshProUGUI tmpTextComponent;

    #endregion

    #region LifeCycle
    protected override void OnValidate()
    {
        if (!CanValidate()) return;
        
        objectNamePrefix = "T_";

        if (overrideUINameFromText)
        {
            UIElementName = text;
        }

        base.OnValidate();

        // Get references
        SetComponentIfNull(ref textUI);
        SetComponentIfNull(ref textMesh);
        SetComponentIfNull(ref textMeshPro);
        SetComponentIfNull(ref tmpTextComponent);

        if (defaultFontAsset == null
            && tmpTextComponent != null)
        {
            defaultFontAsset = tmpTextComponent.font;
        }

        if (defaultFontAsset != null
            && tmpTextComponent != null)
        {
            tmpTextComponent.font = defaultFontAsset;
        }

        if (!useLocalization)
        {
            SetText(text);
        }

        SetColor(color);
    }

    protected override void Awake()
    {
        base.Awake();

        // Initialize Components
        if ((textUI == null) && (textMesh == null) && (textMeshPro == null))
        {
            LogConsoleError("No Text, TextMesh or TextMeshPro component found on this gameobject!");
        }

        if (useLocalization && localizedString != null)
        {
            localizedString.StringChanged += OnLocalizedStringChanged;
        }

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (useLocalization && localizedString != null)
        {
            localizedString.StringChanged -= OnLocalizedStringChanged;
        }
    }

    #endregion

    #region Event Callbacks

    public void OnLocalizedStringChanged(string value)
    {
        bool isRTL = LocalizationSettings.SelectedLocale.Identifier.Code.StartsWith("ar");
        SetText(value, isRTL);
    }

    #endregion

    #region Private Methods

    protected override void OnUIShown()
    {
        base.OnUIShown();

        if (textUI)
        {
            textUI.enabled = true;
        }
        if (textMesh)
        {
            textMesh.GetComponent<MeshRenderer>().enabled = true;
        }
        if (textMeshPro)
        {
            textMeshPro.enabled = true;
        }
    }

    protected override void OnUIHidden()
    {
        base.OnUIHidden();

        if (textUI)
        {
            textUI.enabled = false;
        }
        if (textMesh)
        {
            textMesh.GetComponent<MeshRenderer>().enabled = false;
        }
        if (textMeshPro)
        {
            textMeshPro.enabled = false;
        }
    }

    private string GetRTLFormatedText(string newText)
    {
        if (IS_NOT_NULL(tmpTextComponent)
            && !string.IsNullOrEmpty(newText))
        {
            string rtlText = ArabicSupport.Fix(newText, true, false);
            rtlText = rtlText.Replace("\r", ""); // the Arabix fixer Return \r\n for everyy \n .. need to be removed

            string finalText = "";
            string[] rtlParagraph = rtlText.Split('\n');

            tmpTextComponent.text = "";
            for (int lineIndex = 0; lineIndex < rtlParagraph.Length; lineIndex++)
            {
                string[] words = rtlParagraph[lineIndex].Split(' ');
                Array.Reverse(words);
                tmpTextComponent.text = string.Join(" ", words);
                Canvas.ForceUpdateCanvases();
                for (int i = 0; i < tmpTextComponent.textInfo.lineCount; i++)
                {
                    int startIndex = tmpTextComponent.textInfo.lineInfo[i].firstCharacterIndex;
                    int endIndex = (i == tmpTextComponent.textInfo.lineCount - 1) ? tmpTextComponent.text.Length
                        : tmpTextComponent.textInfo.lineInfo[i + 1].firstCharacterIndex;
                    endIndex = Mathf.Clamp(endIndex, 0, tmpTextComponent.text.Length);
                    int length = Mathf.Clamp(endIndex - startIndex, 0, tmpTextComponent.text.Length - startIndex);
                    if (startIndex < 0 || startIndex >= tmpTextComponent.text.Length || length <= 0)
                        continue;
                    string[] lineWords = tmpTextComponent.text.Substring(startIndex, length).Split(' ');
                    Array.Reverse(lineWords);
                    finalText = finalText + string.Join(" ", lineWords).Trim() + "\n";
                }
            }
            return finalText.TrimEnd('\n');
        }        
        return newText;
    }
    
    private TextAlignmentOptions GetTextAlignment(bool isRTL)
    {
        if (IS_NOT_NULL(tmpTextComponent))
        {
            if (isRTL == true)
            {
                switch (tmpTextComponent.alignment)
                {
                    case TextAlignmentOptions.Left:
                        return TextAlignmentOptions.Right;

                    case TextAlignmentOptions.BaselineLeft:
                        return TextAlignmentOptions.BaselineRight;

                    case TextAlignmentOptions.BottomLeft:
                        return TextAlignmentOptions.BaselineRight;

                    case TextAlignmentOptions.CaplineLeft:
                        return TextAlignmentOptions.CaplineRight;

                    case TextAlignmentOptions.MidlineLeft:
                        return TextAlignmentOptions.MidlineRight;

                    case TextAlignmentOptions.TopLeft:
                        return TextAlignmentOptions.TopRight;

                    default:
                        return tmpTextComponent.alignment;
                }
            }
            else if (isRTL == false)
            {
                switch (tmpTextComponent.alignment)
                {
                    case TextAlignmentOptions.Right:
                        return TextAlignmentOptions.Left;

                    case TextAlignmentOptions.BaselineRight:
                        return TextAlignmentOptions.BaselineLeft;

                    case TextAlignmentOptions.BottomRight:
                        return TextAlignmentOptions.BottomLeft;

                    case TextAlignmentOptions.CaplineRight:
                        return TextAlignmentOptions.CaplineLeft;

                    case TextAlignmentOptions.MidlineRight:
                        return TextAlignmentOptions.MidlineLeft;

                    case TextAlignmentOptions.TopRight:
                        return TextAlignmentOptions.TopLeft;

                    default:
                        return tmpTextComponent.alignment;
                }
            }
        }

        return tmpTextComponent.alignment;
    }

    #endregion
}