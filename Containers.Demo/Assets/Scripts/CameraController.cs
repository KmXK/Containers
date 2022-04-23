using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private bool _isRotating;

    private void Update()
    {
        if (_isRotating)
        {
            
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _isRotating = true;
        }
    }

    private void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(2))
        {
            _isRotating = false;
        }
    }
}
