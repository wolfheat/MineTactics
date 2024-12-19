using TMPro;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [Header("Normal")]
    [SerializeField] TextMeshProUGUI normalWon;
    [SerializeField] TextMeshProUGUI normalLost;
    [SerializeField] TextMeshProUGUI normalPercent;
    [Header("Challenge")]
    [SerializeField] TextMeshProUGUI challengeWon;
    [SerializeField] TextMeshProUGUI challengeLost;
    [SerializeField] TextMeshProUGUI challengePercent;
    [Header("Additional")]
    [SerializeField] TextMeshProUGUI player;
    [SerializeField] TextMeshProUGUI rating;
    [SerializeField] TextMeshProUGUI register;
    [Header("Final")]
    [SerializeField] TextMeshProUGUI status;
    [SerializeField] TextMeshProUGUI votes;

    public static StatsPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        UpdateStats();
    }
    public void UpdateStats()
    {
        Debug.Log("** ** Updating Stats Panel info, player: "+ SavingUtility.gameSettingsData.PlayerName);

        // WIN LOSS PERCENT - NORMAL
        int won = SavingUtility.gameSettingsData.NormalWon;
        int lost = SavingUtility.gameSettingsData.NormalLost;
        float percent = (won+lost)==0?0:(100f*won/ (won+ lost));
        normalPercent.text = percent.ToString("F2") + "%";
        normalLost.text = lost.ToString();
        normalWon.text = won.ToString();

        // WIN LOSS PERCENT - CHALLENGE
        won = SavingUtility.gameSettingsData.ChallengeWon;
        lost = SavingUtility.gameSettingsData.ChallengeLost;
        percent = (won + lost) == 0 ? 0 : (100f * won / (won+ lost));
        challengePercent.text = percent.ToString("F2") + "%";
        challengeLost.text = lost.ToString();
        challengeWon.text= won.ToString();
        player.text = SavingUtility.gameSettingsData.PlayerName.ToString();   
        rating.text = SavingUtility.gameSettingsData.Rating.ToString();
        register.text = SavingUtility.gameSettingsData.Registration.ToString();


    }

}
