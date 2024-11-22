using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameSize {S,M,L}

public class SettingsPanel : MonoBehaviour
{
    int enumGameSize = Enum.GetNames(typeof(GameSize)).Length;
    public static int activeGameSize = 0;
    [SerializeField] TextMeshProUGUI boardSizeText;
    [SerializeField] TextMeshProUGUI mineDens;
    [SerializeField] TextMeshProUGUI winprob;
    [SerializeField] Toggle pendingToggle;
    [SerializeField] Slider slider;

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

    public void ConfirmSettings()
    {
        // Apply the size settings
        if(USerInfo.Instance.BoardSize != (int)slider.value)
        {
            USerInfo.Instance.BoardSize = (int)slider.value;
            Debug.Log("Setting new boardSize to "+slider.value);
            LevelCreator.Instance.RestartGame();
        }
    }
    public void UpdateSizeText(Slider slider)
    {
        // Read value of slider and update
        int nexValue = (int)slider.value;
        boardSizeText.text = nexValue+"x"+ nexValue;

    }
}
