using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BUIObjectContainer : BUIElement
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables

        [BoxGroup("BUIObjectContainer", centerLabel: true)]
        [SerializeField]
        private GameObject UIGameObject;
        
        #endregion

        #region Variables


        #endregion

        #region Life Cycle
        
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "O_";

            if (!UIGameObject)
            {
                UIGameObject = gameObject;    
            }
            
            base.OnValidate();
        }


        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        protected override void OnUIShown()
        {
            base.OnUIShown();

            if (IS_NOT_NULL(UIGameObject))
            {
                UIGameObject.SetActive(true);
            }
        }

        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (IS_NOT_NULL(UIGameObject))
            {
                UIGameObject.SetActive(false);
            }
        }

        #endregion
        
    }
}
