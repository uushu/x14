using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class DelayActiveObject : MonoBehaviour
    {
        public List<GameObject> delayShowList;
        
        [LabelText("—”≥Ÿœ‘ æ")]
        public float delayTime;

        private void OnEnable()
        {
            StartCoroutine(DelayTimeShow());
        }

        private IEnumerator DelayTimeShow()
        {
            for (int i = 0; i < delayShowList.Count; i++)
            {
                delayShowList[i].SetActive(false);
            }
            
            yield return new WaitForSeconds(delayTime);

            for (int i = 0; i < delayShowList.Count; i++)
            {
                delayShowList[i].SetActive(true);
            }
        }
    }
}