using System.Linq;
using UnityEngine;

public class PlaceAlgorithm : MonoBehaviour
{
    [SerializeField] private StateManager _stateManager;
    [SerializeField] private ContainerSelector _containerSelector;

    [SerializeField] private GameObject _containerShadow;

    private GameObject _shadow;

    public void SelectBestContainer()
    {
        var column = _stateManager.ColumnsData
            .Where(c => c.MinLoadingDepth >= 0)
            .OrderBy(c => c.MinLoadingDepth)
            .First();

        _containerSelector.ContainerClick(column.Column.ToArray()[^1].Container);
    }

    public void ShowBestMove(Container container)
    {
        var c = GetBestColumn(container);
        if (c == null)
        {
            return;
        }

        var column = c.Value;

        _shadow = Instantiate(_containerShadow, column.Platform.transform);
        var st = _shadow.transform;
        
        st.localScale = container.VisualTransform.localScale;
        st.localPosition = column.Platform.GetContainerPosition(container, column.Column, false);
    }

    public void ClearShadow()
    {
        if(_shadow != null)
        {
            Destroy(_shadow);
            _shadow = null;
        }
    }
    
    private ColumnData? GetBestColumn(Container container)
    {
        if (container.Data.Column != null && !container.Platform.CanTake(container))
            return null;

        if (_stateManager.ColumnsData.Count == 0)
            return null;

        ColumnData column;
        do
        {
            column = _stateManager.ColumnsData[Random.Range(0, _stateManager.ColumnsData.Count)];
        } while (!column.Platform.CanPlace(container));

        return column;
    }
}