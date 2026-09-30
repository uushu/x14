using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class FlowerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public FlowerShape flowerParent;

        public Image imgRotate;

        private bool _sendDownEvent;

        //防止手机上的多点同时点击
        private int _eventPointerId;

        public void OnDrag(PointerEventData eventData)
        {
            if (_eventPointerId != eventData.pointerId) return;

            if (_sendDownEvent == false)
            {
                flowerParent.OnPointerDown(eventData);
                _sendDownEvent = true;
            }

            flowerParent.OnDrag(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _eventPointerId = eventData.pointerId;

            if (imgRotate.gameObject.activeSelf &&
                RectTransformUtility.RectangleContainsScreenPoint(imgRotate.rectTransform, eventData.position, UIWindowMgr.Instance.uiCamera))
            {
                //不变大
                _sendDownEvent = false;
            }
            else
            {
                _sendDownEvent = true;
                flowerParent.OnPointerDown(eventData);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_eventPointerId != eventData.pointerId) return;

            if (_sendDownEvent == false)
            {
                flowerParent.OnGridClick(eventData);
            }
            else
            {
                flowerParent.OnPointerUp(eventData);
            }
        }
    }
}