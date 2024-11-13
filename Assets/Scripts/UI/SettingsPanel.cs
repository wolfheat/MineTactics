using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameSize {S,M,L}

public class SettingsPanel : MonoBehaviour
{
    int enumGameSize = Enum.GetNames(typeof(GameSize)).Length;
    public static int activeGameSize = 0;
    [SerializeField] TextMeshProUGUI gameSizeText;
    [SerializeField] TextMeshProUGUI mineDens;
    [SerializeField] TextMeshProUGUI winprob;
    [SerializeField] Toggle pendingToggle;

    private float[] winProbs = {30.2f, 1.08f, 1.56f};
    private float[] densities = {15f, 34.38f, 30.02f};

    public static SettingsPanel Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public static Action GameSizeChange;

    public void OnLeftArrow()
    {
        Debug.Log("Left");
        activeGameSize = (activeGameSize+enumGameSize-1)%enumGameSize;
        UpdateSizeText();
        GameSizeChange.Invoke();
    }
    public void OnRightArrow()
    {
        Debug.Log("Right");
        activeGameSize = (activeGameSize + 1) % enumGameSize;
        UpdateSizeText();
        GameSizeChange?.Invoke();
    }
    private void UpdateSizeText()
    {
        gameSizeText.text = Enum.GetNames(typeof(GameSize))[activeGameSize].ToString();
        // Also change mine density and win prob
        winprob.text = "Win Prob: " + winProbs[activeGameSize]+"%";
        mineDens.text = "Mine Density: " + densities[activeGameSize]+"%";

    }
}
