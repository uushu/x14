using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class UIPropList : MonoBehaviour
    {
        public List<ItemUI_BagProp> uiBagProps;

        public void RefreshBagProp(List<ItemData_BagProp> dataList)
        {
            for (int i = 0; i < uiBagProps.Count; i++)
            {
                if (dataList.Count > i)
                {
                    uiBagProps[i].gameObject.SetActive(true);
                    uiBagProps[i].RefreshBagPropUI(dataList[i]);
                }
                else
                {
                    uiBagProps[i].gameObject.SetActive(false);
                }
            }
        }

#if UNITY_EDITOR

        [Button("刷新")]
        private void OnValidate()
        {
            uiBagProps = transform.GetComponentsInChildren<ItemUI_BagProp>().ToList();
        }
#endif
    }
}
