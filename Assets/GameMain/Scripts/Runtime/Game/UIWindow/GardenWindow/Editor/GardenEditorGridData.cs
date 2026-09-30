using cfg;
using Luban;
using UnityEngine;

namespace GameMain.Garden.Editor
{
#if UNITY_EDITOR

    public class GardenEditorGridData
    {
        public GardenFlowerGridTypeConfig GridTypeConfig { get; private set; }

        public Vector2Int GridStartXY { get; private set; }

        public GardenEditorGridData(GardenFlowerGridTypeConfig config, Vector2Int startGridXY)
        {
            GridTypeConfig = config;
            GridStartXY = startGridXY;
        }
    }

#endif
}