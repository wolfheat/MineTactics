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
        LevelCreator.Instance.RestartGame();
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
