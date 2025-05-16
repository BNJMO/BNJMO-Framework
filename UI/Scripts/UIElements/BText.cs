using UnityEngine;
using UnityEngine.UI;
using BNJMO;
using TMPro;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public enum ELanguage
{
    ENGLISH = 0,
    FRENCH,
    TOUNSI,
}

public class BText : BUIElement
{
    #region Public Events
    public event Action<string> BTextUpdated;
    #endregion

    #region Public Methods
    
    public void SetText(string newText, bool isArabicRTL = false)
    {
        if (writeTextUppercase == true)
        {
            newText = newText.ToUpper();
        }

        if (isArabicRTL == true)
        {
            newText = GetRTLFormatedText(newText);
        }

        text = newText;

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
    
    public void SetLanguage(ELanguage language)
    {
        if (useLanguages == true)
        {
            // Check if language is contained
            if (languageTextMap.ContainsKey(language))
            {
                UpdateTextLanguage(languageTextMap[language]);
            }
            else if (languageTextMap.ContainsKey(ELanguage.ENGLISH))
            {
                UpdateTextLanguage(languageTextMap[ELanguage.ENGLISH]);
            }
            else
            {
                LogConsoleWarning("No appropriate language found for the text");
            }
        }
    }

    public void FormatTextRTL()
    {
        if (useLanguages == false)
        {
            SetText(GetRTLFormatedText(text));
        } 
        else
        {
            foreach (LanguageTupple languageTupple in languageTexts)
            {
                if (languageTupple.Language == ELanguage.TOUNSI)
                {
                    languageTupple.Text = GetRTLFormatedText(languageTupple.Text);
                }
            }
        }
    }
    #endregion

    #region Inspector Values
    [BoxGroup("BText", centerLabel: true)]

    [SerializeField]
    [BoxGroup("BText")] 
    private bool useLanguages = false;

    [SerializeField]
    [BoxGroup("BText")] 
    [HideIf("@this.useLanguages == true")]
    [TextArea(4, 8)]
    private string text = "BText";

    [SerializeField]
    [BoxGroup("BText")] 
    private TMP_FontAsset defaultFontAsset;
    
    [SerializeField]
    [BoxGroup("BText")] 
    [HideIf("@this.useLanguages == false")]
    private LanguageTupple[] languageTexts = new LanguageTupple[1]
    {
         new LanguageTupple()
         {
             Language = ELanguage.ENGLISH,
             Text = "",
         }
    };

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

    private Dictionary<ELanguage, LanguageTupple> languageTextMap = new Dictionary<ELanguage, LanguageTupple>();

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

        if (useLanguages == true
            && languageTexts.Length > 0)
        {
            UpdateTextLanguage(languageTexts[0]);
        }
      
        SetText(text);
        SetColor(color);
    }

    //protected override void OnEnable()
    //{
    //    base.OnEnable();

    //    BEventsCollection.UI_NewLanguageSet += On_UI_NewLanguageSet;
    //}

    //protected override void OnDisable()
    //{
    //    base.OnDisable();

    //    BEventsCollection.UI_NewLanguageSet -= On_UI_NewLanguageSet;
    //}

    protected override void Awake()
    {
        base.Awake();

        // Initialize Components
        if ((textUI == null) && (textMesh == null) && (textMeshPro == null))
        {
            LogConsoleError("No Text, TextMesh or TextMeshPro component found on this gameobject!");
        }

        // Check languages
        if (useLanguages == true)
        {
            if (languageTexts.Length == 0)
            {
                LogConsoleError("BText is using languages but no language is provided. Fallback to normal mode.");
                useLanguages = false;
            }
        
            // Initiliaze language textx map
            foreach (LanguageTupple languageTupple in languageTexts)
            {
                if (IS_KEY_NOT_CONTAINED(languageTextMap, languageTupple.Language))
                {
                    languageTextMap.Add(languageTupple.Language, languageTupple);
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        //SetLanguage(MotherOfManagers.Instance.Language);
    }

    #endregion

    #region Event Callbacks
    private void On_UI_NewLanguageSet(BEHandle<ELanguage> handle)
    {
        SetLanguage(handle.Arg1);
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

    private void UpdateTextLanguage(LanguageTupple languageTupple)
    {
        if (IS_NOT_NULL(languageTupple)
            && tmpTextComponent != null)
        {
            // Use RTL for arabic languages
            if (languageTupple.Language == ELanguage.TOUNSI)
            {
                tmpTextComponent.alignment = GetTextAlignment(true);
            }
            else
            {
                tmpTextComponent.alignment = GetTextAlignment(false);
            }

            // Update text
            SetText(languageTupple.Text);

            // Has custom Font Asset?
            if (languageTupple.CustomFontAsset != null)
            {
                tmpTextComponent.font = languageTupple.CustomFontAsset;
                tmpTextComponent.UpdateFontAsset();
            }
            else if (defaultFontAsset != null)
            {
                tmpTextComponent.font = defaultFontAsset;
            }
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
                    int length = endIndex - startIndex;
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


[Serializable]
public class LanguageTupple
{
    public ELanguage Language = ELanguage.ENGLISH;

    [TextArea]
    [HideLabel]
    public string Text = "";

    public TMP_FontAsset CustomFontAsset;
}

