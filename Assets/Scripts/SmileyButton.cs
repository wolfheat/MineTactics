using System;
using UnityEngine;

public class SmileyButton : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    

    // On mouse down show pressed button, on release start new game

    public static SmileyButton Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        FirestoreManager.OnLevelCollectionListChange += UpdateCollectionSize;
    }

    private void UpdateCollectionSize(int select)
    {
        // Amount of loaded challenge levels changed - if there is levels set the smiley to evil?
        if(FirestoreManager.Instance.LoadedAmount > 0)
            spriteRenderer.sprite = sprites[4];
        else
            spriteRenderer.sprite = sprites[0];
    }

    public void Click()
    {
        Debug.Log("Smiley?");
        spriteRenderer.sprite = sprites[1];
        if (USerInfo.Instance.currentType == GameType.Normal)
            LevelCreator.Instance.RestartGame();
        else if (USerInfo.Instance.currentType == GameType.Challenge)
            LevelCreator.Instance.LoadRandomLevel();
        else if (USerInfo.Instance.currentType == GameType.Create)
        {
            spriteRenderer.sprite = sprites[0];
            Debug.Log("No smiley in create mode");
            // Make smiley cleat in Create Mode A and unselect all in Mode B
            if (USerInfo.EditMode == 0)
                PanelController.Instance.Clear();
            else
                GameAreaMaster.Instance.MainGameArea.ResetAllNonMine();
            return;
        }

    }

    public void ShowWin()
    {
        spriteRenderer.sprite = sprites[3];
    }
    
    public void ShowBust()
    {
        Debug.Log("Show Bust?");
        spriteRenderer.sprite = sprites[2];
    }
    
    public void ShowNormal()
    {
        Debug.Log("Show Normal Gametype:"+ USerInfo.Instance.currentType + " Loaded AMt: "+ FirestoreManager.Instance.LoadedAmount);
        spriteRenderer.sprite = sprites[0];
        if(USerInfo.Instance.currentType==GameType.Challenge && FirestoreManager.Instance.LoadedAmount > 0)
            spriteRenderer.sprite = sprites[4];

    }



}
