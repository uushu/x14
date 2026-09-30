using System.Collections.Generic;

namespace GameMain
{
    [System.Serializable]
    public class WindowOpenQueue
    {

        public List<string> list;

        public void EnqueueWindow(string windowName)
        {
            list.Add(windowName);
        }

        public string PeekWindow()
        {
            if (IsEmptyWindow() == false)
            {
                var lastIndex = list.Count - 1;

                return list[lastIndex];
            }

            Debugger.Log("窗口的数量=0");
            return null;
        }

        public string DequeueWindow()
        {
            if (IsEmptyWindow() == false)
            {
                var lastIndex = list.Count - 1;

                var windowName = list[lastIndex];

                list.RemoveAt(lastIndex);

                return windowName;
            }

            Debugger.LogError("数组数量=0");
            return null;
        }

        public bool RemoveWindow(string windowName)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(windowName))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool IsEmptyWindow()
        {
            return list.Count == 0;
        }

        public void ClearWindows()
        {
            list.Clear();
        }
    }
}