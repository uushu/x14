using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public abstract class ILogicBase : MonoBehaviour
    {
        public virtual void Initialize() { }

        public virtual void OnStart() { }
        public virtual void OnSuccess() { }
        public virtual void OnFailed() { }
        public virtual void UnInitialize() { }
    }
}