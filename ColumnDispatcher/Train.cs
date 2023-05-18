namespace ColumnDispatcher.TrainModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Train: ITrain
{
    public Train(IStateMachine controller)
    {
        StateMachine = controller;
    }
    public ColumnState TargetState { get; set; }

    public IStateMachine StateMachine { get; set; }

    public ITrainUseCase JoinCurrentUseCase()
    {
        if (_useCases.Count == 0)
        {
            throw new InvalidOperationException("No use case to join");
        }

        Logger.Log("New Use Case", $"Joining current use case {_useCases.First().Name}");
        var joiner = new JoinerUseCase(_useCases.First(), this);
        _useCases.Add(joiner);
        return joiner;
    }

    public ITrainUseCase ReplaceCurrentUseCase(IUseCase newUseCase)
    {
        // TODO: parallelize
        foreach (var useCase in _useCases)
        {
            Logger.Log(newUseCase.Name,$"Cancelling use case {useCase.Name}");
            useCase.GetCancellationTokenSource().Cancel();
        }
        var wrapper = new WrapperUseCase(newUseCase, this);
        _useCases = new() { wrapper };
        return wrapper;
    }

    public ITrainUseCase ExecuteImmediately(IUseCase newUseCase)
    {
        var wrapper = new WrapperUseCase(newUseCase, this);
        _useCases.Add(wrapper);
        return wrapper;
    }


    public ITrainUseCase PlaceNewUseCase(IUseCase newUseCase)
    {
        if (_useCases.Any())
        {
            throw new InvalidOperationException("Trying to place new use case during other use case execution.");
        }

        var wrapper = new WrapperUseCase(newUseCase, this);
        _useCases = new() { wrapper };
        return wrapper;
    }

    public ITrainUseCase AmendCurrentUseCase(IUseCase newUseCase)
    {
        if (_useCases.Count == 0)
        {
            throw new InvalidOperationException("No use case to amend");
        }

        Logger.Log($"Amending current use case with {_useCases.First().Name}", $"");
        var amender = new AmenderUseCase(_useCases.First(), newUseCase, this);
        _useCases.Add(amender);
        return amender;
    }

    public IEnumerable<IUseCase> GetRunningUseCases()
    {
        return _useCases;
    }

    // not on ITrain interface, just for the implementation
    public void RemoveUseCase(IUseCase useCase)
    {
        _useCases.Remove(useCase);
    }


    private List<IUseCase> _useCases = new();
}
