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
        Debug.Log("Container place click!");
        
        ContainerSelector.Instance.SelectContainerPlatform(this);
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
        var containerTransform = container.transform;
        containerTransform.SetParent(transform);

        var (column, height) = _place.GetColumnInfo(container.Data.Column);

        var containerScale = containerTransform.localScale;
        var localScale = transform.localScale;
        var y = (localScale.y + containerScale.y) / 2 +
                (height - 1) * containerScale.y;
        float x;

        switch (container.Data.Type)
        {
            case ContainerType.Small:
                x = (column * 2 - 1) * (11 * containerScale.x / 20);
                break;
            case ContainerType.Large:
                x = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        containerTransform.localPosition = new Vector2(x, y);
    }
}
