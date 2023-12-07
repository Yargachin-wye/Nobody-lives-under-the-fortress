using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataContainer
{
    public List<DialogueNodeData> dataList;

    public DataContainer(List<DialogueNodeData> _dataList)
    {
        dataList = _dataList;
    }
}
