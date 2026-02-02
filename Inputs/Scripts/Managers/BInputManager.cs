using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
            connectedControllerTypes[id] = type;
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
        /// <typeparam name="S"> Specification from AbstractInputSource </typeparam>
        /// <returns> The first Input Source found </returns>
        public S GetInputSource<S>() where S : AbstractInputSource
        {
            inputSourcesMap.TryGetValue(typeof(S), out var controller);
            return controller as S;
        }

        public void Rumble(EControllerID controllerID, float lowFreq, float highFreq, float duration)
        {
            if (IS_NONE(controllerID, true)
                || IS_KEY_NOT_CONTAINED(connectedControllerTypes, controllerID, true)
                || IS_NONE(connectedControllerTypes[controllerID], true))
                return;

            var controllerType = connectedControllerTypes[controllerID];
            if (BUtils.IsDeviceInput(controllerType))
            {
                var deviceInputSource = GetInputSource<DeviceInputSource>();
                if (IS_VALID(deviceInputSource))
                {
                    deviceInputSource.Rumble(controllerID, lowFreq, highFreq, duration);
                }
            }
            else if (controllerType == EControllerType.TouchScreen)
            {
                // TODO:
            }
            else if (controllerType == EControllerType.NetworkRemote)
            {
                // TODO:
            }
        }

        #endregion

        #region Inspector Variables

        #endregion

        #region Variables

        public EControllerID[] ConnectedControllers => connectedControllers.ToArray();

        private List<EControllerID> connectedControllers = new();
        private Dictionary<EControllerID, EControllerType> connectedControllerTypes = new();
        private readonly Dictionary<Type, AbstractInputSource> inputSourcesMap = new ();

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            RegisterInputSourcesEvents();
        }

        protected override void LateStart()
        {
            base.LateStart();

            if (BManager.Inst.Config.ConnectTouchControllerInLateStart)
            {
                ConnectController(EControllerID.TOUCH_1, EControllerType.TouchScreen);
            }

            ConnectAIControllers();
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
        
        private void RegisterInputSourcesEvents()
        {
            var inputSources = GetComponents<AbstractInputSource>();
            foreach (AbstractInputSource inputSourceItr in inputSources)
            {
                if (!inputSourcesMap.TryAdd(inputSourceItr.GetType(), inputSourceItr))
                {
                    LogConsoleWarning($"Duplicate Input Source for type {inputSourceItr.GetType()} on {name}.");
                    continue;
                }
                
                inputSourceItr.ButtonPressed += On_InputSource_ButtonPressed;
                inputSourceItr.ButtonReleased += On_InputSource_ButtonReleased;
                inputSourceItr.AxisUpdated += On_InputSource_JoystickMoved;
            }
        }
        
        private void ConnectAIControllers()
        {
            for (int i = 0; i < BConfig.Inst.NumberOfAIControllersToSpawnInLateStart; i++)
            {
                EControllerID aiControllerItr = GetNextFreeAIControllerID();
                if (IS_NONE(aiControllerItr, true))
                    continue;

                ConnectController(aiControllerItr);
            }
        }

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
            foreach (EControllerID controllerIDItr in BConsts.DEVICE_CONTROLLERS)
            {
                if (connectedControllers.Contains(controllerIDItr) == false)
                {
                    controllerID = controllerIDItr;
                    break;
                }
            }
            return controllerID;
        }
        
        private EControllerID GetNextFreeRemoteControllerID()
        {
            EControllerID controllerID = EControllerID.NONE;
            foreach (EControllerID controllerIDItr in BConsts.REMOTE_CONTROLLERS)
            {
                if (connectedControllers.Contains(controllerIDItr) == false)
                {
                    controllerID = controllerIDItr;
                    break;
                }
            }
            return controllerID;
        }
         
        private EControllerID GetNextFreeAIControllerID()
        {
            EControllerID controllerID = EControllerID.NONE;
            foreach (EControllerID controllerIDItr in BConsts.AI_CONTROLLERS)
            {
                if (connectedControllers.Contains(controllerIDItr) == false)
                {
                    controllerID = controllerIDItr;
                    break;
                }
            }
            return controllerID;
        }
        
        #endregion
    }
}