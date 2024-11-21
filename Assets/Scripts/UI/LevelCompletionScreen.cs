﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class LevelCompletionScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelID;
    [SerializeField] TextMeshProUGUI rating;
    [SerializeField] TextMeshProUGUI creatorId;
    [SerializeField] TextMeshProUGUI votes;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI playerRatingChange;
    [SerializeField] TextMeshProUGUI levelRatingChange;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI playCount;
    [SerializeField] Star[] stars;

    private LevelData LoadedData = new LevelData();
    private int vote = 3;

    private void Start() => OnClickStar(vote);
    public void RequestUpdateLevelInfo() => UpdateLevelInfo(FirestoreManager.Instance.LevelData);

    public void UpdateLevelInfo(LevelData data)
    {
        if (USerInfo.Instance.currentType == GameType.Normal)
        {
            levelID.text = USerInfo.Instance.levelID;
            time.text = Timer.TimeElapsed.ToString();
        }
        else {         
            if (data == null)
                return;
            LoadedData = data;
            creatorId.text = data.CreatorId.ToString();
            rating.text = data.DifficultyRating.ToString();
            votes.text = data.Upvotes.ToString()+"/"+ data.Downvotes.ToString();
            levelID.text = data.LevelId.ToString();
            status.text = data.Status.ToString();
            playCount.text = data.PlayCount.ToString();
            time.text = Timer.TimeElapsed.ToString();
        }
    }
    
    public void SetVote(int set)
    {
        Debug.Log("Setting Vote to "+vote);
        vote = set;
    }
    public void Next()
    {
        Debug.Log("Level Completion Next clicked");
        gameObject.SetActive(false);
        SendLevelUpdates();
        // Send Update to the database for this level? / this will create a write...
    }
    public void Back()
    {
        Debug.Log("Level Completion Back clicked");
        gameObject.SetActive(false);
        SendLevelUpdates();
    }

    private void SendLevelUpdates()
    {
        // Generate Dictionary of changes to the Level
        Dictionary<string, object> dict = new();

        int newUpVotes   = (vote > 3 ? 1 : 0) + LoadedData.Upvotes;
        int newDownVotes = (vote < 3 ? 1 : 0) + LoadedData.Downvotes;
        Debug.Log("New Up Votes: "+newUpVotes+" New down votes: "+newDownVotes+" vote = "+vote);
        // VOTES
        if (vote > 3)
            dict.Add("Upvotes", LoadedData.Upvotes + 1);
        if (vote < 3)
            dict.Add("Downvotes", LoadedData.Downvotes + 1);
        
        // Status Change
        if(newUpVotes-newDownVotes >= 10 && LoadedData.Status == "Pending")
            dict.Add("Status", "Approved");
        if(newUpVotes-newDownVotes < 10 && LoadedData.Status == "Approved")
            dict.Add("Status", "Pending");

        // PlayCount
        dict.Add("PlayCount", LoadedData.PlayCount + 1);


        bool bust = LevelCreator.Instance.LevelBusted;
        // Rating - Depends on Players Rating and Game Result and Time
        // IF bust and Level is Higher than player lower its rating by a linear amount from diff with a max change
        // For now increase Level rating by 1 if busted, or lower it if completed

        // Level Rating (Temporatily +1 when cleared -1 when bust)
        dict.Add("DifficultyRating", LoadedData.DifficultyRating + (bust?-1:1));

        FirestoreManager.Instance.UpdateLevel(dict,LoadedData.LevelId);
    }

    public void OnClickStar(int amt)
    {
        Debug.Log("Highlight "+amt+" stars");
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].ShowStar(i<amt?1:0);
        }
        SetVote(amt);
    }
}
