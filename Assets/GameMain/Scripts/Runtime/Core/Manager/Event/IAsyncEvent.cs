using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

namespace GameMain
{
    public interface IAsyncEvent
    {
        UniTask InvokeAsync(IMessage message);
    }
}