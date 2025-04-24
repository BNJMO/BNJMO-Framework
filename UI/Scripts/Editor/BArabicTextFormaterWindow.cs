using UnityEngine;
using BNJMO;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using System;
using TMPro;

public class BArabicTextFormaterWindow : OdinEditorWindow
{
    [MenuItem("BNJMO/Arabic Text Formater Window")]
    private static void OpenWindow()
    {
        GetWindow<BArabicTextFormaterWindow>().Show();
    }

    [SerializeField]
    private TextMeshProUGUI tmpTextComponent;

    [SerializeField]
    private bool showTashkeel = false;

    [SerializeField]
    private bool useHinduNumbers = false;

    [TextArea(5, 10)]
    [OnValueChanged("OnInputChanged")]
    public string Input;
    private void OnInputChanged()
    {
        Output = FixArabic(Input);
    }


    [TextArea(5, 10)]
    public string Output;

    private string FixArabic(string newText)
    {
        if ((tmpTextComponent)
            && !string.IsNullOrEmpty(newText))
        {
            string rtlText = ArabicSupport.Fix(newText, showTashkeel, useHinduNumbers);
            rtlText = rtlText.Replace("\r", ""); // the Arabix fixer Return \r\n for everyy \n .. need to be removed

            string startText = tmpTextComponent.text;
            string finalText = "";
            string[] rtlParagraph = rtlText.Split('\n');

            //tmpTextComponent.text = "";
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

            tmpTextComponent.text = startText;
            return finalText.TrimEnd('\n');
        }
        return newText;
    }

}
