using System;
using Sources;
using UnityEngine;

public class ContainerPlatform : MonoBehaviour
{
    [Range(5, 10)]
    [SerializeField] private int _maxHeight = 5;
    
    private ContainerPlace _place;

    private void Start()
    {
        _place = new ContainerPlace(_maxHeight);
    }

    private void OnMouseUpAsButton()
    {
        ContainerSelector.Instance.PlatformClick(this);
    }

    public bool CheckContainerType(ContainerType type)
    {
        return _place.CheckContainerType(type);
    }

    public void Place(Container container)
    {
        _place.Place(container.Data);
        container.Platform = this;
        MoveContainer(container);
    }

    public void PlaceOn(Container containerToPlace, Container originContainer)
    {
        _place.Place(containerToPlace.Data, originContainer.Data.Column);
        containerToPlace.Platform = this;
        MoveContainer(containerToPlace);
    }

    public bool Remove(Container container)
    {
        return _place.TryTake(container.Data);
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
