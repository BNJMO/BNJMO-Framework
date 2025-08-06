using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace BNJMO
{
    public class BAuthenticationManager : AbstractSingletonManager<BAuthenticationManager>
    {
        #region Public Events

        #endregion

        #region Public Methods

        public async Task<bool> SignIn(EAuthenticationServiceType authenticationServiceType, bool forceSignOut = true)
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                LogConsoleError($"Failed to initialize Unity Services : {e.Message}");
                return false;
            }
            
            if (AuthenticationService.Instance.IsSignedIn)
            {
                if (forceSignOut)
                {
                    SignOut();
                }
                else
                {
                    LogConsoleWarning("Trying to sign in but the user is already signed in.");
                    return false;
                }
            }
            
            BEvents.AUTHENTICATION_Started.Invoke(new (authenticationServiceType));

            bool succeeded = false;
            switch (authenticationServiceType)
            {
                case EAuthenticationServiceType.Anonymous:
                    succeeded = await SignInAnonymously();
                    break;
                
                case EAuthenticationServiceType.Google:
                    succeeded = await SignInWithGoogle();
                    break;
                
                case EAuthenticationServiceType.GooglePlayGames:
                    succeeded = await SignInWithGooglePlayGames();
                    break;
                
                default:
                    LogConsoleError($"Trying to sign in with a service that is not yet implemented: {authenticationServiceType}");
                    break;
            }

            if (succeeded == false)
            {
                BEvents.AUTHENTICATION_Failed.Invoke(new (EAuthenticationFailureType.SignInWithServiceFailed));
                return false;
            }
            
            SAuthenticationArg authenticationArg = new()
            {
                AuthenticationServiceType = authenticationServiceType,
                PlayerID = AuthenticationService.Instance.PlayerId,
                AccessToken = AuthenticationService.Instance.AccessToken,
            };
            BEvents.AUTHENTICATION_Succeeded.Invoke(new (authenticationArg));

            return true;
        }

        public async Task<bool> SignIn()
        {
            if (IsSignedIn)
                return true;
            
            // TODO: Try to sign in with cached information

            // Fallback sign in anonymously 
            var result = await SignIn(EAuthenticationServiceType.Anonymous);
            return result;
        }
        
        public bool SignOut()
        {
            if (AuthenticationService.Instance.IsSignedIn == false)
            {
                LogConsoleWarning("Trying to sign the out user he but is not signed in.");   
                return false;
            }
            
            LogConsole("Signing out the user");
            AuthenticationService.Instance.SignOut();

            return true;
        }
        
        #endregion

        #region Inspector Variables


        #endregion

        #region Variables

        public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
        
        private IEnumerator updateUIEnumerator;
        
        #endregion

        #region Life Cycle
        
        protected async override void Start()
        {
            base.Start();
            
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                LogConsoleError($"Failed to initialize Unity Services : {e.Message}");
            }

            if (BConfig.Inst.SignInAnounymousOnStart)
            {
                SignIn(EAuthenticationServiceType.Anonymous);
            }
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others
        
        /* Sign In Services */
        private async Task<bool> SignInAnonymously()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                return true;
            }
            catch (AuthenticationException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
            catch (RequestFailedException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
        }
        
        private async Task<bool> SignInWithGoogle()
        {
            var googleService = GetService(EAuthenticationServiceType.Google);
            if (!googleService)
                return false;
            
            var task = googleService.GetToken();
            string googleToken = task.Result;
            
            try
            {
                await AuthenticationService.Instance.SignInWithGoogleAsync(googleToken);

                return true;
            }
            catch (AuthenticationException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
            catch (RequestFailedException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
        }
        
        private async Task<bool> SignInWithGooglePlayGames()
        {
            var googlePlayGamesService = GetService(EAuthenticationServiceType.GooglePlayGames);
            if (!googlePlayGamesService)
                return false;
            
            var result = googlePlayGamesService.GetToken();
            string googleToken = result.Result;
            
            try
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(googleToken);

                return true;
            }
            catch (AuthenticationException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
            catch (RequestFailedException exception)
            {
                LogConsoleError(exception.Message);
                return false;
            }
        }
        
        /* Service Getter */
        private AbstractAuthenticationService GetService(EAuthenticationServiceType authenticationServiceType)
        {
            var services = GetComponents<AbstractAuthenticationService>();
            foreach (var serviceItr in services)
            {
                if (serviceItr == null)
                    continue;

                if (serviceItr.AuthenticationServiceType == authenticationServiceType)
                    return serviceItr;
            }

            LogConsoleError($"Couldn't find the authentication service of type : {authenticationServiceType}");
            return null;
        }

        #endregion
    }
}
