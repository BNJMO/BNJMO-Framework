#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BNJMO
{
    public class NestedScrollRect : ScrollRect
    {
        #region Public Events


        #endregion

        #region Public Methods

        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            base.OnInitializePotentialDrag(eventData);
            
            if (parentScrollRect != null)
            {
                parentScrollRect.OnInitializePotentialDrag(eventData);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (parentScrollRect == null)
            {
                base.OnBeginDrag(eventData);
                return;
            }

            // Decide axes for this drag based on current settings & initial gesture
            childAxis  = DeterminePrimaryAxis(this, eventData.delta);
            parentAxis = DeterminePrimaryAxis(parentScrollRect, eventData.delta);

            // Compare projection of the drag onto each axis
            float childMag  = Mathf.Abs(Vector2.Dot(eventData.delta, childAxis));
            float parentMag = Mathf.Abs(Vector2.Dot(eventData.delta, parentAxis));

            if (childMag > parentMag * dominantAxisBias)
            {
                routeToParent = false;
                base.OnBeginDrag(eventData);
            }
            else
            {
                routeToParent = true;
                parentScrollRect.OnBeginDrag(eventData);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (routeToParent && parentScrollRect != null)
            {
                parentScrollRect.OnDrag(eventData);
            }
            else
            {
                base.OnDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (routeToParent && parentScrollRect != null)
            {
                parentScrollRect.OnEndDrag(eventData);
            }
            else
            {
                base.OnEndDrag(eventData);
            }

            routeToParent = false;
        }

        // Make mouse wheel / touchpad scroll feel natural in nested setups
        public override void OnScroll(PointerEventData data)
        {
            if (parentScrollRect == null)
            {
                base.OnScroll(data);
                return;
            }

            // Use the same axis inference for scroll events
            Vector2 cAxis = DeterminePrimaryAxis(this, data.scrollDelta);
            Vector2 pAxis = DeterminePrimaryAxis(parentScrollRect, data.scrollDelta);

            float childMag  = Mathf.Abs(Vector2.Dot(data.scrollDelta, cAxis));
            float parentMag = Mathf.Abs(Vector2.Dot(data.scrollDelta, pAxis));

            if (childMag > parentMag * dominantAxisBias)
            {
                base.OnScroll(data);
            }
            else
            {
                parentScrollRect.OnScroll(data);
            }
        }
        
        #endregion

        #region Inspector Variables

        [SerializeField]
        public ScrollRect parentScrollRect;

        [SerializeField, Tooltip("Child drag must exceed the parent drag by this factor to keep control.")]
        public float dominantAxisBias  = 1.1f;
        
        #endregion

        #region Variables
        
        private bool routeToParent;
        private Vector2 childAxis;
        private Vector2 parentAxis;

        #endregion

        #region Life Cycle

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (parentScrollRect == null)
            {
                parentScrollRect = GetComponentInParents<ScrollRect>();
            }
        }
        #endif

        #endregion

        #region Events Callbacks


        #endregion

        #region Others
        /// <summary>
        /// Determine the primary scroll axis (normalized) for a given ScrollRect.
        /// If only Horizontal is enabled → (1,0), if only Vertical → (0,1).
        /// If both or neither are enabled, infer from the drag delta direction.
        /// </summary>
        private static Vector2 DeterminePrimaryAxis(ScrollRect scrollRect, Vector2 initialDelta)
        {
            bool isHorizontal = scrollRect != null && scrollRect.horizontal;
            bool isVertical = scrollRect != null && scrollRect.vertical;

            if (isHorizontal && !isVertical) return Vector2.right;
            if (isVertical && !isHorizontal) return Vector2.up;

            // If both (or neither) are enabled, infer from the gesture direction
            float deltaX = Mathf.Abs(initialDelta.x);
            float deltaY = Mathf.Abs(initialDelta.y);

            // If no movement yet, default to whichever dimension has larger content span
            if (deltaX < Mathf.Epsilon && deltaY < Mathf.Epsilon)
            {
                if (scrollRect != null && scrollRect.content != null)
                {
                    RectTransform rt = scrollRect.content;
                    float width = rt.rect.width;
                    float height = rt.rect.height;
                    return (width >= height) ? Vector2.right : Vector2.up;
                }
                return Vector2.up; // safe default
            }

            return (deltaX >= deltaY) ? Vector2.right : Vector2.up;
        }
        
#if UNITY_EDITOR
        [CustomEditor(typeof(NestedScrollRect))]
        public class NestedScrollRectEditor : UnityEditor.UI.ScrollRectEditor
        {
            private SerializedProperty parentScrollRectProperty;
            private SerializedProperty dominantAxisBiasProperty;

            protected override void OnEnable()
            {
                base.OnEnable();
                parentScrollRectProperty = serializedObject.FindProperty("parentScrollRect");
                dominantAxisBiasProperty = serializedObject.FindProperty("dominantAxisBias");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("BNJMO Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(parentScrollRectProperty);
                EditorGUILayout.PropertyField(dominantAxisBiasProperty);

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif

        private T GetComponentInParents<T>(bool excludeSelf = true)
        {
            T result = default(T);

            if (!excludeSelf)
            {
                result = GetComponent<T>();
                if (result != null)
                    return result;
            }
            
            Transform parent = transform.parent;
            while (parent != null && result == null)
            {
                result = parent.GetComponent<T>();
                parent = parent.parent;
            }
            return result;
        }
        #endregion
    }
}
