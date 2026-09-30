using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace GameMain.Garden
{
    public enum EPassGameRule
    {
        [LabelText("收集分数")]
        CollectScore,
        [LabelText("收集蝴蝶")]
        CollectButterFly,
        [LabelText("无尽模式")]
        Endless,
    }

    public abstract class ABaseGardenSuccessRule : MonoBehaviour
    {
        public abstract EPassGameRule PassGameRule { get; }

        public Action ShowGamSuccessUI { get; set; }

        public Action ShowFailedUI { get; set; }

        public GameObject addScoreGoRoot;

        public GameObject comboRoot;

        public Animation comboAnimation;

        public TextMeshProUGUI textComboCount;

        public TextMeshProUGUI textAddScore;

        public SkeletonGraphic skeleton;

        public int LevelId { get; private set; }

        private Dictionary<int, string> _awardNameDict = new()
        {
            { 2, "good"},{ 3, "nice"}, { 4, "excellrnr"}, { 5, "Awesoms"}, { 6,"Amazing" }
        };

        private Dictionary<int, float> _comboShowDelayDict = new()
        {
            { 2, 1.9f },{ 3, 2.5f}, { 4, 3f}, { 5, 4f}, { 6, 3.5f }
        };

        protected int _currentComboCount = 0;
        private float _comboTimer;
        private float _comboInterval => ConfigMgr.Instance.Tables.GlobalConfigCategory.GardenComboInterval;

        private Dictionary<int, AudioClipConfig> _removeRowColsAudioMap;
        private Dictionary<int, AudioClipConfig> RemoveRowColsAudioMap
        {
            get
            {
                if (_removeRowColsAudioMap == null)
                {
                    _removeRowColsAudioMap = new()
                    {
                        { 1, AudioName.RemoveLine1}, { 2, AudioName.RemoveLine2},
                        { 3, AudioName.RemoveLine3}, { 4, AudioName.RemoveLine4},
                        { 5, AudioName.RemoveLine5}, { 6, AudioName.RemoveLine6},
                    };
                }

                return _removeRowColsAudioMap;
            }
        }

        public virtual void Initialize(int levelId, object parameter)
        {
            LevelId = levelId;

            comboRoot.SetActive(false);
            addScoreGoRoot.SetActive(false);
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack += OnPlaceToMapBefore;
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack += OnPlaceToMapAfter;
        }

        public virtual void UnInitialize()
        {
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapBeforeCallBack -= OnPlaceToMapBefore;
            GardenGameRoot.Instance.flowerGridsMap.OnPlaceToMapAfterCallBack -= OnPlaceToMapAfter;
        }

        private void Update()
        {
            _comboTimer -= Time.deltaTime;
        }

        protected virtual void ShowGameSuccessUI()
        {
            Debugger.Log("游戏成功 - CollectFlowerRule");
            ShowGamSuccessUI?.Invoke();
            ShowGamSuccessUI = null;
            GameEventDefine.SendGameSuccessResult(EMiniGameType.GardenFlower, LevelId);
        }

        protected virtual void OnPlaceToMapBefore(FlowerRemoveResult result)
        {
            var rows = result.rowList;
            var cols = result.colList;
            var removeFlowers = result.removeFlowers;
            var screenPoint = result.putInMapScreenPoint;

            //播放消除动画
            var removeTotalCount = rows.Count + cols.Count;
            if (removeTotalCount > 0) AddComboCount(screenPoint);

            PlayRemoveRowColsAudio(removeTotalCount);
            int addScore = GetAddScore(result);
            PlayAwardSpineAnimation(removeTotalCount, addScore, screenPoint);

            if (IsGameSuccess())
            {
                //游戏成功
                GardenGameRoot.Instance.ChangeStatus(EStatus.Success);
            }

            GameEventDefine.SendGameRemoveRowCols(rows.Count, cols.Count);
        }

        protected virtual void OnPlaceToMapAfter()
        {
            if (IsGameFailed())
            {
                ShowFailedUI?.Invoke();
            }
        }

        private void PlayRemoveRowColsAudio(int comboCount)
        {
            if (comboCount > 0)
            {
                if (RemoveRowColsAudioMap.TryGetValue(comboCount, out var audioConfig) == false)
                {
                    //超过了最大值,用最大值
                    audioConfig = RemoveRowColsAudioMap.LastOrDefault().Value;
                }

                AudioMgr.Instance.PlayAudioOneShot(audioConfig);
            }
        }

        private void ShowAddScoreAnimation(int addScore, Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(addScoreGoRoot.transform.parent.GetComponent<RectTransform>(),
                screenPoint,
                UIWindowMgr.Instance.uiCamera,
                out var localPoint);

            localPoint.x = Mathf.Clamp(localPoint.x, -360f + 200f, 360f - 200f);
            addScoreGoRoot.transform.localPosition = localPoint;

            textAddScore.alpha = 1;
            textAddScore.text = $"+{addScore}";
            textAddScore.gameObject.SetActive(addScore > 0);

            addScoreGoRoot.SetActive(true);
            textAddScore.transform.localScale = Vector3.zero;
            var sequence = DOTween.Sequence();
            var t1 = textAddScore.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            var t2 = textAddScore.DOFade(0, 0.2f);
            sequence.Append(t1);
            sequence.AppendInterval(1f);
            sequence.Append(t2);
            sequence.OnComplete(() =>
            {
                addScoreGoRoot.SetActive(false);
            });
        }

        protected virtual int GetAddScore(FlowerRemoveResult result)
        {
            return 0;
        }

        private async void PlayAwardSpineAnimation(int removeTotalCount, int addScore, Vector3 screenPoint)
        {
            skeleton.gameObject.SetActive(false);

            if (removeTotalCount > 1)
            {
                if (_comboShowDelayDict.TryGetValue(removeTotalCount, out var delayTime) == false)
                {
                    delayTime = _comboShowDelayDict.LastOrDefault().Value;
                }
                await UniTask.WaitForSeconds(delayTime);

                if (_awardNameDict.TryGetValue(removeTotalCount, out var comboName) == false)
                {
                    comboName = _awardNameDict.LastOrDefault().Value;
                }

                skeleton.gameObject.SetActive(true);
                skeleton.AnimationState.SetAnimation(0, comboName, false);

                ShowAddScoreAnimation(addScore, screenPoint);
            }
        }

        private void AddComboCount(Vector3 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(comboRoot.transform.parent.GetComponent<RectTransform>(),
              screenPoint,
              UIWindowMgr.Instance.uiCamera,
              out var localPoint);

            if (_comboTimer < 0)
            {
                _currentComboCount = 0;
            }
            else
            {
                _currentComboCount++;
            }

            _comboTimer = _comboInterval;

            if (_currentComboCount > 0)
            {
                comboRoot.SetActive(true);

                textComboCount.text = _currentComboCount.ToString();

                localPoint.x = Mathf.Clamp(localPoint.x, -360f + 200f, 360f - 200f);
                comboRoot.transform.localPosition = localPoint;
                var sequence = DOTween.Sequence();
                comboAnimation.Play("ComboAnimationopen");

                StartCoroutine(DelayAction(1f, () =>
                {
                    comboAnimation.Play("ComboAnimationclose");
                }));
            }
            else
            {
                comboRoot.SetActive(false);
            }
        }

        protected abstract bool IsGameSuccess();

        protected virtual bool IsGameFailed()
        {
            if (GardenGameRoot.Instance.currentStatus == EStatus.Gaming)
            {
                //判断场上的格子是否还可以放置在地图上
                var isFailed = GardenGameRoot.Instance.waitingArea.IsGameFailed;
                return isFailed;
            }
            return false;
        }

        protected IEnumerator DelayHide(GameObject target, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            target.SetActive(false);
        }

        protected IEnumerator DelayAction(float delayTime, Action delayAction)
        {
            yield return new WaitForSeconds(delayTime);
            delayAction?.Invoke();
        }
    }
}