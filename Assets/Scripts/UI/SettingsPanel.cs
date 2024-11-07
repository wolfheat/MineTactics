using System;
using TMPro;
using UnityEngine;

public enum GameSize {S,M,L}

public class SettingsPanel : MonoBehaviour
{
    int enumGameSize = Enum.GetNames(typeof(GameSize)).Length;
    public static int activeGameSize = 0;
    [SerializeField] TextMeshProUGUI gameSizeText;

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
    private void UpdateSizeText() => gameSizeText.text = Enum.GetNames(typeof(GameSize))[activeGameSize].ToString();


}
