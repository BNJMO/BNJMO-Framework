using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace BNJMO
{
    public class BMenu : BUIElement
    {
        #region Public Events

        public event Action<BMenu> HighlightEnter;
        public event Action<BMenu> HighlightExit;

        #endregion

        #region Public Methods


        public void HighlightBMenu()
        {
            if (IS_VALID(parentBFrame))
            {
                parentBFrame.UpdateHighlightedBMenu(this);
            }
        }

        public void OnHighlighted()
        {
            IsActive = true;

            ShowUI();

            // Highlighted BBUtton
            if (startHighlightedBButton)
            {
                startHighlightedBButton.OnHighlighted();
                highlightedBButtonReference = startHighlightedBButton;
            }
            
            HighlightEnter?.Invoke(this);
        }
                
        public void OnHighlightExit()
        {
            IsActive = false;

            HideUI();
            
            HighlightExit?.Invoke(this);
        }

        public void OnBButtonPressed(BButton bButton)
        {
            if (IS_NOT_NULL(bButton))
            {
                if (bButton != highlightedBButtonReference)
                {
                    if (highlightedBButtonReference)
                    {
                        highlightedBButtonReference.OnUnhighlighted();
                    }
                    highlightedBButtonReference = bButton;
                }
            }
        }

        public void OnBButtonHighlighted(BButton bButton)
        {
            if (IS_NOT_NULL(bButton))
            {
                if (bButton != highlightedBButtonReference)
                {
                    if (highlightedBButtonReference)
                    {
                        highlightedBButtonReference.OnUnhighlighted();
                    }
                    highlightedBButtonReference = bButton;
                }
            }
        }

        public void SetStartHighlightedBButton(BButton bButton)
        {
            if (IS_NOT_NULL(bButton))
            {
                if (highlightedBButtonReference)
                {
                    highlightedBButtonReference.OnUnhighlighted();
                }

                startHighlightedBButton = bButton;
                highlightedBButtonReference = bButton;

                highlightedBButtonReference.OnHighlighted();
            }
        }

        #endregion

        #region Inspector Variables
        
        [BoxGroup("BMenu", centerLabel: true)]
        [BoxGroup("BMenu")] 
        [Button("Highlight this BMenu")] private void Button_HighlightThis()
        {
            if (parentBFrame)
            {
                parentBFrame.UpdateHighlightedBMenu(this);
            }
        }
        
        [SerializeField] [ChildGameObjectsOnly] 
        [InfoBox("$infoNoStartBButtonHighlight", InfoMessageType.Info, "showNoStartButtonHighlight")]
        private BButton startHighlightedBButton;
        
        [Title("Debugging")]
        [SerializeField] [BoxGroup("BMenu")] [ReadOnly] 
        private BButton highlightedBButtonReference;
        
        [SerializeField] [BoxGroup("BMenu")]  [ReadOnly] 
        private bool isActive;
        
        [BoxGroup("BMenu")] [TableList(DrawScrollView = true)] 
        public List<BBMenuChildBButton> childrenBButtonsList = new ();

        #endregion

        #region Variables

        public bool IsActive 
        { 
            get => isActive;
            private set => isActive = value;
        }

        private string infoNoStartBButtonHighlight = "You might need to select one of the children BButton as Start Hihghlight.";
        private bool showNoStartButtonHighlight = false;
        private BButton[] childrenBButtons = new BButton[0];
        
        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "M_";

            base.OnValidate();

            // Get all children BMenu
            childrenBButtons = GetComponentsInChildren<BButton>();
            childrenBButtonsList = new List<BBMenuChildBButton>();
            foreach (BButton bButton in childrenBButtons)
            {
                childrenBButtonsList.Add(new BBMenuChildBButton(bButton, this));
            }


            // Check Start Highlight BMenu
            highlightedBButtonReference = null;
            if (startHighlightedBButton)
            {
                showNoStartButtonHighlight = false;
                highlightedBButtonReference = startHighlightedBButton;
            }
            else
            {
                if ((childrenBButtons.Length > 0)
                    && (childrenBButtons[0]))
                {
                    startHighlightedBButton = childrenBButtons[0];
                    showNoStartButtonHighlight = false;
                    highlightedBButtonReference = startHighlightedBButton;
                }
                else
                {
                    showNoStartButtonHighlight = true;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            highlightedBButtonReference = startHighlightedBButton;
        }


        #endregion

        #region Events Callbacks

        protected override void OnUIHidden()
        {
            IsActive = false;
        }

        #endregion

        #region Others


        #endregion
    }

    [Serializable]
    public class BBMenuChildBButton
    {
        public BBMenuChildBButton(BButton bButton, BMenu bMenu)
        {
            childBButton = bButton;
            bMenuReference = bMenu;
        }

        [SerializeField] [ReadOnly] BButton childBButton;
        [TableColumnWidth(90, resizable: false)]
        [Button("Set"), VerticalGroup("Start Highlight")]
        public void Button_StartHighlight()
        {
            if (bMenuReference)
            {
                bMenuReference.SetStartHighlightedBButton(childBButton);
            }
        }

        private BMenu bMenuReference;
    }

}
