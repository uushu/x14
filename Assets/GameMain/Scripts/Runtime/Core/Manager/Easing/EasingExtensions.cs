using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public static class EasingExtensions
    {
        public static IEasingTask DoAnchorPosition(this RectTransform rect, Vector2 targetPosition, float duration, float delay = 0)
        {
            return new Vector3EasingTask(rect.anchoredPosition3D, new Vector3(targetPosition.x, targetPosition.y, rect.anchoredPosition3D.z), duration, delay, (position) => rect.anchoredPosition3D = position);
        }

        public static IEasingTask DoSizeDelta(this RectTransform rect, Vector2 targetSizeDelta, float duration, float delay = 0)
        {
            Vector2 startSizeDelta = rect.sizeDelta;

            return new Vector2EasingTask(startSizeDelta, targetSizeDelta, duration, delay, (position) => rect.sizeDelta = position);
        }

        public static IEasingTask DoPosition(this Transform transform, Vector3 targetPosition, float duration, float delay = 0)
        {
            Vector3 startPosition = transform.position;

            return new Vector3EasingTask(startPosition, targetPosition, duration, delay, (position) =>
            {
                if (transform != null)
                {
                    transform.position = position;
                }
            });
        }

        public static IEasingTask DoPosition(this Transform transform, Transform targetTransform, float duration, float delay = 0)
        {
            Vector3 startPosition = transform.position;

            var coroutine = EasingManager.DoFloat(0, 1, duration, (value) =>
            {
                if (transform != null)
                {
                    transform.position = Vector3.LerpUnclamped(startPosition, targetTransform.position, value);
                }

            }, delay);

            return coroutine;
        }

        public static IEasingTask DoLocalScale(this Transform transform, Vector3 targetScale, float duration, float delay = 0)
        {
            Vector3 startScale = transform.localScale;

            return new Vector3EasingTask(startScale, targetScale, duration, delay, (scale) => transform.localScale = scale);
        }

        public static void StopIfExists(this IEasingTask coroutine)
        {
            if(coroutine != null && coroutine.IsActive) coroutine.Cancel();
        }

        public static bool ExistsAndActive(this IEasingTask coroutine)
        {
            return coroutine != null && coroutine.IsActive;
        }

        public static IEasingTask DoColor(this MaterialPropertyBlock block, string property, Color color, float duration, float delay = 0)
        {
            var id = Shader.PropertyToID(property);

            var startColor = block.GetColor(id);

            return new ColorEasingTask(startColor, color, duration, delay, (color) => block.SetColor(id, color));
        }

        public static IEasingTask DoColor(this Material material, string property, Color color, float duration, float delay = 0)
        {
            var id = Shader.PropertyToID(property);

            var startColor = material.GetColor(id);

            return new ColorEasingTask(startColor, color, duration, delay, (color) => material.SetColor(id, color));
        }

        public static IEasingTask DoColor(this Material material, int propertyId, Color color, float duration, float delay = 0)
        {
            var startColor = material.GetColor(propertyId);

            return new ColorEasingTask(startColor, color, duration, delay, (color) => material.SetColor(propertyId, color));
        }

        public static IEasingTask DoFloat(this Material material, string property, float value, float duration, float delay = 0)
        {
            var id = Shader.PropertyToID(property);

            var startValue = material.GetFloat(id);

            return new FloatEasingTask(startValue, value, duration, delay, (value) => material.SetFloat(id, value));
        }

        public static IEasingTask DoFloat(this Material material, int propertyId, float value, float duration, float delay = 0)
        {
            var startValue = material.GetFloat(propertyId);

            return new FloatEasingTask(startValue, value, duration, delay, (value) => material.SetFloat(propertyId, value));
        }

        public static IEasingTask DoAlpha(this Graphic graphic, float targetAlpha, float duration, float delay = 0)
        {
            var initialAplha = graphic.color.a;

            return new FloatEasingTask(initialAplha, targetAlpha, duration, delay, (alpha) => graphic.SetAlpha(alpha));
        }

        public static IEasingTask DoAlpha(this CanvasGroup canvasGroup, float targetAlpha, float duration, float delay = 0)
        {
            var initialAplha = canvasGroup.alpha;

            return new FloatEasingTask(initialAplha, targetAlpha, duration, delay, (alpha) => canvasGroup.alpha = alpha);
        }

        public static IEasingTask DoAlpha(this SpriteRenderer spriteRederer, float targetAlpha, float duration, float delay = 0)
        {
            var initialAplha = spriteRederer.color.a;

            return new FloatEasingTask(initialAplha, targetAlpha, duration, delay, (alpha) => spriteRederer.SetAlpha(alpha));
        }

        public static IEasingTask DoVolume(this AudioSource audioSource, float targetVolume, float duration, float delay = 0)
        {
            return new FloatEasingTask(audioSource.volume, targetVolume, duration, delay, (volume) => audioSource.volume = volume);
        }
    }
}