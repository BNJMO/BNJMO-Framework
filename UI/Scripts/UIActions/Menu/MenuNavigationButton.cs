using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace BNJMO
{
    [RequireComponent(typeof(BButton))]
    public class MenuNavigationButton : BBehaviour
    { 
        private string infoToMenuReferenceNotSet = "To Menu reference not set!";
        private bool showToMenuReferenceNotSet = false;
        [BoxGroup("MenuNavigationButton", centerLabel: true)] [InfoBox("$infoToMenuReferenceNotSet", InfoMessageType.Error, "showToMenuReferenceNotSet")] 
        [BoxGroup("MenuNavigationButton")] [SerializeField] [SceneObjectsOnly]  private BMenu toBMenu;
        [BoxGroup("MenuNavigationButton")] [SerializeField] private bool overrideBButtonUIName = false;
        [BoxGroup("MenuNavigationButton")] [ShowInInspector] [ReadOnly] private BButton bButtonReference;

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            bButtonReference = GetComponent<BButton>();
            if (bButtonReference)
            {
                if (toBMenu)
                {
                    showToMenuReferenceNotSet = false;

                    if (overrideBButtonUIName == true)
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
        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            bButtonReference = GetComponent<BButton>();
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            if (IS_NOT_NULL(bButtonReference))
            {
                bButtonReference.Released += On_ButtonReleased;
            }
        }

        private void On_ButtonReleased(BButton bButton, bool cursorInside)
        {
            if (cursorInside
                && IS_NOT_NULL(toBMenu))
            {
                toBMenu.HighlightBMenu();
            }
        }
    }
}
