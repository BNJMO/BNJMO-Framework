using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BNJMO
{
    public class DeviceInputSource : AbstractInputSource
    {
        #region Public Events


        #endregion

        #region Public Methods

        /// <summary>
        /// Called from a PlayerInputListener when spawned to connect to the next available ControllerID
        /// </summary>
        /// <param name="playerInput"> New player input listener </param>
        /// <returns> Assigned ControllerID </returns>
        public EControllerID OnNewDeviceJoined(DeviceInputPlayerListener deviceInputPlayerListener)
        {
            IS_NOT_NULL(deviceInputPlayerListener);
            // Assign a ControllerID

            EControllerType controllerType = GetControllerTypeFromName(deviceInputPlayerListener.DeviceName);
            
            // Connect controller on Input Manager
            EControllerID controllerID = BInputManager.Inst.ConnectNextDeviceController(controllerType);
            if (controllerID != EControllerID.NONE)
            {
                connectedDeviceControllers.Add(controllerID, deviceInputPlayerListener);

                // Bind Input events
                deviceInputPlayerListener.ButtonPressed += On_PlayerInputListener_ButtonPressed;
                deviceInputPlayerListener.ButtonReleased += On_PlayerInputListener_ButtonReleased;
                
                LogConsole($"New device '{deviceInputPlayerListener.DeviceName}' joined as : {controllerID}");
            }
            
            return controllerID;
        }
        
        public void OnDeviceHasLeft(EControllerID controllerID, DeviceInputPlayerListener deviceInputPlayerListener)
        {
            IS_VALID(deviceInputPlayerListener);

            if (connectedDeviceControllers.ContainsKey(controllerID) &&
                connectedDeviceControllers[controllerID] == deviceInputPlayerListener)
            {
                // Unsubscribe input events
                deviceInputPlayerListener.ButtonPressed -= On_PlayerInputListener_ButtonPressed;
                deviceInputPlayerListener.ButtonReleased -= On_PlayerInputListener_ButtonReleased;

                // Remove from dictionary
                connectedDeviceControllers.Remove(controllerID);

                // Disconnect from InputManager
                BInputManager.Inst.DisconnectController(controllerID);

                LogConsole($"Device '{deviceInputPlayerListener.DeviceName}' has disconnected from: {controllerID}");
            }
            else
            {
                LogConsoleWarning("Attempted to remove untracked or mismatched device: " + deviceInputPlayerListener.DeviceName);
            }
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
        
        #endregion

        #region Inspector Variables


        #endregion

        #region Variables

        private Dictionary<EControllerID, DeviceInputPlayerListener> connectedDeviceControllers = new();
        
        #endregion

        #region Life Cycle

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
                DeviceInputPlayerListener deviceInputPlayerListener = connectedDeviceControllers[controllerID];
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.MOVEMENT, deviceInputPlayerListener.MoveAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.ROTATION, deviceInputPlayerListener.RotateAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.TRIGGER_AXIS_L, deviceInputPlayerListener.RotateAxis);
                On_PlayerInputListener_AxisUpdated(controllerID, EInputAxis.TRIGGER_AXIS_R, deviceInputPlayerListener.RotateAxis);
            }
        }
        
        #endregion

        #region Events Callbacks


        #endregion

        #region Others

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
        
        private void INPUT_OnControllerDisconnected(BEventHandle<EControllerID> handle)
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

        #endregion
    }
}