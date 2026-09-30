using UnityEngine;

namespace GameMain
{
    public class CanvasScale : MonoBehaviour
    {
        public RectTransform canvas;
        
        private void Start()
        {
            var safeArea = Screen.safeArea;

            float scale = Screen.width / (float)Screen.height;

            float scale2 = 750 / 1334f;
            
            Debugger.LogError($"scale={scale}");
            
            Debugger.LogError($"scale2={scale2}");

            scale = scale / scale2;
            
            canvas.transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
