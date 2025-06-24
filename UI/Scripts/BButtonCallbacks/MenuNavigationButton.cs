using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace BNJMO
{
    [RequireComponent(typeof(BButton))]
    public class BMenuNavigationButton : BBehaviour
    { 
        #region Public Events


        #endregion

        #region Public Methods

        public void NavigateToMenu()
        {
            if (IS_NOT_NULL(toBMenu))
            {
                toBMenu.HighlightBMenu();
            }
        }

        #endregion

        #region Inspector Variables

        [BoxGroup("MenuNavigationButton", centerLabel: true)] 
        [InfoBox("$infoToMenuReferenceNotSet", InfoMessageType.Error, "showToMenuReferenceNotSet")] 
        [SerializeField] [BoxGroup("MenuNavigationButton")]  [SceneObjectsOnly]  
        private BMenu toBMenu;
        
        [SerializeField] [BoxGroup("MenuNavigationButton")]  
        private bool navigateOnButtonPress = true;
        
        [SerializeField] [BoxGroup("MenuNavigationButton")]  
        private bool overrideBButtonUIName;
        
        [ShowInInspector] [BoxGroup("MenuNavigationButton")] [ReadOnly] 
        private BButton bButtonReference;

        #endregion

        #region Variables

        private string infoToMenuReferenceNotSet = "To Menu reference not set!";
        private bool showToMenuReferenceNotSet;
        
        #endregion

        #region Life Cycle
        
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!CanValidate()) 
                return;

            SetComponentIfNull(ref bButtonReference);

            if (!bButtonReference) 
                return;
            
            if (toBMenu)
            {
                showToMenuReferenceNotSet = false;

                if (overrideBButtonUIName)
                {
                    bButtonReference.UIElementName = "To_M_" + toBMenu.UIElementName;
                }
            }
            else
            {
                showToMenuReferenceNotSet = true;
                LogConsoleError(infoToMenuReferenceNotSet);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (bButtonReference)
            {
                bButtonReference.Released += Button_OnReleased;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (bButtonReference)
            {
                bButtonReference.Released -= Button_OnReleased;
            }
        }


        #endregion

        #region Events Callbacks
        
        private void Button_OnReleased(BButton bButton, bool cursorInside)
        {
            if (cursorInside == false
                || navigateOnButtonPress == false)
                return;
            
            NavigateToMenu();
        }

        #endregion

        #region Others
        
        
        #endregion
    }
}
