using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameBox : MonoBehaviour
{
    // Make this clickable
    public Vector2Int Pos { get; set; }
    public int value { get; set; }
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite bustedSprite;
    [SerializeField] Sprite mineSprite;
    [SerializeField] Sprite markedSprite;
    [SerializeField] Sprite unmarkedSprite;
    [SerializeField] Sprite wrongFlagSprite;
    [SerializeField] Sprite[] cleared;
    [SerializeField] Collider2D boxCollider;


    public bool Marked { get; set; } = false;
    public bool Busted { get; set; } = false;
    private void Start()
    {
        Inputs.Instance.Controls.Main.Mouse.started += MouseDown;
    }

    public void SetType(int type)
    {
        value = type;
        if(value == -1)
            spriteRenderer.sprite = bustedSprite;
        else
            spriteRenderer.sprite = cleared[type];

        boxCollider.enabled = false;
    }

    public void MakeInteractable(bool doSet = true)
    {
        boxCollider.enabled = doSet;
    }

    private void MouseDown(InputAction.CallbackContext context)
    {
        //Debug.Log("Mouse triggered");
    }

    internal void Click()
    {
        if(value > 0)
        {
            Debug.Log("This is a number, chard?");
            Chard();
            return;
        }
        if (Marked)
            return;
        Debug.Log("Clicking this box at "+Pos+" notice Levelcreator to respond accordingly");
        if (LevelCreator.Instance.OpenBox(Pos))
        {
            //RemoveAndSetUnderActive();
        }
        else
        {
            Debug.Log("Show a busted mine here");
            spriteRenderer.sprite = bustedSprite;
        }
    }

    public void RemoveAndSetUnderActive()
    {
        if (!boxCollider.enabled) return;
        MakeInteractable(false);
        transform.gameObject.SetActive(false);
    }

    private void Chard()
    {
        LevelCreator.Instance.Chord(Pos);
    }

    internal void RightClick()
    {
        if (value > 0) return;
        Debug.Log("Clicking this box at "+Pos+" mark or demark as mine");
        Marked = !Marked;
        spriteRenderer.sprite = Marked ? markedSprite : unmarkedSprite;
    }

    internal void Bust()
    {
        Debug.Log("Show a busted mine here");
        spriteRenderer.sprite = bustedSprite;
        Busted = true;
        SmileyButton.Instance.ShowBust();
    }

    internal void ShowWrongFlag()
    {
        spriteRenderer.sprite = wrongFlagSprite;
    }

    internal void ShowMine()
    {
        spriteRenderer.sprite = mineSprite;        
    }

    internal bool UnSolved()
    {
        return boxCollider.enabled;
    }

    internal void Reset()
    {
        transform.gameObject.SetActive(true);
        Busted = false;
        Marked = false;
        spriteRenderer.sprite = unmarkedSprite;
        boxCollider.enabled = true;
    }
}
