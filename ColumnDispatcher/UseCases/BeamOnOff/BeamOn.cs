namespace ColumnDispatcher.TrainModel;

public class BeamOn: IUseCase
{
    public BeamOn(ITrain train)
    {
        _task = new Task(Execute);
        _train = train;
        _helper = new UseCaseHelper(train, _cancellationTokenSource.Token);
    }
    public void Execute()
    {
        _train.TargetState = ColumnState.BeamOn;
        _helper.EnsureAtLeastBeamOff();
        _helper.CheckCancellation();
        _helper.EnsureAtLeastBeamBlocked();
        _helper.CheckCancellation();
        _helper.EnsureAtLeastL1ParkedAbortable();
        _helper.CheckCancellation();
        _helper.EnsureAtLeastBlanked();
        _helper.CheckCancellation();
        _helper.EnsureAtLeastBeamOn();
    }

    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name => "BeamOn";
    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITrain _train;
    private readonly UseCaseHelper _helper;
}