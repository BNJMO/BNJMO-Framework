using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BCanvas : BUIElement
    {
        #region Public Events


        #endregion

        #region Public Methods


        #endregion

        #region Inspector Variables


        #endregion

        #region Private Variables


        #endregion

        #region Life Cycle
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "CN_";

            base.OnValidate();
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
