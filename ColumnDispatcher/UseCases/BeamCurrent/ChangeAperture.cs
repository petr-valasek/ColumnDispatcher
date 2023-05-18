namespace ColumnDispatcher.TrainModel;

public class ChangeAperture : IUseCase
{
    public ChangeAperture(TrainFacade trainFacade, AperturePosition dataAperture)
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

    public string Name { get; }
}