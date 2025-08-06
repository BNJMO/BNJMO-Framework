using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNJMO;
using UnityEngine.Serialization;

namespace BNJMO
{
    public class SignInButton : AbstractBButtonCallback
    {
        #region Public Events


        #endregion

        #region Public Methods

        public override void OnBButtonPressed()
        {
            if (!BAuthenticationManager.Inst)
                return;
            
            BAuthenticationManager.Inst.SignIn(authenticationServiceType);
        }
        
        #endregion

        #region Inspector Variables

        [SerializeField] 
        private EAuthenticationServiceType authenticationServiceType;

        #endregion

        #region Variables

        public EAuthenticationServiceType AuthenticationServiceType => authenticationServiceType;

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
    }
}
