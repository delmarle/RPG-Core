

using Station;

public interface IContainersHandler
{
    BaseItemContainer GetContainer(string containerId);
}

public class ContainerReference
{
    private readonly string _containerId;
    private readonly IContainersHandler _handler;

    public ContainerReference(string id, IContainersHandler handler)
    {
        _containerId = id;
        _handler = handler;
    }

    public BaseItemContainer GetContainer()
    {
        return _handler.GetContainer(_containerId);
    }
}
