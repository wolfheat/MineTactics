using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] Color normal;
    [SerializeField] Color tactics;
    [SerializeField] Color editMode;
    [SerializeField] Color editModeB;
    [SerializeField] SpriteRenderer spriteRenderer;


    public static BackgroundController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SetColorNormal() => spriteRenderer.color = normal;
    public void SetColorTactics() => spriteRenderer.color = tactics;
    public void SetColorEditMode() => spriteRenderer.color = editMode;
    public void SetColorEditModeB() => spriteRenderer.color = editModeB;


}
