using System.Collections.Generic;
using UnityEngine;
using System;

namespace BNJMO
{
    public class TouchJoystickInputSource : AbstractInputSource
    {
        private Dictionary<EInputAxis, Vector2> oldInputValuesMap = new Dictionary<EInputAxis, Vector2>();
        private Dictionary<BTouchJoystick, bool> registredTouchControllerMap = new Dictionary<BTouchJoystick, bool>();

        public void RegisterTouchController(BTouchJoystick touchJoystick)
        {
            if ((IS_NOT_NULL(touchJoystick))
                && (registredTouchControllerMap.ContainsKey(touchJoystick) == false))
            {
                registredTouchControllerMap.Add(touchJoystick, true);
                touchJoystick.TouchJoystickPressed += On_TouchJoystick_Pressed;
                touchJoystick.TouchJoystickMoved += On_TouchJoystick_Moved;
                touchJoystick.TouchJoystickReleased += On_TouchJoystick_Released;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            BTouchJoystick[] touchJoysticks = FindObjectsOfType<BTouchJoystick>();
            foreach (BTouchJoystick touchJoystick in touchJoysticks)
            {
                if (registredTouchControllerMap.ContainsKey(touchJoystick) == false)
                {
                    registredTouchControllerMap.Add(touchJoystick, true);
                    touchJoystick.TouchJoystickPressed += On_TouchJoystick_Pressed;
                    touchJoystick.TouchJoystickMoved += On_TouchJoystick_Moved;
                    touchJoystick.TouchJoystickReleased += On_TouchJoystick_Released;
                }
            }

            // Initialize old input axis map
            foreach (EInputAxis inputAxis in Enum.GetValues(typeof(EInputAxis)))
            {
                if (inputAxis != EInputAxis.NONE)
                {
                    oldInputValuesMap.Add(inputAxis, Vector2.zero);
                }
            }
        }

        private void On_TouchJoystick_Pressed(EInputAxis inputAxis, EInputButton inputButton)
        {
            if (inputButton != EInputButton.NONE)
            {
                InvokeButtonPressed(EControllerID.TOUCH_1, inputButton);
            }
        }

        private void On_TouchJoystick_Moved(EInputAxis inputAxis, EInputButton inputButton, Vector2 newInputValues)
        {
            if ((inputAxis != EInputAxis.NONE)
                && (IS_KEY_CONTAINED(oldInputValuesMap, inputAxis)))
                //&& (Mathf.Abs(newInputValues.x) + Mathf.Abs(newInputValues.y) > BConsts.JOYSTICK_DEAD_ZONE))
            {
                Vector2 oldInputValues = oldInputValuesMap[inputAxis];
                float inputDistance = Vector2.Distance(oldInputValues, newInputValues);

                // Is new joystick input different enough from last registred one?
                //if (inputDistance > BConsts.THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                //{
                    //newInputValues.Normalize();
                    float newX = newInputValues.x;
                    float newY = newInputValues.y;

                    if (BManager.Inst.Config.TransformInpuAxisToCameraDirection == true)
                    {
                        BUtils.TransformAxisToCamera(ref newX, ref newY, Camera.main.transform.forward);
                    }

                    oldInputValuesMap[inputAxis] = new Vector2(newX, newY);

                    InvokeAxisUpdated(EControllerID.TOUCH_1, EInputAxis.MOVEMENT, newX, newY);
                //}
            }
        }

        private void On_TouchJoystick_Released(EInputAxis inputAxis, EInputButton inputButton)
        {
            // Reinitialize Movement and Rotation
            if (inputAxis != EInputAxis.NONE)
            {
                InvokeAxisUpdated(EControllerID.TOUCH_1, inputAxis, 0.0f, 0.0f);

                // Reinitialize old input values map
                if (IS_KEY_CONTAINED(oldInputValuesMap, inputAxis))
                {
                    oldInputValuesMap[inputAxis] = Vector2.zero;
                }
            }

            // Button Released event
            if (inputButton != EInputButton.NONE)
            {
                InvokeButtonReleased(EControllerID.TOUCH_1, inputButton);
            }
        }
    }
}
