using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain
{
    public abstract class ABaseWindow : ABaseComponent, IUISystem
    {
        [System.Serializable]
        public class PopWindowAnimation
        {
            [LabelText("动画根节点")] public Transform animationRoot;

            [LabelText("打开动画时间")] public float openAnimationTime = 0.15f;

            [LabelText("关闭动画时间")] public float closeAnimationTime = 0f;

            [LabelText("弹窗遮罩背景")] public Image imgMask;
        }

        [LabelText("窗口的层级")] [FoldoutGroup("窗口基础")]
        public EWindowHierarchy windowHierarchy;

        [FoldoutGroup("窗口基础")] [LabelText("忽略加入UI栈")]
        public bool isIgnoreAddToUIStack;

        [FoldoutGroup("窗口基础")] [LabelText("弹窗动画")] [ShowIf("IsPopWindow")]
        public bool showPopAnimation;

        [FoldoutGroup("窗口基础")] [LabelText("动画参数")] [ShowIf("showPopAnimation")]
        public PopWindowAnimation popAnimation;

        [LabelText("UI面板")] public List<ABasePanel> panelList;

        [LabelText("关闭列表")] public List<UICustomButton> closeList;
        [LabelText("适配刘海")] public bool matchingBangs;

        [ShowIf("matchingBangs")] [LabelText("适配刘海的UI")]
        public List<RectTransform> matchingList;

        public System.Action OnCloseEvent { get; set; }

        public bool IsPopWindow => windowHierarchy == EWindowHierarchy.Pop;

        protected readonly EventGroup _eventGroup = new EventGroup();

#if UNITY_EDITOR

        [Button("获取所有Panel")]
        private void GetAllPanels()
        {
            panelList = GetComponentsInChildren<ABasePanel>(true).ToList();
        }

        protected virtual void OnValidate()
        {
            gameObject.name = GetType().Name;

            GetAllPanels();

            if (IsPopWindow && showPopAnimation)
            {
                popAnimation.imgMask = transform.Find("imgMask").GetComponent<Image>();

                if (popAnimation.imgMask == null)
                {
                    Debug.LogError("弹窗遮罩未按照规则命名=imgMask");
                }
            }
        }
#endif

        public virtual void OnWindowAwake()
        {
        }

        public virtual void OnWindowDestroy()
        {
        }

        public void Initialize(object parameters)
        {
            OnShow(parameters);

            foreach (var panel in panelList)
            {
                panel.Initialize(parameters);

                panel.BaseWindow = this;
            }

            foreach (var btnClose in closeList)
            {
                btnClose.AddListener(OnCloseClick);
            }

            if (matchingBangs)
            {
                PlatformMgr.Instance.GetBangsHeight((result, value) =>
                {
                    Debug.Log($"获取屏幕刘海:{result},{value}");
                    if (result)
                    {
                        for (int i = 0; i < matchingList.Count; i++)
                        {
                            matchingList[i].anchoredPosition =
                                new Vector2(matchingList[i].anchoredPosition.x, -value / 2);
                            matchingList[i].sizeDelta = new Vector2(matchingList[i].sizeDelta.x, -value);
                        }
                    }
                });
            }
        }

        public virtual void OnTop() { }

        public virtual void OnCover(string aboveWindowName) { }

        protected virtual bool OnBeforeClose()
        {
            return true;
        }

        protected virtual void OnCloseClick()
        {
            if (OnBeforeClose())
            {
                CloseSelfWindow();
            }
        }

        protected void CloseSelfWindow(bool doCloseAnimation = true)
        {
            UIWindowMgr.Instance.CloseWindow(this, doCloseAnimation && showPopAnimation).Forget();
        }

        protected async void CloseSelfWindowYield(bool doCloseAnimation = true)
        {
            await UniTask.Yield();
            UIWindowMgr.Instance.CloseWindow(this, doCloseAnimation && showPopAnimation).Forget();
        }

        public T GetPanel<T>() where T : ABasePanel
        {
            foreach (var item in panelList)
            {
                if (item is T t)
                {
                    return t;
                }
            }

            Debugger.LogError($"窗口:{GetType().Name} 不存在的组件:{typeof(T).Name}");
            return null;
        }

        public void UnInitialize()
        {
            OnHide();

            foreach (var panel in panelList)
            {
                panel.UnInitialize();
            }

            _eventGroup.RemoveAllListener();

            OnCloseEvent?.Invoke();
        }

        protected virtual void OnShow(object parameters)
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnRefresh()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }

        public void Tick(float deltaTime)
        {
            foreach (var panel in panelList)
            {
                if (panel.gameObject.activeInHierarchy)
                {
                    panel.Tick(deltaTime);
                }
            }

            OnUpdate(deltaTime);
        }

        public void UIRefresh()
        {
            OnRefresh();

            foreach (var panel in panelList)
            {
                if (panel.gameObject.activeInHierarchy)
                {
                    panel.UIRefresh();
                }
            }
        }

        public void RefreshPanel<T>() where T : ABasePanel
        {
            var panel = GetPanel<T>();

            if (panel != null)
            {
                panel.UIRefresh();
            }
        }

        protected virtual void OnAnimationStart(bool isOpen)
        {
        }

        protected virtual void OnAnimationEnd(bool isOpen)
        {
        }

        public virtual async UniTask DoShowPopAnimation()
        {
            if (popAnimation.animationRoot == null)
            {
                Debugger.LogError($"未设置的弹窗的根节点 {GetType().Name} = NULL");
                return;
            }

            foreach (var closeBtn in closeList)
            {
                closeBtn.enabled = false;
            }

            popAnimation.animationRoot.DOKill();

            OnAnimationStart(true);

            if (IsPopWindow && showPopAnimation && popAnimation.imgMask == null)
            {
                popAnimation.imgMask = transform.Find("imgMask").GetComponent<Image>();
            }

            if (popAnimation.imgMask != null)
            {
                popAnimation.imgMask.color = new Color(0, 0, 0, 0.7f);
            }

            popAnimation.animationRoot.localScale = Vector3.one * 0.4f;

            await popAnimation.animationRoot.DOScale(Vector3.one, popAnimation.openAnimationTime).SetEase(Ease.OutBack).SetUpdate(true);

            foreach (var closeBtn in closeList)
            {
                if (closeBtn) closeBtn.enabled = true;
            }

            OnAnimationEnd(true);
        }

        public virtual async UniTask DoClosePopAnimation()
        {
            if (popAnimation.animationRoot == null)
            {
                Debugger.LogError("未设置的弹窗的根节点 = NULL");
                return;
            }

            if (DOTween.IsTweening(popAnimation.animationRoot))
            {
                Debugger.Log("正在播放弹窗动画");
                return;
            }

            popAnimation.animationRoot.DOKill();

            OnAnimationStart(false);

            if (popAnimation.imgMask != null)
            {
                var t1 = popAnimation.imgMask.DOFade(0, popAnimation.closeAnimationTime).SetUpdate(true);;
            }

            var t2 = popAnimation.animationRoot.DOScale(Vector3.one * 0.3f, popAnimation.closeAnimationTime)
                .SetEase(Ease.InBack).SetUpdate(true);;
            await t2;

            OnAnimationEnd(false);
        }
    }
}