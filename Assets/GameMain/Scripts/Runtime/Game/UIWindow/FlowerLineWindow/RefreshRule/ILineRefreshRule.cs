using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain.FlowerLine
{
    public interface ILineRefreshRule
    {
        void Initialize(int levelId);

        void Refresh(out int flowerId, out int flowerLevel);
    }
}