using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractManager : BBehaviour
    {
        [SerializeField, FoldoutGroup("Manager")]
        private bool setDontDestroyOnLoad = true;
        
        /// <summary>
        /// Mark this object as should not be destroyed when a new scene is loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (setDontDestroyOnLoad
                && transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
