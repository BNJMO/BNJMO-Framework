using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace BNJMO
{
    [Serializable]
    public struct SSceneBuildName
    {
        public EAppScene AppScene;
        public string SceneBuildName;
    }

    public class AppSceneManager : AbstractSingletonManager<AppSceneManager>
    {
        public EAppScene CurrentAppScene { get; private set; } = EAppScene.NONE;
        public EAppScene CurrentLoadingAppScene { get; private set; } = EAppScene.NONE;

        [Header("AppScene")]
        [SerializeField] private SSceneBuildName[] sceneBuildNames = new SSceneBuildName[0];

        private Dictionary<EAppScene, string> sceneBuildNamesMap = new Dictionary<EAppScene, string>();

        #region Behaviour Lifecycle
        protected override void Awake()
        {
            base.Awake();

            InitializeSceneBuildNamesMap();
        }

        protected override void Start()
        {
            base.Start();

            CurrentAppScene = BManager.Instance.Config.StartScene;
        }

        protected override void LateStart()
        {
            base.LateStart();

            BEvents.APP_AppSceneUpdated.Invoke(new StateBEHandle<EAppScene>(CurrentAppScene, EAppScene.NONE));
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            SceneManager.sceneLoaded += On_SceneLoaded;
        }
        protected override void UpdateDebugText()
        {
            base.UpdateDebugText();

            DebugManager.Instance.DebugLogCanvas("AppScene", CurrentAppScene.GetType() + " : " + CurrentAppScene);
        }

        #endregion

        #region Initialization
        private void InitializeSceneBuildNamesMap()
        {
            foreach (SSceneBuildName sceneBuildName in sceneBuildNames)
            {
                if (IS_KEY_NOT_CONTAINED(sceneBuildNamesMap, sceneBuildName.AppScene))
                {
                    sceneBuildNamesMap.Add(sceneBuildName.AppScene, sceneBuildName.SceneBuildName);
                }
            }
        }
        #endregion

        #region Scene Update
        /// <summary>
        /// Start loading process of a new scene (or reload of current scene).
        /// </summary>
        public void UpdateScene(EAppScene newAppScene)
        {
            //if (newAppScene == CurrentAppScene)
            //{
            //    LogConsoleWarning("Reloading the same scene!");
            //    return;
            //}

            CurrentLoadingAppScene = newAppScene;
            BEvents.APP_SceneWillChange.Invoke(new StateBEHandle<EAppScene>(newAppScene, CurrentAppScene));

            // Wait some frames before changing scene
            StartCoroutine(LateUpdateSceneCoroutine(newAppScene));
        }

        private IEnumerator LateUpdateSceneCoroutine(EAppScene newScene)
        {
            yield return new WaitForSeconds(BConsts.TIME_BEFORE_SCENE_CHANGE);

            LoadScene(newScene);
        }

        private void LoadScene(EAppScene sceneToLoad)
        {
            if (sceneBuildNamesMap.ContainsKey(sceneToLoad))
            {
                SceneManager.LoadScene(sceneBuildNamesMap[sceneToLoad]);
            }
            else
            {
                LogConsoleError("No scene build name was specified for : " + sceneToLoad);
            }
        }

        private void On_SceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
        {
            Debug.Log("Loading level done : " + newScene.name);

            BEvents.APP_AppSceneUpdated.Invoke(new StateBEHandle<EAppScene>(CurrentLoadingAppScene, CurrentAppScene));
            
            CurrentAppScene = CurrentLoadingAppScene;
            CurrentLoadingAppScene = EAppScene.NONE;
        }
        #endregion
    }
}
