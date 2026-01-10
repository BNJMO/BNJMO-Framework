using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BNJMO
{
    /// <summary>
    /// Utility class that makes it possible to start and test any scene (instead of entry scene) by spawning needed managers if they are not found in the scene.
    /// </summary>
    public class ManagersSpawner : BManager
    {
        [BoxGroup("Spawner", centerLabel: true)]
        [SerializeField] private GameObject BManagerPrefab;
        
        protected override void OnValidate()
        {
            // Do not call base.OnValidate() to prevent setting Instance of MotherOfManagers to this one
        }

        protected override void Awake()
        {
            // Do not call base.Awake() to prevent setting Instance of MotherOfManagers to this one

            InitializeComponents();
            InitializeObjecsInScene();

            if (!Inst)
            {
                // Load managers prefab 

                if (IS_NOT_NULL(BManagerPrefab))
                {
                    // Spawn managers prefab
                    BManager spawnedManager = Instantiate(BManagerPrefab).GetComponent<BManager>();
                    spawnedManager.StartupSceneBuildId = StartupSceneBuildId;
                    spawnedManager.Config = Config;
                }
            }
        }
    }
}