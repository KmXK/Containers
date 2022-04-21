using UnityEngine;

public class Platform : MonoBehaviour, IPlaceable
{
    private Transform _transform;
    
    public int IndexInHolder { get; set; }
    public PlatformHolder Holder { get; set; }

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    private void OnMouseUpAsButton()
    {
        DragController.PlaceClick(this);
    }

    public void Place(Container container)
    {
        Holder.Place(container, this);
        
        var containerTransform = container.GetComponent<Transform>();
        containerTransform.SetParent(_transform);

        var placement = Holder.Placeholder.Placements[IndexInHolder];

        var localScale = containerTransform.localScale;
        var t = 2 * (placement.Count - 1) * localScale.y;
        containerTransform.localPosition =
            Vector3.up * ((t + localScale.y) / 2 +
                          _transform.localScale.y / 2);
    }
}
