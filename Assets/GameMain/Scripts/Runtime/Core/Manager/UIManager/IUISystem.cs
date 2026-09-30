using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public interface IUISystem
    {
        void Initialize(object parameters);
        void Tick(float deltaTime);
        void UnInitialize();
        void UIRefresh();
    }
}