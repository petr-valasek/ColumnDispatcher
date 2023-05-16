namespace ColumnDispatcher.TrainModel;

public class UseCaseHelper
{
    public UseCaseHelper(ITrain train, CancellationToken token)
    {
        _token = token;
        _train = train;
    }

    public void CheckCancellation()
    {
        if (_token.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }
    }
    public Task WaitFor(ColumnState state, params ColumnState[] transitional)
    {
        var t = new Task(() =>
        {
            EventWaitHandle ev = new(false, EventResetMode.AutoReset);

            void StateChanged


                (object o, ColumnState s)
            {
                if (s == state)
                {
                    ev.Set();
                }
                else if (!transitional.Contains(s))
                {
                    throw new InvalidOperationException();
                }
            }

            _train.StateMachine.StateChanged += StateChanged;
            if (_train.StateMachine.State == state)
            {
                _train.StateMachine.StateChanged -= StateChanged;
                return;
            }

            WaitHandle.WaitAny(new[] { ev, _token.WaitHandle });
            _train.StateMachine.StateChanged -= StateChanged;
        });
        t.Start();
        return t;
    }

    public void SendCommandAndWait(ColumnCommand command, object data, ColumnState state, params ColumnState[] transitionStates)
    {
        _train.StateMachine.SendCommand(command, data);
        WaitFor(state, transitionStates).Wait();
    }
    public void SendCommandAndWait(ColumnCommand command, ColumnState state, params ColumnState[] transitionStates)
    {
        SendCommandAndWait(command, null, state, transitionStates);
    }

    public void SendCommandAndWaitAbortable(
        ColumnCommand command, 
        object data, 
        ColumnState state, 
        ColumnCommand abortCmd, 
        params ColumnState[] transitionStates)
    {
        _train.StateMachine.SendCommand(command, data);
        try
        {
            WaitFor(state, transitionStates).Wait(_token);
        }
        catch (OperationCanceledException)
        {
            Logger.Log("UseCaseHelper", $"Aborting by {abortCmd}");
            _train.StateMachine.SendCommand(abortCmd);
            WaitFor(state).Wait();
            throw;
        }
    }

    public void SendCommandAndWaitAbortable(
        ColumnCommand command,
        ColumnState state,
        ColumnCommand abortCmd,
        params ColumnState[] transitionStates)
    {
        SendCommandAndWaitAbortable(command, null, state, abortCmd, transitionStates);
    }

    public void EnsureAtLeast(ColumnState state, ColumnCommand byCommand, params ColumnState[] transitionStates)
    {
        if (ColumnStatePrecedence.Precedes(_train.StateMachine.State, state))
        {
            SendCommandAndWait(byCommand, state, transitionStates);
        }
    }

    public void EnsureAtMost(ColumnState state, ColumnCommand byCommand, params ColumnState[] transitionStates)
    {
        if (ColumnStatePrecedence.Precedes(state, _train.StateMachine.State))
        {
            SendCommandAndWait(byCommand, state, transitionStates);
        }
    }

    public void EnsureAtLeastAbortable(
        ColumnState state, 
        ColumnCommand byCommand, 
        ColumnCommand abortCommand, 
        params ColumnState[] transitionStates)
    {
        if (ColumnStatePrecedence.Precedes(_train.StateMachine.State, state))
        {
            SendCommandAndWaitAbortable(byCommand, state, abortCommand, transitionStates);
        }
    }

    public void EnsureAtMostAbortable(
        ColumnState state, 
        ColumnCommand byCommand, 
        ColumnCommand abortCommand, 
        params ColumnState[] transitionStates)
    {
        if (ColumnStatePrecedence.Precedes(state, _train.StateMachine.State))
        {
            SendCommandAndWaitAbortable(byCommand, state, abortCommand, transitionStates);
        }
    }

    public void EnsureAtLeastBeamOff()
    {
        EnsureAtLeast(ColumnState.BeamOff, ColumnCommand.StartEmission, ColumnState.StartingEmission);
    }
    public void EnsureAtMostBeamOff()
    {
        EnsureAtMost(ColumnState.BeamOff, ColumnCommand.BeamOff, ColumnState.StartingEmission);
    }

    public void EnsureAtLeastBeamBlocked()
    {
        EnsureAtLeast(ColumnState.BeamBlocked, ColumnCommand.BeamOn, ColumnState.TurningOn);
    }
    public void EnsureAtMostBeamBlocked()
    {
        EnsureAtMost(ColumnState.BeamBlocked, ColumnCommand.SetApertureBlocking, ColumnState.TurningOn);
    }

    public void EnsureAtLeastL1Parked()
    {
        EnsureAtLeast(ColumnState.L1Parked, ColumnCommand.SetAperture, ColumnState.ApertureMovingNoL1);
    }

    public void EnsureAtLeastL1ParkedAbortable()
    {
        EnsureAtLeastAbortable(
            ColumnState.L1Parked,
            ColumnCommand.SetAperture, 
            ColumnCommand.AbortAperture, 
            ColumnState.ApertureMovingNoL1);
    }

    public void EnsureAtMostL1Parked()
    {
        EnsureAtMost(ColumnState.L1Parked, ColumnCommand.ParkL1, ColumnState.ApertureMovingNoL1);
    }
    public void EnsureAtLeastBlanked()
    {
        EnsureAtLeast(ColumnState.Blanked, ColumnCommand.UnparkL1, ColumnState.UnparkingL1);
    }
    public void EnsureAtMostBlanked()
    {
        EnsureAtMost(ColumnState.Blanked, ColumnCommand.Blank, ColumnState.UnparkingL1);
    }
    public void EnsureAtLeastBeamOn()
    {
        EnsureAtLeast(ColumnState.BeamOn, ColumnCommand.Unblank);
    }

    private CancellationToken _token = new();
    private readonly ITrain _train;
}