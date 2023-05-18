using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace ColumnDispatcher.TrainModel;

public class StateMachineMock : IStateMachine
{
    public struct FlowItem
    {
        public FlowItem(ColumnCommand expectedCommand, ColumnState nextState)
        {
            ExpectedCommand = expectedCommand;
            ExpectedData = null;
            NextState = nextState;
        }
        public FlowItem(ColumnCommand expectedCommand, object expectedData, ColumnState nextState)
        {
            ExpectedCommand  = expectedCommand;
            ExpectedData = expectedData;
            NextState = nextState;
        }
        public ColumnCommand ExpectedCommand;
        public object ExpectedData;
        public ColumnState NextState;
    }

    public StateMachineMock(TestContext testContext)
    {
        _testContext = testContext;
    }
    public void SendCommand(ColumnCommand command, object? data)
    {
        Logger.Log($"ColumnControl", $"Command {command} received ");
        Assert.IsTrue(_flow.Count > 0);
        var first = _flow.First();

        Logger.Log($"ColumnControl", $"Flow expects {first.ExpectedCommand} command, with [{first.ExpectedData}]; it will change state to {first.NextState} ");
        Assert.AreEqual(first.ExpectedCommand,command);
        Assert.AreEqual(first.ExpectedData,data);
        State = first.NextState;
        _flow = _flow.Skip(1).ToList();
        Logger.Log($"ColumnControl", $"Flow has {_flow.Count} items left");
    }


    public event EventHandler<ColumnState>? StateChanged;

    public void SetStateSilently(ColumnState state)
    {
        _state = state;
    }

    public void SetExpectedFlow(params FlowItem[] flow)
    {
        _flow = flow.ToList();
    }

    public void CheckExpectedFlowIsExhausted()
    {
        Logger.Log($"ColumnControl", $"Checking: Flow has {_flow.Count} items left");
        Assert.AreEqual(0, _flow.Count);
    }

    public ColumnState State
    {
        get => _state;
        set // private for IStateMachine users, public for unit tests
        {
            if (value != State)
            {
                Logger.Log($"ColumnControl", $"State change: {_state} -> {value}");
                _state = value;
                StateChanged?.Invoke(this, State);
            }
        }
    }

    private ColumnState _state = ColumnState.EmissionOff;
    private List<FlowItem> _flow = new();
    private ColumnState? pausedState = null;
    private TestContext _testContext;

    public void WaitForStateChange(ColumnState desiredState, TimeSpan timeout)
    {
        var stateReached = new AutoResetEvent(false);
        StateChanged += (sender, state) =>
        {
            if (state == desiredState)
            {
                stateReached.Set();
            }
        };
        if (!stateReached.WaitOne(timeout))
        {
            throw new TimeoutException($"State {desiredState} not reached in {timeout} seconds");
        }
    }
}