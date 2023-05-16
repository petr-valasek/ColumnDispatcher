namespace ColumnDispatcher.TrainModel;

public class ColumnDispatcher
{
    public ColumnDispatcher(IEnumerable<IUseCaseLibrary> useCaseLibraries, ITrain train)
    {
        _train = train;
        foreach (var lib in useCaseLibraries)
        {
            _selector.RegisterUseCaseLibrary(lib);
        }
    }
    public void Execute(Request r)
    {
        Start(r).Wait();

    }
    public Task Start(Request r)
    {
        var useCase = _selector.CreateUseCase(r, _train);
        var task = useCase.GetWaitableTask();
        if (!task.IsCompleted)
        {
            _runningUseCases.Add(useCase);
            task.Start();
        }
        return task;
    }

    private UseCaseSelector _selector = new();

    // TODO: is it useful?
    private List<IUseCase> _runningUseCases = new ();
    private ITrain _train;
}