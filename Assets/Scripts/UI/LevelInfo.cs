using TMPro;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI creatorId;
    [SerializeField] TextMeshProUGUI rating;
    [SerializeField] TextMeshProUGUI votes;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI levelID;
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI playCount;



    public void RequestUpdateLevelInfo() => UpdateLevelInfo(FirestoreManager.Instance.LevelData);

    public void UpdateLevelInfo(LevelData data)
    {
        creatorId.text = data.CreatorId.ToString();
        rating.text = data.DifficultyRating.ToString();
        votes.text = data.Upvotes.ToString()+"/"+ data.Downvotes.ToString();
        level.text = data.Level.ToString();
        levelID.text = data.LevelId.ToString();
        status.text = data.Status.ToString();
        playCount.text = data.PlayCount.ToString();
    }



}
