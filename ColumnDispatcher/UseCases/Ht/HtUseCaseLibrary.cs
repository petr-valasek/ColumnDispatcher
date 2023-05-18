namespace ColumnDispatcher.TrainModel;

public class HtUseCaseLibrary : IUseCaseLibrary
{
    public IUseCase? GetUseCase(Request request, ITrain train)
    {
        if (request.Target == null && request.Change.Contains(ChangeTypeSingle.Ht))
        {
            if (request.Data == null)
            {
                throw new InvalidOperationException("Change HT request doesn't contain data");
            }
            if (ColumnStatePrecedence.PrecedesOrEquals(train.TargetState, ColumnState.BeamOff))
            {
                return train.ExecuteImmediately(new CacheHt(new TrainFacade(train, "CacheHt"), request.Data.Ht));
            }
            else
            {
                if ((train.StateMachine.State == ColumnState.BeamBlocked ||
                    train.StateMachine.State == ColumnState.RampingHt) &&
                    train.GetRunningUseCases().Any())

                {
                    return train.AmendCurrentUseCase(new CacheHt(new TrainFacade(train, "UpdateHtTarget"), request.Data.Ht));
                }
                else
                {
                    return train.ReplaceCurrentUseCase(new ChangeHt(new TrainFacade(train, "ChangeHt"), request.Data.Ht));
                }
            }
        }
        return null;
    }
}