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
    }

    internal void Click()
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
                GameArea.Instance.ResetAllNonMine();
            return;
        }

    }

    internal void ShowWin()
    {
        spriteRenderer.sprite = sprites[3];
    }
    
    internal void ShowBust()
    {
        Debug.Log("Show Bust?");
        spriteRenderer.sprite = sprites[2];
    }
    
    internal void ShowNormal()
    {
        Debug.Log("Show Normal");
        spriteRenderer.sprite = sprites[0];
    }



}
