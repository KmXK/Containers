using UnityEngine;

public static class DragController
{
    private static Container _targetContainer;
    
    public static void ContainerClick(Container container)
    {
        if (container == null)
            return;
        
        if (_targetContainer != container)
        {
            _targetContainer = container;
            container.Select();
        }
        else
        {
            _targetContainer.Deselect();
            _targetContainer = null;
        }
    }

    public static void PlaceClick(IPlaceable placeable)
    {
        if (_targetContainer == null)
            return;

        placeable.Place(_targetContainer);
        _targetContainer.Deselect();
       
        _targetContainer = null;
    }
}
