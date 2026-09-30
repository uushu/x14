using System;
using cfg;
using System.Runtime.Serialization;

namespace GameMain
{
    [Serializable]
    public abstract class ASaveData
    {
        public abstract void SetDefaultData(GlobalConfigCategory category);
        public virtual void OnAfterDeserialize() { }
    }
}