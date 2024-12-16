using System;
using TMPro;
using UnityEngine;

public class LevelInfoPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatorId;
    [SerializeField] TextMeshProUGUI collection;
    [SerializeField] TextMeshProUGUI rating;
    [SerializeField] TextMeshProUGUI votes;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI levelID;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI playCount;
    [SerializeField] GameObject panel;

    private LevelData Data;
    public void RequestUpdateLevelInfo() => UpdateLevelInfo(FirestoreManager.Instance.LevelData);

    public void LoadSelected()
    {
        LocalLevelsPanel.Instance.LoadLevel(Data);
    }
    
    public void HideLevelInfo()
    {
        panel.gameObject.SetActive(false);
    }
    
    public void UpdateLevelInfo(LevelData data)
    {
        // Shows panel
        if(!panel.activeSelf)
            panel.gameObject.SetActive(true);

        // Updates if new data
        if (Data?.LevelId == data.LevelId)
            return;
        Data = data;
        creatorId.text = data.CreatorId.ToString();
        collection.text = data.Collection.ToString();
        rating.text = data.DifficultyRating.ToString();
        votes.text = data.Upvotes.ToString()+"/"+ data.Downvotes.ToString();
        level.text = data.Level.ToString();
        levelID.text = data.LevelId.ToString();
        status.text = data.Status.ToString();
        playCount.text = data.PlayCount.ToString();

        // 
        ShowMiniView();
    }

    private void ShowMiniView() => GameAreaMaster.Instance.MiniViewGameArea.ShowLevel(Data);
}
