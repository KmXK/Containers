using UnityEngine;

public class DragController : MonoBehaviour
{
    private Container _targetContainer;
    
    public void ContainerClick(Container container)
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

    public void PlaceClick(IPlaceable placeable)
    {
        if (_targetContainer == null)
            return;

        placeable.Place(_targetContainer);
        _targetContainer.Deselect();
       
        _targetContainer = null;
    }
}
