using System.Collections.Generic;
using System.Linq;
using Sources;
using UnityEngine;

public class PlaceAlgorithm : MonoBehaviour
{
    [SerializeField] private StateManager _stateManager;
    [SerializeField] private ContainerSelector _containerSelector;
    [SerializeField] private Transform _trainsContainer;
    
    [SerializeField] private GameObject _containerShadow;
    [SerializeField] private GameObject _linePrefab;

    private GameObject _shadow;
    private GameObject _line;
    
    private (ContainerPlatform Platform, ContainerColumn Column)? _bestMove;

    public void PerformAlgorithm()
    {
        if (_containerSelector.SelectedContainer != null)
        {
            if (_bestMove != null)
            {
                if (_bestMove.Value.Column.Height > 0)
                {
                    _containerSelector.ContainerClick(_bestMove.Value.Column.Top().Container);
                }
                else
                {
                    _containerSelector.PlatformClick(_bestMove.Value.Platform);
                }
            }
        }
        
        SelectBestContainer();
    }
    
    private void SelectBestContainer()
    {
        var column = _stateManager.ColumnsData
            .Where(c => c.MinLoadingDepth >= 0)
            .OrderBy(c => c.MinLoadingDepth)
            .FirstOrDefault();

        if (column == null)
            return;

        _containerSelector.ContainerClick(column.Column.Top().Container);
    }

    public void ShowBestMove(Container container)
    {
        var c = GetBestColumn(container);
        if (c == null)
        {
            return;
        }

        _bestMove = c.Value;

        _shadow = Instantiate(_containerShadow, _bestMove.Value.Platform.transform);
        var st = _shadow.transform;
        
        st.localScale = container.VisualTransform.localScale;
        st.localPosition = _bestMove.Value.Platform.GetContainerPosition(container, _bestMove.Value.Column, false);

        _line = Instantiate(_linePrefab, transform);
        var p1 = container.transform.position + Vector3.up * .5f;
        var p2 = st.position + Vector3.up * .5f;

        var line = _line.GetComponent<LineRenderer>();
        line.positionCount = 4;
        line.SetPositions(new []
        {
            p1,
            p1 + Vector3.up * 10,
            p2 + Vector3.up * 10,
            p2
        });
    }

    public void ClearShadow()
    {
        if(_shadow != null)
        {
            Destroy(_shadow);
            Destroy(_line);
            _shadow = null;
            _bestMove = null;
        }
    }
    
    private (ContainerPlatform Platform, ContainerColumn Column)? GetBestColumn(Container container)
    {
        var train = _trainsContainer.GetComponentInChildren<Train>();

        if (container.State == ContainerState.Default)
            return null;
        
        if (container.Data.Column != null && !container.Platform.CanTake(container))
            return null;

        if (_stateManager.ColumnsData.Count == 0)
            return null;
        
        if (container.State == ContainerState.Loading && train != null)
        {
            var column = train.GetColumn(container);
            return column;
        }

        var columnsData = _stateManager.ColumnsData;

        if (container.State == ContainerState.Blocking)
        {
            var list = new List<ColumnData>();
            foreach (var column in columnsData)
            {
                if ((column.Column.Height == 0 || column.Column.Top().Container.State == ContainerState.Default) &&
                    column.Platform.CanPlace(container))
                {
                    list.Add(column);
                }
            }

            if (list.Any())
            {
                var column = list.OrderBy(c =>
                    (c.Platform.transform.position - container.transform.position).magnitude).First();
                
                return (column.Platform, column.Column);
            }


            container.WindowIndex = 100000;
        }

        foreach (var columnData in columnsData)
        {
            columnData.LowerSum = columnData.UpperSum = columnData.MaxLowerDepth = 0;
            var i = columnData.Column.Height - 1;
            foreach (var c in columnData.Column)
            {
                if (c.Container.WindowIndex < container.WindowIndex)
                {
                    columnData.LowerSum += (container.WindowIndex - c.Container.WindowIndex);
                    if (columnData.MaxLowerDepth == 0)
                    {
                        columnData.MaxLowerDepth = i;
                    }
                }
                else
                {
                    columnData.UpperSum += (c.Container.WindowIndex - container.WindowIndex);
                }

                i--;
            }
        }

        var columns = columnsData.OrderBy(c => 0.7 * c.LowerSum + 0.2 * c.UpperSum + 0.2 * c.MaxLowerDepth);

        foreach (var columnData in columns)
        {
            if (columnData.Platform != container.Platform && columnData.Platform.CanPlace(container, columnData.Column))
            {
                return (columnData.Platform, columnData.Column);
            }
        }

        return null;
    }
}