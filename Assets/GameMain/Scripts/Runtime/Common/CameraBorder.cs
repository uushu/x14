using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    [RequireComponent(typeof(Camera))]
    public class CameraBorder : MonoBehaviour
    {
        public static Camera GameCamera { get; private set; }

        [LabelText("屏幕左下角-世界坐标系位置")]
        public static Vector3 ScreenLDWorldPoint { get; private set; }

        [LabelText("屏幕左角-世界坐标系位置")]
        public static Vector3 ScreenLUWorldPoint { get; private set; }

        [LabelText("屏幕右下角-世界坐标系位置")]
        public static Vector3 ScreenRDWorldPoint { get; private set; }

        [LabelText("屏幕右上角-世界坐标系位置")]
        public static Vector3 ScreenRUWorldPoint { get; private set; }

        [LabelText("屏幕中心点-世界坐标系位置")]
        public static Vector3 ScreenCenterWorldPoint { get; private set; }

        public static float ScreenHalfWidth { get; private set; }

        public static float ScreenHalfHeight { get; private set; }

        private void Awake()
        {
            GameCamera = GetComponent<Camera>();
            GetScreenBorderPoint();
        }

        private void GetScreenBorderPoint()
        {
            var screenHeight = UIWindowMgr.Instance.safeAreaBase.ScreenHeight;
            var screenWidth = UIWindowMgr.Instance.safeAreaBase.ScreenWidth;

            ScreenHalfWidth = screenWidth * 0.5f;
            ScreenHalfHeight = screenHeight * 0.5f;

            ScreenLDWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0));
            ScreenLUWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, screenHeight));
            ScreenRDWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, 0));
            ScreenRUWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight));
            ScreenCenterWorldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenWidth * 0.5f, screenHeight * 0.5f));
        }

        public static Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            return GameCamera.WorldToScreenPoint(worldPosition);
        }

        public static Vector3 ScreenToWorldPoint(Vector3 inputPosition)
        {
            inputPosition.z = -GameCamera.transform.position.z;
            return GameCamera.ScreenToWorldPoint(inputPosition);
        }
    }
}