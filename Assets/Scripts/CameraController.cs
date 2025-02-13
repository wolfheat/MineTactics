using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float adjust = 0;

    [Range(3, 5)]
    [SerializeField] float zoom;
    private const float zoomMin = 2;
    private const float zoomMax = 4;
    private Vector2 startTouchPosition;
    private Vector3 startCameraPosition;

    // Camera positions for Zoom
    public float OriginalOrthogonalSize = 5;
    public float MaxZoomOrthogonalSize = 3;

    public static CameraController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        zoom = cam.orthographicSize;
        Inputs.OnMoveCameraMovement += TouchMoveCamera;
    }

    public void SetPosition(Vector3 camPos)
    {
        transform.position = camPos;
    }
    public void SetPositionLerped(Vector3 camPos)
    {
        StartCoroutine(LerpToPosition(camPos));
    }

    public void SetStartMovePosition(Vector2 startTouch)
    {
        startTouchPosition = startTouch;
        startCameraPosition = transform.position;
    }
    public void MoveCamera(Vector2 vector2)
    {
        float multiplyer = OriginalOrthogonalSize/ Camera.main.orthographicSize;
        Debug.Log("Original: "+OriginalOrthogonalSize+" Current: "+ Camera.main.orthographicSize + " MULT = "+multiplyer);

        transform.position += (Vector3)(vector2*multiplyer);

        // Clamp to game area
        ClampToGameArea();
    }

    public void AlignTop()
    {
        float totalHalfHeight = Camera.main.orthographicSize;
        float topButtonsHeight = cam.orthographicSize * 2 * (ButtonController.Instance.Height() / 1920);
        float HeightUsableAreaUp = totalHalfHeight - topButtonsHeight;

        // Sprite Size
        float SpriteHalfHeight = spriteRenderer.size.y / 2;
        float CameraMovableUp = HeightUsableAreaUp - SpriteHalfHeight;
        transform.position = new Vector3(transform.position.x, -CameraMovableUp, transform.position.z);
    }

    private void ClampToGameArea()
    {
        // World Game Size
        float totalHalfHeight = Camera.main.orthographicSize;
        float totalHalfWidth = Camera.main.aspect*totalHalfHeight;
        
        // Convert from screen space to world space - Need to use 1920 instead of screen.height cause it says 900
        float topButtonsHeight = cam.orthographicSize * 2 * (ButtonController.Instance.Height() / 1920);
        float bottomButtonsHeight = cam.orthographicSize * 2 * (BottomInfoController.Instance.Height() / 1920);
        
        // What areas are usable. i.e not buttons
        float HeightUsableArea = totalHalfHeight * 2 - topButtonsHeight - bottomButtonsHeight;
        float HeightUsableAreaUp = totalHalfHeight - topButtonsHeight;
        float HeightUsableAreaDown = totalHalfHeight - bottomButtonsHeight;

        // Sprite Size
        float SpriteHalfHeight = spriteRenderer.size.y/2;
        float SpriteHalfWidth = spriteRenderer.size.x/2;

        // Final Camera vertical move interval
        float CameraMovableUp   = HeightUsableAreaUp - SpriteHalfHeight;
        float CameraMovableDown = HeightUsableAreaDown - SpriteHalfHeight;

        // Booleans checking if game board extends outside the view
        bool boardIsLargerThanCameraX = SpriteHalfWidth > totalHalfWidth;
        bool boardIsLargerThanCameraY = SpriteHalfHeight > HeightUsableArea/2;

        // When board is smaller than camera more to edges
        float Xpos = CalculateXpos();
        float Ypos = CalculateYpos();

        transform.position = new Vector3(Xpos, Ypos, transform.position.z);

        float CalculateXpos()
        {
            if (boardIsLargerThanCameraX)
            {
                // When larger move to other edges
                return Mathf.Clamp(transform.position.x, totalHalfWidth - spriteRenderer.size.x / 2, spriteRenderer.size.x / 2 - totalHalfWidth);
            }
            return Mathf.Clamp(transform.position.x, -spriteRenderer.size.x / 2 + totalHalfWidth, spriteRenderer.size.x / 2 - totalHalfWidth);
        }
        
        float CalculateYpos()
        {
            if (boardIsLargerThanCameraY)
            {
                // When larger move to other edges
                return Mathf.Clamp(transform.position.y, CameraMovableDown, -CameraMovableUp);                 
            }
            return Mathf.Clamp(transform.position.y, -CameraMovableUp, CameraMovableDown);
        }
    }

    public void TouchMoveCamera(Vector2 cameraChange)
    {
        //Debug.Log("Touch move camera movment "+cameraChange);
        //Debug.Log("X movement = "+cameraChange.x+" Total Screen is "+Camera.main.pixelWidth);        
        // Scaling? WHy this number
        Vector2 cameraChangeScaled = cameraChange / Camera.main.pixelWidth * Camera.main.orthographicSize;
        transform.position += (Vector3)cameraChangeScaled;

        ClampToGameArea();
    }

    public void SetZoom(float zoom)
    {
        Debug.Log("SET ZOOM "+zoom);
        float percentOfWidht = zoom / Screen.width;
        Debug.Log("percentOfWidht: " + percentOfWidht);
        //float ortDiffPercent = percentOfWidht / Camera.main.orthographicSize;
        //Debug.Log("ortDiffPercent: " + ortDiffPercent);
        float newOrtho = Camera.main.orthographicSize - percentOfWidht*80;
        Debug.Log("newOrtho: " + newOrtho);
        newOrtho = Mathf.Clamp(newOrtho, 3, OriginalOrthogonalSize);
        BottomInfoController.Instance.ShowDebugText("Zoom By: "+(newOrtho));
        UpdateCameraZoom(newOrtho);
    }

    public void UpdateCameraZoom(float zoom)
    {
        cam.orthographicSize = zoom;
    }

    private IEnumerator LerpToPosition(Vector3 camPos, float time = 5f)
    {
        float lerpTimer = 0;
        Vector3 startPos = transform.position;
        while (Vector3.Distance(transform.position, camPos) > 0.1f)
        {
            lerpTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, camPos, lerpTimer / time);
            yield return null;
        }
        transform.position = camPos;
        ClampToGameArea();
    }

    internal void ResetCamera()
    {
        Debug.Log("** ** ** RESET CAMERA ** ** **");
        transform.position = new Vector3(0,0,-10);
    }
}
