using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;

namespace BNJMO.Examples
{
    public class BFrameOnFocusColorChanger : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [SerializeField] private BFrame targetBFrame;
        [SerializeField] private BImage bImageColorToChange;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color focusedColor;

        #endregion

        #region Variables


        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            base.OnValidate();
            
            SetComponentIfNull(ref targetBFrame);
            SetComponentInChildrenIfNull(ref bImageColorToChange);

            if (bImageColorToChange)
            {
                defaultColor = bImageColorToChange.ImageColor;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            BEvents.UI_FocusedFrameUpdated += BEvents_UI_OnFocusedFrameUpdated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            BEvents.UI_FocusedFrameUpdated -= BEvents_UI_OnFocusedFrameUpdated;
        }

        #endregion

        #region Events Callbacks

        private void BEvents_UI_OnFocusedFrameUpdated(BEventHandle<BFrame> handle)
        {
            if (!bImageColorToChange)
                return;
            
            var newFocusedBFrame = handle.Arg1;
            bImageColorToChange.SetColor(newFocusedBFrame == targetBFrame ? focusedColor : defaultColor);
        }

        #endregion

        #region Others


        #endregion
    }
}
