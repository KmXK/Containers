using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _distance = 20f;
    
    private Camera _camera;
    private Transform _cameraTransform;
    
    public Transform _target;

    public void LookAt(Transform t)
    {
        var pos = GetIntersectWithLineAndPlane(t.position, _camera.transform.forward,
            Vector3.up, Vector3.zero);
        StartCoroutine(MoveTo(pos));
    }
    
    private static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
    {
        var d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
 
        return d * direct.normalized + point;
    }

    private IEnumerator MoveTo(Vector3 pos)
    {
        var delta = .5f;
        var time = 0f;
        var startPosition = _target.position;
        while (time < delta)
        {
            _target.position = Vector3.Lerp(startPosition, pos, time / delta);
            CalculateCameraPosition();
            yield return null;
            time += Time.deltaTime;
        }

        _target.position = pos;
    }
    
    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _cameraTransform = _camera.transform;
        _cameraTransform.LookAt(_target);
        CalculateCameraPosition();
    }
    
    private Vector3 previousPosition;
    private float rotationAroundXAxis, rotationAroundYAxis;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;
            
            rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            rotationAroundXAxis = direction.y * 180; // camera moves vertically

            CalculateCameraPosition();
            previousPosition = newPosition;
        }
        
        if (Input.GetMouseButton(1))
        {
            var rightVector = _cameraTransform.right.normalized * Input.GetAxis("Mouse X");
            var forwardVector = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized *
                              Input.GetAxis("Mouse Y");

            var vector = rightVector + forwardVector;
            _cameraTransform.position -= vector;
            _target.position -= vector;
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            _distance -= scroll * 10f;
            _distance = Math.Clamp(_distance, 5, 35);
            CalculateCameraPosition();
        }
    }

    private void CalculateCameraPosition()
    {
        _cameraTransform.position = _target.position;
            
        _cameraTransform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
        _cameraTransform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

        var r = _cameraTransform.rotation;
        var e = r.eulerAngles;
        if (e.x is < 10 or > 180)
            e.x = 10;
        if (e.z > 179)
        {
            e.z = 0;
            e.y -= 180;
        }
        
        r.eulerAngles = e;
        _cameraTransform.rotation = r;
        
        _cameraTransform.Translate(new Vector3(0, 0, -_distance));
    }
}
