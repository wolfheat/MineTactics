using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public Controls Controls { get; set; }
    public InputAction Actions { get; set; }

    public static Inputs Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Created Inputs Controller");
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        Controls = new Controls();
        Controls.Enable();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        // This method handles clicks on items below UI so exit if hitting UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Clicked on the UI");
            return;
        }

        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        if (context.canceled)
        {
            Debug.Log("Release "+rayHit.collider.gameObject.name);
            GameBox box = rayHit.collider.GetComponent<GameBox>();
            if (box != null)
            {
                if (Timer.Instance.Paused && !LevelCreator.Instance.WaitForFirstMove)
                    return;
                box.Click();
            }
            SmileyButton smiley = rayHit.collider.GetComponent<SmileyButton>();
            if (smiley != null)
            {
                smiley.Click();
            }

        }

        if (!context.started) return;
        if (!rayHit.collider) return;

        else
            Debug.Log("Hit "+rayHit.collider.gameObject.name);
    }
    
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (Timer.Instance.Paused && !LevelCreator.Instance.WaitForFirstMove)
            return;
        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        if (!context.started) return;
        GameBox box = rayHit.collider.GetComponent<GameBox>();
        box.RightClick();

    }
    
}
