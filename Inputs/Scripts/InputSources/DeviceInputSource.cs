using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNJMO
{
    public class DeviceInputSource : AbstractInputSource
    {
        private Dictionary<EControllerID, PlayerInputListener> connectedDeviceControllers = new Dictionary<EControllerID, PlayerInputListener>();

        protected override void Start()
        {
            base.Start();
            
            BEvents.INPUT_ControllerDisconnected += INPUT_OnControllerDisconnected;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            foreach (EControllerID controllerID in connectedDeviceControllers.Keys)
            {
                PlayerInputListener playerInputListener = connectedDeviceControllers[controllerID];
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.MOVEMENT, playerInputListener.MoveAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.ROTATION, playerInputListener.RotateAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.TRIGGER_AXIS_L, playerInputListener.RotateAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.TRIGGER_AXIS_R, playerInputListener.RotateAxis);
            }
        }

        /// <summary>
        /// Called from a PlayerInputListener when spawned to connect to the next available ControllerID
        /// </summary>
        /// <param name="playerInput"> New player input listener </param>
        /// <returns> Assigned ControllerID </returns>
        public EControllerID OnNewDeviceJoined(PlayerInputListener playerInputListener)
        {
            IS_NOT_NULL(playerInputListener);
            // Assign a ControllerID
            EControllerID controllerID = GetNextFreeDeviceControllerID();

            if (controllerID != EControllerID.NONE)
            {
                EControllerType controllerType = GetControllerTypeFromName(playerInputListener.DeviceName);
                
                // Connect controller on Input Manager
                if (BInputManager.Inst.ConnectController(controllerID, controllerType) == true)
                {
                    connectedDeviceControllers.Add(controllerID, playerInputListener);

                    // Bind Input events
                    playerInputListener.ButtonPressed += On_PlayerInputListener_ButtonPressed;
                    playerInputListener.ButtonReleased += On_PlayerInputListener_ButtonReleased;
                    
                    LogConsole($"New device '{playerInputListener.DeviceName}' joined as : {controllerID}");
                }
                else
                {
                    return EControllerID.NONE;
                }
            }
            else
            {
                LogConsoleWarning("No free Controller ID found for new connected device : " + playerInputListener.DeviceName);
            }

            return controllerID;
        }
        
        public void OnDeviceHasLeft(EControllerID controllerID, PlayerInputListener playerInputListener)
        {
            IS_VALID(playerInputListener);

            if (connectedDeviceControllers.ContainsKey(controllerID) &&
                connectedDeviceControllers[controllerID] == playerInputListener)
            {
                // Unsubscribe input events
                playerInputListener.ButtonPressed -= On_PlayerInputListener_ButtonPressed;
                playerInputListener.ButtonReleased -= On_PlayerInputListener_ButtonReleased;

                // Remove from dictionary
                connectedDeviceControllers.Remove(controllerID);

                // Disconnect from InputManager
                BInputManager.Inst.DisconnectController(controllerID);

                LogConsole($"Device '{playerInputListener.DeviceName}' has disconnected from: {controllerID}");
            }
            else
            {
                LogConsoleWarning("Attempted to remove untracked or mismatched device: " + playerInputListener.DeviceName);
            }
        }
        
        private void On_PlayerInputListener_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonPressed(controllerID, inputButton);
        }

        private void On_PlayerInputListener_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            InvokeButtonReleased(controllerID, inputButton);
        }

        private void On_PlayerInputListener_AxisUpdated(EControllerID controllerID, EInputAxis inputAxis, Vector2 axisValues)
        {
            InvokeAxisUpdated(controllerID, inputAxis, axisValues.x, axisValues.y);
        }
        
        private void INPUT_OnControllerDisconnected(BEHandle<EControllerID> handle)
        {
            EControllerID disconnectedID = handle.Arg1;

            if (connectedDeviceControllers.TryGetValue(disconnectedID, out var playerInputListener))
            {
                LogConsole($"Handling controller disconnection: {disconnectedID}");
                OnDeviceHasLeft(disconnectedID, playerInputListener);
            }
            else
            {
                // Already removed, likely from PlayerInputListener.onDeviceLost
                LogConsole($"Disconnected controller ID {disconnectedID} was already cleaned up.");
            }
        }

        private EControllerID GetNextFreeDeviceControllerID()
        {
            EControllerID controllerID = EControllerID.NONE;
            foreach (EControllerID controllerIDitr in BConsts.DEVICE_CONTROLLERS)
            {
                if (connectedDeviceControllers.ContainsKey(controllerIDitr) == false)
                {
                    controllerID = controllerIDitr;
                    break;
                }
            }
            return controllerID;
        }

        public EControllerType GetControllerTypeFromName(string name)
        {
            name = name.ToLower();

            if (name.Contains("xinput") || name.Contains("xbox one"))
            {
                return EControllerType.XboxOne;
            }
            if (name.Contains("xbox") || name.Contains("series"))
            {
                return EControllerType.XboxSeries;
            }
            if (name.Contains("dualshock4") || name.Contains("ps4") || name.Contains("playstation 4"))
            {
                return EControllerType.Dualshock4;
            }
            if (name.Contains("dualsense") || name.Contains("ps5") || name.Contains("playstation 5"))
            {
                return EControllerType.Dualsense;
            }
            if (name.Contains("keyboard") || name.Contains("mouse"))
            {
                return EControllerType.MouseKeyboard;
            }

            return EControllerType.MiscController;
        }
    }
}