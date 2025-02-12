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

    private void ClampToGameArea()
    {
        // World Game Size
        float totalHalfHeight = Camera.main.orthographicSize;
        float totalHalfWidth = Camera.main.aspect*totalHalfHeight;

        // From top ButtonController size
        //float buttonControllerHeight = ButtonController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize- adjust;
        //float buttonControllerHeight = ButtonController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize*2;
        //float bottomControllerHeight = BottomInfoController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize- adjust;
        //float bottomControllerHeight = BottomInfoController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize * 2;

        float screenheight = Screen.height;
                
        // Convert from screen space to world space
        float buttonsTop = cam.orthographicSize * 2 * (ButtonController.Instance.Height() / 1920);
        float buttonsBottom = cam.orthographicSize * 2 * (BottomInfoController.Instance.Height() / 1920);
        //float buttonsTop = cam.orthographicSize * 2 * (ButtonController.Instance.Height() / Screen.height);
        //float buttonsBottom = cam.orthographicSize * 2 * (BottomInfoController.Instance.Height() / Screen.height);

        Debug.Log("Buttons Height = "+ ButtonController.Instance.Height()+" Screen height = "+Screen.height);

        //float worldHeight = cam.orthographicSize * 2 * (uiHeight / Screen.height);


        float HeightUsableArea = totalHalfHeight * 2 - buttonsTop - buttonsBottom;
        float HeightUsableAreaUp = totalHalfHeight - buttonsTop;
        float HeightUsableAreaDown = totalHalfHeight - buttonsBottom;

        float SpriteHalfHeight = spriteRenderer.size.y/2;
        float SpriteHalfWidth = spriteRenderer.size.x/2;
        float CameraMovableUp   = HeightUsableAreaUp - SpriteHalfHeight;
        float CameraMovableDown = HeightUsableAreaDown - SpriteHalfHeight;


        bool boardIsLargerThanCameraX = SpriteHalfWidth > totalHalfWidth;
        bool boardIsLargerThanCameraY = SpriteHalfHeight > HeightUsableArea/2;

        Debug.Log("buttonControllerHeight: " + buttonsTop + " bottomControllerHeight " + buttonsBottom);

        Debug.Log("Sprite Half Height: "+ SpriteHalfHeight);

        Debug.Log(" Size of pieces above and below");
        Debug.Log(" TOP half = "+totalHalfHeight+" Top Buttons: "+ buttonsTop + " Rest: "+HeightUsableAreaUp);
        Debug.Log(" BOTTOM half = "+totalHalfHeight+" Bottom Buttons: "+buttonsBottom+" Rest: "+HeightUsableAreaDown);


        Debug.Log("screenheight Height: " + screenheight + " Camera Up height: " + CameraMovableUp + " Camera Down height: " + CameraMovableDown);

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
                Debug.Log("boardLargerThanCameraY");
                // When larger move to other edges
                //return Mathf.Clamp(transform.position.y, -bottomControllerHeight - spriteRenderer.size.y / 2 + totalHeightHalf, buttonControllerHeight + spriteRenderer.size.y / 2 - totalHeightHalf);
                return Mathf.Clamp(transform.position.y, CameraMovableDown, -CameraMovableUp); 
                //return Mathf.Clamp(transform.position.y, HeightUsableAreaDown, HeightUsableAreaUp);
                //Ypos = Mathf.Clamp(transform.position.y, -buttonControllerHeight - spriteRenderer.size.y / 2 + totalHeightHalf, buttonControllerHeight + spriteRenderer.size.y / 2 - totalHeightHalf);
            }
            Debug.Log("Clamp  Y ["+ transform.position.y + "] between ["+(buttonsTop + spriteRenderer.size.y / 2 - totalHalfHeight) +","+(totalHalfHeight - buttonsBottom - spriteRenderer.size.y / 2) +"]");
            Debug.Log("ClampB Y ["+ transform.position.y + "] between ["+(-CameraMovableUp) +","+(CameraMovableDown) +"]");
            Debug.Log("");
            //return Mathf.Clamp(transform.position.y, buttonControllerHeight + spriteRenderer.size.y / 2 - totalHeightHalf, totalHeightHalf - bottomControllerHeight - spriteRenderer.size.y / 2);
            return Mathf.Clamp(transform.position.y, -CameraMovableUp, CameraMovableDown);

        }


    }

    /*private void ClampToGameAreaNEW()
    {
        Debug.Log("CLAMP - TO GAME AREA");
        // World Game Size
        float totalHeightHalf = Camera.main.orthographicSize;
        float totalWidthHalf = Camera.main.aspect * totalHeightHalf;

        // From top ButtonController size
        //float buttonControllerHeight = ButtonController.Instance.Height() / (Screen.height * Camera.main.orthographicSize * 2);


        // Convert from screen space to world space
        float buttonsWorldHeight = cam.orthographicSize * 2 * (ButtonController.Instance.Height() / Screen.height);


        //float buttonControllerHeight = ButtonController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize*2;
        float bottomControllerHeight = cam.orthographicSize * 2 * (BottomInfoController.Instance.Height() / Screen.height);

        //float bottomControllerHeight = BottomInfoController.Instance.Height() / Camera.main.pixelHeight * Camera.main.orthographicSize * 2;

        bool boardIsLargerThanCameraX = spriteRenderer.size.x > totalWidthHalf * 2;

        float HeightUsableArea = totalHeightHalf * 2 - buttonsWorldHeight - bottomControllerHeight;
        float HeightUsableAreaUp = totalHeightHalf * 2 - buttonsWorldHeight;
        float HeightUsableAreaDown = totalHeightHalf * 2 - bottomControllerHeight;
        bool boardIsLargerThanCameraY = spriteRenderer.size.y > (HeightUsableArea);


        // When board is smaller than camera more to edges
        float Xpos = CalculateXpos();
        float Ypos = CalculateYpos();

        transform.position = new Vector3(Xpos, Ypos, transform.position.z);

        gizmo_positions[0] = new Vector3(-totalWidthHalf, HeightUsableAreaUp, 0);
        gizmo_positions[1] = new Vector3(totalWidthHalf, HeightUsableAreaUp, 0);

        gizmo_positions[2] = new Vector3(-totalWidthHalf, HeightUsableAreaDown, 0);
        gizmo_positions[3] = new Vector3(totalWidthHalf, HeightUsableAreaDown, 0);


        float CalculateXpos()
        {
            if (boardIsLargerThanCameraX) {
                // When larger move to other edges
                return Mathf.Clamp(transform.position.x, totalWidthHalf - spriteRenderer.size.x / 2, spriteRenderer.size.x / 2 - totalWidthHalf);
            }
            return Mathf.Clamp(transform.position.x, -spriteRenderer.size.x / 2 + totalWidthHalf, spriteRenderer.size.x / 2 - totalWidthHalf);
        }

        float CalculateYpos()
        {
            if (boardIsLargerThanCameraY) {
                Debug.Log("boardLargerThanCameraY");
                // When larger move to other edges
                //return Mathf.Clamp(transform.position.y, -bottomControllerHeight - spriteRenderer.size.y / 2 + totalHeightHalf, buttonControllerHeight + spriteRenderer.size.y / 2 - totalHeightHalf);
                return Mathf.Clamp(transform.position.y, HeightUsableAreaDown, HeightUsableAreaUp);
                //Ypos = Mathf.Clamp(transform.position.y, -buttonControllerHeight - spriteRenderer.size.y / 2 + totalHeightHalf, buttonControllerHeight + spriteRenderer.size.y / 2 - totalHeightHalf);
            }
            return Mathf.Clamp(transform.position.y, HeightUsableAreaDown, HeightUsableAreaUp);
        }


    }

    */


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
