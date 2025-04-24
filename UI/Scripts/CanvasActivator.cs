using UnityEngine;

namespace BNJMO
{
    public class CanvasActivator : BBehaviour
    {
        protected override void Awake()
        {
            base.Awake();

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
