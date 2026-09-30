using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public class ItemUI_CollectButterfly : MonoBehaviour
    {
        public Image imgFlwoer;

        public Image imgComplete;

        public TextMeshProUGUI txtCollectFlowerCount;

        //最大收集数量
        private int _maxCollectButterflyCount;

        //剩余的蝴蝶数量,用于动画显示
        private int _lastFlowerCount;

        //已经手机的蝴蝶数量
        private int _collectButterflyCount;

        public int flowerId;

        public bool AnimationComplete => _lastFlowerCount <= 0;
        public bool LogicComplete => _collectButterflyCount >= _maxCollectButterflyCount;

        public void Initialize(int flowerId, int maxCollectCount, string flowerSpriteLocation)
        {
            this.flowerId = flowerId;
            _maxCollectButterflyCount = maxCollectCount;
            _lastFlowerCount = maxCollectCount;
            _collectButterflyCount = 0;

            var flowerLevelConfig = ConfigMgr.Instance.Tables.GardenFlowerLevelCategory.Get(flowerId, 1);
            ResourceMgr.Instance.SetImageSprite(imgFlwoer, flowerSpriteLocation, true);

            RefreshCollectFlowerCount();
        }

        //用于收集是否成功的逻辑判断
        public void AddLogic_CollectCount(int addCollectCount)
        {
            _collectButterflyCount += addCollectCount;
            _collectButterflyCount = Mathf.Min(_collectButterflyCount, _maxCollectButterflyCount);
        }

        public void SubAnimation_LastCollectCount(int subCount = 1)
        {
            _lastFlowerCount -= subCount;
            _lastFlowerCount = Mathf.Max(0, _lastFlowerCount);
            RefreshCollectFlowerCount();
            DoHitScaleAnimation();
        }

        public void DoHitScaleAnimation()
        {
            if (DOTween.IsTweening(transform))
            {
                return;
            }

            AudioMgr.Instance.PlayAudioOneShot(AudioName.ButterflyHitAudio);
            var sequence = DOTween.Sequence();
            var t1 = transform.DOScale(Vector3.one * 1.2f, 0.1f);
            var t2 = transform.DOScale(Vector3.one, 0.1f);
            sequence.Append(t1);
            sequence.Append(t2);
            sequence.SetTarget(transform);
        }

        private void RefreshCollectFlowerCount()
        {
            if (AnimationComplete)
            {
                imgComplete.gameObject.SetActive(true);
                txtCollectFlowerCount.gameObject.SetActive(false);
            }
            else
            {
                imgComplete.gameObject.SetActive(false);
                txtCollectFlowerCount.gameObject.SetActive(true);

                txtCollectFlowerCount.text = $"X{_lastFlowerCount}";
            }
        }
    }
}