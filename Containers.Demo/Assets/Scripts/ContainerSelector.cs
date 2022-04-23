using UnityEngine;

public class ContainerSelector : MonoBehaviour
{
    private Container _selectedContainer;

    public static ContainerSelector Instance { get; private set; }
    
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectContainer(Container container)
    {
        if (_selectedContainer != null && _selectedContainer != container && container.Platform != null)
        {
            if (!container.Data.Column.CheckContainerType(_selectedContainer.Data.Type))
            {
                _selectedContainer = null;
                return;
            }
            
            if (_selectedContainer.Platform != null && !_selectedContainer.Platform.Remove(_selectedContainer))
            {
                _selectedContainer = null;
                return;
            }
            
            container.Platform.PlaceOn(_selectedContainer, container);

            _selectedContainer = null;
        }
        else
        {
            _selectedContainer = container;
        }
    }

    public void SelectContainerPlatform(ContainerPlatform platform)
    {
        if (_selectedContainer == null)
            return;
        
        if (!platform.CheckContainerType(_selectedContainer.Data.Type))
        {
            _selectedContainer = null;
            return;
        }
        
        if (_selectedContainer.Platform != null && !_selectedContainer.Platform.Remove(_selectedContainer))
        {
            _selectedContainer = null;
            return;
        }

        platform.Place(_selectedContainer);

        _selectedContainer = null;
    }
}
