using System;
using TMPro;
using UnityEngine;

public enum GameSize {S,M,L}

public class Settings : MonoBehaviour
{
    int enumGameSize = Enum.GetNames(typeof(GameSize)).Length;
    int activeGameSize = 0;
    [SerializeField] TextMeshProUGUI gameSizeText;

    public void OnLeftArrow()
    {
        Debug.Log("Left");
        activeGameSize = (activeGameSize+enumGameSize-1)%enumGameSize;
        UpdateSizeText();
    }
    public void OnRightArrow()
    {
        Debug.Log("Right");
        activeGameSize = (activeGameSize + 1) % enumGameSize;
        UpdateSizeText();
    }
    private void UpdateSizeText() => gameSizeText.text = Enum.GetNames(typeof(GameSize))[activeGameSize].ToString();


}
