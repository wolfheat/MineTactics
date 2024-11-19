using System;
using UnityEngine;

public class ProgressRotation : MonoBehaviour
{

    float timer = 0;
    float lapTime = 0.7f;
    float rotate = -230 / 36f;
    float rotateAdd = 0f;


    void Update()
    {
        rotateAdd = Time.deltaTime/ lapTime * (-360);
        Step();
    }

    private void Step()
    {
        gameObject.transform.Rotate(0,0,rotateAdd); 
    }
}
