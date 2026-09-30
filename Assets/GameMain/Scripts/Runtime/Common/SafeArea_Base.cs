using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public abstract class SafeArea_Base : MonoBehaviour
    {
         public List<RectTransform> rects;

        public RectTransform topRect;

        public RectTransform bottomRect;

        public CanvasScaler canvasScaler;

        protected Rect LastSafeArea = new Rect(0, 0, 0, 0);

        public float ScreenWidth { get; protected set; }
        public float ScreenHeight { get; protected set; }

        private void Start()
        {
            OnStart();
            
            StartCoroutine(FirstStart());
        }

        protected abstract void OnStart();

        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
            {
                ApplySafeArea(safeArea);
            }
        }

        protected virtual Rect GetSafeArea()
        {
#if UNITY_EDITOR
            
            return Screen.safeArea;
#else
            return LastSafeArea;
#endif
        }

        protected IEnumerator FirstStart()
        {
            var safeArea = GetSafeArea();

            while (float.IsNaN(safeArea.x))
            {
                safeArea = GetSafeArea();

                yield return new WaitForEndOfFrame();
            }

            ApplySafeArea(safeArea);
        }

        private void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= ScreenWidth;
            anchorMin.y /= ScreenHeight;
            anchorMax.x /= ScreenWidth;
            anchorMax.y /= ScreenHeight;

            foreach (var rect in rects)
            {
                rect.anchorMin = anchorMin;
                rect.anchorMax = anchorMax;
            }

            float multi = canvasScaler.referenceResolution.x / ScreenWidth;
            topRect.sizeDelta = new Vector2(ScreenWidth * multi, (ScreenHeight - r.y - r.height) * multi);
            bottomRect.sizeDelta = new Vector2(ScreenWidth * multi, r.y * multi);

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, ScreenWidth, ScreenHeight);
        }
    }
}