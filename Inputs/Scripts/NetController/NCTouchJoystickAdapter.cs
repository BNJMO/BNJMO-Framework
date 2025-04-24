using UnityEngine;
using System;
using System.Collections.Generic;

namespace BNJMO
{
    public class NCTouchJoystickAdapter : BBehaviour
    {
        public event Action<EInputAxis, float, float> AxisUpdated;
        public event Action<EInputButton> ButtonPressed;
        public event Action<EInputButton> ButtonReleased;

        private Dictionary<EInputAxis, Vector2> oldInputValuesMap = new Dictionary<EInputAxis, Vector2>();

        private bool canPerformDirectionalButton = true;

        protected override void Awake()
        {
            base.Awake();

            // Joysticks
            BTouchJoystick[] maleficusJoystics = FindObjectsOfType<BTouchJoystick>();
            foreach (BTouchJoystick maleficusJoystick in maleficusJoystics)
            {
                maleficusJoystick.TouchJoystickPressed += On_MaleficusJoystick_TouchJoystickPressed;
                maleficusJoystick.TouchJoystickMoved += On_MaleficusJoystick_TouchJoystickMoved;
                maleficusJoystick.TouchJoystickReleased += On_MaleficusJoystick_TouchJoystickReleased;
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

        protected override void Update()
        {
            base.Update();

            UpdateDirectionalInput();
        }


# region Events Callbacks
        private void On_MaleficusJoystick_TouchJoystickPressed(EInputAxis inputAxis, EInputButton inputButton)
        {
            if (inputButton != EInputButton.NONE)
            {
                InvokeEventIfBound(ButtonPressed, inputButton);
            }
        }

        private void On_MaleficusJoystick_TouchJoystickMoved(EInputAxis inputAxis, EInputButton inputButton, Vector2 newInputValues)
        {
            if ((inputAxis != EInputAxis.NONE)
                && (IS_KEY_CONTAINED(oldInputValuesMap, inputAxis))
                && (Mathf.Abs(newInputValues.x) + Mathf.Abs(newInputValues.y) > BConsts.JOYSTICK_DEAD_ZONE))
            {
                Vector2 oldInputValues = oldInputValuesMap[inputAxis];
                float inputDistance = Vector2.Distance(oldInputValues.normalized, newInputValues.normalized);

                // Is new joystick input different enough from last registred one?
                if (inputDistance > BConsts.THRESHOLD_JOYSTICK_DISTANCE_MOVEMENT)
                {
                    newInputValues.Normalize();
                    float newX = newInputValues.x;
                    float newY = newInputValues.y;

                    if (BManager.Instance.Config.TransformInpuAxisToCameraDirection == true)
                    {
                        BUtils.TransformAxisToCamera(ref newX, ref newY, Camera.main.transform.forward);
                    }

                    oldInputValuesMap[inputAxis] = new Vector2(newX, newY);

                    InvokeEventIfBound(AxisUpdated, inputAxis, newX, newY);
                }
            }
        }

        private void On_MaleficusJoystick_TouchJoystickReleased(EInputAxis inputAxis, EInputButton inputButton)
        {
            // Reinitialize Movement and Rotation
            if (inputAxis != EInputAxis.NONE)
            {
                InvokeEventIfBound(AxisUpdated, inputAxis, 0.0f, 0.0f);

                // Reinitialize old input values map
                if (IS_KEY_CONTAINED(oldInputValuesMap, inputAxis))
                {
                    oldInputValuesMap[inputAxis] = Vector2.zero;
                }
            }

            // Button Released event
            if (inputButton != EInputButton.NONE)
            {
                InvokeEventIfBound(ButtonReleased, inputButton);
            }
        }
        #endregion

        private void UpdateDirectionalInput()
        {
            if (oldInputValuesMap.ContainsKey(EInputAxis.MOVEMENT))
            {
                Vector2 oldMovementInput = oldInputValuesMap[EInputAxis.MOVEMENT];

                if (canPerformDirectionalButton == true)
                {
                    // Joystick moved beyond threshold
                    if (Vector2.Distance(oldMovementInput, Vector2.zero) > 0.5f)
                    {
                        // Horizontal move
                        if (Mathf.Abs(oldMovementInput.x) > Mathf.Abs(oldMovementInput.y))
                        {
                            // Right move
                            if (oldMovementInput.x > 0.0f)
                            {
                                InvokeEventIfBound(ButtonPressed, EInputButton.RIGHT);
                            }
                            // Left move
                            else
                            {
                                InvokeEventIfBound(ButtonPressed, EInputButton.LEFT);
                            }
                        }
                        // Vertical move
                        else
                        {
                            // Up move
                            if (oldMovementInput.y > 0.0f)
                            {
                                InvokeEventIfBound(ButtonPressed, EInputButton.UP);
                            }
                            // Down move
                            else
                            {
                                InvokeEventIfBound(ButtonPressed, EInputButton.DOWN);
                            }
                        }
                        canPerformDirectionalButton = false;
                    }
                }
                else
                {
                    // Joystick moved below threshold
                    if (Vector2.Distance(oldMovementInput, Vector2.zero) < 0.5f)
                    {
                        canPerformDirectionalButton = true;
                    }
                }
            }
        }
    }
}
