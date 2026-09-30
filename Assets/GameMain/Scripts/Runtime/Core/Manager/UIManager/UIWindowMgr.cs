using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using GameMain.RedDot;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMain
{
    public enum EWindowHierarchy
    {
        [LabelText("底部")]
        Bottom = 0,
        [LabelText("场景层")]
        Middle1 = 100,
        [LabelText("中部")]
        Middle2 = 120,
        [LabelText("屏幕适配层")] 
        Middle3 = 140,
        [LabelText("小游戏层")]
        Middle4 = 160,
        [LabelText("小游戏层-屏幕适配")]
        Middle5 = 170,
        [LabelText("小游戏层2")] 
        Middle6 = 180,
        [LabelText("弹窗")]
        Pop = 200,
        [LabelText("引导")] 
        Guide = 300,
        [LabelText("顶部")]
        Top = 501,
    }

    public class UIWindowMgr : AMonoManager<UIWindowMgr>
    {
        private Dictionary<string, ABaseWindow> _windowDict = new Dictionary<string, ABaseWindow>();

        private readonly List<string> _loadingWindow = new List<string>();

        [LabelText("UI相机")]
        public Camera uiCamera;
        [LabelText("Canvas")]
        public Canvas canvas;

        [LabelText("底部层")]
        public RectTransform bottomHierarchy;
        [LabelText("场景层")]
        public RectTransform middleHierarchy;
        [LabelText("中部")]
        public RectTransform middleHierarchy2;
        [LabelText("导航栏层-屏幕适配")]
        public RectTransform middleHierarchy3;
        [LabelText("小游戏层")]
        public RectTransform middleHierarchy4;
        [LabelText("小游戏层-屏幕适配")]
        public RectTransform middleHierarchy5;
        [LabelText("小游戏层2")]
        public RectTransform middleHierarchy6;
        [LabelText("弹窗")]
        public RectTransform popHierarchy;
        [LabelText("引导")]
        public RectTransform guideHierarchy;
        [LabelText("顶部")]
        public RectTransform topHierarchy;

        [LabelText("加载中窗口")]
        public GameObject loadingWindow;

        [LabelText("池子根节点")]
        public RectTransform poolRoot;

        [LabelText("UI置灰材质")]
        public Material uiGray_Static;
        [ReadOnly]
        [LabelText("窗口打开顺序列表")]
        public WindowOpenQueue windowShowQueue;

        [LabelText("点击空白处特效")]
        public ParticleSystem clickEmptyEffect;

        public event Action<string> OnWindowOpen;

        public event Action<string> OnWindowClose;

        [HideInInspector]
        public SafeArea_Base safeAreaBase;

        private SingleObjPool<ABaseWindow> _windowPool;

        private ObjPool _clickEffectPool;

        private void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
            var safeDy = GetComponent<SafeArea_DY>();
            var safeWx = GetComponent<SafeArea_WX>();
            var safeMobile = GetComponent<SafeArea_Mobile>();
            _windowPool = new(poolRoot, loadingWindow.transform);

            safeMobile.enabled = safeDy.enabled = safeWx.enabled = false;

            _clickEffectPool = new(clickEmptyEffect.gameObject, poolRoot);

#if UNITY_WECHAT_GAME && !UNITY_EDITOR
 
            safeAreaBase = safeWx;
            safeWx.enabled = true;
            //  transform.Find("EventSystem").gameObject.AddComponent<WXTouchInputOverride>();

#elif UNITY_DOUYIN_GAME && !UNITY_EDITOR
            
            safeAreaBase = safeDy;
            safeDy.enabled = true;

#else
            safeAreaBase = safeMobile;
            safeMobile.enabled = true;
#endif
        }

        protected override void OnEnter()
        {
            windowShowQueue.ClearWindows();
        }

        protected override void OnExit()
        
        
        
        {
            _windowDict.Clear();
            _windowDict = null;

            windowShowQueue.ClearWindows();
            windowShowQueue = null;
        }

        protected override void OnUpdateInternal(float deltaTime)
        {
            RedDotMgr.Instance.OnUpdate(deltaTime);

            foreach (var window in _windowDict)
            {
                window.Value.Tick(deltaTime);
            }

            if (Input.GetMouseButtonDown(0) && _windowDict.Count > 0)
            {
                if (IsPointerOverUI(Input.mousePosition) == false)
                {
                    var mousePosition = Input.mousePosition;

                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(guideHierarchy, mousePosition, uiCamera,
                        out var localPoint))
                    {
                        var clickEffectEntity = _clickEffectPool.GetFromPoolSync();
                        var clickEffectGo = clickEffectEntity.Item1;

                        clickEffectGo.transform.SetParent(topHierarchy);
                        clickEffectGo.transform.localScale = Vector3.one;
                        clickEffectGo.transform.localPosition = localPoint;

                        DelayPushEffectToPool(clickEffectGo);
                    }
                }
            }
        }

        private async void DelayPushEffectToPool(GameObject clickEffect)
        {
            await UniTask.WaitForSeconds(2f);
            _clickEffectPool.PushToPool(clickEffect);
        }

        public bool IsPointerOverUI(Vector3 mousePosition)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            eventData.pressPosition = mousePosition;
            eventData.position = mousePosition;

            var list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, list);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].gameObject.tag.Equals("IgnoreUI"))
                {
                    list.RemoveAt(i);
                }
            }

            return list.Count > 0;
        }

        private Transform GetWindowParent(EWindowHierarchy hierarchy)
        {
            switch (hierarchy)
            {
            case EWindowHierarchy.Bottom:

                return bottomHierarchy;

            case EWindowHierarchy.Middle1:

                return middleHierarchy;

            case EWindowHierarchy.Middle2:

                return middleHierarchy2;

            case EWindowHierarchy.Middle3:

                return middleHierarchy3;

            case EWindowHierarchy.Middle4:

                return middleHierarchy4;

            case EWindowHierarchy.Middle5:

                return middleHierarchy5;

            case EWindowHierarchy.Middle6:

                return middleHierarchy6;

            case EWindowHierarchy.Pop:

                return popHierarchy;

            case EWindowHierarchy.Top:

                return topHierarchy;

            case EWindowHierarchy.Guide:

                return guideHierarchy;
            }

            return bottomHierarchy;
        }

        private async UniTask<ABaseWindow> InstantiateWindow(string windowName)
        {
            var (baseWindow, isFromPool) = await _windowPool.GetFromPool(windowName);

            if (isFromPool == false)
            {
                baseWindow.OnWindowAwake();
            }

            return baseWindow;
        }

        public async UniTask<ABaseWindow> OpenWindowAsync(string windowName, object parameters = null)
        {
            if (_loadingWindow.Contains(windowName))
            {
                Debugger.LogError($"正在加载窗口:{windowName}");
                return null;
            }

            Debugger.Log($"打开窗口:{windowName}", Debugger.ELogColor.Orange);

            if (_windowDict.TryGetValue(windowName, out ABaseWindow baseWindow))
            {
                Debugger.Log($"窗口已经被打开:{windowName}", Debugger.ELogColor.Yellow);

                baseWindow.transform.SetAsLastSibling();
                baseWindow.UIRefresh();
                return baseWindow;
            }

            //防止多次加载
            _loadingWindow.Add(windowName);
            baseWindow = await InstantiateWindow(windowName);
            _loadingWindow.Remove(windowName);

            var windowParent = GetWindowParent(baseWindow.windowHierarchy);
            baseWindow.transform.SetParent(windowParent);
            var windowRect = baseWindow.GetComponent<RectTransform>();
            windowRect.offsetMin = Vector2.zero;
            windowRect.offsetMax = Vector2.zero;
            baseWindow.transform.localRotation = Quaternion.Euler(Vector3.zero);
            baseWindow.transform.localPosition = Vector3.zero;
            baseWindow.transform.localScale = Vector3.one;

            if (!baseWindow.isIgnoreAddToUIStack)
            {
                //如果上一层打开的是二级弹窗 关闭二级弹窗
                string topWindow = windowShowQueue.PeekWindow();
                if (topWindow != null && _windowDict.TryGetValue(topWindow, out ABaseWindow topBase) &&
                    topBase.IsPopWindow)
                {
                    topBase.gameObject.SetActive(false);
                    topBase.OnCover(windowName);
                }

                windowShowQueue.EnqueueWindow(windowName);
            }

            _windowDict.Add(windowName, baseWindow);

            baseWindow.Initialize(parameters);
            baseWindow.UIRefresh();
            baseWindow.OnTop();

            //动画
            if (baseWindow.showPopAnimation)
            {
                await baseWindow.DoShowPopAnimation();
            }

            OnWindowOpen?.Invoke(windowName);

            return baseWindow;
        }

        public async UniTask<T1> OpenWindowAsync<T1, T2>(T2 parameter) where T1 : ABaseWindow where T2 : class, new()
        {
            return await OpenWindowAsync<T1>(parameter);
        }

        public async UniTask<T> OpenWindowAsync<T>(object parameters = null) where T : ABaseWindow
        {
            Type type = typeof(T);

            string windowName = type.Name;
            ABaseWindow temp = await OpenWindowAsync(windowName, parameters);
            return temp as T;
        }

        public void OpenWindow<T>(object parameters = null) where T : ABaseWindow, new()
        {
            OpenWindowAsync<T>(parameters).Forget();
        }

        public bool WindowIsOpen(string windowName)
        {
            if (_windowDict.TryGetValue(windowName, out var baseWindow))
            {
                return true;
            }

            return false;
        }

        public bool WindowIsLoading(string windowName)
        {
            return _loadingWindow.Contains(windowName);
        }

        public void RefreshUIWindow<T>() where T : ABaseWindow
        {
            RefreshUIWindow(typeof(T).Name);
        }

        public async void ShowPromptBox(string content)
        {
            var instance = await ResourceMgr.Instance.InstantiateGameObject(nameof(UIPromptBox), topHierarchy, false);
            instance.GetComponent<UIPromptBox>().ShowPromptBox(content);
        }

        public async UniTask ShowMaskWindow(float alpha)
        {
          
        }

        public void CloseMaskWindow()
        {
        }
        
        public void RefreshUIWindow(string windowName)
        {
            if (_windowDict.TryGetValue(windowName, out ABaseWindow baseWindow))
            {
                baseWindow.UIRefresh();
            }
            else
            {
                Debugger.LogWarning($"不存在的窗口:{windowName}");
            }
        }

        public async UniTask CloseWindow(string windowName, bool doCloseAnimation)
        {
            if (_windowDict.TryGetValue(windowName, out ABaseWindow window))
            {
                Debugger.Log($"关闭窗口:{windowName}", Debugger.ELogColor.Orange);

                if (doCloseAnimation)
                {
                    await window.DoClosePopAnimation();

                    OnCloseWindow(windowName, window);
                }
                else
                {
                    OnCloseWindow(windowName, window);
                }
            }
            else
            {
                Debugger.Log($"关闭窗口失败,不存在的窗口:{windowName}", Debugger.ELogColor.Red);
            }
        }

        public async UniTask CloseWindow(EWindowHierarchy hierarchy, List<string> exclude = null)
        {
            var removeList = new List<ABaseWindow>();

            //批量关闭这个层级的UI
            foreach (var window in _windowDict.Values)
            {
                if (window.windowHierarchy == hierarchy)
                {
                    if (exclude != null && exclude.Contains(window.GetType().Name))
                    {
                        continue;
                    }

                    removeList.Add(window);
                }
            }

            removeList.Reverse();

            foreach (var window in removeList)
            {
                await CloseWindow(window, false);
            }
        }

        public async UniTask CloseAllWindow(List<string> exclude)
        {
            var closeList = new List<ABaseWindow>();

            foreach (var window in _windowDict.Values)
            {
                if (exclude != null && exclude.Contains(window.GetType().Name))
                {
                    continue;
                }

                closeList.Add(window);
            }

            foreach (var window in closeList)
            {
                await CloseWindow(window, false);
            }
        }

        private void UnInitializeWindow(ABaseWindow window)
        {
            if (window == null) return;

            window.UnInitialize();
            window.OnWindowDestroy();
            GameObject.Destroy(window.gameObject);
            //_windowPool.PushToPool(window);
        }

        private void OnCloseWindow(string windowName, ABaseWindow window)
        {
            UnInitializeWindow(window);

            var isQueue = windowShowQueue.RemoveWindow(windowName);

            if (isQueue && windowShowQueue.IsEmptyWindow() == false)
            {
                var topWindow = windowShowQueue.PeekWindow();

                if (_windowDict.TryGetValue(topWindow, out window))
                {
                    Debugger.Log($"刷新顶部窗口:{topWindow}", Debugger.ELogColor.Orange);

                    window.gameObject.SetActive(true);

                    window.UIRefresh();
                    window.OnTop();
                }
                else
                {
                    Debug.LogError($"不存在的显示的窗口:{topWindow}");
                }
            }

            _windowDict.Remove(windowName);

            OnWindowClose?.Invoke(windowName);
        }

        public async UniTask CloseWindow(ABaseWindow window, bool doCloseAnimation)
        {
            Type type = window.GetType();

            string windowName = type.Name;

            await CloseWindow(windowName, doCloseAnimation);
        }

        public async UniTask CloseWindow<T>(bool doCloseAnimation) where T : ABaseWindow
        {
            Type type = typeof(T);

            string windowName = type.Name;

            await CloseWindow(windowName, doCloseAnimation);
        }

        public T GetWindow<T>() where T : ABaseWindow
        {
            Type type = typeof(T);

            string windowName = type.Name;

            var window = GetWindow(windowName);

            if (window == null) return null;

            return window as T;
        }

        public ABaseWindow GetWindow(string windowName)
        {
            _windowDict.TryGetValue(windowName, out ABaseWindow window);

            return window;
        }

        public string GetTopWindowName()
        {
            if (_loadingWindow.Count > 0)
            {
                return _loadingWindow[0];
            }

            return windowShowQueue.PeekWindow();
        }

        public void ClearAllWindow()
        {
            foreach (var window in _windowDict.Values)
            {
                window.UnInitialize();
                window.OnWindowDestroy();
                GameObject.Destroy(window.gameObject);
            }

            _windowDict.Clear();

            GC.Collect();

            _loadingWindow.Clear();
        }
    }
}