using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

namespace BNJMO
{
    public class BFrameCameraTransform : BBehaviour
    {
        #region Public Events
        #endregion

        #region Inspector Variables
        [SerializeField] private BMenu targetMenu;
        [SerializeField] private float transitionTime = 2.0f;
        
        [Button("Update This From Camera Transform")]
        private void SetThisToCamera()
        {
            SetThisToCameraTransform();
        }
        
        [Button("Update Camera Transform From This")]
        private void SetCameraToThis()
        {
            SetCameraTransformToThis();
        }

        #endregion

        #region Variables
        public BMenu TargetMenu => targetMenu;
        public float TransitionTime => transitionTime;
        #endregion

        #region Life Cycle
        #endregion

        #region Callbacks
        #endregion

        #region Public Methods
        public void SetCameraTransformToThis()
        {
            if (Camera.main)
            {
                Camera.main.transform.position = transform.position;
                Camera.main.transform.rotation = transform.rotation;
            }
        }

        public void SetThisToCameraTransform()
        {
            if (Camera.main)
            {
                transform.position = Camera.main.transform.position;
                transform.rotation = Camera.main.transform.rotation;
            }
        }
        #endregion

        #region Others
        #endregion
    }
}
