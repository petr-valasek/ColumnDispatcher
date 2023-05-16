namespace ColumnDispatcher.TrainModel;

public class UseCaseNotFound : Exception
{
}
public class UseCaseSelector
{
    public void RegisterUseCaseLibrary(IUseCaseLibrary lib)
    {
        _libraries.Add(lib);
    }
    public IUseCase CreateUseCase(Request r, ITrain train)
    {
        foreach (var lib in _libraries)
        {
            var useCase = lib.GetUseCase(r, train);
            if (useCase != null) return useCase;
        }

        throw new UseCaseNotFound();
    }

    private readonly List<IUseCaseLibrary> _libraries = new();
}