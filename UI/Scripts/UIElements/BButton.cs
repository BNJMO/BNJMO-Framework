using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace BNJMO
{
    [SelectionBase]
    public class BButton : BUIElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Public Events

        public event Action<BButton> Highlighted;
        public event Action<BButton> Pressed;
        public event Action<BButton> Unhighlighted;
        public event Action<BButton, bool> Released;
        public event Action<BButton> HoveredEnter;
        public event Action<BButton> HoveredExit;
        public event Action<BButton> Enabled;
        public event Action<BButton> Disabled;

        #endregion

        #region Public Methods
        
        /* Button Enability */
        public void EnableButton(bool forceEnable = false)
        {
            if (IsButtonDisabled
                || forceEnable)
            {
                IsButtonDisabled = false;

                if (bImageReference)
                {
                    bImageReference.SetColor(imageNormalColor);
                }

                if (bTextReference)
                {
                    bTextReference.SetColor(textNormalColor);
                }

                SetRaycast(true);

                InvokeEventIfBound(Enabled, this);
            }
        }

        public void DisableButton(bool forceDisable = false)
        {
            if (IsButtonDisabled == false
                || forceDisable)
            {
                IsButtonDisabled = true;

                if (bImageReference)
                {
                    bImageReference.SetColor(imageDisabledColor);
                }

                if (bTextReference)
                {
                    bTextReference.SetColor(textDisabledColor);
                }

                SetRaycast(false);

                InvokeEventIfBound(Disabled, this);
            }
        }

        /* Button Member Setters/Getters */
        public void SetButtonSprite(Sprite newSprite)
        {
            if (bImageReference)
            {
                buttonSprite = newSprite;
                bImageReference.SetSprite(buttonSprite);
            }
        }

        public void SetButtonLocalizedSprite(LocalizedSprite newLocalizedSprite)
        {
            imageLocalizedSprite = newLocalizedSprite;
            bImageReference.SetLocalizedSprite(newLocalizedSprite);
        }

        public void SetButtonImageColor(Color newColor)
        {
            if (bImageReference)
            {
                imageNormalColor = newColor;
                bImageReference.SetColor(imageNormalColor);
            }
        }
        
        public void SetButtonImageOpacity(float newOpacity)
        {
            if (bImageReference)
            {
                imageNormalColor = new Color(imageNormalColor.r, imageNormalColor.g, imageNormalColor.b, newOpacity);
                bImageReference.SetColor(imageNormalColor);
            }
        }

        public void SetButtonText(string newText)
        {
            if (!bTextReference) 
                return;
            
            buttonText = newText;
            bTextReference.SetText(buttonText);
        }

        public void SetButtonTextFontSize(float newFontSize)
        {
            if (!bTextReference)
                return;
            
            bTextReference.SetFontsize(textFontSize);
        }
        
        public void SetButtonLocalizedString(LocalizedString newLocalizedString)
        {
            textLocalizedString = newLocalizedString;
            bTextReference.SetLocalizedText(textLocalizedString);
        }
        
        public void SetButtonTextFont(TMP_FontAsset newFontAsset)
        {
            defaultFontAsset = newFontAsset;
            if (bTextReference)
            {
                bTextReference.SetTMPFontAsset(newFontAsset);
            }
        }
        
        public void SetButtonTextColor(Color newColor)
        {
            if (bTextReference)
            {
                textNormalColor = newColor;
                bTextReference.SetColor(newColor);
            }
        }
        
        public void SetButtonTextOpacity(float newOpacity)
        {
            if (bTextReference)
            {
                textNormalColor = new Color(textNormalColor.r, textNormalColor.g, textNormalColor.b, newOpacity);
                bTextReference.SetColor(textNormalColor);
            }
        }

        public void SetRaycast(bool isEnabled)
        {
            if (BImage)
            {
                BImage.SetRaycast(isEnabled);
            }
        }

        public bool GetRaycast()
        {
            if (BImage)
            {
                return BImage.GetRaycast();
            }
            return false;
        }

        /* Button Pointer Interface */ 
        public void OnHighlighted()
        {
            if (IsButtonDisabled)
            {
                return;
            }

            IsHighlighted = true;

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonSprite);
            }

            if (bImageReference
                && useImageSpecialColors)
            {
                bImageReference.SetColor(imageHighlightedColor);
            }

            if (bTextReference
                && useTextSpecialColors)
            {
                bTextReference.SetColor(textHighlightedColor);
            }

            if (parentBMenu)
            {
                parentBMenu.OnBButtonHighlighted(this);
            }

            InvokeEventIfBound(Highlighted, this);

            if (ButtonHighlightedUEvent != null)
            {
                ButtonHighlightedUEvent.Invoke();
            }

            if (Application.isPlaying
                && BManager.Inst)
            {
                BEvents.UI_ButtonHighlighted.Invoke(new BEventHandle<BButton>(this), BEventBroadcastType.LOCAL, 
                    BConfig.Inst.LogUIBEvents && BConfig.Inst.LogBButtonBEvents);
            }
        }

        public void OnReleased(bool cursorInside)
        {
            if (IsButtonDisabled
                || isBeingPressed == false)
            {
                return;
            }

            isBeingPressed = false;

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonSprite);
            }

            if (bImageReference
                && useImageSpecialColors)
            {
                bImageReference.SetColor(imageHighlightedColor);
            }

            if (bTextReference
                && useTextSpecialColors)
            {
                bTextReference.SetColor(textHighlightedColor);
            }

            if ((onPressedSound)
                && (cursorInside))
            {
                BAudioManager.SpawnSoundObject(onSuccessfullyReleasedSound);
            }

            InvokeEventIfBound(Released, this, cursorInside);

            if (ButtonReleasedUEvent != null)
            {
                ButtonReleasedUEvent.Invoke();
            }

            if (Application.isPlaying
                && BManager.Inst)
            {
                BEvents.UI_ButtonReleased.Invoke(new BEventHandle<BButton, bool>(this, cursorInside), 
                    BConfig.Inst.LogUIBEvents && BConfig.Inst.LogBButtonBEvents);
            }
        }

        public void OnPressed()
        {
            if (IsButtonDisabled)
            {
                return;
            }

            isBeingPressed = true;

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonPressedImage);
            }

            if (bImageReference
                && useImageSpecialColors)
            {
                bImageReference.SetColor(ImagePressedColor);
            }

            if (bTextReference
                && useTextSpecialColors)
            {
                bTextReference.SetColor(textPressedColor);
            }

            if (onPressedSound)
            {
                BAudioManager.SpawnSoundObject(onPressedSound);
            }

            if (parentBMenu)
            {
                parentBMenu.OnBButtonPressed(this);
            }

            InvokeEventIfBound(Pressed, this);

            if (ButtonPressedUEvent != null)
            {
                ButtonPressedUEvent.Invoke();
            }

            if (Application.isPlaying
                && BManager.Inst)
            {
                BEvents.UI_ButtonPressed.Invoke(new BEventHandle<BButton>(this), BEventBroadcastType.LOCAL,
                    BConfig.Inst.LogUIBEvents && BConfig.Inst.LogBButtonBEvents);
            }
        }

        public void OnUnhighlighted()
        {
            if (IsButtonDisabled)
            {
                return;
            }

            IsHighlighted = false;

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonSprite);
            }

            if (bImageReference)
            {
                bImageReference.SetColor(imageNormalColor);
            }

            if (bTextReference)
            {
                bTextReference.SetColor(textNormalColor);
            }

            InvokeEventIfBound(Unhighlighted, this);

            if (ButtonUnhighlightedUEvent != null)
            {
                ButtonUnhighlightedUEvent.Invoke();
            }

            if (Application.isPlaying
                && BManager.Inst)
            {
                BEvents.UI_ButtonUnhighlighted.Invoke(new BEventHandle<BButton>(this), BEventBroadcastType.LOCAL,
                    BConfig.Inst.LogUIBEvents && BConfig.Inst.LogBButtonBEvents);
            }
        }

        public void OnHoveredEnter()
        {
            if (IsButtonDisabled)
            {
                return;
            }

            OnHighlighted();

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonSprite);
            }

            if (bImageReference
                && useImageSpecialColors)
            {
                bImageReference.SetColor(imageHoveredColor);
            }

            if (bTextReference
                && useTextSpecialColors)
            {
                bTextReference.SetColor(textHoveredColor);
            }

            InvokeEventIfBound(HoveredEnter, this);

            if (ButtonHoveredEnterUEvent != null)
            {
                ButtonHoveredEnterUEvent.Invoke();
            }
        }

        public void OnHoveredExit()
        {
            if (IsButtonDisabled)
            {
                return;
            }

            isBeingPressed = false;

            if (bImageReference
                && useImagePressedSprite)
            {
                bImageReference.SetSprite(buttonSprite);
            }

            if (bImageReference
                && useImageSpecialColors)
            {
                bImageReference.SetColor(imageHighlightedColor);
            }

            if (bTextReference
                && useTextSpecialColors)
            {
                bTextReference.SetColor(textHighlightedColor);
            }

            InvokeEventIfBound(HoveredExit, this);

            if (ButtonHoveredExitUEvent != null)
            {
                ButtonHoveredExitUEvent.Invoke();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressed();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            bool cursorInside = eventData.pointerCurrentRaycast.gameObject
                                 && eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<BButton>() == this;
            OnReleased(cursorInside);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHoveredEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoveredExit();
        }
        
        #endregion

        #region Inspector Variables
        [BoxGroup("BButton", centerLabel: true)]
        
        [SerializeField] 
        private bool automaticallyFindChildBUIElements = true;
        
        [SerializeField] [FormerlySerializedAs("isDisabled")] [BoxGroup("BButton")]
        private bool isButtonDisabled = false;
        
        [FoldoutGroup("BButton/Settings")]
        /* Image */
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")]
        private BImage bImageReference;
        
        [Header("Sprite")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")]
        private bool useImageLocalization = false;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")] [FormerlySerializedAs("buttonImage")] [ShowIf("@useImageLocalization == false")]
        private Sprite buttonSprite;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")] [FormerlySerializedAs("localizedSprite")] [ShowIf("@useImageLocalization == true")]
        private LocalizedSprite imageLocalizedSprite;

        [SerializeField] [FoldoutGroup("BButton/Settings/Image")]        // TODO: Add localization for pressed Sprite
        private bool useImagePressedSprite = false;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")] [HideIf("@this.useImagePressedSprite == false")]
        private Sprite buttonPressedImage;
        
        [Header("Colors")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")]
        private Color imageNormalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image/More Colors")]
        private Color imageDisabledColor = new Color(1.0f, 1.0f, 1.0f, 0.125f);
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image/More Colors")]
        private bool useImageSpecialColors = false;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image/More Colors")] [HideIf("@this.useImageSpecialColors == false")]
        private Color imageHoveredColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image/More Colors")] [HideIf("@this.useImageSpecialColors == false")]
        private Color imageHighlightedColor = new Color(0.61f, 0.96f, 1.0f, 1.0f);
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Image/More Colors")] [HideIf("@this.useImageSpecialColors == false")]
        private Color ImagePressedColor = new Color(0.46f, 0.67f, 0.69f, 1.0f);
        
        [Header("Match Parent Size")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Image")]
        private bool imageMatchesParentSize = false;
        
        [Button("Match Image Parent Size")] [FoldoutGroup("BButton/Settings/Image")] 
        private void MatchImageParentSize_Button() => MatchImageParentSize();

        /* Text */
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private BText bTextReference;
        
        [Header("Text")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private bool useTextLocalization = false;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")] [HideIf("@this.useTextLocalization == true")] [TextArea]
        private string buttonText = "Button";
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")] [ShowIf("useTextLocalization")]
        private LocalizedString textLocalizedString;
                             
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private TMP_FontAsset defaultFontAsset;

        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private float textFontSize = 0;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private bool writeButtonTextUppercase = false;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private bool overrideUINameFromText = false;

        [Header("Colors")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private Color textNormalColor = Color.black;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text/More Colors")]
        private Color textDisabledColor = Color.black;

        [SerializeField] [FoldoutGroup("BButton/Settings/Text/More Colors")]
        private bool useTextSpecialColors = true;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text/More Colors")] [HideIf("@this.useTextSpecialColors == false")]
        private Color textHoveredColor = Color.black;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text/More Colors")] [HideIf("@this.useTextSpecialColors == false")]
        private Color textHighlightedColor = Color.black;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Text/More Colors")] [HideIf("@this.useTextSpecialColors == false")]
        private Color textPressedColor = Color.black;
        
        [Header("Match Parent Size")]
        [SerializeField] [FoldoutGroup("BButton/Settings/Text")]
        private bool textMatchesParentSize = false;
        
        [Button("Match Text Parent Size")] [FoldoutGroup("BButton/Settings/Text")] 
        private void MatchTextParentSize_Button() => MatchTextParentSize();
        
        [Button("Derive Button Name From UIElement Name")] [FoldoutGroup("BButton/Settings/Text")] 
        private void DeriveName_Button() => DeriveName();

        /* Sounds */
        [SerializeField] [FoldoutGroup("BButton/Settings/Sounds")]
        private AudioClip onPressedSound;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Sounds")]
        private AudioClip onSuccessfullyReleasedSound;

        /* Navigations */
        [SerializeField] [FoldoutGroup("BButton/Settings/Navigation")]
        private BButton upperButton;
        
        [SerializeField] [HorizontalGroup("BButton/Settings/Navigation/LeftRightRow")]
        private BButton leftButton;
        
        [SerializeField] [HorizontalGroup("BButton/Settings/Navigation/LeftRightRow")]
        private BButton rightButton;
        
        [SerializeField] [FormerlySerializedAs("buttomButton")] [FoldoutGroup("BButton/Settings/Navigation")]
        private BButton bottomButton;

        [FoldoutGroup("BButton/Settings/Navigation")]
        [Button("Set Self On Reciprocal Buttons")]
        private void SetSelfOnReciprocalButtons_Button() => SetSelfOnReciprocalButtons();
        
        /* Events */
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonHighlightedUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonPressedUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonUnhighlightedUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonReleasedUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonCancelReleasedUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonHoveredEnterUEvent;
        
        [SerializeField] [FoldoutGroup("BButton/Settings/Events")]
        public UnityEvent ButtonHoveredExitUEvent;

        #endregion

        #region Variables
        
        public bool IsHighlighted { get; private set; }
        
        public bool IsButtonDisabled { get { return isButtonDisabled; } private set { isButtonDisabled = value; } }
        
        public bool UseImageSpecialColors { get { return useImageSpecialColors; } set { useImageSpecialColors = value; } }
        
        public BImage BImage { get { return bImageReference; } }
        
        public BText BText { get { return bTextReference; } }
        
        public BButton LeftButton { get { return leftButton; } set { leftButton = value; } }
        
        public BButton RightButton { get { return rightButton; } set { rightButton = value; } }
        
        public BButton UpperButton { get { return upperButton; } set { upperButton = value; } }
        
        public BButton BottomButton { get { return bottomButton; } set { bottomButton = value; } }
        
        private bool isBeingPressed = false;
        
        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "B_";

            if (overrideUINameFromText)
            {
                UIElementName = buttonText;
            }

            base.OnValidate();

            // Update Children BUIElements references
            if (automaticallyFindChildBUIElements)
            {
                SetComponentIfNull(ref bTextReference);
                SetComponentIfNull(ref bImageReference); 
            }

            // Update BText
            if (bTextReference)
            {
                if (buttonText == "")
                {
                    buttonText = bTextReference.Text;
                }
                if (useTextLocalization == false)
                {
                    bTextReference.SetText(buttonText);
                }
                
                if (defaultFontAsset == null)
                {
                    defaultFontAsset = bTextReference.TextTMPFont;
                }
                bTextReference.SetTMPFontAsset(defaultFontAsset);

                bTextReference.UseLocalization = useTextLocalization;
                if (textLocalizedString == null)
                {
                    textLocalizedString = bTextReference.LocalizedString;
                }
                bTextReference.SetLocalizedText(textLocalizedString);

                if (textFontSize <= 0.0f)
                {
                    textFontSize = bTextReference.FontSize;
                }
                if (textFontSize >= 0.0f)
                {
                    SetButtonTextFontSize(textFontSize);
                }
                
                bTextReference.SetColor(textNormalColor);
                bTextReference.SetTextUppercase(writeButtonTextUppercase);
                bTextReference.SetMatchParentSize(textMatchesParentSize);
                bTextReference.UIElementName = UIElementName;
            }

            // Update BImage
            if (bImageReference)
            {
                if (buttonSprite == null)
                {
                    buttonSprite = bImageReference.Sprite;
                }
                bImageReference.SetSprite(buttonSprite);

                bImageReference.UseLocalization = useImageLocalization;
                if (imageLocalizedSprite == null)
                {
                    imageLocalizedSprite = bImageReference.LocalizedSprite;
                }
                bImageReference.SetLocalizedSprite(imageLocalizedSprite);
                
                bImageReference.SetColor(imageNormalColor);
                bImageReference.SetMatchParentSize(imageMatchesParentSize);
                bImageReference.UIElementName = UIElementName;
            }

            if (isButtonDisabled)
            {
                DisableButton(true);
            }
            else
            {
                EnableButton(true);
            }
        }

        #endregion

        #region Events Callbacks
    
      
        #endregion

        #region Others
    
        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            IsHighlighted = false;

            if (IsButtonDisabled)
            {
                return;
            }

            if (bImageReference)
            {
                bImageReference.SetColor(imageNormalColor);
            }
        }

        private void DeriveName()
        {
            buttonText = UIElementName;
            Revalidate();
        }
        
        private void SetSelfOnReciprocalButtons()
        {
            if (UpperButton)
            {
                UpperButton.BottomButton = this;
            }
            if (BottomButton)
            {
                BottomButton.UpperButton = this;
            }
            if (LeftButton)
            {
                LeftButton.RightButton = this;
            }
            if (RightButton)
            {
                RightButton.LeftButton = this;
            }
        }
        
        private void MatchImageParentSize()
        {
            bImageReference?.MatchParentSize();
        }

        private void MatchTextParentSize()
        {
            bTextReference?.MatchParentSize();
        }

        #endregion
        
        #region Button Press
       
        #endregion

        #region Navigation
        
        public BButton GetNextButton(EButtonDirection buttonDirection)
        {
            BButton buttonToReturn = null;
            switch (buttonDirection)
            {
                case EButtonDirection.LEFT:
                    buttonToReturn = leftButton;
                    break;

                case EButtonDirection.RIGHT:
                    buttonToReturn = rightButton;
                    break;

                case EButtonDirection.UP:
                    buttonToReturn = upperButton;
                    break;

                case EButtonDirection.DOWN:
                    buttonToReturn = bottomButton;
                    break;
            }
            return buttonToReturn;
        }

        public void UnPopulateNavigationButtons()
        {
            LogConsole("Unpopulating buttons");
            LeftButton = null;
            RightButton = null;
            UpperButton = null;
            BottomButton = null;
        }
        #endregion
    }
}
