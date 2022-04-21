namespace Containers.Core;

public class Container
{
    public ContainerType Type { get; set; }
    public int Id { get; set; }
    public string Company { get; set; }
    public string SenderCountry { get; set; }
    public ContainerColumn Column { get; set; }
}