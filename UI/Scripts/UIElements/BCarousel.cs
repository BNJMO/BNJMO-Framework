using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasGroup))]
    public class BCarousel : BContainer
    {
        #region Public Events

        public event Action<int> SelectionChanged;

        #endregion

        #region Public Methods
        
        public void AutoCollectChildren()
        {
            items.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var rt = transform.GetChild(i) as RectTransform;
                if (rt != null) items.Add(rt);
            }
        }

        public void SetItems(List<RectTransform> newItems, int startSelectedIndex = 0)
        {
            items = newItems ?? new List<RectTransform>();
            selectedIndex = Mathf.Clamp(startSelectedIndex, 0, Mathf.Max(0, items.Count - 1));
            EnsureCanvasGroups();
            ApplyImmediate();
        }

        /// <summary>
        /// Directly sets the selected index without animation.
        /// </summary>
        public void SetSelectedIndex(int index)
        {
            if (items == null || items.Count == 0) return;
            
            selectedIndex = Mathf.Clamp(index, 0, Mathf.Max(0, items.Count - 1));
            EnsureCanvasGroups();
            ApplyImmediate();
            SelectionChanged?.Invoke(selectedIndex);
        }

        [FoldoutGroup("BCarousel/Debug"), Button("Move Next")]
        public void MoveNext() => Move(+1);
        
        [FoldoutGroup("BCarousel/Debug"), Button("Move Previous")]
        public void MovePrev() => Move(-1);

        public void Move(int direction)
        {
            if (items == null || items.Count == 0) 
                return;
            
            if (isAnimating) 
                return;

            int next = GetIndex(selectedIndex + direction);

            // If not looping and we're at the end, do nothing.
            if (!loop && (next == selectedIndex)) 
                return;

            // Play sound
            if (direction > 0)
            {
                BAudioManager.SpawnSoundObject(moveNextSound);
            }
            else
            {
                BAudioManager.SpawnSoundObject(movePreviousSound);
            }

            StartCoroutine(AnimateSlide(direction, next));
        }


        #endregion

        #region Inspector Variables

        [BoxGroup("BCarousel", centerLabel: true)]
        [Header("Items (in order)")]
        [BoxGroup("BCarousel"), SerializeField] private List<RectTransform> items = new ();  // TODO: Change to BUIElement

        [Header("Window")]
        [Min(1)]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private int visibleCount = 5; // Must be odd for perfect centering (we'll enforce).
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private bool loop = true;

        [Header("Layout")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float spacing = 220f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float centerScale = 1.2f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float nearScale = 1.07f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float sideScale = 0.9f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private AnimationCurve spacingByScale =
            AnimationCurve.Linear(0f, 0.8f, 1f, 1f);

        [Header("Animation")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float slideDuration = 0.25f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private AnimationCurve slideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private float fadeDuration = 0.15f;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private AnimationCurve fadeEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Sounds")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private SoundData moveNextSound;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private SoundData movePreviousSound;
        
        [Header("Optional Input")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private bool enableKeyboardInput = true;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private KeyCode nextKey = KeyCode.RightArrow;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private KeyCode prevKey = KeyCode.LeftArrow;
        
        #endregion

        #region Variables

        public int SelectedIndex => selectedIndex;
        public bool IsAnimating => isAnimating;
        public IReadOnlyList<RectTransform> Items => items;
        
        public RectTransform SelectedItem => items != null && items.Count > 0 ? items[selectedIndex] : null;

        private int selectedIndex = 0;
        private bool isAnimating;

        private readonly Dictionary<RectTransform, CanvasGroup> canvasGroups = new();

        private int HalfWindow => (GetVisibleCountOdd() - 1) / 2;

        #endregion

        #region Life Cycle

        
        private void Reset()
        {
            AutoCollectChildren();
            selectedIndex = 0;
            ApplyImmediate();
        }

        protected override void Awake()
        {
            base.Awake();
            
            EnsureCanvasGroups();
            ApplyImmediate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            EnsureCanvasGroups();
            ApplyImmediate();
        }

        protected override void Update()
        {
            base.Update();
            
            if (!Application.isPlaying)
            {
                EnsureCanvasGroups();
                ApplyImmediate();
                return;
            }

            if (!enableKeyboardInput || isAnimating) return;

            if (Input.GetKeyDown(prevKey)) MovePrev();
            if (Input.GetKeyDown(nextKey)) MoveNext();
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        private IEnumerator AnimateSlide(int direction, int nextSelected)
        {
            isAnimating = true;

            int oddVisible = GetVisibleCountOdd();
            int half = HalfWindow;

            // Determine which item is leaving and entering the visible window.
            // direction +1 means we moved selection forward (right),
            // content should slide left, so the visible window shifts forward:
            // leaving = selected - half, entering = nextSelected + half
            // direction -1 shifts backward:
            // leaving = selected + half, entering = nextSelected - half
            int leavingIndex = GetIndex(selectedIndex - direction * half);
            int enteringIndex = GetIndex(nextSelected + direction * half);

            // If clamped (no loop), indices may be the same => handle by invalidating fade.
            bool doFade = loop || (IsIndexInRange(leavingIndex) && IsIndexInRange(enteringIndex) && leavingIndex != enteringIndex);

            // Snapshot start states
            var startPos = new Vector2[items.Count];
            var startScale = new Vector3[items.Count];
            var startAlpha = new float[items.Count];

            for (int i = 0; i < items.Count; i++)
            {
                startPos[i] = items[i].anchoredPosition;
                startScale[i] = items[i].localScale;
                startAlpha[i] = canvasGroups[items[i]].alpha;
            }

            // Prepare the entering one for fade-in (if it’s currently hidden)
            if (doFade)
            {
                var entering = items[enteringIndex];
                canvasGroups[entering].alpha = 0f;
                entering.gameObject.SetActive(true);
            }

            float t = 0f;

            // Target layout is based on nextSelected
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, slideDuration);
                float eased = slideEase.Evaluate(Mathf.Clamp01(t));

                // Lerp positions/scales towards target
                for (int i = 0; i < items.Count; i++)
                {
                    Vector2 targetP = GetTargetPositionForItem(i, nextSelected);
                    Vector3 targetS = GetTargetScaleForItem(i, nextSelected);

                    items[i].anchoredPosition = Vector2.LerpUnclamped(startPos[i], targetP, eased);
                    items[i].localScale = Vector3.LerpUnclamped(startScale[i], targetS, eased);
                }

                // Fade leaving/entering
                if (doFade)
                {
                    float fadeT = fadeEase.Evaluate(Mathf.Clamp01(t * (slideDuration / Mathf.Max(0.0001f, fadeDuration))));
                    var leaving = items[leavingIndex];
                    var entering = items[enteringIndex];

                    canvasGroups[leaving].alpha = Mathf.LerpUnclamped(startAlpha[leavingIndex], 0f, fadeT);
                    canvasGroups[entering].alpha = Mathf.LerpUnclamped(0f, 1f, fadeT);
                }

                yield return null;
            }

            selectedIndex = nextSelected;

            // Finalize states
            ApplyImmediate();
            
            // Notify listeners of selection change
            SelectionChanged?.Invoke(selectedIndex);

            isAnimating = false;
        }

        private void ApplyImmediate()
        {
            if (items == null || items.Count == 0) return;

            EnsureCanvasGroups();

            int oddVisible = GetVisibleCountOdd();
            int half = HalfWindow;

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i])
                    continue;
                
                items[i].anchoredPosition = GetTargetPositionForItem(i, selectedIndex);
                items[i].localScale = GetTargetScaleForItem(i, selectedIndex);

                bool visible = IsVisible(i, selectedIndex, half, oddVisible);
                items[i].gameObject.SetActive(visible);

                canvasGroups[items[i]].alpha = visible ? 1f : 0f;
                canvasGroups[items[i]].interactable = visible;
                canvasGroups[items[i]].blocksRaycasts = visible;
            }
        }

        private Vector2 GetTargetPositionForItem(int itemIndex, int centerIndex)
        {
            int rel = RelativeOffset(itemIndex, centerIndex);
            if (rel == 0)
                return Vector2.zero;

            int dir = rel > 0 ? 1 : -1;
            int steps = Mathf.Abs(rel);

            float x = 0f;

            // Accumulate spacing step by step, scaled by neighbor sizes
            for (int i = 0; i < steps; i++)
            {
                float scaleA = GetScaleForRelative(i);
                float scaleB = GetScaleForRelative(i + 1);

                float avgScale = (scaleA + scaleB) * 0.5f;
                float scaledSpacing = spacing * spacingByScale.Evaluate(avgScale);
                x += scaledSpacing;
            }

            return new Vector2(x * dir, 0f);
        }

        private Vector3 GetTargetScaleForItem(int itemIndex, int centerIndex)
        {
            int rel = Mathf.Abs(RelativeOffset(itemIndex, centerIndex));

            if (rel == 0)
                return Vector3.one * centerScale;

            if (rel == 1)
                return Vector3.one * nearScale;   // ⬅ NEW behavior

            return Vector3.one * sideScale;
        }

        private bool IsVisible(int itemIndex, int centerIndex, int half, int oddVisible)
        {
            int rel = Mathf.Abs(RelativeOffset(itemIndex, centerIndex));
            return rel <= half;
        }

        // Returns smallest signed offset from centerIndex to itemIndex in a circular list if loop==true.
        // If loop==false, it becomes linear difference.
        private int RelativeOffset(int itemIndex, int centerIndex)
        {
            int diff = itemIndex - centerIndex;

            if (!loop || items.Count <= 1) return diff;

            int n = items.Count;
            // Wrap to [-n/2, n/2]
            diff = (diff % n + n) % n;
            if (diff > n / 2) diff -= n;
            return diff;
        }

        private int GetIndex(int idx)
        {
            if (items == null || items.Count == 0) return 0;

            if (loop)
            {
                int n = items.Count;
                idx = (idx % n + n) % n;
                return idx;
            }

            // clamped
            return Mathf.Clamp(idx, 0, items.Count - 1);
        }

        private bool IsIndexInRange(int idx) => items != null && idx >= 0 && idx < items.Count;

        private int GetVisibleCountOdd()
        {
            int v = Mathf.Clamp(visibleCount, 1, Mathf.Max(1, items.Count));
            if (v % 2 == 0) v += 1; // enforce odd for proper centering
            v = Mathf.Clamp(v, 1, Mathf.Max(1, items.Count));
            return v;
        }

        private void EnsureCanvasGroups()
        {
            if (items == null) return;

            for (int i = 0; i < items.Count; i++)
            {
                var rt = items[i];
                if (rt == null) continue;

                if (!canvasGroups.TryGetValue(rt, out var cg) || cg == null)
                {
                    cg = rt.GetComponent<CanvasGroup>();
                    if (cg == null) cg = rt.gameObject.AddComponent<CanvasGroup>();
                    canvasGroups[rt] = cg;
                }
            }
        }
        
        private float GetScaleForRelative(int relAbs)
        {
            if (relAbs == 0) return centerScale;
            if (relAbs == 1) return nearScale;
            return sideScale;
        }

        #endregion
    }
}
