using System;
using TMPro;
using UnityEngine;

public class ListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI index_text;
    [SerializeField] TextMeshProUGUI id_text;
    private int index = 0;
    private LevelData levelData;

    public void RequestDeleteLevel()
    {
        Debug.Log("Request Delete Level (ListItem)");
        LocalLevelsPanel.Instance.RemoveIndexFromList(index);
    }
    public void RequestLoadLevel()
    {
        Debug.Log("Request Load Level (ListItem)");
    }

    internal void UpdateData(int i, LevelData newlevelData)
    {
        levelData = newlevelData;
        index_text.text = i.ToString();
        id_text.text    = levelData.LevelId;
        index = i;
    }

    internal void UpdateIndex(int i)
    {
        index_text.text = i.ToString();
        index = i;
    }
}
