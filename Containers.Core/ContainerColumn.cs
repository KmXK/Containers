namespace Containers.Core;

public class ContainerColumn
{
    private readonly int _maxHeight;
    private readonly Stack<Container> _containers;

    public int Height => _containers.Count;
    public ContainerPlace ContainerPlace { get; private set; }

    public ContainerColumn(int maxHeight)
    {
        _maxHeight = maxHeight;
        _containers = new Stack<Container>(maxHeight);
    }

    public void Place(Container container)
    {
        if (Height == _maxHeight)
            throw new ArgumentOutOfRangeException("PlacesCount");

        _containers.Push(container);
    }

    public Container Take(Container container)
    {
        if (Height <= 0)
            throw new ArgumentOutOfRangeException("PlacesCount");

        if (Peek() != container)
            throw new InvalidOperationException();

        return _containers.Pop();
    }

    public Container Peek()
    {
        if (Height <= 0)
            throw new ArgumentOutOfRangeException("PlacesCount");

        return _containers.Peek();
    }

    public Container[] Containers => _containers.ToArray();

    //private readonly int _maxHeight;
    //private readonly Stack<Container> _containers;
    //public int Count => _containers.Count;

    //public ContainerColumn(int maxHeight)
    //{
    //    _maxHeight = maxHeight;
    //    _containers = new Stack<Container>(maxHeight);

    //}
    
    //public void Place(Container container)
    //{
    //    if (!_containers.Any())
    //        _containerType = container.Type;

    //    if (container.Type != _containerType)
    //        throw new InvalidOperationException("Foo");

    //    if (Height == _maxHeight)
    //        throw new ArgumentOutOfRangeException("PlacesCount");

    //    _containers.Push(container);
    //    container.Column = this;
    //}

    //public Container Take()
    //{
    //    if (Height == 0)
    //        throw new ArgumentOutOfRangeException("PlacesCount");

    //    return _containers.Pop();
    //}
}