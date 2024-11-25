﻿using UnityEngine;
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
    [SerializeField] Sprite hiddenMineSprite;
    [SerializeField] Sprite[] cleared;
    [SerializeField] Collider2D boxCollider;


    public bool Active => boxCollider.enabled;
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
        //Debug.Log("Clicking Box value = "+value);

        if (USerInfo.Instance.currentType == GameType.Create)
        {
            GameArea.Instance.OpenBox(Pos);
            return;
        }else if (USerInfo.EditMode == 1 && GameArea.Instance.IsMine(Pos))
        {
            RightClick(true);
            return;
        }
        if (value > 0)
        {
            Chord();
            return;
        }
        if (Marked)
            return;
        //Debug.Log("Clicking this box at "+Pos+" notice Levelcreator to respond accordingly");
        if (GameArea.Instance.OpenBox(Pos))
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

    private void Chord()
    {
        GameArea.Instance.Chord(Pos);
    }



    internal void Mark()
    {
        if (Marked) return;
        Marked = true;
        spriteRenderer.sprite = markedSprite;
        GameArea.Instance.DecreaseMineCount();
    }

    internal void RightClick(bool hidden = false)
    {
        Debug.Log("Clicking this box at "+Pos+" mark or demark as mine value ="+value);
        if (value > 0) return;
        Marked = !Marked;
        spriteRenderer.sprite = Marked ? markedSprite : (hidden?hiddenMineSprite:unmarkedSprite);
        
        Debug.Log("Right Clicking Box, hidden = "+hidden);

        if (LevelCreator.Instance.EditMode)
        {
            GameArea.Instance.UpdateMineCount();
            return; // Breaks if in edit mode and placing Mines
        }
        if(!Marked)
            GameArea.Instance.IncreaseMineCount();
        else
            GameArea.Instance.DecreaseMineCount();
    }

    internal void Bust()
    {
        Debug.Log("Show a busted mine here");
        spriteRenderer.sprite = bustedSprite;
        Busted = true;
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
