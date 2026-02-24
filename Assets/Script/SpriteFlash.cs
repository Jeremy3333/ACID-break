using System.Collections;
using UnityEngine;

namespace SpriteFlash
{
    public class SimpleFlash : MonoBehaviour
    {
        [Tooltip("Color to use for the flash.")]
        [SerializeField]
        private Color flashColor = Color.white;

        [Tooltip("Duration of the flash.")]
        [SerializeField]
        private float duration = 0.2f;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashRoutine;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        public void Flash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            // Changer la couleur pour le flash
            spriteRenderer.color = flashColor;

            // Attendre la durée du flash
            yield return new WaitForSeconds(duration);

            // Revenir à la couleur originale
            spriteRenderer.color = originalColor;

            flashRoutine = null;
        }
    }
}