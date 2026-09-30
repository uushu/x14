using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class FlowerMask : ARecoverable, IEquatable<FlowerMask>
    {
        //可放置的鲜花的阴影
        public Image imgFlowerMask;

        //可消除的特效
        public GameObject removeEffect;

        public void SetFlowerSprite(Transform parent, string flowerLocation)
        {
            imgFlowerMask.gameObject.SetActive(true);
            removeEffect.SetActive(false);

            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.transform.localPosition = Vector3.zero;
            ResourceMgr.Instance.SetImageSprite(imgFlowerMask, flowerLocation, true);
        }

        public void SetFlowerRemoveEffect(Transform parent)
        {
            transform.SetParent(parent);
            transform.localScale = Vector3.one;
            transform.transform.localPosition = Vector3.zero;

            imgFlowerMask.gameObject.SetActive(false);
            removeEffect.SetActive(true);
        }

        public bool Equals(FlowerMask other)
        {
            return this == other;
        }
    }
}