namespace ColumnDispatcher.TrainModel;

public interface IUseCaseLibrary
{
    IUseCase? GetUseCase(Request request, ITrain train);
}