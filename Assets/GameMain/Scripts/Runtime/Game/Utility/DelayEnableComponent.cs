using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public class DelayEnableComponent : MonoBehaviour
    {
        public MonoBehaviour targetComponent;

        public float delayEnableTime;

        private float _timer;
        
        private void OnEnable()
        {
            targetComponent.enabled = false;
            _timer = 0;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > delayEnableTime)
            {
                targetComponent.enabled = true;
            }
        }
    }
}