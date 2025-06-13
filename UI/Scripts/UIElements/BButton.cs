using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
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
                buttonImage = newSprite;
                bImageReference.SetSprite(buttonImage);
            }
        }

        public void SetButtonText(string newText)
        {
            if (bTextReference)
            {
                buttonText = newText;
                bTextReference.SetText(buttonText);
            }
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
                bImageReference.SetSprite(buttonImage);
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
                    BManager.Inst.Config.LogInputButtonEvents);
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
                bImageReference.SetSprite(buttonImage);
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
                    BEventBroadcastType.LOCAL, BManager.Inst.Config.LogInputButtonEvents);
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
                    BManager.Inst.Config.LogInputButtonEvents);
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
                bImageReference.SetSprite(buttonImage);
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
                    BManager.Inst.Config.LogInputButtonEvents);
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
                bImageReference.SetSprite(buttonImage);
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
                bImageReference.SetSprite(buttonImage);
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

        [BoxGroup("BButton", centerLabel: true)] [SerializeField] private bool automaticallyFindChildBUIElements = true;
        [FormerlySerializedAs("isDisabled")] [BoxGroup("BButton")] [SerializeField] private bool isButtonDisabled = false;

        [Space(15)] 
        [FoldoutGroup("BButton/Text")] [SerializeField] [TextArea] private string buttonText = "Button";
        [FoldoutGroup("BButton/Text")] [SerializeField] private bool writeButtonTextUppercase = false;
        [FoldoutGroup("BButton/Text")] [SerializeField] private bool overrideUINameFromText = false;
        [FoldoutGroup("BButton/Text")] [SerializeField] private Color textNormalColor = Color.black;

        [FoldoutGroup("BButton/Text")] 
        [SerializeField] private Color textDisabledColor = Color.black;
        [FoldoutGroup("BButton/Text")] [SerializeField] private bool useTextSpecialColors = true;
        [FoldoutGroup("BButton/Text")] [SerializeField] [HideIf("@this.useTextSpecialColors == false")] private Color textHoveredColor = Color.black;
        [FoldoutGroup("BButton/Text")] [SerializeField] [HideIf("@this.useTextSpecialColors == false")] private Color textHighlightedColor = Color.black;
        [FoldoutGroup("BButton/Text")] [SerializeField] [HideIf("@this.useTextSpecialColors == false")] private Color textPressedColor = Color.black;
        [FoldoutGroup("BButton/Text")] [SerializeField] private BText bTextReference;
        [FoldoutGroup("BButton/Text")]
        [Button("Derive Button Name From UIElement Name")]
        private void Button_DeriveName()
        {
            buttonText = UIElementName;
            Revalidate();
        }

        [FoldoutGroup("BButton/Image")] [SerializeField] private Sprite buttonImage;
        [FoldoutGroup("BButton/Image")] [SerializeField] private bool useImagePressedSprite = false;
        [FoldoutGroup("BButton/Image")] [SerializeField] [HideIf("@this.useImagePressedSprite == false")]  private Sprite buttonPressedImage;
        [FoldoutGroup("BButton/Image")] [SerializeField] private Color imageNormalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        [FoldoutGroup("BButton/Image")] [SerializeField] private Color imageDisabledColor = new Color(1.0f, 1.0f, 1.0f, 0.125f);
        [FoldoutGroup("BButton/Image")] [SerializeField] private bool useImageSpecialColors = false;
        [FoldoutGroup("BButton/Image")] [SerializeField] [HideIf("@this.useImageSpecialColors == false")] private Color imageHoveredColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        [FoldoutGroup("BButton/Image")] [SerializeField] [HideIf("@this.useImageSpecialColors == false")] private Color imageHighlightedColor = new Color(0.61f, 0.96f, 1.0f, 1.0f);
        [FoldoutGroup("BButton/Image")] [SerializeField] [HideIf("@this.useImageSpecialColors == false")] private Color ImagePressedColor = new Color(0.46f, 0.67f, 0.69f, 1.0f);
        [FoldoutGroup("BButton/Image")] [SerializeField] private BImage bImageReference;

        [FoldoutGroup("BButton/Sounds")] [SerializeField] private AudioClip onPressedSound;
        [FoldoutGroup("BButton/Sounds")] [SerializeField] private AudioClip onSuccessfullyReleasedSound;

        [FoldoutGroup("BButton/Navigation")] [SerializeField] private BButton upperButton;
        [HorizontalGroup("BButton/Navigation/LeftRightRow")] [SerializeField] private BButton leftButton;
        [HorizontalGroup("BButton/Navigation/LeftRightRow")] [SerializeField] private BButton rightButton;
        [FormerlySerializedAs("buttomButton")] [FoldoutGroup("BButton/Navigation")] [SerializeField] private BButton bottomButton;

        [FoldoutGroup("BButton/Navigation")]
        [Button("Set Self On Reciprocal Buttons")]
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

        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonHighlightedUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonPressedUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonUnhighlightedUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonReleasedUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonCancelReleasedUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonHoveredEnterUEvent;
        [FoldoutGroup("BButton/Events")] [SerializeField] public UnityEvent ButtonHoveredExitUEvent;

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
        
        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks
    
      
        #endregion

        #region Others

        
        private bool isBeingPressed = false;

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
                bTextReference.WriteTextUppercase = writeButtonTextUppercase;
                bTextReference.SetText(buttonText);
                bTextReference.SetColor(textNormalColor);
                bTextReference.UIElementName = UIElementName;
            }

            // Update BImage
            if (bImageReference)
            {
                bImageReference.SetSprite(buttonImage);
                bImageReference.SetColor(imageNormalColor);
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
