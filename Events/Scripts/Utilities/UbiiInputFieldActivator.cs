using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BNJMO
{
    public class UbiiInputFieldActivator : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables
        [SerializeField]
        private BUIObjectContainer uIObjectContainer;

        [SerializeField]
        private InputField inputField_IP;

        [SerializeField]
        private InputField inputField_Port;

        #endregion

        #region Private Variables

        #endregion

        #region Life Cycle
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();
            
            if (uIObjectContainer == null)
            {
                uIObjectContainer = GetComponent<BUIObjectContainer>();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BEvents.NETWORK_NewBEventDispatcherSet += On_NETWORK_NewBEventDispatcherSet;

            if (IS_NOT_NULL(inputField_IP)
                && IS_NOT_NULL(inputField_Port))
            {
                inputField_IP.onValueChanged.AddListener(On_inputField_IP_ValueChanged);
                inputField_Port.onValueChanged.AddListener(On_inputField_Port_ValueChanged);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            BEvents.NETWORK_NewBEventDispatcherSet -= On_NETWORK_NewBEventDispatcherSet;

            if (inputField_IP
                && inputField_Port)
            {
                inputField_IP.onValueChanged.RemoveListener(On_inputField_IP_ValueChanged);
                inputField_Port.onValueChanged.RemoveListener(On_inputField_Port_ValueChanged);
            }
        }

        protected override void Start()
        {
            base.Start();

            if (IS_NOT_NULL(inputField_IP)
                        && IS_NOT_NULL(inputField_Port))
            {
                inputField_IP.text = BManager.Inst.Config.UbiiBackendServerIP;
                inputField_Port.text = BManager.Inst.Config.UbiiBackendServerPort.ToString();
            }

            //UpdateUIObjectContainer(MotherOfManagers.Instance.EventDispatcherType);
        }

        #endregion

        #region Events Callbacks
        private void On_NETWORK_NewBEventDispatcherSet(BEHandle<AbstractBEventDispatcher> handle)
        {
            //UpdateUIObjectContainer(handle.Arg1.GetBEventDispatcherType());
        }

        private void UpdateUIObjectContainer(BEventDispatcherType dispatcherType)
        {
            if (IS_NOT_NULL(uIObjectContainer))
            {
                if (dispatcherType == BEventDispatcherType.UBI_INTERACT)
                {
                    uIObjectContainer.ShowUI(true);

                    if (IS_NOT_NULL(inputField_IP)
                        && IS_NOT_NULL(inputField_Port))
                    {
                        inputField_IP.text = BManager.Inst.Config.UbiiBackendServerIP;
                        inputField_Port.text = BManager.Inst.Config.UbiiBackendServerPort.ToString();
                    }
                }
                else
                {
                    uIObjectContainer.DisableUI();
                }
            }
        }

        private void On_inputField_IP_ValueChanged(string newValue)
        {
            BManager.Inst.Config.UbiiBackendServerIP = newValue;
        }
        
        private void On_inputField_Port_ValueChanged(string newValue)
        {
            BManager.Inst.Config.UbiiBackendServerPort = int.Parse(newValue);
        }

        #endregion

        #region Others


        #endregion
    }
}
