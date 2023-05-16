namespace ColumnDispatcher.TrainModel;

public class BeamOff : IUseCase
{
    public BeamOff(ITrain train)
    {
        _task = new Task(Execute);
        _train = train;
        _helper = new UseCaseHelper(train, _cancellationTokenSource.Token);
    }

    public void Execute()
    {
        _train.TargetState = ColumnState.BeamOff;
        _helper.EnsureAtMostBlanked();
        _helper.CheckCancellation();
        _helper.EnsureAtMostL1Parked();
        _helper.CheckCancellation();
        _helper.EnsureAtMostBeamBlocked();
        _helper.CheckCancellation();
        _helper.EnsureAtMostBeamOff();

    }

    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name => "BeamOff";

    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITrain _train;
    private readonly UseCaseHelper _helper;
}