using System;
using Sources;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerPlatform : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private StateManager _stateManager;
    
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

    private void Start()
    {
        Placed += (_, _) => FindObjectOfType<StateManager>().CalculateStates();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ContainerSelector.Instance.PlatformClick(this);
    }

    public bool CanTake(Container container) => _isTakeable && _place.CanTake(container.Data);

    public bool CanPlace(Container container) => CanPlace(container, null);

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
        if (!CanTake(container))
            return false;
        
        var hasTaken = _place.TryTake(container.Data);
        if (hasTaken && _place.IsEmpty())
        {
            Emptied?.Invoke(this);
        }

        return hasTaken;
    }

    private void MoveContainer(Container container)
    {
        container.transform.SetParent(transform);
        
        container.transform.localPosition = GetContainerPosition(container, container.Data.Column, true);
    }

    public Vector3 GetContainerPosition(Container container, ContainerColumn column, bool containerInColumn)
    {
        var (columnIndex, height) = _place.GetColumnInfo(column);   
        var containerVisualTransform = container.VisualTransform;
        var containerScale = containerVisualTransform.localScale;

        var h = containerInColumn ? height - 1 : height;
        var y = h * containerScale.y;

        var x = container.Data.Type switch
        {
            ContainerType.Small => (columnIndex * 2 - 1) * (11 * containerScale.x / 20),
            ContainerType.Large => 0,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new Vector2(x, y);
    }
}
