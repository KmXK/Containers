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

    public void ContainerClick(Container container)
    {
        if (_selectedContainer != null && _selectedContainer != container && container.Platform != null)
        {
            if (!container.Data.Column.CheckContainerType(_selectedContainer.Data.Type))
            {
                DeselectContainer();
                return;
            }
            
            if (_selectedContainer.Platform != null && !_selectedContainer.Platform.Remove(_selectedContainer))
            {
                DeselectContainer();
                return;
            }
            
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
        
        if (!platform.CheckContainerType(_selectedContainer.Data.Type))
        {
            DeselectContainer();
            return;
        }
        
        if (_selectedContainer.Platform != null && !_selectedContainer.Platform.Remove(_selectedContainer))
        {
            DeselectContainer();
            return;
        }

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
