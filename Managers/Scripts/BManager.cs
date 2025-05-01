using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BNJMO
{
    [System.Serializable]
    public struct PlayerTupple
    {
        public EControllerID controllerID;
        public EPlayerID playerID;
    }

    public class BManager : AbstractSingletonManager<BManager>
    {
        // When adding a new attribute here, remember to add profile setup in SpawnManager class !!!

        #region Public Events


        #endregion

        #region Public Methods
        public void QuitApp()
        {
            Application.Quit();
        }

        #endregion

        #region Inspector Variables

        [BoxGroup("BManager", centerLabel: true)]
        [SerializeField] private BConfig config;
        
        #endregion

        #region Variables
        
        public BConfig Config { get => config; set => config = value; }

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();
            
            QualitySettings.vSyncCount = Config.VSyncCount;
            Application.targetFrameRate = Config.TargetFramRate;
            Screen.sleepTimeout = Config.SleepTimeout;
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
        

    }
}