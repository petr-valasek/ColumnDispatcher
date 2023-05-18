namespace ColumnDispatcher.TrainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrainFacade: ITrain
{

    public TrainFacade(ITrain impl, string name)
    {
        _impl = impl;
        _name = name;
        StateMachine = new StateMachineFacade(impl.StateMachine, name);
    }

    public ColumnState TargetState
    {
        get =>_impl.TargetState;
        set => _impl.TargetState = value;
    }

    public IStateMachine StateMachine { get; private set; }
    public ITrainUseCase JoinCurrentUseCase()
    {
        Logger.Log(_name, $"Joining current UseCase");
        return _impl.JoinCurrentUseCase();
    }

    public ITrainUseCase ReplaceCurrentUseCase(IUseCase newUseCase)
    {
        Logger.Log(_name, $"Replacing current UseCase with {newUseCase.Name}");
        return _impl.ReplaceCurrentUseCase(newUseCase);
    }

    public ITrainUseCase ExecuteImmediately(IUseCase newUseCase)
    {
        Logger.Log(_name, $"Executing immediately UseCase {newUseCase.Name}");
        return _impl.ExecuteImmediately(newUseCase);
    }

    public ITrainUseCase AmendCurrentUseCase(IUseCase newUseCase)
    {
        Logger.Log(_name, $"Amending current UseCase with {newUseCase.Name}");
        return _impl.AmendCurrentUseCase(newUseCase);
    }

    public ITrainUseCase PlaceNewUseCase(IUseCase newUseCase)
    {
        Logger.Log(_name, $"Placing new UseCase {newUseCase.Name}");
        return _impl.PlaceNewUseCase(newUseCase);
    }

    public IEnumerable<IUseCase> GetRunningUseCases()
    {
        return _impl.GetRunningUseCases();
    }

    private readonly ITrain _impl;
    private readonly string _name;
}