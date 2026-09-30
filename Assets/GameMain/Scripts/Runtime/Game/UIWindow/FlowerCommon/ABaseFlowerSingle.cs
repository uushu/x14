using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace GameMain
{
    public class ABaseFlowerSingle : ARecoverable
    {
        public SkeletonGraphic flowerSpine;

        public Material flowerMaterial;

        public Material materialGray;

        public void SetGray(bool gray)
        {
            if (gray)
            {
                flowerSpine.material = materialGray;
            }
            else
            {
                flowerSpine.material = flowerMaterial;
            }

            SetSpineAlpha(1);
        }

        //0-1
        public void SetSpineAlpha(float alpha)
        {
            flowerSpine.SetAlpha(alpha);
        }

        protected IEnumerator DelayTime(float delaySeconds, Action complete)
        {
            yield return new WaitForSeconds(delaySeconds);
            complete?.Invoke();
        }

#if UNITY_EDITOR

        [Button("Ë¢ÐÂ")]
        protected virtual void OnValidate()
        {
            flowerSpine = transform.GetComponentInChildren<SkeletonGraphic>();
            flowerMaterial = flowerSpine.material;
            materialGray = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/GameMain/AssetBundle/Effect/UIGray_Static.mat");
            flowerSpine.raycastTarget = false;
            flowerSpine.maskable = false;
        }
#endif
    }
}