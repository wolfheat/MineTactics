using TMPro;
using UnityEngine;

public class BottomInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI collectionSizeText;
    [SerializeField] TextMeshProUGUI levelAmtText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI debugText;


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


    public void ShowDebugText(string showText) => debugText.text = showText;
    public void UpdateCollectionSize(int itemToSelect = -1)
    {
        collectionSizeText.text = "" + FirestoreManager.Instance.LocalCollectionList.Count.ToString();
        levelAmtText.text = "" + FirestoreManager.Instance.LoadedAmount.ToString();
        levelText.text = FirestoreManager.Instance.LevelData?.LevelId ?? "";
    }
}
