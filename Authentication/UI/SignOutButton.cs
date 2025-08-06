using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class SignOutButton : AbstractBButtonCallback
    {
        #region Public Events


        #endregion

        #region Public Methods
        
        
        public override void OnBButtonPressed()
        {
            if (!BAuthenticationManager.Inst)
                return;
            
            BAuthenticationManager.Inst.SignOut();
        }

        #endregion

        #region Inspector Variables


        #endregion

        #region Variables


        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
