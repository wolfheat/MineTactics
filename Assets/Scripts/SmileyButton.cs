using System;
using UnityEngine;

public class SmileyButton : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider;

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

    public void SetColliderWidth(float newWidth) => boxCollider.size = new Vector2(newWidth, boxCollider.size.y);
    public void UpdateCollectionSize(int select)
    {
        Debug.Log("** UPDATING SMILEY");
        // Amount of loaded challenge levels changed - if there is levels set the smiley to evil?
        if(FirestoreManager.Instance.LoadedAmount > 0 && USerInfo.Instance.currentType == GameType.Challenge)
            spriteRenderer.sprite = sprites[4];
        else
            spriteRenderer.sprite = sprites[0];
    }

    public void Click()
    {
        Debug.Log("Clicking Smiley!");
        spriteRenderer.sprite = sprites[1];
        if (USerInfo.Instance.currentType == GameType.Normal)
            LevelCreator.Instance.RestartGame(true);
        else if (USerInfo.Instance.currentType == GameType.Challenge)
            LevelCreator.Instance.LoadRandomLevel();
        else if (USerInfo.Instance.currentType == GameType.Create)
        {
            spriteRenderer.sprite = sprites[0];
            // Make smiley cleat in Create Mode A and unselect all in Mode B
            if (USerInfo.EditMode == 0)
                PanelController.Instance.Clear();
            
            else
                GameAreaMaster.Instance.MainGameArea.ResetAllNonMine();
            return;
        }else if (USerInfo.Instance.currentType == GameType.Test)
        {
            // Reset the game to startPosition
            Debug.Log("Reset to startPosition");
            CreatePanelController.Instance.ResetTest();
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
        //Debug.Log("SMILEY - Show Normal Gametype:"+ USerInfo.Instance.currentType + " Loaded AMt: "+ FirestoreManager.Instance.LoadedAmount);
        spriteRenderer.sprite = sprites[0];
        if(USerInfo.Instance.currentType==GameType.Challenge && FirestoreManager.Instance.LoadedAmount > 0)
            spriteRenderer.sprite = sprites[4];

    }



}
