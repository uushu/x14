using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public interface IEvent 
    {
        void Invoke(IMessage message);
    }
}