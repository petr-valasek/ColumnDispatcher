namespace ColumnDispatcher.TrainModel;

public class StateMachineFacade : IStateMachine
{
    
    public StateMachineFacade(IStateMachine impl, string name)
    {
        _impl = impl;
        _name = name;
    }

    public void SendCommand(ColumnCommand command, object? data)
    {
        Logger.Log(_name, $"Sending command '{command}'");
        _impl.SendCommand(command, data);
    }

    public ColumnState State { get => _impl.State; }
    public event EventHandler<ColumnState>? StateChanged
    {
        add => _impl.StateChanged += value;
        remove => _impl.StateChanged -= value;
    }

    private readonly string _name;
    private readonly IStateMachine _impl;
}