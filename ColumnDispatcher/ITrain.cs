namespace ColumnDispatcher.TrainModel;

// tagged interface to ensure that every use case library adds the use case to the train
public interface ITrainUseCase: IUseCase
{}

public interface ITrain
{
    public ColumnState TargetState { get; set; }
    public IStateMachine StateMachine { get;}

    public ITrainUseCase JoinCurrentUseCase();
    public ITrainUseCase ReplaceCurrentUseCase(IUseCase newUseCase);
    public ITrainUseCase AmendCurrentUseCase(IUseCase newUseCase);
    public ITrainUseCase PlaceNewUseCase(IUseCase newUseCase);
    public IEnumerable<IUseCase> GetRunningUseCases();
}