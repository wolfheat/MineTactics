using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class LevelCompletionScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI collection;
    [SerializeField] TextMeshProUGUI levelID;
    [SerializeField] TextMeshProUGUI Clicks;
    [SerializeField] TextMeshProUGUI B3V;
    [SerializeField] TextMeshProUGUI efficiency;
    [SerializeField] TextMeshProUGUI B3Vs;
    [SerializeField] GameObject B3VRecordText;
    [SerializeField] TextMeshProUGUI rating;
    [SerializeField] TextMeshProUGUI creatorId;
    [SerializeField] TextMeshProUGUI votes;
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] GameObject timeRecordText;
    [SerializeField] TextMeshProUGUI playerRatingChange;
    [SerializeField] TextMeshProUGUI levelRatingChange;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI playCount;
    [SerializeField] Star[] stars;

    private LevelData LoadedData = new LevelData();
    private int vote = 3;

    private void Start() => OnClickStar(vote);
    public void RequestUpdateLevelInfo() => UpdateLevelInfo(FirestoreManager.Instance.LevelData);

    public void UpdateLevelInfo(LevelData data,bool record = false,bool record3BV = false)
    {
        levelID.text = USerInfo.Instance.levelID;
        time.text = Timer.TimeElapsed.ToString("F3") + "s";
        if(timeRecordText != null)
            timeRecordText.SetActive(record);

        B3V.text = GameAreaMaster.Instance.MainGameArea.B3V.ToString();

        if (USerInfo.Instance.currentType == GameType.Challenge) { 
            if (data == null)
                return;
            LoadedData = data;
            creatorId.text = data.CreatorId.ToString();
            //rating.text = data.DifficultyRating.ToString();
            votes.text = data.Upvotes.ToString()+"/"+ data.Downvotes.ToString();
            levelID.text = data.LevelId.ToString();
            collection.text = data.Collection.ToString();
            status.text = data.Status.ToString();
            playCount.text = data.PlayCount.ToString();
            Clicks.text = GameAreaMaster.Instance.MainGameArea.Clicks.ToString();

            //  
        }
        else
        {
            int extraClicks = (GameAreaMaster.Instance.MainGameArea.TotalClicks - GameAreaMaster.Instance.MainGameArea.Clicks);

            Debug.Log("Clicks = "+ GameAreaMaster.Instance.MainGameArea.Clicks);
            Debug.Log("extraClicks = " + extraClicks);

            Clicks.text = GameAreaMaster.Instance.MainGameArea.TotalClicks + " (" + extraClicks + " + "+ GameAreaMaster.Instance.MainGameArea.Clicks + ")";

            float B3VsValue = GameAreaMaster.Instance.MainGameArea.B3V / Timer.TimeElapsed;
            B3Vs.text = B3VsValue.ToString("F3");
            float efficencyValue = (float)GameAreaMaster.Instance.MainGameArea.B3V / GameAreaMaster.Instance.MainGameArea.TotalClicks;
            efficiency.text = (efficencyValue*100).ToString("F0")+"%";
            if (B3VRecordText != null)
                B3VRecordText?.SetActive(record3BV);

        }
        OnClickStar(3); // Sets to 3 star as default
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
        // Send Update to the database for this level? / this will create a write...
        if(USerInfo.Instance.currentType == GameType.Challenge)
        {
            // Select a random level from the retrieved documents
            //FirestoreManager.Instance.GetRandomLevel(1000);

            bool loadNext = SendLevelUpdatesOrStoreForLaterCollectionUpdate();

            // Start Next Level automatically
            if(loadNext)
                LevelCreator.Instance.LoadRandomLevel();
        }
        else if(USerInfo.Instance.currentType == GameType.Normal)
        {
            Debug.Log("Closing Normal Game Result screen - currently do nothing");
        }

    }
    public void Back()
    {
        Debug.Log("Level Completion Back clicked");
        gameObject.SetActive(false);
        if (USerInfo.Instance.currentType == GameType.Challenge)
        {
            // Sent Level Updates when closing resultscreen for loaded level
            SendLevelUpdatesOrStoreForLaterCollectionUpdate();
        }
    }

    private bool SendLevelUpdatesOrStoreForLaterCollectionUpdate()
    {
        // Check if last level in this collection was cleared
        // If so Update the collcetion in the DB
        if(USerInfo.Instance.currentType == GameType.Challenge)
        {

            Debug.Log("Wont send Update cause this is a collection");

            // Sets players Vote and Adds Playcount

            LoadedData.vote = vote;
            Debug.Log("Voting "+vote+" place in LoadedData");

            /* OLD VOTE SYSTEM
            LoadedData.Upvotes = (vote > 3 ? 1 : 0);
            LoadedData.Downvotes = (vote < 3 ? 1 : 0);
            LoadedData.PlayCount = 1;
            */

            // Update the Collection in the Dictionary
            bool updatedSuccess = FirestoreManager.Instance.ReplaceLevelInLevelDataCollections(LoadedData);

            if (updatedSuccess)
                Debug.Log("Updated Level in Collection successfully");                   
            else
                Debug.Log("Could not update Level in Collection.");

            string currentCollection = LoadedData.Collection;

            return FirestoreManager.Instance.SendCollectionToDatabase_IfLastLevelWasCompleted(currentCollection);
            
        }

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


        // Rating - Depends on Players Rating and Game Result and Time
        // IF bust and Level is Higher than player lower its rating by a linear amount from diff with a max change
        // For now increase Level rating by 1 if busted, or lower it if completed

        // Level Rating (Temporatily +1 when cleared -1 when bust)
        dict.Add("DifficultyRating", LoadedData.DifficultyRating);
        //dict.Add("DifficultyRating", LoadedData.DifficultyRating + (bust?-1:1));

        FirestoreManager.Instance.UpdateLevel(dict,LoadedData.LevelId);
        return true;
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
