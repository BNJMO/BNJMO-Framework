using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

namespace BNJMO.Examples
{
    public class BFrameExit : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [SerializeField] private BFrame targetBFrame;
        [SerializeField] private EInputButton exitButton;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref targetBFrame);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            BEvents.INPUT_ButtonPressed += BEvents_INPUT_OnButtonPressed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            BEvents.INPUT_ButtonPressed -= BEvents_INPUT_OnButtonPressed;
        }

        #endregion

        #region Events Callbacks

        private void BEvents_INPUT_OnButtonPressed(BEventHandle<EControllerID, EControllerType, EInputButton> handle)
        {
            if (handle.Arg3 != exitButton)
                return;
            
            targetBFrame?.SetFocus();
        }

        #endregion

        #region Others


        #endregion
    }
}
