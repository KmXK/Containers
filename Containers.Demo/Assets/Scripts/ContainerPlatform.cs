using System;
using Sources;
using UnityEngine;
using UnityEngine.Events;

public class ContainerPlatform : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] private int _maxHeight = 5;
    [SerializeField] private bool _isPlaceable = true;
    [SerializeField] private bool _isTakeable = true;

    private ContainerPlace _place;

    public ContainerPlace ContainerPlace => _place;

    public bool IsPlaceable
    {
        get => _isPlaceable;
        set => _isPlaceable = value;
    }

    public event Action<ContainerPlatform> Emptied;
    public event Func<ContainerPlatform, Container, bool> Placing;
    public event Action<ContainerPlatform, Container> Placed;

    private void Awake()
    {
        _place = new ContainerPlace(_maxHeight);
    }

    private void OnMouseUpAsButton()
    {
        ContainerSelector.Instance.PlatformClick(this);
    }

    public bool CheckContainerType(ContainerType type)
    {
        return _isPlaceable && _place.CheckContainerType(type);
    }

    public bool CanPlace(Container container)
    {
        return CanPlace(container, null);
    }

    private bool CanPlace(Container container, ContainerColumn column)
    {
        return _place.CanPlace(container.Data, column) && Placing?.Invoke(this, container) != false;
    }

    public void Place(Container container)
    {
        if (!CanPlace(container))
            return;
        
        _place.Place(container.Data);
        container.Platform = this;
        MoveContainer(container);
        
        Placed?.Invoke(this, container);
    }

    public void Place(Container container, ContainerColumn column)
    {
        if (!CanPlace(container, column))
            return;
        
        _place.Place(container.Data, column);
        container.Platform = this;
        MoveContainer(container);
        
        Placed?.Invoke(this, container);
    }

    public void PlaceOn(Container containerToPlace, Container originContainer)
    {
        if (!CanPlace(containerToPlace, originContainer.Data.Column))
            return;
        
        _place.Place(containerToPlace.Data, originContainer.Data.Column);
        containerToPlace.Platform = this;
        MoveContainer(containerToPlace);
        
        Placed?.Invoke(this, containerToPlace);
    }

    public bool TryRemove(Container container)
    {
        if (!_isTakeable)
            return false;
        
        var isTaken = _place.TryTake(container.Data);
        if (isTaken && _place.IsEmpty())
        {
            Emptied?.Invoke(this);
        }

        return isTaken;
    }

    private void MoveContainer(Container container)
    {
        var containerVisualTransform = container.VisualTransform;
        container.transform.SetParent(transform);

        var (column, height) = _place.GetColumnInfo(container.Data.Column);

        var containerScale = containerVisualTransform.localScale;
        var y = (height - 1) * containerScale.y;

        var x = container.Data.Type switch
        {
            ContainerType.Small => (column * 2 - 1) * (11 * containerScale.x / 20),
            ContainerType.Large => 0,
            _ => throw new ArgumentOutOfRangeException()
        };

        container.transform.localPosition = new Vector2(x, y);
    }
}
