namespace ColumnDispatcher.TrainModel;

public class BeamOnOffUseCaseLibrary: IUseCaseLibrary
{
    public IUseCase? GetUseCase(Request request, ITrain train)
    {
        if (request.Target == ColumnState.BeamOn && request.Change.Count == 0)
        {
            if (train.TargetState == ColumnState.BeamOn)
            {
                if (train.StateMachine.State == ColumnState.BeamOn)
                    return new NopUseCase();
                else
                    return train.JoinCurrentUseCase();
            }
            else if (train.GetRunningUseCases().Any())
            {
                return train.ReplaceCurrentUseCase(new BeamOn(new TrainFacade(train, "BeamOn")));
            }
            else
            {
                return train.PlaceNewUseCase(new BeamOn(new TrainFacade(train, "BeamOn")));
            }

        }
        if (request.Target == ColumnState.BeamOff && request.Change.Count == 0)
        {
            if (train.TargetState == ColumnState.BeamOff)
            {
                if (train.StateMachine.State == ColumnState.BeamOff)
                    return new NopUseCase();
                else
                    return train.JoinCurrentUseCase();
            }
            else if (train.GetRunningUseCases().Any())
            {
                return train.ReplaceCurrentUseCase(new BeamOff(new TrainFacade(train, "BeamOff")));
            }
            else
            {
                return train.PlaceNewUseCase(new BeamOff(new TrainFacade(train, "BeamOff")));
            }
        }
        return null;
    }
}