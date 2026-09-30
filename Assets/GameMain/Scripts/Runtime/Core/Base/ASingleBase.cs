using System.Collections;
using System.Collections.Generic;

namespace GameMain
{

    public abstract class ASingleBase<T> : AManager where T : AManager, new()
    {
        private static object _lockObj = 1;

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
