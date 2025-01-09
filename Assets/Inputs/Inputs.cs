using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class Inputs : MonoBehaviour
{
    public Controls Controls { get; set; }
    public InputAction Actions { get; set; }
    public Vector2 StartPos { get; set; }
    public Vector2 LastPos { get; set; }
    public Vector2 EndPos { get; set; }
    public static Inputs Instance { get; private set; }
    public static InputAction touchAction { get; private set; }
    public const float MoveDistaneLimit = 16f;
    private const float MoveMinimalTime = 0.2f;
    private const float ZoomSpeed = 3f;
    private float startPinch = 0;

    public static Action<Vector2> OnMoveCameraMovement;
    
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("** Created Inputs Controller **");
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        Controls = new Controls();
        
    }


    private float startTouch = 0;

    private void OnEnable()
    {
        Controls.Enable();
    }
    private void OnDisable()
    {
        Controls.Disable();
    }
    private void Start()
    {
        Controls.Main.Mouse.performed += ctx => IsMousePressed = true;
        Controls.Main.Mouse.canceled += ctx => IsMousePressed = false;

        Controls.Main.Mouse.started += OnTouchStart;
            //Controls.Main.Mouse.performed += TouchMove;
        Controls.Main.Mouse.canceled += OnTouchEnd;
        Controls.Main.TouchPosition2.performed += OnTouch2Performed;    
        Controls.Main.TouchPosition2.canceled += OnTouch2Canceled;    
    }
    private void Update()
    {
        UpdateTouchCount();
        if (IsMousePressed && BoxClickValidStart && ActiveTouchCount==1 && !DidZoom)
            TouchMove();
    }

    private void UpdateTouchCount()
    {
        if (Touchscreen.current == null)
            return;
        // Get active touches
        var activeTouches = Touchscreen.current.touches;

        // Count active touches in relevant phases
        int activeTouchCount = 0;
        foreach (var touch in activeTouches)
        {
            if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began ||
                touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved ||
                touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Stationary)
            {
                activeTouchCount++;
            }
        }
        ActiveTouchCount=activeTouchCount;
        if(ActiveTouchCount==0)
            DidZoom = false;
    }

    private void OnTouch1Start(InputAction.CallbackContext context)
    {
        Debug.Log("Started new Touch");
    }
   private void OnTouch2Canceled(InputAction.CallbackContext context)
    {
        startPinch = 0;
        Debug.Log("Pinch ended");   
    }
   private void OnTouch2Performed(InputAction.CallbackContext context)
    {
        // Limit Pan and Zoom for small Boards
        if (DisAllowMoveAndZoom())
            return;

        DidZoom = true;
        var touches = Touchscreen.current.touches;
        if (touches[1].position.ReadValue().x == 0 && touches[1].position.ReadValue().y == 0)
            return;
        Debug.Log("Touch performed");
        float magnitude = TouchMagnitudeScreen();
        BottomInfoController.Instance.ShowDebugText("Works. Pos: "+touches[0].position.ReadValue()+"Pos: "+touches[1].position.ReadValue()+" magnitude: "+magnitude+" magnScreen: "+TouchMagnitudeScreen());


        // Reading a second position so we know two fingers are used
        if (startPinch == 0)
        {
            startPinch = magnitude;
            Debug.Log("Pinch start set "+magnitude);
            return;
        }
        float changeFraction = magnitude /startPinch;
        int sign = magnitude>startPinch ? 1 : -1;
        //float changePercentofWidth = change/Camera.main.orthographicSize;
        Debug.Log("Change "+changeFraction);
        BottomInfoController.Instance.ShowDebugText("Works. Pos: "+touches[0].position.ReadValue()+"Pos: "+touches[1].position.ReadValue()+" magn: "+magnitude+" change: "+changeFraction);
        startPinch = magnitude;

        //float newZoom = change * ZoomSpeed;
        CameraController.Instance.SetZoom(sign*changeFraction);
    }

    private float TouchMagnitude() => (Controls.Main.TouchPosition1.ReadValue<Vector2>() - Controls.Main.TouchPosition1.ReadValue<Vector2>()).magnitude;
    private float TouchMagnitudeScreen() => (Touchscreen.current.touches[0].position.ReadValue() - Touchscreen.current.touches[1].position.ReadValue()).magnitude;

    private void OnTouchStart(InputAction.CallbackContext context)
    {
        Vector2 pos = Controls.Main.TouchPosition.ReadValue<Vector2>(); 
        //Vector2 pos = Touchscreen.current.touches[0].position.ReadValue();
            
        Debug.Log("OnTouchStart "+pos   );
        var rayHit = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(pos));
        //var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Touchscreen.current.touches[0].position.ReadValue()),100f,LayerMask.NameToLayer("GameAreaLayer"));
        //Debug.Log("Starting Touch on position "+ Touchscreen.current.touches[0].position.ReadValue() + " has collider "+(rayHit.collider!=null));
        GameArea gameArea= null;
        GameBox box= null;
        if (rayHit.Length > 0)
        {
            //Debug.Log("HItting collider "+rayHit.collider.name);
            gameArea = rayHit[0].collider.GetComponent<GameArea>();
            box = rayHit[0].collider.GetComponent<GameBox>();
            //Debug.Log("HItting box "+box.name);
            //Debug.Log("Touch Start on "+(rayHit.collider == null ? "null": rayHit.collider?.name));
            //BoxClickValidStart = gameArea != null;
            BoxClickValidStart = true;

            //Debug.Log("box"+box);
            startTouch = Time.time;
            StartPos = Controls.Main.TouchPosition.ReadValue<Vector2>();
            LastPos = StartPos;
        }
        else
        {
            Debug.Log("OnTouchStart - Disable");
            if (MoveEnabled)
            {
                MoveEnabled = false;
                return; // Dont click if dragging
            }
            BoxClickValidStart = false;

        }
            BottomInfoController.Instance.ShowDebugText("BoxClickValidStart: "+ BoxClickValidStart);


    }
    public float TimeHeld { get; private set; } = 0;
    public bool MoveEnabled { get; private set; }
    public bool IsMousePressed { get; private set; }
    public int ActiveTouchCount { get; private set; }
    public bool DidZoom { get; private set; } = false;
    public bool BoxClickValidStart { get; private set; } = false;

    private void OnTouchEnd(InputAction.CallbackContext context)
    {
        BoxClickValidStart = false;
        if (MoveEnabled)
        {
            MoveEnabled = false;
            return; // Dont click if dragging
        }

        Debug.Log("Ended Touch "+DidZoom);
        TimeHeld = (Time.time - startTouch);
        
        Vector2 pos = Controls.Main.TouchPosition.ReadValue<Vector2>();
        TouchDebug.Instance.ShowText((TimeHeld > USerInfo.Instance.SensitivityMS)?"R: ":"L: ["+ (int)pos.x +","+(int)pos.y+"] "+TimeHeld.ToString("F2")+"s");
        if(!DidZoom)
            OnTouchClick(pos, TimeHeld > USerInfo.Instance.SensitivityMS ? true : false);        
    }

    private void TouchMove()
    {
        // Disable Move and Scale for smaller levels than 10x10 ??
        // Limit Pan and Zoom for small Boards

        if (DisAllowMoveAndZoom())
            return;


        if (StartPos == null) return;
        
        Vector2 CurrentPosition = Controls.Main.TouchPosition.ReadValue<Vector2>();

        if (CurrentPosition == StartPos) return;

        // If moved screen do not execute click - Need at least a distance and a duration to execute
        float dist = Vector2.Distance(StartPos, CurrentPosition);

        // Enable Once while pressing down
        if (!MoveEnabled && dist < MoveDistaneLimit)
        {
            return; // No Move camera if not far away from startpress
        }
        MoveEnabled = true;
        //Debug.Log("move >");    


        OnMoveCameraMovement.Invoke(LastPos-CurrentPosition);
        LastPos = CurrentPosition;
    }

    private bool DisAllowMoveAndZoom()
    {
        if (USerInfo.Instance.BoardType != BoardTypes.Slider)
        {
            return USerInfo.Instance.BoardType == BoardTypes.Beginner; // All standard sizes except Beginner allows for zoom
        }
        return (USerInfo.Instance.BoardSize <= 10);
    }

    public void OnTouchClick(Vector2 touchPos,bool rightClick = false)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("Pointer is hitting UI discard touch");
            return;
        }

        var rayHits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(touchPos));
        RaycastHit2D rayHit = new();
        foreach (var hit in rayHits)
        {
            Debug.Log("Hitting "+hit.collider?.name);
            if(hit.collider.GetComponent<GameBox>() != null || hit.collider.GetComponent<SmileyButton>() != null)
            {
                rayHit = hit;
                break;
            }
        }
        //var rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(touchPos),100f,LayerMask.NameToLayer("Default"));
        Debug.Log("Ray Hit "+(rayHit.collider != null));
        if (!rayHit.collider) return;
        
        Debug.Log("TOUCH! Rightclick ="+rightClick);
        GameBox box = rayHit.collider.GetComponent<GameBox>();
        Debug.Log("box ="+box?.name);
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
        box.RightClick(USerInfo.Instance.currentType==GameType.Create);

    }
    
}
