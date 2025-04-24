using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace BNJMO
{
    public class NCUIContext : AbstractSingletonManager<NCUIContext>
    {
        public string IpAddress { get; private set; }

        [SerializeField] private InputField iF_ipAddress;
        [SerializeField] private GameObject c_buttons;

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            iF_ipAddress = GetComponentInChildren<InputField>();
            if (IS_NOT_NULL(iF_ipAddress))
            {
                iF_ipAddress.text = "192.168.";
                iF_ipAddress.onEndEdit.AddListener(OnInputFieldUpdated);
            }
        }

        private void OnInputFieldUpdated(string newInput)
        {
            IpAddress = newInput;
        }
    }
}