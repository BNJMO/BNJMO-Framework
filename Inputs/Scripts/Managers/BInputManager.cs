using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Responsible for managing input controller sources attached to the same gameobject
    /// </summary>
    public class BInputManager : AbstractSingletonManager<BInputManager>
    {
        #region Public Events

        public void RegisterControllerType(EControllerID id, EControllerType type)
        {
            if (connectedControllerTypes.ContainsKey(id))
            {
                connectedControllerTypes[id] = type;
            }
            else
            {
                connectedControllerTypes.Add(id, type);
            }
        }

        public EControllerType GetControllerType(EControllerID id)
        {
            return connectedControllerTypes.TryGetValue(id, out var type) ? type : EControllerType.None;
        }

        #endregion

        #region Public Methods

        public bool ConnectController(EControllerID controllerID, EControllerType controllerType = EControllerType.None)
        {
            if (controllerID == EControllerID.NONE)
            {
                LogConsoleError("Trying to connect a controller that is NONE.");
                return false;
            }

            if (connectedControllers.Contains(controllerID))
            {
                LogConsoleError("Trying to connect a controller that is already connected.");
                return false;
            }

            connectedControllers.Add(controllerID);

            // Determine controller type
            if (controllerType == EControllerType.None)
            {
                if (BUtils.IsControllerIDAI(controllerID))
                {
                    controllerType = EControllerType.AI;
                }
                else if (BUtils.IsControllerIDRemote(controllerID))
                {
                    controllerType = EControllerType.NetworkRemote;
                }
                else if (BUtils.IsControllerIDTouch(controllerID))
                {
                    controllerType = EControllerType.TouchScreen;
                }
                else if (BUtils.IsControllerIDDevice(controllerID))
                {
                    controllerType = EControllerType.MiscController;
                }
            }
            RegisterControllerType(controllerID, controllerType);

            // Invoke event
            BEvents.INPUT_ControllerConnected.Invoke(new (controllerID, controllerType));
            return true;
        }
        
        public EControllerID ConnectNextDeviceController(EControllerType controllerType)
        {
            EControllerID controllerID = GetNextFreeDeviceControllerID();
            if (ConnectController(controllerID, controllerType))
            {
                return controllerID;
            }

            LogConsoleWarning("No free ControllerID of type Device found to connect new controller");
            return EControllerID.NONE;
        }
           
        public EControllerID ConnectNextRemoteController()
        {
            EControllerID controllerID = GetNextFreeRemoteControllerID();
            if (ConnectController(controllerID, EControllerType.NetworkRemote))
            {
                return controllerID;
            }
            
            LogConsoleWarning("No free ControllerID of type Remote found to connect new controller");
            return EControllerID.NONE;
        }
        
        public bool DisconnectController(EControllerID controllerID)
        {
            if (!connectedControllers.Contains(controllerID))
            {
                LogConsoleError($"Trying to disconnect a controller that is not connected: {controllerID}");
                return false;
            }

            EControllerType controllerType = GetControllerType(controllerID);

            connectedControllers.Remove(controllerID);
            connectedControllerTypes.Remove(controllerID);

            BEvents.INPUT_ControllerDisconnected.Invoke(new (controllerID, controllerType));
            return true;
        }

        public bool IsControllerConnected(EControllerID controllerID)
        {
            return connectedControllers.Contains(controllerID);
        }
        
        /// <summary>
        /// Returns (the first) Input Source of type "A" attached on the Input Manager.
        /// </summary>
        /// <typeparam name="A"> Specification from AbstractInputSource </typeparam>
        /// <returns> The first Input Source found </returns>
        public A GetInputSource<A>() where A : AbstractInputSource
        {
            A result = null;
            inputSources = GetComponents<AbstractInputSource>();
            foreach (AbstractInputSource inputSource in inputSources)
            {
                if (inputSource.GetType() == typeof(A))
                {
                    result = (A)inputSource;
                    break;
                }
            }

            if (result == null)
            {
                LogConsoleError("No Input Source of the given type (<color=cyan>" + typeof(A) + "</color>) found attached on InputManager!");
            }

            return result;
        }

        #endregion

        #region Inspector Variables

        #endregion

        #region Private Variables

        public EControllerID[] ConnectedControllers => connectedControllers.ToArray();

        private List<EControllerID> connectedControllers = new();
        private Dictionary<EControllerID, EControllerType> connectedControllerTypes = new();
        private AbstractInputSource[] inputSources;

        #endregion

        #region Life Cycle

        protected override void Start()
        {
            base.Start();

            inputSources = GetComponents<AbstractInputSource>();
            foreach (AbstractInputSource inputSource in inputSources)
            {
                inputSource.ButtonPressed += On_InputSource_ButtonPressed;
                inputSource.ButtonReleased += On_InputSource_ButtonReleased;
                inputSource.AxisUpdated += On_InputSource_JoystickMoved;
            }
        }

        protected override void LateStart()
        {
            base.LateStart();

            if (BManager.Inst.Config.ConnectTouchController)
            {
                ConnectController(EControllerID.TOUCH_1, EControllerType.TouchScreen);
            }

            if (BManager.Inst.Config.ConnectAIControllers)
            {
                ConnectAllAIControllers();
            }
        }

        protected override void Update()
        {
            base.Update();

            UpdateControllersDebugText();
        }

        #endregion

        #region Events Callbacks

        private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputButton != EInputButton.NONE)
            {
                EControllerType controllerType = GetControllerType(controllerID);
                BEvents.INPUT_ButtonPressed.Invoke(
                    new (controllerID, controllerType,inputButton),
                    BEventBroadcastType.LOCAL,
                    BConfig.Inst.LogInputBEvents && BConfig.Inst.LogInputButtonBEvents
                );
            }
        }

        private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputButton != EInputButton.NONE)
            {
                EControllerType controllerType = GetControllerType(controllerID);
                BEvents.INPUT_ButtonReleased.Invoke(
                    new (controllerID, controllerType, inputButton),
                    BEventBroadcastType.LOCAL,
                    BConfig.Inst.LogInputBEvents && BConfig.Inst.LogInputButtonBEvents
                );
            }
        }

        private void On_InputSource_JoystickMoved(EControllerID controllerID, EInputAxis inputAxis, float x, float y)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputAxis != EInputAxis.NONE)
            {
                EControllerType controllerType = GetControllerType(controllerID);

                BEvents.INPUT_AxisUpdated.Invoke(
                    new (controllerID, controllerType, inputAxis, x, y),
                    BEventBroadcastType.LOCAL,
                    BConfig.Inst.LogInputBEvents && BConfig.Inst.LogInputJoystickBEvents
                );
            }
        }

        #endregion

        #region Others

        private void ConnectAllAIControllers()
        {
            foreach (EControllerID aiControllerID in BConsts.AI_CONTROLLERS)
            {
                ConnectController(aiControllerID, EControllerType.AI);
            }
        }

        private void UpdateControllersDebugText()
        {
            string controllersLog = "Connected controllers : \n";
            foreach (EControllerID controllerID in connectedControllers)
            {
                controllersLog += controllerID + "\n";
            }

            LogCanvas(BConsts.DEBUGTEXT_ConnectedControllers, controllersLog);
        }

        private EControllerID GetNextFreeDeviceControllerID()
        {
            EControllerID controllerID = EControllerID.NONE;
            foreach (EControllerID controllerIDitr in BConsts.DEVICE_CONTROLLERS)
            {
                if (connectedControllers.Contains(controllerIDitr) == false)
                {
                    controllerID = controllerIDitr;
                    break;
                }
            }
            return controllerID;
        }
        
        private EControllerID GetNextFreeRemoteControllerID()
        {
            EControllerID controllerID = EControllerID.NONE;
            foreach (EControllerID controllerIDitr in BConsts.REMOTE_CONTROLLERS)
            {
                if (connectedControllers.Contains(controllerIDitr) == false)
                {
                    controllerID = controllerIDitr;
                    break;
                }
            }
            return controllerID;
        }
        
        
        #endregion
    }
}