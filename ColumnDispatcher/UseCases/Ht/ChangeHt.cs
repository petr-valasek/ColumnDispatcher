namespace ColumnDispatcher.TrainModel;

public class ChangeHt : IUseCase
{
    public ChangeHt(ITrain train, double value)
    {
        _task = new Task(Execute);
        _train = train;
        _helper = new UseCaseHelper(train, _cancellationTokenSource.Token);
        _value = value;
    }

    public void Execute()
    {
        _helper.EnsureAtMostBlanked();
        _helper.CheckCancellation();
        _helper.EnsureAtMostL1Parked();
        _helper.CheckCancellation();
        _helper.EnsureAtMostBeamBlocked();
        _helper.CheckCancellation();
        _helper.SendCommandAndWait(ColumnCommand.ChangeHt, _value, ColumnState.BeamBlocked, ColumnState.RampingHt);
        _helper.CheckCancellation();
        if (ColumnStatePrecedence.PrecedesOrEquals(ColumnState.L1Parked, _train.TargetState))
        {
            _helper.EnsureAtLeastL1Parked();
            _helper.CheckCancellation();
        }
        if (ColumnStatePrecedence.PrecedesOrEquals(ColumnState.Blanked, _train.TargetState))
        {
            _helper.CheckCancellation();
            _helper.EnsureAtLeastBlanked();
        }
        if (ColumnStatePrecedence.PrecedesOrEquals(ColumnState.BeamOn, _train.TargetState))
        {
            _helper.CheckCancellation();
            _helper.EnsureAtLeastBeamOn();
        }
    }

    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name => "ChangeHt";

    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITrain _train;
    private readonly UseCaseHelper _helper;
    private readonly double _value;
}