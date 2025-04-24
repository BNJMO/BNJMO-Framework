using System.Collections;
using System.Collections.Generic;
using BNJMO;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class BTextWriter : BBehaviour
{

	#region Public Events
	public event Action<string> StartedWriting;
	public event Action<string> TextUpdated;
	public event Action<string> FinishedWriting;

    #endregion

    #region Public Methods
	public void StartWriting()
    {
		StartNewCoroutine(ref WriteTextEnumerator, WriteTextCoroutine());
	}

	public void StartWriting(string text)
    {
		originalText = text;

		StartNewCoroutine(ref WriteTextEnumerator, WriteTextCoroutine());
	}

	public void PauseWriting()
    {
		IsPaused = true;
	}

	public void FinishWriting()
    {
		StopCoroutineIfRunning(ref WriteTextEnumerator);
		StopCoroutineIfRunning(ref WriteRegularTextEnumerator);
		StopCoroutineIfRunning(ref WriteRTLTextEnumerator);

		// Remove all instances from the special wait character from final string
		bText.SetText(originalText.Replace(specialWaitCharacter + "", ""));
	}

	#endregion

	#region Inspector Variables
	[SerializeField]
	private bool startWritingWhenEnabled = false;

	[SerializeField] 
	private float characterTypeDelay = 0.125f;

	[SerializeField] 
	private float randomVariation = 0.05f;

	[SerializeField] 
	private float leaveSpaceDelay = 0.22f;

	[SerializeField] 
	private float startDelay = 0.5f;

	[SerializeField]
	private char specialWaitCharacter = '^';

	[SerializeField]
	private float waitCharacterDuration = 0.5f;

	[SerializeField]
	private bool isWriteRTL = false;

	[SerializeField]
	private float test;

	[Button("Write Text")]
	private void Button_WriteText()
    {
		StartWriting();
	}
	
	#endregion

	#region Variables
	public bool IsPaused { get; private set; }

	public bool IsWriteRTL { get { return isWriteRTL; } set { isWriteRTL = value; } }

	public bool IsWriting { get { return WriteTextEnumerator != null; } }

    private BText bText;

	private string originalText;

	#endregion

    #region Life Cycle
	protected override void OnValidate()
	{
		if (!CanValidate()) return;
		
		base.OnValidate();

		if (randomVariation >= characterTypeDelay)
		{
			randomVariation = characterTypeDelay / 10.0f;
		}
	}
	
	protected override void Awake()
	{
		base.Awake();

		bText = GetComponent<BText>();
		if (IS_NOT_NULL(bText))
		{
			originalText = bText.Text;
			bText.SetText("");
			bText.BUIElementShown += On_BText_BUIElementEnabled;
			bText.BUIElementHidden += On_BText_BUIElementDisabled;
		}
	}
	
	#endregion

    #region Events Callbacks
	private void On_BText_BUIElementEnabled(BUIElement obj)
	{
		if (startWritingWhenEnabled == true)
		{
			StartNewCoroutine(ref WriteTextEnumerator, WriteTextCoroutine());
		}
	}

	private void On_BText_BUIElementDisabled(BUIElement obj)
	{
		StopCoroutineIfRunning(ref WriteTextEnumerator);
	}
	
	#endregion

    #region Private Methods
	private IEnumerator WriteTextEnumerator;
	private IEnumerator WriteTextCoroutine()
	{
		bText.SetText("");

		// Start delay
		yield return new WaitForSeconds(startDelay);

		InvokeEventIfBound(StartedWriting, originalText);
		
		if (IsWriteRTL == true)
        {
			StartNewCoroutine(ref WriteRTLTextEnumerator, WriteRTLTextCoroutine());
		}
		else
        {
			StartNewCoroutine(ref WriteRegularTextEnumerator, WriteRegularTextCoroutine());
		}

		// Wait for started coroutines to finish
		while (WriteRTLTextEnumerator != null
			|| WriteRegularTextEnumerator != null)
        {
			yield return new WaitForEndOfFrame();
        }

		// Remove all instances from the special wait character from final string
		bText.SetText(bText.Text.Replace(specialWaitCharacter + "", ""));

		InvokeEventIfBound(FinishedWriting, originalText);

		WriteTextEnumerator = null;
	}

	private IEnumerator WriteRegularTextEnumerator;
	private IEnumerator WriteRegularTextCoroutine()
    {
		for (int i = 0; i < originalText.Length; i++)
		{
			char c = originalText[i];

			// Paused?
			if (IsPaused == true)
			{
				yield return new WaitForEndOfFrame();
			}

			// Enter press delay
			if (c == '\n')
			{
				yield return new WaitForSeconds(leaveSpaceDelay);
			}

			// Special wait character
			if (c == specialWaitCharacter)
			{
				yield return new WaitForSeconds(waitCharacterDuration);
			}
			else
			{
				// Write character
				bText.SetText(bText.Text + c);
			}

			InvokeEventIfBound(TextUpdated, bText.Text);

			// wait until next character
			yield return new WaitForSeconds(characterTypeDelay + UnityEngine.Random.Range(-randomVariation, randomVariation));
		}

		WriteRegularTextEnumerator = null;
	}
	
	private IEnumerator WriteRTLTextEnumerator;
	private IEnumerator WriteRTLTextCoroutine()
    {
		string[] splitTexts = originalText.Split('\n');

		string finalText = "";
		for (int stringIndex = 0; stringIndex < splitTexts.Length; stringIndex++)
		{
			string finalSplitText = "";
			string splitText = splitTexts[stringIndex];

			for (int charIndex = splitText.Length - 1; charIndex >= 0; charIndex--)
			{
				char c = splitText[charIndex];

				// Paused?
				if (IsPaused == true)
				{
					yield return new WaitForEndOfFrame();
				}

				// Special wait character
				if (c == specialWaitCharacter)
				{
					yield return new WaitForSeconds(waitCharacterDuration);
				}
				else
				{
					finalSplitText = c + finalSplitText;
					if (stringIndex > 0)
					{
						bText.SetText(finalText + '\n' + finalSplitText);
					}
					else
                    {
						bText.SetText(finalSplitText);
                    }
				}

				InvokeEventIfBound(TextUpdated, bText.Text);

				// wait until next character
				yield return new WaitForSeconds(characterTypeDelay + UnityEngine.Random.Range(-randomVariation, randomVariation));
			}

			if (stringIndex > 0)
			{
				finalText = finalText + '\n' + finalSplitText;
			}
			else
            {
				finalText = finalSplitText;
			}
			bText.SetText(finalText);
			yield return new WaitForSeconds(leaveSpaceDelay);
		}

		WriteRTLTextEnumerator = null;

	}
	#endregion

}