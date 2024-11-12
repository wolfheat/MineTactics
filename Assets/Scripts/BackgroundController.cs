using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] Color normal;
    [SerializeField] Color editMode;
    [SerializeField] Color editModeB;
    [SerializeField] SpriteRenderer spriteRenderer;


    public static BackgroundController Instance { get; private set; }

    private void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SetColorNormal() => spriteRenderer.color = normal;
    public void SetColorEditMode() => spriteRenderer.color = editMode;
    public void SetColorEditModeB() => spriteRenderer.color = editModeB;


}
