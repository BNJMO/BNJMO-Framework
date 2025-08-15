using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Serialization;

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
            sprite = newSprite;

            if (UnityImage)
            {
                UnityImage.sprite = newSprite;
            }
            if (UnityRawImage)
            {
                UnityRawImage.texture = newSprite ? newSprite.texture : null;
            }
            if (UnitySpriteRenderer)
            {
                UnitySpriteRenderer.sprite = newSprite;
            }
        }

        public void SetLocalizedSprite(LocalizedSprite newLocalizedSprite)
        {
            if (newLocalizedSprite == null)
                return;
            
            localizedSprite = newLocalizedSprite;
            RefreshLocalizedSprite();
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

        [BoxGroup("BImage"), SerializeField, LabelText("Use Localization")]
        private bool useLocalization;
    
        [BoxGroup("BImage"), SerializeField, ShowIf("@UseLocalization == false")]
        private Sprite sprite;

        [BoxGroup("BImage"), SerializeField, ShowIf("@UseLocalization == true")]
        private LocalizedSprite localizedSprite;

        [BoxGroup("BImage"), SerializeField]
        private Color color = Color.white;
        
        [BoxGroup("BImage"), SerializeField]
        private bool matchSpriteResolutionSize = false;

        [Header("References")]
        [BoxGroup("BImage"), SerializeField] 
        private Image unityImage;

        [BoxGroup("BImage"), SerializeField] 
        private RawImage unityRawImage;

        [BoxGroup("BImage"), SerializeField] 
        private SpriteRenderer unitySpriteRenderer;

        [BoxGroup("BImage")] [Button("Match Parent Size")]
        private void MatchParentSize_Button() => MatchParentSize();

        [BoxGroup("BImage")] [Button("Match Match Sprite Resolution Size")]
        private void MatchSpriteResolutionSize_Button() => MatchSpriteResolutionSize();

        [BoxGroup("BImage")] [Button("Refresh Localized Sprite")]
        private void RefreshLocalizedSprite_Button() => RefreshLocalizedSprite();
        
        #endregion
        
        #region Variables
        
        public bool UseLocalization
        {
            get => useLocalization;
            set => useLocalization = value;
        }

        public Sprite Sprite => useLocalization ? localizedSpriteValue : sprite;

        public LocalizedSprite LocalizedSprite => localizedSprite;
        
        public Color ImageColor => color;
    
        public float ImageOpacity => color.a;

        public Image UnityImage => unityImage;

        public RawImage UnityRawImage => unityRawImage;

        public SpriteRenderer UnitySpriteRenderer => unitySpriteRenderer;
        
        private Sprite localizedSpriteValue = null;

        #endregion

        #region LifeCycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;

            objectNamePrefix = "I_";

            base.OnValidate();

            // Get or refresh components
            SetComponentIfNull(ref unityImage);
            SetComponentIfNull(ref unityRawImage);
            SetComponentIfNull(ref unitySpriteRenderer);

            // Only assign sprite if not using localization
            if (!useLocalization)
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
            }
            
            if (matchSpriteResolutionSize)
            {
                MatchSpriteResolutionSize();
            }

            SetSprite(sprite);
            SetColor(color);
        }

        protected override void Awake()
        {
            base.Awake();

            // Get or refresh components
            SetComponentIfNull(ref unityImage);
            SetComponentIfNull(ref unityRawImage);
            SetComponentIfNull(ref unitySpriteRenderer);
            if (UnityImage == null 
                && UnityRawImage == null 
                && UnitySpriteRenderer == null)
            {
                LogConsoleError("No Image, RawImage, or SpriteRenderer component found on this GameObject!");
            }

            if (useLocalization 
                && localizedSprite != null)
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


        private void RefreshLocalizedSprite()
        {
            if (!useLocalization
                || localizedSprite == null)
                return;
            
#if UNITY_EDITOR
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
#endif
        }
        
        private void MatchSpriteResolutionSize()
        {
            if (sprite == null)
                return;
            
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sprite.texture.width);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sprite.texture.height);
            }
        }
        
        #endregion
    }
}
