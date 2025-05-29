using UnityEngine;
using System;
using System.Collections.Generic;

namespace BNJMO
{
    public class BUIManager : AbstractSingletonManager<BUIManager>
    {
        public BFrame CurrentBFrameFocused          { get; private set; }
        public BMenu CurrentBMenuHighlighted        { get; private set; }
        public BButton CurrentBButtonHighlighted    { get; private set; }

        private bool canPressButton;

        protected override void Start()
        {
            base.Start();

            BEvents.UI_FocusedFrameUpdated += On_UI_FocusedFrameUpdated;
            BEvents.UI_HighlightedBMenuUpdated += On_UI_HighlightedBMenuUpdated;     
            BEvents.UI_ButtonHighlighted += On_UI_ButtonHighlighted;

            BEvents.INPUT_ButtonPressed += On_INPUT_ButtonPressed;
            BEvents.INPUT_ButtonReleased += On_INPUT_ButtonReleased;
        }

        #region Events Callbacks

        private void On_UI_FocusedFrameUpdated(BEventHandle<BFrame> bEHandle)
        {
            CurrentBFrameFocused = bEHandle.Arg1;
        }

        private void On_UI_HighlightedBMenuUpdated(BEventHandle<BMenu, BMenu> bEHandle)
        {
            CurrentBMenuHighlighted = bEHandle.Arg1;
        }

        private void On_UI_ButtonHighlighted(BEventHandle<BButton> bEHandle)
        {
            CurrentBButtonHighlighted = bEHandle.Arg1;
        }

        private void On_INPUT_ButtonPressed(BEventHandle<EControllerID, EControllerType, EInputButton> eventHandle)
        {
            EInputButton inputButton = eventHandle.Arg3;

            if (CurrentBButtonHighlighted)
            {
                BButton nextButton = null;
                switch (inputButton)
                {
                    case EInputButton.CONFIRM:
                        CurrentBButtonHighlighted.OnPressed();
                        canPressButton = true;
                        break;

                    case EInputButton.LEFT:
                        nextButton = CurrentBButtonHighlighted.GetNextButton(EButtonDirection.LEFT);
                        break;

                    case EInputButton.RIGHT:
                        nextButton = CurrentBButtonHighlighted.GetNextButton(EButtonDirection.RIGHT);
                        break;

                    case EInputButton.UP:
                        nextButton = CurrentBButtonHighlighted.GetNextButton(EButtonDirection.UP);
                        break;

                    case EInputButton.DOWN:
                        nextButton = CurrentBButtonHighlighted.GetNextButton(EButtonDirection.DOWN);
                        break;
                }

                // Update highlighted button
                if (nextButton != null)
                {
                    CurrentBButtonHighlighted.OnUnhighlighted();
                    CurrentBButtonHighlighted = nextButton;
                    nextButton.OnHighlighted();
                    canPressButton = false;
                }
            }
        }

        private void On_INPUT_ButtonReleased(BEventHandle<EControllerID, EControllerType, EInputButton> eventHandle)
        {
            EInputButton inputButton = eventHandle.Arg3;

            if (CurrentBButtonHighlighted)
            {
                switch (inputButton)
                {
                    case EInputButton.CONFIRM:
                        if (canPressButton == true)
                        {
                            CurrentBButtonHighlighted.OnReleased(true);
                        }
                        break;
                }
            }
        }

        #endregion



    }
}
