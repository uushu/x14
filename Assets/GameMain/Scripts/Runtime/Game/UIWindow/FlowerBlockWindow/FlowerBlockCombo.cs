using cfg;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerBlock
{
    public class FlowerBlockCombo : ABaseComponent
    {
        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.Animation mComboAnimation;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public TMPro.TextMeshProUGUI mTxtComboCount;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public Spine.Unity.SkeletonGraphic mSkeletonGraphic_piaozi;

        [Sirenix.OdinInspector.FoldoutGroup("组件")]
        public UnityEngine.UI.Image mRedMask;

        [LabelText("连击间隔")]
        public float comboMaxInterval;

        //连击倒计时
        private float _comboTimer;

        [LabelText("当前已经累积的连击次数")]
        public int currentComboCount;

        private Dictionary<int, string> _awardNameDict;

        private Dictionary<int, AudioClipConfig> _comboAudioDict;

        private Dictionary<int, AudioClipConfig> _removeAudioDict;

        private float _awardLeftClampX;
        private float _awardRightClampX;

        public void Initialize()
        {
            mComboAnimation.gameObject.SetActive(false);

            _awardNameDict = new()
            {
                { 2, "good"},
                { 3, "great"}, 
                { 4, "excellrnr"}, 
                { 5, "Amazing"}, 
                { 6,"Unblievable" }
            };

            _comboAudioDict = new()
            {
                { 2, AudioName.Block_Combo1},
                { 3, AudioName.Block_Combo2},
                { 4, AudioName.Block_Combo3},
                { 5, AudioName.Block_Combo4},
                { 6, AudioName.Block_Combo5},
            };

            _removeAudioDict = new()
            {
                {1, AudioName.Block_Remove1 },
                {2, AudioName.Block_Remove2 },
                {3, AudioName.Block_Remove3 },
                {4, AudioName.Block_Remove4 },
            };

            BlockGameRoot.Instance.blockMap.OnBeforeRemoveFlowerBlockCallBack += OnBeforeRemoveFlowerBlock;
            BlockGameRoot.Instance.OnAfterDragFlowerBlockItemCallback += OnAfterDragFlowerBlockItem;

            //限制特效在屏幕内
            var worldLeftX = UIWindowMgr.Instance.uiCamera.ScreenToWorldPoint(
                new Vector3(0, 0));

            var worldRightX = UIWindowMgr.Instance.uiCamera.ScreenToWorldPoint(
                new Vector3(UIWindowMgr.Instance.safeAreaBase.ScreenWidth, 0));

            var rectSize = mSkeletonGraphic_piaozi.GetComponent<RectTransform>().sizeDelta;
            _awardLeftClampX = mSkeletonGraphic_piaozi.transform.parent.InverseTransformPoint(worldLeftX).x + rectSize.x * 0.5f;
            _awardRightClampX = mSkeletonGraphic_piaozi.transform.parent.InverseTransformPoint(worldRightX).x - rectSize.x * 0.5f;

            mRedMask.gameObject.SetActive(false);
        }

        public void UnInitialize()
        {
            BlockGameRoot.Instance.blockMap.OnBeforeRemoveFlowerBlockCallBack -= OnBeforeRemoveFlowerBlock;
            BlockGameRoot.Instance.OnAfterDragFlowerBlockItemCallback -= OnAfterDragFlowerBlockItem;
        }

        private void OnBeforeRemoveFlowerBlock(FlowerBlockRemoveResult result)
        {
            currentComboCount++;
            _comboTimer = comboMaxInterval;

            StopAllCoroutines();
            StartCoroutine(ShowComboCount(currentComboCount));

            if (result.downBlockItems.Count > 0)
            {
                var randomRIndex = UtilityTool.RandomNumber(result.removeRowsIndex);
                if (result.downBlockItems.TryGetValue(randomRIndex, out var downBlockItem))
                {
                    ShowAwardAnimation(currentComboCount, randomRIndex, downBlockItem);
                }

                AudioMgr.Instance.PlayAudioOneShot(AudioName.Block_FlowerBloom);
            }
        }

        private void OnAfterDragFlowerBlockItem()
        {
            if (BlockGameRoot.Instance.currentStatus != EStatus.Gaming) return;

            //判断是否顶格了 红色提示
            var maxBlockIndex = BlockGameRoot.Instance.blockMap.GetMaxBlockYIndex();
            if (BlockGameRoot.Instance.blockMap.uicustomGrids.rowCount == (maxBlockIndex + 1))
            {
                //红色提示
                ShowRedMaskAnimation();
            }
        }

        private void ShowRedMaskAnimation()
        {
            mRedMask.gameObject.SetActive(true);
            mRedMask.SetAlpha(0f);
            mRedMask.DOFade(1, 0.3f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
            {
                mRedMask.gameObject.SetActive(false);
            });
        }

        private void ShowAwardAnimation(int comboCount, int yIndex, ItemUI_FlowerBlock downBlockItem)
        {
            //连击的动画
            if (_awardNameDict.DictTryGetOrLast(comboCount, out var animationName, (key) => key > _awardNameDict.Count))
            {
                var localPoint = mSkeletonGraphic_piaozi.transform.parent.InverseTransformPoint(downBlockItem.transform.position);
                //y上移动一行
                var offsetY = BlockGameRoot.Instance.blockMap.uicustomGrids.gridHeight;
                localPoint.y = localPoint.y + offsetY;
                localPoint.x = Mathf.Clamp(localPoint.x, _awardLeftClampX, _awardRightClampX);

                mSkeletonGraphic_piaozi.transform.localPosition = localPoint;
                mSkeletonGraphic_piaozi.AnimationState.SetAnimation(0, animationName, false);
            }

            //连击的音效
            if (_comboAudioDict.DictTryGetOrLast(comboCount, out var audioConfig, (key) => key > _comboAudioDict.Count))
            {
                AudioMgr.Instance.PlayAudioOneShot(audioConfig);
            }

            if (_removeAudioDict.DictTryGetOrLast(comboCount, out audioConfig, (key) => key > _removeAudioDict.Count))
            {
                AudioMgr.Instance.PlayAudioOneShot(audioConfig);
            }
        }

        private IEnumerator ShowComboCount(int comboCount)
        {
            if (comboCount < 2) yield break;

            mTxtComboCount.text = comboCount.ToString();
            mComboAnimation.gameObject.SetActive(true);

            mComboAnimation.Play("ComboAnimationopen");
            yield return new WaitForSeconds(2f);
            mComboAnimation.Play("ComboAnimationclose");
            yield return new WaitForSeconds(1f);

            mComboAnimation.gameObject.SetActive(false);
        }

        public void ResetTimer()
        {
            _comboTimer = 0;
            currentComboCount = 0;
        }

        private void Update()
        {
            if (BlockGameRoot.Instance.currentStatus == EStatus.Gaming)
            { 
                _comboTimer -= Time.deltaTime;

                if (_comboTimer < 0)
                {
                    currentComboCount = 0;
                }
            }
        }
    }
}