using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Responsible for managing input controller sources attached to the same gameobject
    /// </summary>
    public class InputManager : AbstractSingletonManager<InputManager>
    {
        #region Public Events

        public void RegisterControllerType(EControllerID id, EControllerType type)
        {
            if (connectedControllerTypes.ContainsKey(id))
            {
                connectedControllerTypes[id] = type;
                LogConsole($"Updated controller type: {id} -> {type}");
            }
            else
            {
                connectedControllerTypes.Add(id, type);
                LogConsole($"Registered new controller type: {id} -> {type}");
            }
        }

        public EControllerType GetControllerType(EControllerID id)
        {
            return connectedControllerTypes.TryGetValue(id, out var type) ? type : EControllerType.None;
        }

        #endregion

        #region Public Methods

        public bool ConnectController(EControllerID controllerID, EControllerType controllerType)
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
            RegisterControllerType(controllerID, controllerType);

            // Invoke event
            BEvents.INPUT_ControllerConnected.Invoke(new BEHandle<EControllerID>(controllerID));
            return true;
        }

        public bool DisconnectController(EControllerID controllerID)
        {
            if (!connectedControllers.Contains(controllerID))
            {
                LogConsoleError("Trying to disconnect a controller that is not connected.");
                return false;
            }

            connectedControllers.Remove(controllerID);
            connectedControllerTypes.Remove(controllerID);

            BEvents.INPUT_ControllerDisconnected.Invoke(new BEHandle<EControllerID>(controllerID));
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

        protected override void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
            base.OnNewSceneReinitialize(newScene, lastScene);
        }

        #endregion

        #region Events Callbacks

        private void On_InputSource_ButtonPressed(EControllerID controllerID, EInputButton inputButton)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputButton != EInputButton.NONE)
            {
                BEvents.INPUT_ButtonPressed.Invoke(
                    new BEHandle<EControllerID, EInputButton>(controllerID, inputButton),
                    BEventReplicationType.LOCAL,
                    BManager.Inst.Config.DebugButtonEvents
                );
            }
        }

        private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputButton != EInputButton.NONE)
            {
                BEvents.INPUT_ButtonReleased.Invoke(
                    new BEHandle<EControllerID, EInputButton>(controllerID, inputButton),
                    BEventReplicationType.LOCAL,
                    BManager.Inst.Config.DebugButtonEvents
                );
            }
        }

        private void On_InputSource_JoystickMoved(EControllerID controllerID, EInputAxis inputAxis, float x, float y)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID) && inputAxis != EInputAxis.NONE)
            {
                BEvents.INPUT_AxisUpdated.Invoke(
                    new BEHandle<EControllerID, EInputAxis, float, float>(controllerID, inputAxis, x, y),
                    BEventReplicationType.LOCAL,
                    BManager.Inst.Config.DebugJoystickEvents
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

        #endregion
    }
}