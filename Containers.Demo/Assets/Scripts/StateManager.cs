using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private Transform _trainsContainer;
    [SerializeField] private Transform _platformsContainer;
    
    public void CalculateStates()
    {
        var train = _trainsContainer.GetComponentInChildren<Train>();
        if (train == null)
            return;
        
        var platforms = _platformsContainer.GetComponentsInChildren<ContainerPlatform>();

        foreach (var platform in platforms)
        {
            var place = platform.ContainerPlace;
            if (place.IsEmpty())
                continue;

            foreach (var column in place.GetColumns())
            {
                var defaultState = ContainerState.Default;
                foreach (var containerData in column)
                {
                    var container = containerData.Container;
                    var state = train.GetContainerState(container);

                    if (state == ContainerState.Default)
                    {
                        state = defaultState;
                    }
                    else
                    {
                        defaultState = ContainerState.Blocking;
                    }
                    
                    container.SetState(state);
                }
            }
        }
    }

    public void ClearStates()
    {
        foreach (var container in FindObjectsOfType<Container>())
        {
            container.SetState(ContainerState.Default);
        }
    }

    private void Awake()
    {
        foreach (var platform in FindObjectsOfType<ContainerPlatform>())
        {
            platform.Placed += (_, _) => CalculateStates();
        }
    }
}
