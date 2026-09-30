using System.Collections;
using System.Collections.Generic;
using cfg;
using Luban;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


[LabelText("背包道具类型")]
[System.Serializable]
public class BagPropId
{
    [ValueDropdown("GetIdList")]
    [LabelText("道具Id")]
    public int bagPropId;

    public static implicit operator int(BagPropId boxItemId)
    {
        return boxItemId.bagPropId;
    }

    public static implicit operator BagPropId(int bagPropId)
    {
        return new BagPropId() { bagPropId = bagPropId };
    }

    public override string ToString()
    {
        return $"bagPropId={bagPropId}";
    }

#if UNITY_EDITOR

    private IEnumerable GetIdList()
    {
        var bytes = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameMain/GameRes/LubanData/Bytes/BagPropCategory.bytes");
        var category = new BagPropCategory(new ByteBuf(bytes.bytes));
        var list = new ValueDropdownList<int>();
        foreach (var config in category.DataList)
        {
            list.Add(new ValueDropdownItem<int>($"{config.Name}({config.Id})", config.Id));
        }
        return list;
    }
#endif
}
