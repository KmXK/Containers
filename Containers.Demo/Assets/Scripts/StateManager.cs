using System;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [SerializeField] private Transform _trainsContainer;
    [SerializeField] private Transform _platformsContainer;

    public List<ColumnData> ColumnsData;
    
    public void CalculateStates()
    {
        var train = _trainsContainer.GetComponentInChildren<Train>();
        if (train == null)
            return;

        ColumnsData.Clear();
        
        var platforms = _platformsContainer.GetComponentsInChildren<ContainerPlatform>();

        foreach (var platform in platforms)
        {
            var place = platform.ContainerPlace;
            // if (place.IsEmpty())
            //     continue;

            foreach (var column in place.GetColumns())
            {
                var loadingDepth = -1;
                var i = column.Height - 1;
                
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

                        if (state == ContainerState.Loading)
                        {
                            loadingDepth = i;
                        }
                    }
                    
                    container.SetState(state);

                    i--;
                }
                
                
                ColumnsData.Add(new ColumnData
                {
                    Platform =  platform,
                    Column = column,
                    MinLoadingDepth = loadingDepth
                });
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
        ColumnsData = new List<ColumnData>();
    }
}
