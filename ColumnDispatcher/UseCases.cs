using System.Security.Cryptography.X509Certificates;

namespace ColumnDispatcher.TrainModel;

public class ActionWrapperUseCase: ITrainUseCase
{
    public ActionWrapperUseCase(Action<CancellationToken> action, string name, Train parent)
    {
        Name = name;
        _parent = parent;
        _task = new Task(() =>
        {
            action(_cancellationTokenSource.Token);
            _parent.RemoveUseCase(this);

        });
    }
    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name { get; private set; }

    private Task _task;
    private CancellationTokenSource _cancellationTokenSource = new ();
    private readonly Train _parent;
}

public class WrapperUseCase: ITrainUseCase
{
    public WrapperUseCase(IUseCase useCase, Train parent)
    {
        _useCase = useCase;
        _parent = parent;
        _task = new Task(() =>
        {
            _useCase.GetWaitableTask().Start();
            _useCase.GetWaitableTask().Wait();
            _parent.RemoveUseCase(this);
        });
    }
    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _useCase.GetCancellationTokenSource();
    }

    public string Name => _useCase.Name;

    private readonly IUseCase _useCase;
    private readonly Train _parent;
    private readonly Task _task;
}

public class NopUseCase : ITrainUseCase
{
    public Task GetWaitableTask()
    {
        return Task.CompletedTask;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return new CancellationTokenSource();
    }

    public string Name => "No operation";
}
public class JoinerUseCase : ITrainUseCase
{
    public JoinerUseCase(IUseCase joinee, Train parent)
    {
        _joinee = joinee;
        _parent = parent;
        _task = new Task(() =>
        {
            _joinee.GetWaitableTask().Wait();
            _parent.RemoveUseCase(this);
        });
    }
    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _joinee.GetCancellationTokenSource();
    }

    public string Name => $"Joined to {_joinee.Name}";

    private readonly IUseCase _joinee;
    private readonly Train _parent;
    private readonly Task _task;
}

public class AmenderUseCase : ITrainUseCase
{
    public AmenderUseCase(IUseCase joinee, IUseCase amendWith, Train parent)
    {
        _joinee = joinee;
        _amendWith = amendWith;
        _parent = parent;
        _task = new Task(() =>
        {
            _amendWith.GetWaitableTask().Start();
            _amendWith.GetWaitableTask().Wait();
            _joinee.GetWaitableTask().Wait();
            _parent.RemoveUseCase(this);
        });

    }

    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _amendWith.GetCancellationTokenSource();
    }

    public string Name => $"Amended {_joinee.Name}";

    private readonly IUseCase _joinee;
    private readonly IUseCase _amendWith;
    private readonly Train _parent;
    private readonly Task _task;
}

