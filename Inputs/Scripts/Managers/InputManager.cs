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
            connectedControllerTypes.Add(controllerID, controllerType);
            
            // Invoke event
            BEHandle<EControllerID> controllerConnected = new BEHandle<EControllerID>(controllerID);
            BEvents.INPUT_ControllerConnected.Invoke(controllerConnected);

            return true;
        }

        public bool DisconnectController(EControllerID controllerID)
        {
            if (connectedControllers.Contains(controllerID) == false)
            {
                LogConsoleError("Trying to disconnect a controller that is not connected.");
                return false;
            }

            connectedControllers.Remove(controllerID);
            if (connectedControllerTypes.ContainsKey(controllerID))
            {
                connectedControllerTypes.Remove(controllerID);
            }

            // Invoke event
            BEHandle<EControllerID> controllerDisconnected = new BEHandle<EControllerID>(controllerID);
            BEvents.INPUT_ControllerDisconnected.Invoke(controllerDisconnected);
            return true;
        }

        public bool IsControllerConnected(EControllerID controllerID)
        {
            return connectedControllers.Contains(controllerID);
        }

        public EControllerType GetControllerType(EControllerID controllerID)
        {
            if (connectedControllers.Contains(controllerID) == false)
            {
                LogConsoleError($"No controller connected with the id {controllerID}");
                return EControllerType.None;
            }
            if (connectedControllerTypes.ContainsKey(controllerID) == false)
            {
                LogConsoleError($"{controllerID} has no valid controller type!");
                return EControllerType.None;
            }
            
            return connectedControllerTypes[controllerID];
        }

        /// <summary>
        /// Returns (the first) Input Source of type "A" attached on the Input Manager.
        /// </summary>
        /// <typeparam name="A"> Specification from AbstractInputSource </typeparam>
        /// <returns> The first Inpupt Source found </returns>
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
        
        public EControllerID[] ConnectedControllers { get => connectedControllers.ToArray(); }
        
        private List<EControllerID> connectedControllers { get; } = new List<EControllerID>();

        private Dictionary<EControllerID, EControllerType> connectedControllerTypes =
            new Dictionary<EControllerID, EControllerType>();
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

            if (BManager.Instance.Config.ConnectTouchController == true)
            {
                ConnectController(EControllerID.TOUCH, EControllerType.TouchScreen);
            }

            if (BManager.Instance.Config.ConnectAIControllers == true)
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
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID))
            {
                EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);

                if (inputButton != EInputButton.NONE)
                {
                    BEvents.INPUT_ButtonPressed.Invoke(new BEHandle<EControllerID, EInputButton>(controllerID, inputButton), BEventReplicationType.LOCAL, BManager.Instance.Config.DebugButtonEvents);
                }
            }
        }

        private void On_InputSource_ButtonReleased(EControllerID controllerID, EInputButton inputButton)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID))
            {
                EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);

                if (inputButton != EInputButton.NONE)
                {
                    BEvents.INPUT_ButtonReleased.Invoke(new BEHandle<EControllerID, EInputButton>(controllerID, inputButton), BEventReplicationType.LOCAL, BManager.Instance.Config.DebugButtonEvents);
                }
            }
        }

        private void On_InputSource_JoystickMoved(EControllerID controllerID, EInputAxis inputAxis, float x, float y)
        {
            if (IS_VALUE_CONTAINED(connectedControllers, controllerID))
            {
                EPlayerID playerID = PlayerManager.Instance.GetAssignedPlayerID(controllerID);

                if (inputAxis != EInputAxis.NONE)
                {
                    BEvents.INPUT_AxisUpdated.Invoke(new BEHandle<EControllerID, EInputAxis, float, float>(controllerID, inputAxis, x, y), BEventReplicationType.LOCAL, BManager.Instance.Config.DebugJoystickEvents);
                }
            }
        }
        
        #endregion

        #region Others
        
        private void ConnectAllAIControllers()
        {
            foreach (EControllerID aIControllerID in BConsts.AI_CONTROLLERS)
            {
                ConnectController(aIControllerID, EControllerType.AI);
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