namespace ColumnDispatcher.TrainModel;

public interface IUseCase
{
    Task GetWaitableTask();
    CancellationTokenSource GetCancellationTokenSource();
    public string Name { get;}
}