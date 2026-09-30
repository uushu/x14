using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class HorizontalEx : MonoBehaviour
    {
        public List<SpriteRenderer> allRenders;

        [LabelText("间距")]
        public float distance;

#if UNITY_EDITOR

        [Button("刷新")]
        private void RefreshPosition()
        {
            var startPosition = transform.position;

            for (int i = 0; i < allRenders.Count; i++)
            {
                allRenders[i].transform.position = startPosition;
                startPosition += new Vector3(distance, 0, 0);
            }
        }
#endif
    }
}