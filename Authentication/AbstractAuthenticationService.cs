using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;

namespace BNJMO
{
    public abstract class AbstractAuthenticationService : BBehaviour
    {
        #region Public Events

        public abstract Task<string> GetToken();
        
        public abstract Task<string> GetUserName();
        
        public abstract Task<string> GetUserEmail();
        
        public abstract Task<Texture2D> GetUserPicture();

        #endregion

        #region Public Methods

        
        #endregion

        #region Inspector Variables

        [SerializeField, ReadOnly] 
        private EAuthenticationServiceType authenticationServiceType = EAuthenticationServiceType.None;

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
