using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public abstract class ABasePanel : ABaseComponent, IUISystem
    {
        public ABaseWindow BaseWindow { get; set; }

        public void Initialize(object parameters)
        {
            OnShow(parameters);
        }

        public void Tick(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void UIRefresh()
        {
            OnRefresh();
        }

        public void UnInitialize()
        {
            OnHide();
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

        protected void CloseSelfWindow()
        {
            if (BaseWindow != null)
            {
                UIWindowMgr.Instance.CloseWindow(BaseWindow, BaseWindow.showPopAnimation).Forget();
            }
        }
    }
}