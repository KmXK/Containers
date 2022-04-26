using UnityEngine;

public class ContainerSelector : MonoBehaviour
{
    private Container _selectedContainer;
    
    public static ContainerSelector Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void ContainerClick(Container container)
    {
        if (_selectedContainer != null && _selectedContainer != container && container.Platform != null)
        {
            if (!container.Data.Column.CanPlace(_selectedContainer.Data))
            {
                DeselectContainer();
                return;
            }
            
            if (_selectedContainer.Platform != null && !_selectedContainer.Platform.CanTake(_selectedContainer))
            {
                DeselectContainer();
                return;
            }

            if (_selectedContainer.Platform != null)
                _selectedContainer.Platform.TryRemove(_selectedContainer);
            
            container.Platform.PlaceOn(_selectedContainer, container);

            DeselectContainer();
        }
        else
        {
            SelectContainer(container);
        }
    }

    public void PlatformClick(ContainerPlatform platform)
    {
        if (_selectedContainer == null)
            return;

        if (!platform.CanPlace(_selectedContainer))
        {
            DeselectContainer();
            return;
        }

        if (_selectedContainer.Platform != null && !_selectedContainer.Platform.CanTake(_selectedContainer))
        {
            DeselectContainer();
            return;
        }
        
        if (_selectedContainer.Platform != null)
            _selectedContainer.Platform.TryRemove(_selectedContainer);
        
        platform.Place(_selectedContainer);

        DeselectContainer();
    }

    private void SelectContainer(Container container)
    {
        if(_selectedContainer != null)
            DeselectContainer();
        _selectedContainer = container;
        container.Select();
    }

    private void DeselectContainer()
    {
        if(_selectedContainer != null)
        {
            _selectedContainer.Deselect();
            _selectedContainer = null;
        }
    }
}
