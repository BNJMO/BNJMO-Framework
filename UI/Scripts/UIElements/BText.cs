using System;
using BNJMO;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BText : BUIElement
{
    #region Public Events

    public event Action<string> BTextUpdated;

    #endregion

    #region Public Methods

    public void SetText(string newText, bool isArabicRTL = false)
    {
        text = newText;

        if (writeTextUppercase)
        {
            text = text.ToUpper();
        }

        if (isArabicRTL)
        {
            text = FormatRTL(text);
            if (tmpTextComponent)
            {
                tmpTextComponent.alignment = GetTextAlignment(true);
            }
        }
        else
        {
            if (tmpTextComponent) tmpTextComponent.alignment = GetTextAlignment(false);
        }

        if (textUI)
        {
            textUI.text = text;
        }
        if (textMesh)
        {
            textMesh.text = text;
        }
        if (textMeshPro)
        {
            textMeshPro.text = text;
        }

        InvokeEventIfBound(BTextUpdated, text);
    }

    public void SetLocalizedText(LocalizedString newLocalizedString)
    {
        if (newLocalizedString == null)
            return;

        localizedString = newLocalizedString;
        localizedString.RefreshString();
        ApplyLocalizedText(localizedString.GetLocalizedString());
    }

    public void SetFontsize(float newFontSize)
    {
        fontSize = newFontSize;

        if (textUI)
        {
            textUI.fontSize = (int) fontSize;
        }
        if (textMesh)
        {
            textMesh.fontSize = (int) fontSize;
        }
        if (textMeshPro)
        {
            textMeshPro.fontSize = fontSize;
        }

        InvokeEventIfBound(BTextUpdated, text);
    }

    public void SetTMPFontAsset(TMP_FontAsset newFontAsset)
    {
        defaultTMPFontAsset = newFontAsset;

        if (defaultTMPFontAsset && tmpTextComponent)
        {
            tmpTextComponent.font = defaultTMPFontAsset;
        }
    }

    public void SetTextUppercase(bool isUpperCase)
    {
        writeTextUppercase = isUpperCase;
        SetText(text, isRTL);
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

    public void UpdateLocalizedVariables(params object[] values)
    {
        localizedString.Arguments = values;
        localizedString.RefreshString();
        ApplyLocalizedText(localizedString.GetLocalizedString());
    }

    public TextAlignmentOptions GetTextAlignment(bool isRTL)
    {
        if (!tmpTextComponent)
        {
            return TextAlignmentOptions.Left;
        }

        return isRTL switch
        {
            true => tmpTextComponent.alignment switch
            {
                TextAlignmentOptions.Left => TextAlignmentOptions.Right,
                TextAlignmentOptions.BaselineLeft => TextAlignmentOptions.BaselineRight,
                TextAlignmentOptions.BottomLeft => TextAlignmentOptions.BaselineRight,
                TextAlignmentOptions.CaplineLeft => TextAlignmentOptions.CaplineRight,
                TextAlignmentOptions.MidlineLeft => TextAlignmentOptions.MidlineRight,
                TextAlignmentOptions.TopLeft => TextAlignmentOptions.TopRight,
                _ => tmpTextComponent.alignment
            },
            false => tmpTextComponent.alignment switch
            {
                TextAlignmentOptions.Right => TextAlignmentOptions.Left,
                TextAlignmentOptions.BaselineRight => TextAlignmentOptions.BaselineLeft,
                TextAlignmentOptions.BottomRight => TextAlignmentOptions.BottomLeft,
                TextAlignmentOptions.CaplineRight => TextAlignmentOptions.CaplineLeft,
                TextAlignmentOptions.MidlineRight => TextAlignmentOptions.MidlineLeft,
                TextAlignmentOptions.TopRight => TextAlignmentOptions.TopLeft,
                _ => tmpTextComponent.alignment
            }
        };
    }

    public void FormatTextRTL()
    {
        SetText(FormatRTL(text), true);
    }

    #endregion

    #region Inspector Values

    [BoxGroup("BText", centerLabel: true)]
    [SerializeField, BoxGroup("BText")]
    private bool useLocalization = false;

    [SerializeField, BoxGroup("BText"), HideIf("@this.useLocalization == true"), TextArea(4, 8)]
    private string text = "BText";

    [SerializeField, BoxGroup("BText"), ShowIf("useLocalization")]
    private LocalizedString localizedString;

    [SerializeField, BoxGroup("BText"), FormerlySerializedAs("defaultFontAsset")]
    private TMP_FontAsset defaultTMPFontAsset;

    [SerializeField, BoxGroup("BText")]
    private float fontSize = 0.0f;

    [SerializeField, BoxGroup("BText")]
    private bool isRTL = false;

    [SerializeField, BoxGroup("BText")]
    private bool writeTextUppercase = false;

    [SerializeField, BoxGroup("BText")]
    private bool overrideUINameFromText = false;

    [SerializeField, BoxGroup("BText")]
    private Color color = Color.white;

    [SerializeField, BoxGroup("BText"), ReadOnly]
    private Text textUI;

    [SerializeField, BoxGroup("BText"), ReadOnly]
    private TextMesh textMesh;

    [SerializeField, BoxGroup("BText"), ReadOnly]
    private TMP_Text textMeshPro;

    [SerializeField, BoxGroup("BText"), ReadOnly]
    private TextMeshProUGUI tmpTextComponent;

    [BoxGroup("BText"), Button("Match Parent Size")]
    private void MatchParentSize_Button() => MatchParentSize();

    [BoxGroup("BText"), Button("Format Text To RTL")]
    private void Button_FormatTextToRTL() => FormatTextRTL();

    #endregion

    #region Variables

    public bool UseLocalization
    {
        get => useLocalization;
        set => useLocalization = value;
    }

    public LocalizedString LocalizedString => localizedString;
    public bool WriteTextUppercase => writeTextUppercase;
    public string Text => text;
    public float FontSize => fontSize;
    public bool IsRTL => isRTL;
    public Color TextColor => color;
    public float TextOpacity => color.a;
    public TMP_Text TextMeshPro => textMeshPro;
    public TMP_FontAsset TextTMPFont => defaultTMPFontAsset;

    private (string text, bool isRTL)? pendingTextData = null;

    #endregion

    #region LifeCycle

    protected override void Awake()
    {
        base.Awake();

        if ((textUI == null) 
            && (textMesh == null) 
            && (textMeshPro == null))
        {
            LogConsoleError("No Text, TextMesh or TextMeshPro component found on this gameobject!");
        }

        if (useLocalization 
            && localizedString != null)
        {
            localizedString.StringChanged += OnLocalizedStringChanged;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (useLocalization && localizedString != null && !localizedString.IsEmpty)
        {
            ApplyLocalizedText(localizedString.GetLocalizedString());
        }
        else
        {
            SetText(text, isRTL);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (useLocalization 
            && localizedString != null)
        {
            localizedString.StringChanged -= OnLocalizedStringChanged;
        }
    }

    protected override void OnValidate()
    {
        if (!CanValidate()) return;

        objectNamePrefix = "T_";

        if (overrideUINameFromText)
        {
            UIElementName = text;
        }

        base.OnValidate();

        SetComponentIfNull(ref textUI);
        SetComponentIfNull(ref textMesh);
        SetComponentIfNull(ref textMeshPro);
        SetComponentIfNull(ref tmpTextComponent);

        if (tmpTextComponent && defaultTMPFontAsset == null)
        {
            defaultTMPFontAsset = tmpTextComponent.font;
        }

        SetTMPFontAsset(defaultTMPFontAsset);

        if (fontSize <= 0.0f)
        {
            fontSize = tmpTextComponent ? tmpTextComponent.fontSize :
                       textMeshPro ? textMeshPro.fontSize :
                       textMesh ? textMesh.fontSize :
                       textUI ? textUI.fontSize : 14f;
        }

        if (!Application.isPlaying)
        {
            SetText(text, isRTL);
        }

        SetFontsize(fontSize);
        SetColor(color);
    }

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

        if (pendingTextData.HasValue)
        {
            var (pendingDataText, pendingDataIsRTL) = pendingTextData.Value;
            SetText(pendingDataText, pendingDataIsRTL);
            pendingTextData = null;
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

    #endregion

    #region Event Callbacks

    public void OnLocalizedStringChanged(string value)
    {
        isRTL = LocalizationSettings.SelectedLocale.Identifier.Code.StartsWith("ar");

        if (IsTextComponentInactive())
        {
            pendingTextData = (value, isRTL);
            return;
        }

        SetText(value, isRTL);
    }

    #endregion

    #region Private Methods

    private void ApplyLocalizedText(string value)
    {
        isRTL = LocalizationSettings.SelectedLocale.Identifier.Code.StartsWith("ar");
        SetText(value, isRTL);
    }

    private bool IsTextComponentInactive()
    {
        return (textUI && !textUI.enabled)
            || (textMesh && !textMesh.gameObject.activeInHierarchy)
            || (textMeshPro && !textMeshPro.enabled);
    }

    public string FormatRTL(string newText)
    {
        if (tmpTextComponent == null 
            || string.IsNullOrEmpty(newText))
            return newText;

        string rtlText = ArabicSupport.Fix(newText, true, false).Replace("\r", "");
        string finalText = "";
        string[] rtlParagraph = rtlText.Split('\n');

        tmpTextComponent.text = "";

        foreach (string paragraph in rtlParagraph)
        {
            string[] words = paragraph.Split(' ');
            Array.Reverse(words);
            tmpTextComponent.text = string.Join(" ", words);
            Canvas.ForceUpdateCanvases();

            for (int i = 0; i < tmpTextComponent.textInfo.lineCount; i++)
            {
                int startIndex = tmpTextComponent.textInfo.lineInfo[i].firstCharacterIndex;
                int endIndex = (i == tmpTextComponent.textInfo.lineCount - 1)
                    ? tmpTextComponent.text.Length
                    : tmpTextComponent.textInfo.lineInfo[i + 1].firstCharacterIndex;

                int length = Mathf.Clamp(endIndex - startIndex, 0, tmpTextComponent.text.Length - startIndex);
                if (startIndex < 0 || length <= 0)
                    continue;

                string[] lineWords = tmpTextComponent.text.Substring(startIndex, length).Split(' ');
                Array.Reverse(lineWords);
                finalText += string.Join(" ", lineWords).Trim() + "\n";
            }
        }

        return finalText.TrimEnd('\n');
    }

    #endregion
}
