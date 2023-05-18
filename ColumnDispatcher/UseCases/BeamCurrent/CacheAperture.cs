namespace ColumnDispatcher.TrainModel;

public class CacheAperture : IUseCase
{
    public CacheAperture(TrainFacade train, AperturePosition value)
    {
        throw new NotImplementedException();
    }
    

    public Task GetWaitableTask()
    {
        throw new NotImplementedException();
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        throw new NotImplementedException();
    }

    public string Name => "Cache Aperture";
    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new();
    private readonly ITrain _train;
    private readonly UseCaseHelper _helper;
    private AperturePosition _value;
}