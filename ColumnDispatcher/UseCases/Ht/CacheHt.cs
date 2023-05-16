namespace ColumnDispatcher.TrainModel;

public class CacheHt : IUseCase
{
    public CacheHt(ITrain train, double value)
    {
        _task = new Task(Execute);
        _train = train;
        _helper = new UseCaseHelper(train, _cancellationTokenSource.Token);
        _value = value;
    }
    public void Execute()
    {
        _train.StateMachine.SendCommand(ColumnCommand.ChangeHt, _value);
    }

    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name => "Cache HT";
    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITrain _train;
    private readonly UseCaseHelper _helper;
    private double _value;
}