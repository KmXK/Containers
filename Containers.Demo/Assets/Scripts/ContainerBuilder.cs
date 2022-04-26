using System.Collections.Generic;
using Sources;
using UnityEngine;

public class ContainerBuilder : MonoBehaviour
{
    [SerializeField] private Transform _containersParent;
    
    [SerializeField] private GameObject _smallContainerPrefab;
    [SerializeField] private GameObject _largeContainerPrefab;

    private int _nextContainerId = 3;
    
    public Container GenerateContainer(ContainerType type)
    {
        var containerPrefab = type == ContainerType.Small ? _smallContainerPrefab : _largeContainerPrefab;
        var containerGO = Instantiate(containerPrefab, _containersParent);
        var container = containerGO.GetComponent<Container>();
        container.Data.Id = _nextContainerId++;
        container.Data.Type = type;
        container.Data.Company = "Test Company";
        container.Data.Container = container;

        return container;
    }

    public IEnumerable<Container> GenerateContainersForTruck()
    {
        if (Random.Range(0, 2) == 0)
        {
            yield return GenerateContainer(ContainerType.Small);
            yield return GenerateContainer(ContainerType.Small);
        }
        else
        {
            yield return GenerateContainer(ContainerType.Large);
        }
    }
}
