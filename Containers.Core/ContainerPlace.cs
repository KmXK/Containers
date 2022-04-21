namespace Containers.Core;

public abstract class ContainerPlace
{
    public abstract void Place(Container container);

    public abstract int Count { get; }

    public int IndexInPlaceholder;

    protected ContainerPlace(int indexInPlaceholder)
    {
        IndexInPlaceholder = indexInPlaceholder;
    }

    //private readonly int _maxHeight;

    //private readonly ContainerColumn _firstColumn;
    //private readonly ContainerColumn _secondColumn;

    //public ContainerPlace(int maxHeight)
    //{
    //    _maxHeight = maxHeight;
    //    _firstColumn = new ContainerColumn(maxHeight);
    //    _secondColumn = new ContainerColumn(maxHeight);
    //}

    //public void Place(Container container)
    //{
    //    if (_firstColumn.Height == 0 && _secondColumn.Height == 0)
    //    {
    //        _firstColumn.Place(container);
    //    }
    //    else if (_firstColumn.ContainerType == ContainerType.Small && _secondColumn.Height == 0)
    //    {
    //        if (container.Type != ContainerType.Small)
    //            throw new ArgumentException();
    //        _secondColumn.Place(container);
    //    }
    //    else if (_firstColumn.ContainerType == ContainerType.Large)
    //    {
    //        if (container.Type != ContainerType.Large)
    //            throw new ArgumentException();
    //        _firstColumn.Place(container);
    //    }
    //}
}