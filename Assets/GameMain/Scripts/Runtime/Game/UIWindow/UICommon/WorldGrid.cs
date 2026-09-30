using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class WorldGrid : MonoBehaviour
    {
        public Transform childRoot;
        
        public Transform startPosition;

        [LabelText("xy=横向偏移 zw竖向偏移")]
        public Vector4 directionAdd;

        [Button("刷新世界位置")]
        public void RefreshPosition()
        {
            var list = childRoot.GetComponentsInChildren<Transform>().ToList();
            list.RemoveAt(0);

           var startPos = startPosition.position;
            
            for (int i = 0; i < list.Count; i++)
            {
                list[i].position = startPos;
                startPos += new Vector3(directionAdd.x - directionAdd.y, 0, directionAdd.z - directionAdd.w);
            }
        }
    }
}