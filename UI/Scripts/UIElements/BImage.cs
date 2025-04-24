using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace BNJMO
{
    public class BImage : BUIElement
    {
        #region Public Methods

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

        public void SetOpacity(float opacity)
        {
            color = new Color(color.r, color.g, color.b, opacity);

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

        [BoxGroup("BImage")] 
        [SerializeField] 
        private Sprite sprite = null;

        [BoxGroup("BImage")] 
        [SerializeField] 
        private Color color = Color.white;

        #endregion

        #region Variables
        public Image UnityImage { get; private set; }

        public RawImage UnityRawImage { get; private set; }

        public SpriteRenderer UnitySpriteRenderer { get; private set; }

        #endregion

        #region LifeCycle
        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            
            objectNamePrefix = "I_";

            base.OnValidate();

            // Revalidate Image
            UnityImage = GetComponent<Image>();
            if (UnityImage && sprite == null)
            {
                sprite = UnityImage.sprite;
            }
            UnityRawImage = GetComponent<RawImage>();
            if (UnityRawImage && sprite == null)
            {
                Texture2D texture = (Texture2D)UnityRawImage.texture;
                sprite = Sprite.Create(texture, 
                    new Rect(0, 0, texture.width, texture.height), 
                    new Vector2(0.5f, 0.5f));
            }
            UnitySpriteRenderer = GetComponent<SpriteRenderer>();
            if (UnitySpriteRenderer && sprite == null)
            {
                sprite = UnitySpriteRenderer.sprite;
            }
            SetSprite(sprite);
            SetColor(color);
        }

        protected override void Awake()
        {
            base.Awake();

            // Check Image is note null
            UnityImage = GetComponent<Image>();
            UnityRawImage = GetComponent<RawImage>();
            UnitySpriteRenderer = GetComponent<SpriteRenderer>();
            if ((UnityImage == null) && (UnityRawImage == null) && (UnitySpriteRenderer == null))
            {
                LogConsoleError("No Image, RawImage or SpriteRenderer component found on this gameobject!");
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

        #endregion
    }
}
