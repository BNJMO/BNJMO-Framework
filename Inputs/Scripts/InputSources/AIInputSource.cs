using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AIInputSource : AbstractInputSource
    {
        #region Public Events


        #endregion

        #region Public Methods

        public void UpdateAxis(EControllerID controllerID, EInputAxis axisInput, float x, float y)
        {
            InvokeAxisUpdated(controllerID, axisInput, x, y);
        }

        public void PressButton(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonPressed(controllerID, inputButton);
        }

        public void ReleaseButton(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonReleased(controllerID, inputButton);
        }
        
        public override void Rumble(EControllerID controllerID, float lowFreq, float highFreq, float duration)
        {
        }

        #endregion

        #region Inspector Variables

        
        #endregion
        
        #region Variables
        

        #endregion

        #region Life Cycle

        
        #endregion

        #region Events Callbacks
       

        #endregion

        #region Others


        #endregion
    }
}