using TMPro;
using UnityEngine;

public class BottomInfoController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI collectionSizeText;
    [SerializeField] TextMeshProUGUI levelAmtText;
    [SerializeField] TextMeshProUGUI levelText;


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


    public void UpdateCollectionSize()
    {
        collectionSizeText.text = "" + FirestoreManager.Instance.LocalCollectionList.Count.ToString();
        levelAmtText.text = "" + FirestoreManager.Instance.LoadedAmount.ToString();
        levelText.text = FirestoreManager.Instance.LevelData?.LevelId ?? "";
    }
}
