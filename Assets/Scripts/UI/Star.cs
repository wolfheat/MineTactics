using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite highlightedSprite;
    [SerializeField] Sprite normalSprite;

    public void ShowStar(int type) => image.sprite = type == 0 ? normalSprite : highlightedSprite;
}
