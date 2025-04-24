using UnityEngine;

namespace BNJMO
{
    /// <summary>
    /// Just put this on any GameObject you don't want to be destroyed on scene change
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
