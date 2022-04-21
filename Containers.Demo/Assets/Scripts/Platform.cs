using UnityEngine;

public class Platform : MonoBehaviour, IPlaceable
{
    [SerializeField] private DragController _dragController;

    private Transform _transform;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    private void OnMouseUpAsButton()
    {
        _dragController.PlaceClick(this);
    }

    public void Place(Container container)
    {
        var containerTransform = container.GetComponent<Transform>();
        
        containerTransform.SetParent(_transform);

        containerTransform.localPosition =
            Vector3.up * (containerTransform.localScale.y / 2 + _transform.localScale.y / 2);
    }
}
