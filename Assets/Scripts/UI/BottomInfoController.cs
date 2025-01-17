using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using TMPro;
using UnityEngine;

public class BottomInfoController : MonoBehaviour
{
    [Header("TOP PART")]
    [SerializeField] TextMeshProUGUI collectionSizeTextB;
    [Header("BOTTOM PART")]
    [SerializeField] TextMeshProUGUI collectionSizeText;
    [SerializeField] TextMeshProUGUI collectionAmtText;
    [SerializeField] TextMeshProUGUI levelAmtText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] RectTransform rect;

    private const int AmountDebugMessages = 15;
    private Queue<string> messages = new();
    public static BottomInfoController Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;

        FirestoreManager.OnLevelCollectionListChange += UpdateCollectionSize;        
    }


    public void ShowDebugText(string showText)
    {
        if (messages.Count >= AmountDebugMessages)
            messages.Dequeue();

        messages.Enqueue(showText);
        // Add messages and show last 10 messages
        debugText.text = MessagesAsList();
    }

    private string MessagesAsList()
    {
        StringBuilder sb = new();
        foreach (var message in messages)
        {
            sb.AppendLine(message);
        }
        return sb.ToString();
    }

    public void UpdateCollectionSize(int itemToSelect = -1)
    {
        Debug.Log("** Bottom Info - Currently have a total of "+ FirestoreManager.Instance.LoadedAmount+" loaded levels.");
        // EDIT MODE
        collectionSizeText.text = FirestoreManager.Instance.LocalCollectionList.Count.ToString();

        // CHALLENGE MODE
        collectionAmtText.text = USerInfo.Instance.ActiveCollections.Count.ToString();
        levelAmtText.text = FirestoreManager.Instance.LoadedAmount.ToString();

        collectionSizeTextB.text =FirestoreManager.Instance.LoadedAmount.ToString();
        levelText.text = "" + FirestoreManager.Instance.LevelData?.LevelId ?? "";
    }

    internal float Height()
    {
        return rect.sizeDelta.y;

    }
}
