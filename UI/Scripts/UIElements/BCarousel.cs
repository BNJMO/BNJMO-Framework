using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BNJMO
{
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasGroup))]
    public class BCarousel : BContainer, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Public Events

        public event Action<int> SelectionChanged;

        /// <summary>
        /// Raised with the index of an item clicked with the pointer, once the carousel has slid it into the centre.
        /// </summary>
        public event Action<int> ItemClicked;

        public event Action DragStarted;

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
            StopMoving();
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
            StopMoving();
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

            if (IsAnimating)
                return;

            int next = GetIndex(selectedIndex + direction);

            // If not looping and we're at the end (or there is a single item), do nothing.
            if (next == selectedIndex)
                return;

            SlideTo(selectedIndex + direction);
        }

        /* Pointer */
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!CanUsePointer(eventData)
                || isDragging)
                return;

            int clickedIndex = GetItemIndex(eventData.pointerPressRaycast.gameObject);
            if (clickedIndex < 0)
                return;

            // Take the shortest way to the clicked item and report the click once it is centred
            float centerPosition = Mathf.Round(scrollPosition);
            SlideTo(centerPosition + RelativeOffset(clickedIndex, centerPosition), () => ItemClicked?.Invoke(clickedIndex));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanUsePointer(eventData))
                return;

            StopMoving();
            isDragging = true;
            dragStartPosition = scrollPosition;

            DragStarted?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            float stepWidth = GetSlotX(1);
            if (stepWidth <= 0f
                || !TryGetLocalPointerX(eventData.pressPosition, eventData.pressEventCamera, out float pressX)
                || !TryGetLocalPointerX(eventData.position, eventData.pressEventCamera, out float pointerX))
                return;

            // Dragging to the right brings the items on the left towards the centre
            float pointerPosition = dragStartPosition - (pointerX - pressX) / stepWidth;
            if (!loop)
            {
                pointerPosition = Mathf.Clamp(pointerPosition, 0f, items.Count - 1);
            }

            scrollPosition = GetSnappedDragPosition(pointerPosition);
            ApplyLayout();
            UpdateSelectionWhileDragging();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging)
                return;

            isDragging = false;
            SlideTo(Mathf.Round(scrollPosition));
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
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField, Tooltip("Opacity of the items moving in or out of the window, over the slot they cross")]
        private AnimationCurve fadeEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Sounds")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private SoundData moveNextSound;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private SoundData movePreviousSound;

        [Header("Optional Input")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private bool enableKeyboardInput = true;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private KeyCode nextKey = KeyCode.RightArrow;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private KeyCode prevKey = KeyCode.LeftArrow;

        [Header("Pointer Input")]
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField] private bool enablePointerInput = true;
        [FoldoutGroup("BCarousel/Carousel Settings"), SerializeField, Range(0f, 0.45f)]
        [Tooltip("How far (in items) the pointer can keep dragging while an item that reached the centre stays snapped there")]
        private float dragSnapZone = 0.2f;

        #endregion

        #region Variables

        public int SelectedIndex => selectedIndex;
        /// <summary> True while the carousel slides or is being dragged. </summary>
        public bool IsAnimating => isAnimating || isDragging;
        public IReadOnlyList<RectTransform> Items => items;

        public RectTransform SelectedItem => items != null && items.Count > 0 ? items[selectedIndex] : null;

        private int selectedIndex = 0;
        private bool isAnimating;
        private bool isDragging;

        // Continuous centre of the carousel in item indices: equals selectedIndex at rest, fractional while sliding or dragging.
        // When looping, it may leave [0, items.Count) during a move and is wrapped back once settled.
        private float scrollPosition;
        private float dragStartPosition;
        private IEnumerator slideEnumerator;

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

        protected override void OnDisable()
        {
            base.OnDisable();

            // The slide coroutine stops with the component: settle on the closest item instead of staying between two
            // (laid out again in OnEnable).
            if (isAnimating || isDragging)
            {
                StopMoving();

                int closestIndex = GetIndex(Mathf.RoundToInt(scrollPosition));
                bool hasChanged = closestIndex != selectedIndex;
                selectedIndex = closestIndex;
                scrollPosition = selectedIndex;

                if (hasChanged)
                {
                    SelectionChanged?.Invoke(selectedIndex);
                }
            }
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

            if (!enableKeyboardInput || IsAnimating) return;

            if (Input.GetKeyDown(prevKey)) MovePrev();
            if (Input.GetKeyDown(nextKey)) MoveNext();
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        private void SlideTo(float targetPosition, Action onArrived = null)
        {
            if (GetIndex(Mathf.RoundToInt(targetPosition)) != selectedIndex)
            {
                PlayMoveSound(targetPosition > scrollPosition ? 1 : -1);
            }

            if (Mathf.Approximately(targetPosition, scrollPosition)
                || !gameObject.activeInHierarchy)
            {
                StopMoving();
                SettleAt(targetPosition);
                onArrived?.Invoke();
                return;
            }

            StartNewCoroutine(ref slideEnumerator, SlideCoroutine(targetPosition, onArrived));
        }

        private IEnumerator SlideCoroutine(float targetPosition, Action onArrived)
        {
            isAnimating = true;

            float startPosition = scrollPosition;
            // Sliding over several items takes longer than over one, but not proportionally so
            float duration = slideDuration * Mathf.Sqrt(Mathf.Abs(targetPosition - startPosition));
            float t = 0f;

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.0001f, duration);
                float eased = slideEase.Evaluate(Mathf.Clamp01(t));

                scrollPosition = Mathf.LerpUnclamped(startPosition, targetPosition, eased);
                ApplyLayout();

                yield return null;
            }

            slideEnumerator = null;
            isAnimating = false;

            SettleAt(targetPosition);
            onArrived?.Invoke();
        }

        private void SettleAt(float position)
        {
            int index = GetIndex(Mathf.RoundToInt(position));
            bool hasChanged = index != selectedIndex;
            selectedIndex = index;

            // Finalize states
            ApplyImmediate();

            // Notify listeners of selection change
            if (hasChanged)
            {
                SelectionChanged?.Invoke(selectedIndex);
            }
        }

        private void StopMoving()
        {
            StopCoroutineIfRunning(ref slideEnumerator);
            isAnimating = false;
            isDragging = false;
        }

        private void PlayMoveSound(int direction)
        {
            if (direction > 0)
            {
                BAudioManager.SpawnSoundObject(moveNextSound);
            }
            else
            {
                BAudioManager.SpawnSoundObject(movePreviousSound);
            }
        }

        private void ApplyImmediate()
        {
            scrollPosition = selectedIndex;
            ApplyLayout();
        }

        private void ApplyLayout()
        {
            if (items == null || items.Count == 0) return;

            EnsureCanvasGroups();

            for (int i = 0; i < items.Count; i++)
            {
                if (!items[i])
                    continue;

                float relative = RelativeOffset(i, scrollPosition);
                items[i].anchoredPosition = GetPositionForRelative(relative);
                items[i].localScale = Vector3.one * GetScaleForRelative(relative);

                float alpha = GetAlphaForRelative(relative);
                bool visible = alpha > 0f;
                items[i].gameObject.SetActive(visible);

                canvasGroups[items[i]].alpha = alpha;
                canvasGroups[items[i]].interactable = visible;
                canvasGroups[items[i]].blocksRaycasts = visible;
            }
        }

        // Items between two slots are placed and scaled in between them
        private Vector2 GetPositionForRelative(float relative)
        {
            int lowerSlot = Mathf.FloorToInt(relative);
            float x = Mathf.LerpUnclamped(GetSlotX(lowerSlot), GetSlotX(lowerSlot + 1), relative - lowerSlot);
            return new Vector2(x, 0f);
        }

        private float GetScaleForRelative(float relative)
        {
            int lowerSlot = Mathf.FloorToInt(relative);
            return Mathf.LerpUnclamped(GetScaleForSlot(lowerSlot), GetScaleForSlot(lowerSlot + 1), relative - lowerSlot);
        }

        // Fully visible inside the window, fading out over the next slot (or before the point where looping items wrap
        // around to the other side, so they never pop)
        private float GetAlphaForRelative(float relative)
        {
            int half = HalfWindow;
            float fadeEnd = half + 1f;
            if (loop)
            {
                fadeEnd = Mathf.Min(fadeEnd, items.Count * 0.5f);
            }

            float distance = Mathf.Abs(relative);
            if (distance <= half)
                return 1f;

            if (distance >= fadeEnd)
                return 0f;

            return Mathf.Clamp01(fadeEase.Evaluate(1f - (distance - half) / (fadeEnd - half)));
        }

        private float GetSlotX(int slot)
        {
            int steps = Mathf.Abs(slot);
            float x = 0f;

            // Accumulate spacing step by step, scaled by neighbor sizes
            for (int i = 0; i < steps; i++)
            {
                float scaleA = GetScaleForSlot(i);
                float scaleB = GetScaleForSlot(i + 1);

                float avgScale = (scaleA + scaleB) * 0.5f;
                float scaledSpacing = spacing * spacingByScale.Evaluate(avgScale);
                x += scaledSpacing;
            }

            return slot < 0 ? -x : x;
        }

        private float GetScaleForSlot(int slot)
        {
            int distance = Mathf.Abs(slot);
            if (distance == 0) return centerScale;
            if (distance == 1) return nearScale;
            return sideScale;
        }

        // Returns smallest signed offset from centerPosition to itemIndex in a circular list if loop==true.
        // If loop==false, it becomes linear difference.
        private float RelativeOffset(int itemIndex, float centerPosition)
        {
            float diff = itemIndex - centerPosition;

            if (!loop || items.Count <= 1) return diff;

            // Wrap to (-n/2, n/2]
            float n = items.Count;
            return n * 0.5f - Mathf.Repeat(n * 0.5f - diff, n);
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

        /* Pointer */
        private bool CanUsePointer(PointerEventData eventData)
        {
            return enablePointerInput
                   && eventData.button == PointerEventData.InputButton.Left
                   && items != null
                   && items.Count > 0;
        }

        private int GetItemIndex(GameObject hitObject)
        {
            if (!hitObject)
                return -1;

            for (Transform t = hitObject.transform; t && t != transform; t = t.parent)
            {
                int index = t is RectTransform rt ? items.IndexOf(rt) : -1;
                if (index >= 0)
                    return index;
            }

            return -1;
        }

        private bool TryGetLocalPointerX(Vector2 screenPosition, Camera eventCamera, out float localX)
        {
            bool isOnPlane = RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screenPosition,
                eventCamera, out Vector2 localPoint);
            localX = localPoint.x;
            return isOnPlane;
        }

        // Follows the pointer, except that an item reaching the centre snaps there and stays until the pointer is
        // dragSnapZone further; the carousel then catches up so it is half-way between two items when the pointer is.
        private float GetSnappedDragPosition(float pointerPosition)
        {
            float nearestPosition = Mathf.Round(pointerPosition);
            float distance = pointerPosition - nearestPosition;

            if (Mathf.Abs(distance) <= dragSnapZone)
                return nearestPosition;

            float catchUp = (Mathf.Abs(distance) - dragSnapZone) / (0.5f - dragSnapZone);
            return nearestPosition + Mathf.Sign(distance) * catchUp * 0.5f;
        }

        // An item becomes the selected one when it snaps into the centre (or, without a snap zone, once it is the closest)
        private void UpdateSelectionWhileDragging()
        {
            float nearestPosition = Mathf.Round(scrollPosition);
            if (dragSnapZone > 0f
                && scrollPosition != nearestPosition)
                return;

            int index = GetIndex((int)nearestPosition);
            if (index == selectedIndex)
                return;

            PlayMoveSound(RelativeOffset(index, selectedIndex) > 0f ? 1 : -1);
            selectedIndex = index;
            SelectionChanged?.Invoke(selectedIndex);
        }

        #endregion
    }
}
