using ColumnDispatcher.TrainModel;

namespace ColumnDispatcherUnitTests;

public class TestSetup
{
    public TestContext TestContext { get; set; }


    public void SetUp(ColumnState state)
    {
        Controller = new StateMachineMock(TestContext);
        var train = new Train(Controller);
        Controller.SetStateSilently(state);
        train.TargetState = state;
        var useCaseLibs = new List<IUseCaseLibrary>
        {
            new BeamOnOffUseCaseLibrary(),
            new HtUseCaseLibrary(),
            new BoosterSequenceLibrary(),
            new BeamCurrentUseCaseLibrary(),
        };

        ColumnDispatcher = new ColumnDispatcher.TrainModel.ColumnDispatcher(useCaseLibs, train);
    }

    public StateMachineMock Controller { get; private set; }
    public ColumnDispatcher.TrainModel.ColumnDispatcher ColumnDispatcher { get; private set; }
    public TestContext _testContext;
}