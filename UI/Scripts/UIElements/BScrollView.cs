using UnityEngine;
using UnityEngine.UI;

namespace BNJMO
{
    public class BScrollView : BUIElement
    {
        #region Public Events

        #endregion

        #region Public Methods

        #endregion

        #region Inspector Values

        #endregion

        #region Variables

        private ScrollRect scrollRect;
        private RectTransform viewportTransform;
        private RectTransform contentTransform;

        private Image viewportImage;

        private Scrollbar verticalScrollbar;
        private Scrollbar horizontalScrollbar;
        private Image verticalScrollbarImage;
        private Image horizontalScrollbarImage;
        private Image backgroundImage;
        private Image verticalHandleImage;
        private Image horizontalHandleImage;

        #endregion

        #region LifeCycle

        protected override void Awake()
        {
            base.Awake();
            CacheReferences();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CacheReferences();
        }
#endif

        #endregion

        #region Event Callbacks

        protected override void OnUIShown()
        {
            base.OnUIShown();

            if (backgroundImage != null)
                backgroundImage.enabled = true;

            if (viewportImage != null)
                viewportImage.enabled = true;

            if (verticalScrollbar != null)
            {
                verticalScrollbar.enabled = true;
                if (verticalScrollbarImage != null)
                    verticalScrollbarImage.enabled = true;
                if (verticalHandleImage != null)
                    verticalHandleImage.enabled = true;
            }

            if (horizontalScrollbar != null)
            {
                horizontalScrollbar.enabled = true;
                if (horizontalScrollbarImage != null)
                    horizontalScrollbarImage.enabled = true;
                if (horizontalHandleImage != null)
                    horizontalHandleImage.enabled = true;
            }

            if (contentTransform != null)
            {
                foreach (BUIElement element in contentTransform.GetComponentsInChildren<BUIElement>(includeInactive: true))
                {
                    if (element != this)
                    {
                        element.ShowUI();
                    }
                }
            }
        }
        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (backgroundImage != null)
                backgroundImage.enabled = false;

            if (viewportImage != null)
                viewportImage.enabled = false;

            if (verticalScrollbar != null)
            {
                verticalScrollbar.enabled = false;
                if (verticalScrollbarImage != null)
                    verticalScrollbarImage.enabled = false;
                if (verticalHandleImage != null)
                    verticalHandleImage.enabled = false;
            }

            if (horizontalScrollbar != null)
            {
                horizontalScrollbar.enabled = false;
                if (horizontalScrollbarImage != null)
                    horizontalScrollbarImage.enabled = false;
                if (horizontalHandleImage != null)
                    horizontalHandleImage.enabled = false;
            }

            if (contentTransform != null)
            {
                foreach (BUIElement element in contentTransform.GetComponentsInChildren<BUIElement>(includeInactive: true))
                {
                    if (element != this)
                    {
                        element.HideUI();
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void CacheReferences()
        {
            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogError("BScrollView requires a ScrollRect component.");
                return;
            }

            backgroundImage = GetComponent<Image>();

            viewportTransform = scrollRect.viewport;
            contentTransform = scrollRect.content;

            if (viewportTransform != null)
            {
                viewportImage = viewportTransform.GetComponent<Image>();
            }

            verticalScrollbar = scrollRect.verticalScrollbar;
            if (verticalScrollbar != null)
            {
                verticalScrollbarImage = verticalScrollbar.GetComponent<Image>();
                if (verticalScrollbar.handleRect != null)
                {
                    verticalHandleImage = verticalScrollbar.handleRect.GetComponent<Image>();
                }
            }

            horizontalScrollbar = scrollRect.horizontalScrollbar;
            if (horizontalScrollbar != null)
            {
                horizontalScrollbarImage = horizontalScrollbar.GetComponent<Image>();
                if (horizontalScrollbar.handleRect != null)
                {
                    horizontalHandleImage = horizontalScrollbar.handleRect.GetComponent<Image>();
                }
            }
        }


        #endregion
    }
}
