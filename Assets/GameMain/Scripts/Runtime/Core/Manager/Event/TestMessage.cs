using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    public struct TestMessage : IMessage
    {
        public int Id;

        public string Name; 

        public string Description;

        public override string ToString()
        {
            return $"Id:{Id} Name:{Name} Desc:{Description}";
        }
    }
}