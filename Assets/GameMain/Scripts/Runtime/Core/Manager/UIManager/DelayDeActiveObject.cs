using Sirenix.OdinInspector;
using UnityEngine;

namespace GameMain
{
    public class DelayDeActiveObject : MonoBehaviour
    {
        [LabelText("延迟时间")]
        public float delayTime;

        [LabelText("是否进入池子")]
        public bool pushToPool;

        private float _delayTimer;

        private void OnEnable()
        {
            _delayTimer = 0;
        }

        private void Update()
        {
            _delayTimer += Time.deltaTime;

            if (_delayTimer > delayTime)
            {
                if (pushToPool)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}