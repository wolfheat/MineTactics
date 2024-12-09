using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public Controls Controls { get; set; }
    public InputAction Actions { get; set; }
    public static Inputs Instance { get; private set; }
    public static InputAction touchAction { get; private set; }

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

    private float startTouch = 0;

    private void OnEnable()
    {
        

    }
    private void OnDisable()
    {
        
    }
    private void Start()
    {
        Controls.Main.Mouse.started += OnTouchStart;
        Controls.Main.Mouse.canceled += OnTouchEnd;
    }

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        startTouch = Time.time;
        //Debug.Log("Started new Touch");
    }
    public float TimeHeld { get; private set; } = 0;
    private void OnTouchEnd(InputAction.CallbackContext context)
    {

        //Debug.Log("Ended Touch");
        TimeHeld = (Time.time - startTouch);
        Vector2 pos = Controls.Main.TouchPosition.ReadValue<Vector2>();
        TouchDebug.Instance.ShowText("Touch r-click at: "+ pos +" for "+TimeHeld+"s");
        //if(timeHeld > USerInfo.Instance.SensitivityMS)
        //    Debug.Log("R Click");
        //Debug.Log("TouchTime > Sensitivity "+timeHeld+"/"+USerInfo.Instance.SensitivityMS);
        OnTouchClick(pos, TimeHeld > USerInfo.Instance.SensitivityMS ? true : false);
    }

    public void OnTouchClick(Vector2 touchPos,bool rightClick = false)
    {
        //Debug.Log("TOUCH! Rightclick ="+rightClick);
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Pointer is hitting UI discard touch");
            return;
        }

        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(touchPos));
        if (!rayHit.collider) return;
        
        GameBox box = rayHit.collider.GetComponent<GameBox>();
        if (box != null)
        {
            // Do not allow any clicks if game is Paused, unless you are in normal mode and waiting for the first click
            if (Timer.Instance.Paused && !USerInfo.Instance.WaitForFirstMove && USerInfo.Instance.currentType != GameType.Create)
                return;
            if (rightClick && USerInfo.Instance.currentType != GameType.Create) // Only rightclick in non Edit{
            { 
                if(USerInfo.Instance.currentType == GameType.Challenge && Timer.Instance.Paused)
                {
                    Debug.Log("Challenge and Paused return");
                    return;
                }
                if(!GameAreaMaster.Instance.MainGameArea.UnSolved(box.Pos))
                    box.Click();
                else
                    box.RightClick();
            }
        else
            box.Click();
        }
        SmileyButton smiley = rayHit.collider.GetComponent<SmileyButton>();
        if (smiley != null)
        {
            smiley.Click();
        }
    }



    public void OnClick(InputAction.CallbackContext context)
    {
        // This method handles clicks on items below UI so exit if hitting UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Clicked on the UI");
            return;
        }
        if(Input.touches.Length>0)
            TouchDebug.Instance.ShowText("Touch At: " + Mouse.current.position.ReadValue() + " touch " + Input.touches[0].position);

        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        if (context.canceled)
        {
            //Debug.Log("Release "+rayHit.collider.gameObject.name);
            GameBox box = rayHit.collider.GetComponent<GameBox>();
            if (box != null)
            {
                if (Timer.Instance.Paused && !LevelCreator.Instance.WaitForFirstMove && !LevelCreator.Instance.EditMode)
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
    }
    
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (Timer.Instance.Paused && !LevelCreator.Instance.WaitForFirstMove)
            return;
        // Changing this to detect touches
        var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;
        if (!context.started) return;
        GameBox box = rayHit.collider.GetComponent<GameBox>();
        box.RightClick();

    }
    
}
