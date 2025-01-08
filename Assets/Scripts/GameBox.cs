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
            spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Busted];
        else
            spriteRenderer.sprite = ThemePicker.Instance.current.numbers[type];
            //spriteRenderer.sprite = cleared[type];

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

    public void Click()
    {
        if (GameAreaMaster.Instance.MainGameArea.LevelBusted)
            return;



        Debug.Log("Clicking Box value = "+value+" when Gametype 0 "+USerInfo.Instance.currentType);

        if (USerInfo.Instance.currentType == GameType.Create)
        {
            GameAreaMaster.Instance.MainGameArea.OpenBoxCreate(Pos);
            return;
        }
        // If Busted Level disallow any click on Area


        GameAreaMaster.Instance.MainGameArea.AddClicks();

        // This workds for normal gameplay keep it
        if (value > 0)
        {
            Chord();
            return;
        }
        if (Marked)
            return;

        if (GameAreaMaster.Instance.MainGameArea.OpenBox(Pos))
        {
            //RemoveAndSetUnderActive();
        }
        else
        {
            Debug.Log("Show a busted mine here");
            spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Busted];
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
        GameAreaMaster.Instance.MainGameArea.Chord(Pos);
    }

    public void Mark()
    {
        if (Marked) return;
        Marked = true;
        spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Flagged];
        GameAreaMaster.Instance.MainGameArea.DecreaseMineCount();
    }

    public void RightClick(bool hidden = false)
    {

        // Dont flag non mines in Create B
        if (hidden && USerInfo.EditMode == 1 && !GameAreaMaster.Instance.MainGameArea.IsMine(Pos)) // Edit mode 1 == EDIT mode B
        {
            Click();
            return;
        }

        Debug.Log("Clicking this box at " + Pos + " mark or demark as mine value =" + value+" ");
        if (value > 0) return;
        Marked = !Marked;

        if (Marked)
            SetAsFlaggedMine();
        else
        {
            if(hidden)
                SetAsHiddenMine();
            else
                SetAsUnFlagged();
        }
        Debug.Log("Right Clicking Box, hidden = " + hidden);

        if (LevelCreator.Instance.EditMode)
        {
            GameAreaMaster.Instance.MainGameArea.UpdateMineCount();
            return; // Breaks if in edit mode and placing Mines
        }


        GameAreaMaster.Instance.MainGameArea.AddClicks();
        if (!Marked)
            GameAreaMaster.Instance.MainGameArea.IncreaseMineCount();
        else
            GameAreaMaster.Instance.MainGameArea.DecreaseMineCount();
    }

    public void SetAsHiddenMine() => spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.HiddenMine];
    public void SetAsFlaggedMine() => spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Flagged];
    public void SetAsUnFlagged() => spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Unflagged];

    public void Bust()
    {
        Debug.Log("Show a busted mine here");
        spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Busted];
        Busted = true;
    }

    public void ShowWrongFlag()
    {
        spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.WrongFlag];
    }

    public void ShowMine()
    {
        spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Mine];
    }

    public bool UnSolved()
    {
        return boxCollider.enabled;
    }

    public void Reset()
    {
        transform.gameObject.SetActive(true);
        Busted = false;
        Marked = false;
        spriteRenderer.sprite = ThemePicker.Instance.current.flags[(int)MineBoxType.Unflagged];
        boxCollider.enabled = true;
    }
}
