using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    
    public Transform _target;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
        transform.LookAt(_target);
    }
    
    private Vector3 previousPosition;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = _camera.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;
            
            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically
            
            _camera.transform.position = _target.position;
            
            _camera.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            _camera.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            _camera.transform.Translate(new Vector3(0, 0, -20f));
            
            previousPosition = newPosition;
        }
        
        if (Input.GetMouseButton(1))
        {
            var rightVector = _target.right * Input.GetAxis("Mouse X");

            _camera.transform.position -= rightVector;
            _target.position -= rightVector;
        }
    }
}
