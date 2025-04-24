namespace BNJMO
{
    public abstract class AbstractManager : BBehaviour
    {
        /// <summary>
        /// Mark this object as should not be destroyed when a new scene is loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            BEvents.APP_AppSceneUpdated += On_APP_AppSceneUpdated;
        }

        private void On_APP_AppSceneUpdated(StateBEHandle<EAppScene> bEHandle)
        {
            OnNewSceneReinitialize(bEHandle.NewState, bEHandle.LastState);
        }

        protected virtual void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
        }
    }
}
