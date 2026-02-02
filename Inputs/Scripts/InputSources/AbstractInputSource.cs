using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BNJMO
{
    public abstract class AbstractInputSource : BBehaviour
    {
        public event Action<EControllerID, EInputButton> ButtonPressed;
        public event Action<EControllerID, EInputButton> ButtonReleased;
        public event Action<EControllerID, EInputAxis, float, float> AxisUpdated;

        public abstract void Rumble(EControllerID controllerID, float lowFreq, float highFreq, float duration);
        
        protected void InvokeButtonPressed(EControllerID controllerID, EInputButton inputButton)
        {
            if (ButtonPressed != null)
            {
                ButtonPressed.Invoke(controllerID, inputButton);
            }
        }

        protected void InvokeButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            if (ButtonReleased != null)
            {
                ButtonReleased.Invoke(controllerID, inputButton);
            }
        }

        protected void InvokeAxisUpdated(EControllerID controllerID, EInputAxis inputAxis, float x, float y)
        {
            if (AxisUpdated != null)
            {
                AxisUpdated.Invoke(controllerID, inputAxis, x, y);
            }
        }
    }
}
