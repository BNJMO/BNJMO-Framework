using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace BNJMO
{
    public class BImage : BUIElement
    {
        #region Public Methods

        [Obsolete("Use ImageColor attribute instead.")]
        public Color GetColor()
        {
            if (UnityImage)
            {
                return UnityImage.color;
            }
            if (UnityRawImage)
            {
                return UnityRawImage.color;
            }
            if (UnitySpriteRenderer)
            {
                return UnitySpriteRenderer.color;
            }

            return color;
        }
        
        [Obsolete("Use ImageOpacity attribute instead.")]
        public float GetOpacity()
        {
            if (UnityImage)
            {
                return UnityImage.color.a;
            }
            if (UnityRawImage)
            {
                return UnityRawImage.color.a;
            }
            if (UnitySpriteRenderer)
            {
                return UnitySpriteRenderer.color.a;
            }

            return color.a;
        }
        
        public void SetSprite(Sprite newSprite)
        {
            if (newSprite == null)
            {
                return;
            }
            sprite = newSprite;

            if (UnityImage)
            {
                UnityImage.sprite = newSprite;
            }
            if (UnityRawImage)
            {
                UnityRawImage.texture = newSprite.texture;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.sprite = newSprite;
            }
        }

        public void SetColor(Color newColor)
        {
            color = newColor;

            if (UnityImage)
            {
                UnityImage.color = color;
            }
            if (UnityRawImage)
            {
                UnityRawImage.color = color;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.color = color;
            }
        }

        public void SetOpacity(float alpha)
        {
            Color newColor = new Color(color.r, color.g, color.b, alpha);
            SetColor(newColor);
        }

        public bool GetRaycast()
        {
            if (UnityImage)
            {
                return UnityImage.raycastTarget;
            }
            if (UnityRawImage)
            {
                return UnityRawImage.raycastTarget;
            }
            return false;
        }

        public void SetRaycast(bool isEnabled)
        {
            if (UnityImage)
            {
                UnityImage.raycastTarget = isEnabled;
            }
            if (UnityRawImage)
            {
                UnityRawImage.raycastTarget = isEnabled;
            }
        }

        public void SetFillAmount(float fillAmount)
        {
            if (UnityImage)
            {
                UnityImage.fillAmount = fillAmount;
            }
            else
            {
                LogConsoleWarning("This BImage is not using a Unity Image. Therefore, fillAmount won't be applied.");
            }
        }
        
        public void SetComponentEnabled(bool isEnabled)
        {
            if (UnityImage)
            {
                UnityImage.enabled = isEnabled;
            }
            if (UnityRawImage)
            {
                UnityRawImage.enabled = isEnabled;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.enabled = isEnabled;
            }
        }
        #endregion

        #region Inspector Values
        
        [BoxGroup("BImage", centerLabel: true)]
        [SerializeField] [BoxGroup("BImage")] [ShowIf("@UseLocalization == false")]
        private Sprite sprite;

        [SerializeField] [BoxGroup("BImage")] [ShowIf("@UseLocalization == true")]
        private LocalizedSprite localizedSprite;

        [SerializeField] [BoxGroup("BImage")] [LabelText("Use Localization")]
        private bool UseLocalization;

        [BoxGroup("BImage")] [SerializeField] 
        private Color color = Color.white;

        [BoxGroup("BImage")] [SerializeField] 
        private bool matchParentSize = false;

        [BoxGroup("BImage")] [Button("Match Parent Size")]
        private void MatchParentSize_Button() => MatchParentSize();

        [BoxGroup("BImage")] [Button("Refresh Localized Sprite")]
        private void RefreshLocalizedSprite_Button() => RefreshLocalizedSprite();

        #endregion

        #region Variables

        public Sprite Sprite => UseLocalization ? localizedSpriteValue : sprite;

        public Color ImageColor => color;
    
        public float ImageOpacity => color.a;

        public Image UnityImage { get; private set; }

        public RawImage UnityRawImage { get; private set; }

        public SpriteRenderer UnitySpriteRenderer { get; private set; }
        
        private Sprite localizedSpriteValue = null;

        #endregion

        #region LifeCycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;

            objectNamePrefix = "I_";

            base.OnValidate();

            // Get or refresh components
            UnityImage = GetComponent<Image>();
            UnityRawImage = GetComponent<RawImage>();
            UnitySpriteRenderer = GetComponent<SpriteRenderer>();

            // Only assign sprite if not using localization
            if (!UseLocalization)
            {
                if (UnityImage 
                    && sprite == null)
                {
                    sprite = UnityImage.sprite;
                    color = UnityImage.color;
                }

                if (UnityRawImage 
                    && sprite == null 
                    && UnityRawImage.texture != null)
                {
                    Texture2D texture = UnityRawImage.texture as Texture2D;
                    if (texture != null)
                    {
                        sprite = Sprite.Create(texture, 
                            new Rect(0, 0, texture.width, texture.height), 
                            new Vector2(0.5f, 0.5f));
                        color = UnityRawImage.color;
                    }
                }

                if (UnitySpriteRenderer 
                    && sprite == null)
                {
                    sprite = UnitySpriteRenderer.sprite;
                    color = UnitySpriteRenderer.color;
                }

                SetSprite(sprite);
            }

            SetColor(color);

            if (matchParentSize)
            {
                MatchParentSize();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            UnityImage = GetComponent<Image>();
            UnityRawImage = GetComponent<RawImage>();
            UnitySpriteRenderer = GetComponent<SpriteRenderer>();

            if ((UnityImage == null) && (UnityRawImage == null) && (UnitySpriteRenderer == null))
            {
                LogConsoleError("No Image, RawImage, or SpriteRenderer component found on this GameObject!");
            }

            if (UseLocalization && localizedSprite != null)
            {
                // Subscribe to AssetChanged event to update sprite immediately on localization change
                localizedSprite.AssetChanged += sprite =>
                {
                    localizedSpriteValue = sprite;
                    SetSprite(sprite);
                };

                // Trigger async load and cache result
                localizedSprite.LoadAssetAsync().Completed += handle =>
                {
                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        localizedSpriteValue = handle.Result;
                        SetSprite(handle.Result);
                    }
                };
            }
            else
            {
                // Use regular sprite if not localized
                SetSprite(sprite);
            }

            SetColor(color);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (localizedSprite != null)
            {
                localizedSprite.AssetChanged -= SetSprite;
            }
        }

        #endregion

        #region Private Methods
        
        protected override void OnUIShown()
        {
            base.OnUIShown();

            if (UnityImage)
            {
                UnityImage.enabled = true;
            }
            if (UnityRawImage)
            {
                UnityRawImage.enabled = true;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.enabled = true;
            }
        }

        protected override void OnUIHidden()
        {
            base.OnUIHidden();

            if (UnityImage)
            {
                UnityImage.enabled = false;
            }
            if (UnityRawImage)
            {
                UnityRawImage.enabled = false;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.enabled = false;
            }
        }


#if UNITY_EDITOR
        private void RefreshLocalizedSprite()
        {
            if (UseLocalization && localizedSprite != null)
            {
                // Force reload the localized sprite asynchronously
                localizedSprite.LoadAssetAsync().Completed += handle =>
                {
                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        // Update the sprite manually here (cache field + SetSprite)
                        localizedSpriteValue = handle.Result;
                        SetSprite(handle.Result);
                    }
                };
            }
        }
#endif
        
        private void MatchParentSize()
        {
            RectTransform rectTransform = transform as RectTransform;
            RectTransform parentRectTransform = transform.parent as RectTransform;
            if (parentRectTransform)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentRectTransform.rect.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   parentRectTransform.rect.height);
            }
        }
        
        #endregion
    }
}
