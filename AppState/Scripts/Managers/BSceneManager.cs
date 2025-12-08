using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace BNJMO
{
    public class BSceneManager : AbstractSingletonManager<BSceneManager>
    {
        
        #region Public Events


        #endregion

        #region Public Methods

        /// <summary>
        /// Start loading process of a new scene (or reload of current scene).
        /// </summary>
        public void UpdateScene(int sceneBuildId)
        {
            if (IS_KEY_NOT_CONTAINED(scenesMap, sceneBuildId, true))
                return;
            
            CurrentLoadingSceneBuildID = sceneBuildId;
            SScene newScene = scenesMap[CurrentLoadingSceneBuildID];
            BEvents.APP_SceneWillChange.Invoke(new (newScene));

            // Wait some frames before changing scene
            StartCoroutine(LateUpdateSceneCoroutine(newScene));
        }
        
        #endregion

        #region Inspector Variables

        

        #endregion

        #region Variables

        public int CurrentSceneBuildID { get; private set; }

        public int CurrentLoadingSceneBuildID { get; private set; } = -1;
        
        private Dictionary<int, SScene> scenesMap = new ();

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            InitializeSceneBuildNamesMap();
        }

        protected override void Start()
        {
            base.Start();

            CurrentSceneBuildID = BManager.Inst.Config.StartSceneBuildID;
        }
        
        protected override void LateStart()
        {
            base.LateStart();

            if (scenesMap.ContainsKey(CurrentSceneBuildID))
            {
                BEvents.APP_SceneUpdated.Invoke(new(scenesMap[CurrentSceneBuildID]));
            }
        }
        
        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            SceneManager.sceneLoaded += SceneManager_OnSceneLoaded;
        }
        
        protected override void UpdateDebugText()
        {
            base.UpdateDebugText();

            if (!BDebugManager.Inst)
                return;
            
            BDebugManager.Inst.DebugLogCanvas("AppScene", CurrentSceneBuildID.GetType() + " : " + CurrentSceneBuildID);
        }

        #endregion

        #region Events Callbacks

        private void SceneManager_OnSceneLoaded(Scene newScene, LoadSceneMode loadSceneMode)
        {
            Debug.Log("Loading level done : " + newScene.name);

            CurrentSceneBuildID = CurrentLoadingSceneBuildID;
            CurrentLoadingSceneBuildID = -1;
            
            if (IS_KEY_NOT_CONTAINED(scenesMap, CurrentSceneBuildID, true))
                return;
            
            SScene currentScene = scenesMap[CurrentSceneBuildID];
            
            BEvents.APP_SceneUpdated.Invoke(new (currentScene));
        }

        #endregion

        #region Others

        private void InitializeSceneBuildNamesMap()
        {
            foreach (SScene sceneItr in BManager.Inst.Config.Scenes)
            {
                if (IS_KEY_CONTAINED(scenesMap, sceneItr.SceneBuildID, true))
                    continue;
                
                scenesMap.Add(sceneItr.SceneBuildID, sceneItr);
            }
        }
        
        private IEnumerator LateUpdateSceneCoroutine(SScene newScene)
        {
            yield return new WaitForSeconds(BConsts.TIME_BEFORE_SCENE_CHANGE);

            LoadScene(newScene);
        }

        private void LoadScene(SScene sceneToLoad)
        {
            SceneManager.LoadScene(sceneToLoad.SceneBuildID);
        }
        
        #endregion
    }
}
