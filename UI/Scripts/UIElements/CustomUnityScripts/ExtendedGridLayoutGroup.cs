using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BNJMO
{
    #region UI
    
    public enum BGridType
    {
        Default,
        CenterLastRow
    }

    #endregion
    
    public class ExtendedGridLayoutGroup : GridLayoutGroup
    {
        #region Public Events


        #endregion

        #region Public Methods

        public override void SetLayoutVertical()
        {
            if (gridType == BGridType.CenterLastRow)
            {
                SetCenteredLayout();
            }
            else
            {
                base.SetLayoutVertical();
            }
        }


        #endregion

        #region Inspector Variables

        [SerializeField] 
        private BGridType gridType = BGridType.Default;

        #endregion

        #region Variables

        
        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        private void SetCenteredLayout()
        {
            // center last row if incomplete
            int rectChildrenCount = rectChildren.Count;
            if (rectChildrenCount == 0) return;

            int constraintCellCount = constraintCount;
            int lastRowCount = rectChildrenCount % constraintCellCount;
            int fullRowCount = rectChildrenCount / constraintCellCount;

            Vector2 fullStartOffset = new Vector2(
                GetStartOffset(0, (cellSize.x + spacing.x) * constraintCellCount - spacing.x),
                GetStartOffset(1, (cellSize.y + spacing.y) * Mathf.CeilToInt(rectChildrenCount / (float)constraintCellCount) - spacing.y)
            );

            Vector2 lastRowOffset = fullStartOffset;
            if (lastRowCount > 0)
            {
                float width = (cellSize.x + spacing.x) * lastRowCount - spacing.x;
                lastRowOffset.x = GetStartOffset(0, width);
            }

            for (int i = 0; i < rectChildrenCount; i++)
            {
                int posX = i % constraintCellCount;
                int posY = i / constraintCellCount;

                Vector2 offset = (posY == fullRowCount) ? lastRowOffset : fullStartOffset;

                RectTransform rect = rectChildren[i];
                SetChildAlongAxis(rect, 0, offset.x + (cellSize.x + spacing.x) * posX, cellSize.x);
                SetChildAlongAxis(rect, 1, offset.y + (cellSize.y + spacing.y) * posY, cellSize.y);
            }
        }

    #if UNITY_EDITOR
        [CustomEditor(typeof(ExtendedGridLayoutGroup))]
        public class BGridLayoutGroupEditor : UnityEditor.UI.GridLayoutGroupEditor
        {
            private SerializedProperty gridTypeProperty;

            protected override void OnEnable()
            {
                base.OnEnable();
                gridTypeProperty = serializedObject.FindProperty("gridType");
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("BNJMO Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(gridTypeProperty);

                serializedObject.ApplyModifiedProperties();
            }
        }
    #endif
        
        #endregion
        
    }
}
