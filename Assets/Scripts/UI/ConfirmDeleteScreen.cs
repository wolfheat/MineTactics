using System;
using TMPro;
using UnityEngine;

public class ConfirmDeleteScreen : MonoBehaviour
{
    private int ActiveIndex = -1;
    [SerializeField] TextMeshProUGUI index_text;
    public void SetActiveIndex(int index)
    {
        ActiveIndex = index;
        index_text.text = "Are you sure you want to delete level " +ActiveIndex+ "?";
    }

    public void OnAcceptDelete()
    {
        LocalLevelsPanel.Instance.RemoveIndexFromList(ActiveIndex);

        PanelController.Instance.ShowFadableInfo("Level Deleted");

        gameObject.SetActive(false);
    }
}
