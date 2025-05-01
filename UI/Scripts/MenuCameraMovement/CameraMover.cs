using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace BNJMO
{
    public class CameraMover : BBehaviour
    {
        #region Public Events

        public event Action<BMenu> CameraMovementEnded;
        public event Action<BMenu> CameraMovementStarted;

        #endregion

        #region Inspector Variables

        [SerializeField] private bool canBeStopped = false;
        [Header("MainMenu")] [SerializeField] private Transform entryCameraTransform;
        [SerializeField] private BMenu initialMenuTarget;
        [SerializeField] private float delayBeforeInitialMenu = 1f;
        [Header("Debug")] [SerializeField] private BMenu debugMenuTarget;
        [Button("Force Switch To Menu")]
        private void ForceSwitchToMenu()
        {
            Debug_ForceSwitchToMenu();
        }

        #endregion

        #region Variables

        private Dictionary<BMenu, BFrameCameraTransform> cameraTransforms = new();
        private AnimationLerpTransform cameraTransformLerp;
        private BFrameCameraTransform lastCameraTransform;

        #endregion

        #region Life Cycle

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            foreach (BFrameCameraTransform camTransform in FindObjectsOfType<BFrameCameraTransform>())
            {
                if (camTransform.TargetMenu != null && IS_KEY_NOT_CONTAINED(cameraTransforms, camTransform.TargetMenu))
                {
                    cameraTransforms.Add(camTransform.TargetMenu, camTransform);
                }
            }

            cameraTransformLerp = GetComponentWithCheck<AnimationLerpTransform>();
        }

        protected override void InitializeEventsCallbacks()
        {
            base.InitializeEventsCallbacks();

            BEvents.UI_HighlightedBMenuUpdated += On_UI_HighlightedBMenuUpdated;

            if (IS_NOT_NULL(cameraTransformLerp))
            {
                cameraTransformLerp.AnimationEnded += On_LerpTransform_AnimationEnded;
            }
        }

        protected override void Start()
        {
            base.Start();

            if (entryCameraTransform != null)
            {
                SetCameraToTransform(entryCameraTransform);
                Wait(delayBeforeInitialMenu, ()=>TransitionToMenu(initialMenuTarget));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (BEvents.IsInstanceSet)
            {
                BEvents.UI_HighlightedBMenuUpdated.Event -= On_UI_HighlightedBMenuUpdated;
            }

            if (cameraTransformLerp)
            {
                cameraTransformLerp.AnimationEnded -= On_LerpTransform_AnimationEnded;
            }
        }

        #endregion

        #region Callbacks

        private void On_UI_HighlightedBMenuUpdated(BEHandle<BMenu, BMenu> handle)
        {
            SwitchCameraToMenu(handle.Arg1);
        }

        private void On_LerpTransform_AnimationEnded(AnimationLerp<Transform> animationLerp)
        {
            if (BUIManager.IsInstanceSet)
            {
                InvokeEventIfBound(CameraMovementEnded, BUIManager.Inst.CurrentBMenuHighlighted);
            }
        }

        #endregion

        #region Private Methods

        private void SwitchCameraToMenu(BMenu targetMenu)
        {
            if (targetMenu == null)
            {
                LogConsoleWarning("[BFrameCameraMover] Tried to switch to a null BMenu.");
                return;
            }

            if (cameraTransforms.TryGetValue(targetMenu, out var camTransform))
            {
                cameraTransformLerp.StartValue = lastCameraTransform?.transform;
                cameraTransformLerp.EndValue = camTransform.transform;
                cameraTransformLerp.PlayDuration = camTransform.TransitionTime;
                cameraTransformLerp.StartAnimation();

                lastCameraTransform = camTransform;
            }
            else
            {
                LogConsoleWarning($"[BFrameCameraMover] No camera transform found for menu: {targetMenu.name}");
            }
        }

        private void SwitchCameraToTransform(Transform target, float transitionTime)
        {
            if (!target) return;

            if (Camera.main != null) cameraTransformLerp.StartValue = Camera.main.transform;
            cameraTransformLerp.EndValue = target;
            cameraTransformLerp.PlayDuration = transitionTime;
            cameraTransformLerp.StartAnimation();
        }

        private void SetCameraToTransform(Transform target)
        {
            if (Camera.main && target != null)
            {
                Camera.main.transform.position = target.position;
                Camera.main.transform.rotation = target.rotation;
            }
        }
        
        private void TransitionToMenu(BMenu overrideMenu = null)
        {
            BMenu targetMenu = overrideMenu ?? initialMenuTarget;

            if (targetMenu != null && cameraTransforms.TryGetValue(targetMenu, out var menuTransform))
            {
                SwitchCameraToTransform(menuTransform.transform, menuTransform.TransitionTime);
                lastCameraTransform = menuTransform;
                InvokeEventIfBound(CameraMovementStarted, targetMenu);
            }
            else
            {
                LogConsoleWarning($"[CameraMover] Could not transition — target menu {(targetMenu != null ? targetMenu.name : "NULL")} not found.");
            }
        }

        private void Debug_ForceSwitchToMenu()
        {
            SwitchCameraToMenu(debugMenuTarget);
        }

        #endregion

        #region Others

        #endregion
    }
}
