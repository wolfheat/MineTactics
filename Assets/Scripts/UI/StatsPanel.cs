using System.Collections.Generic;
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
    [Header("Specific")]
    [SerializeField] TextMeshProUGUI[] specifics;
    [SerializeField] TextMeshProUGUI[] specifics3BV;
    [SerializeField] TextMeshProUGUI[] originals;
    [SerializeField] TextMeshProUGUI[] originals3BV;

    [Header("Pages")]
    [SerializeField] GameObject mainPage;
    [SerializeField] GameObject specificPage;

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
        mainPage.SetActive(true);
        specificPage.SetActive(false);
    }
    public void Next()
    {
        Debug.Log("Next!");
        if (mainPage.activeSelf)
        {
            mainPage.SetActive(false);
            specificPage.SetActive(true);
        }
        else
        {
            mainPage.SetActive(true);
            specificPage.SetActive(false);
        }
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
        Debug.Log(" ** Loading player name from gamesettingsdata PLayername: "+ SavingUtility.gameSettingsData.PlayerName.ToString());
        rating.text = SavingUtility.gameSettingsData.Rating.ToString();
        register.text = SavingUtility.gameSettingsData.Registration.ToString();

        List<float> records = SavingUtility.gameSettingsData.Records;
        List<float> records3bv = SavingUtility.gameSettingsData.Records3BV;
        Debug.Log("records size "+ records.Count);
        Debug.Log("specifics size "+specifics.Length);
        // Specifics
        for (int i = 0; i < records.Count; i++)
        {
            float record = records[i];
            float record3bv = records3bv[i];
            if (record == 0)
                specifics[i].text = "-----";
            else
                specifics[i].text = record.ToString("F3");
            if (record3bv == 0)
                specifics3BV[i].text = "-----";
            else
                specifics3BV[i].text = record3bv.ToString("F3");
        }
        List<float> originalRecords = SavingUtility.gameSettingsData.OriginalRecords;
        List<float> originalRecords3bv = SavingUtility.gameSettingsData.OriginalRecords3BV;

        for (int i = 0; i < originalRecords.Count; i++)
        {
            float record = originalRecords[i];
            float record3bv = originalRecords3bv[i];
            if (record == 0)
                originals[i].text = "-----";
            else
                originals[i].text = record.ToString("F3");

            if (record3bv == 0)
                originals3BV[i].text = "-----";
            else
                originals3BV[i].text = record3bv.ToString("F3");
        }

    }

}
