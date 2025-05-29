using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace BNJMO
{
    /// <summary>
    /// Spawned when a new device has joined and responsible for transmetting the input controls to the DeviceInputSource
    /// </summary>
    public class DeviceInputPlayerListener : BBehaviour
    {
        #region Public Events

        public event Action<EControllerID, EInputButton> ButtonPressed;
        public event Action<EControllerID, EInputButton> ButtonReleased;
        // JoystickMoved event not needed as input is evaluated directly inside DeviceInputSource using MoveAxis and RotateAxis

        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables


        #endregion

        #region Variables

        public Vector2 MoveAxis
        {
            get
            {
                return isDisconnected || inputAction_Move == null ? Vector2.zero : inputAction_Move.ReadValue<Vector2>();
            }
        }   
        
        public Vector2 RotateAxis
        {
            get
            {
                if (isDisconnected || inputAction_Rotate == null)
                    return Vector2.zero;

                return inputAction_Rotate.ReadValue<Vector2>();
            }
        }
        
        public Vector2 Trigger_L_Axis
        {
            get
            {
                if (isDisconnected || inputAction_Trigger_Axis_L == null)
                    return Vector2.zero;

                return inputAction_Trigger_Axis_L.ReadValue<Vector2>();
            }
        }
        
        public Vector2 Trigger_R_Axis
        {
            get
            {
                if (isDisconnected || inputAction_Trigger_Axis_R == null)
                    return Vector2.zero;

                return inputAction_Trigger_Axis_R.ReadValue<Vector2>();
            }
        }
        
        public string DeviceName { get; private set; } = "";

        private PlayerInput myPlayerInput;
        private BNJMOControls controls = null;
        private EControllerID myControllerID;

        private InputAction inputAction_Move;
        private InputAction inputAction_Rotate;
        private InputAction inputAction_Trigger_Axis_L;
        private InputAction inputAction_Trigger_Axis_R;

        private bool canPerformDirectionalButton = true;
        private EInputButton lastPerformedDirectionalButton = EInputButton.NONE;
        private bool isDisconnected = false;
        
        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);


        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // Create a control only to get Input Actions IDs 
            controls = new BNJMOControls();
            controls.Disable();

            // Initialze Player Input
            myPlayerInput = GetComponent<PlayerInput>();
            if (IS_NOT_NULL(myPlayerInput))
            {
                InitializePlayerInputAxes();
                InitializePlayerDeviceName();
                InitializePlayerInputCallbacks();
            }
        }

        protected override void Start()
        {
            base.Start();

            
            if (IS_NULL(myPlayerInput,true)) return;
            
            // Inform Device Input Source that a new Player Input has joined and become a ControllerID
            DeviceInputSource deviceInputSource = BInputManager.Inst.GetInputSource<DeviceInputSource>();
            if (IS_NOT_NULL(deviceInputSource))
            {
                myControllerID = deviceInputSource.OnNewDeviceJoined(this);
                IS_NOT_NONE(myControllerID);
            }
                
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            //UpdateAxisInput();            // TODO: Why was this commented out?
            UpdateDirectionalInput();
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        private void InitializePlayerInputAxes()
        {
            inputAction_Move = myPlayerInput.currentActionMap.FindAction(controls.Player.MOVE.id);
            inputAction_Rotate = myPlayerInput.currentActionMap.FindAction(controls.Player.ROTATE.id);
            inputAction_Trigger_Axis_L = myPlayerInput.currentActionMap.FindAction(controls.Player.TRIGGER_AXIS_L.id);
            inputAction_Trigger_Axis_R = myPlayerInput.currentActionMap.FindAction(controls.Player.TRIGGER_AXIS_R.id);
        }

        private void InitializePlayerDeviceName()
        {
            if (myPlayerInput.devices.Count > 0)
            {
                foreach (InputDevice inputDevice in myPlayerInput.devices)
                {
                    DeviceName += inputDevice.displayName + inputDevice.name;
                }
            }
            else
            {
                LogConsoleWarning("Player input has 0 device!");
            }
        }

        private void InitializePlayerInputCallbacks()
        {
            myPlayerInput.onActionTriggered += (InputAction.CallbackContext callbackContext) =>
            {
                Guid actionID = callbackContext.action.id;

                /* Movement */
                // Directional arrows handled in "UpdateDirectionalInput"

                /* Operations */
                // CONFIRM
                if (actionID == controls.Player.CONFIRM.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.CONFIRM);
                }
                // CANCEL
                else if (actionID == controls.Player.CANCEL.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.CANCEL);
                }
                // OPTIONS
                else if (actionID == controls.Player.OPTIONS.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.OPTIONS);
                }

                /* Actinos */
                // SOUTH
                else if (actionID == controls.Player.SOUTH.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.SOUTH);
                }
                // WEST
                else if (actionID == controls.Player.WEST.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.WEST);
                }
                // EAST
                else if (actionID == controls.Player.EAST.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.EAST);
                }
                // NORTH
                else if (actionID == controls.Player.NORTH.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.NORTH);
                }
                // SHOULDER_L
                else if (actionID == controls.Player.SHOULDER_L.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.SHOULDER_L);
                }
                // SHOULDER_R
                else if (actionID == controls.Player.SHOULDER_R.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.SHOULDER_R);
                }
                // TRIGGER_L
                else if (actionID == controls.Player.TRIGGER_L.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.TRIGGER_L);
                }
                // TRIGGER_R
                else if (actionID == controls.Player.TRIGGER_R.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.TRIGGER_R);
                }
                // JOYSTICK_L
                else if (actionID == controls.Player.JOYSTICK_L.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.JOYSTICK_L);
                }
                // JOYSTICK_R
                else if (actionID == controls.Player.JOYSTICK_R.id)
                {
                    ProcessButtonEvent(callbackContext.phase, EInputButton.JOYSTICK_R);
                }
            };

            myPlayerInput.onDeviceLost += (PlayerInput playerInput) =>
            {
                LogConsole("Device lost!");

                isDisconnected = true;

                try
                {
                    playerInput.DeactivateInput();
                    playerInput.user.UnpairDevicesAndRemoveUser();
                }
                catch (Exception ex)
                {
                    Debug.LogError("onDeviceLost error: " + ex.Message);
                }

                // Don't access myPlayerInput after this point!
                myPlayerInput = null;

                // Clean up from DeviceInputSource
                DeviceInputSource deviceInputSource = BInputManager.Inst.GetInputSource<DeviceInputSource>();
                if (deviceInputSource != null)
                {
                    deviceInputSource.OnDeviceHasLeft(myControllerID, this);
                }

                Destroy(gameObject);
            };
            
        }

        private void ProcessButtonEvent(InputActionPhase actionPhase, EInputButton inputButton)
        {
            switch (actionPhase)
            {
                case InputActionPhase.Started:
                    InvokeEventIfBound(ButtonPressed, myControllerID, inputButton);
                    break;

                case InputActionPhase.Performed:
                    InvokeEventIfBound(ButtonReleased, myControllerID, inputButton);
                    break;
            }
        }

        //private void UpdateAxisInput()            // TODO: Why was this commented out?
        //{
        //    MoveAxis = inputAction_Move.ReadValue<Vector2>();
        //    RotateAxis = inputAction_Rotate.ReadValue<Vector2>();
        //    Trigger_L_Axis = inputAction_Rotate.ReadValue<Vector2>();
        //}

        private void UpdateDirectionalInput()
        {
            if (canPerformDirectionalButton == true)
            {
                // Joystick moved beyond threshold
                if (Vector2.Distance(MoveAxis, Vector2.zero) > 0.5f)
                {
                    // Horizontal move
                    if (Mathf.Abs(MoveAxis.x) > Mathf.Abs(MoveAxis.y))
                    {
                        // Right move
                        if (MoveAxis.x > 0.0f)
                        {
                            lastPerformedDirectionalButton = EInputButton.RIGHT;
                        }
                        // Left move
                        else
                        {
                            lastPerformedDirectionalButton = EInputButton.LEFT;
                        }
                    }
                    // Vertical move
                    else
                    {
                        // Up move
                        if (MoveAxis.y > 0.0f)
                        {
                            lastPerformedDirectionalButton = EInputButton.UP;
                        }
                        // Down move
                        else
                        {
                            lastPerformedDirectionalButton = EInputButton.DOWN;
                        }
                    }

                    InvokeEventIfBound(ButtonPressed, myControllerID, lastPerformedDirectionalButton);
                    canPerformDirectionalButton = false;
                }
            }
            else
            {
                // Joystick moved below threshold
                if (Vector2.Distance(MoveAxis, Vector2.zero) < 0.5f)
                {
                    InvokeEventIfBound(ButtonReleased, myControllerID, lastPerformedDirectionalButton);
                    canPerformDirectionalButton = true;
                    lastPerformedDirectionalButton = EInputButton.NONE;
                }
            }
        }

        #endregion
    }
}