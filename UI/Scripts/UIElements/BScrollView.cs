using Sirenix.OdinInspector;
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

        [BoxGroup("Scroll View", centerLabel: true)]

        /* Settings */
        [FoldoutGroup("Scroll View/Scroll View Settings"), SerializeField]
        private Vector3 defaultContentLocalPosition;
        
        [FoldoutGroup("Scroll View/Scroll View Settings"), SerializeField]
        private bool resetContentPositionOnUIHidden = true;

        /* References */
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private ScrollRect scrollRect;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private RectTransform viewportTransform;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private RectTransform contentTransform;

        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage viewportImage;

        [FoldoutGroup("Scroll View/References"), SerializeField]
        private Scrollbar verticalScrollbar;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private Scrollbar horizontalScrollbar;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage verticalScrollbarImage;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage horizontalScrollbarImage;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage backgroundImage;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage verticalHandleImage;
        
        [FoldoutGroup("Scroll View/References"), SerializeField]
        private BImage horizontalHandleImage;
        
        #endregion

        #region Variables

        public ScrollRect ScrollRect => scrollRect;

        #endregion

        #region LifeCycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "SV_";

            base.OnValidate();
            
            scrollRect = GetComponent<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogError("BScrollView requires a ScrollRect component.");
                return;
            }

            viewportTransform = scrollRect.viewport;
            contentTransform = scrollRect.content;

            if (viewportTransform != null)
            {
                viewportImage = viewportTransform.GetComponent<BImage>();
            }

            verticalScrollbar = scrollRect.verticalScrollbar;
            if (verticalScrollbar != null)
            {
                verticalScrollbarImage = verticalScrollbar.GetComponent<BImage>();
                if (verticalScrollbar.handleRect != null)
                {
                    verticalHandleImage = verticalScrollbar.handleRect.GetComponent<BImage>();
                }
            }

            horizontalScrollbar = scrollRect.horizontalScrollbar;
            if (horizontalScrollbar != null)
            {
                horizontalScrollbarImage = horizontalScrollbar.GetComponent<BImage>();
                if (horizontalScrollbar.handleRect != null)
                {
                    horizontalHandleImage = horizontalScrollbar.handleRect.GetComponent<BImage>();
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            contentTransform.localPosition = defaultContentLocalPosition;
        }

        #endregion

        #region Event Callbacks

        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (resetContentPositionOnUIHidden)
            {
                contentTransform.localPosition = defaultContentLocalPosition;
            }
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
