using ColumnDispatcher.TrainModel;

namespace ColumnDispatcherUnitTests
{
    using Item = StateMachineMock.FlowItem;

    [TestClass]
    public class BeamOnTests
    {
        public TestContext TestContext
        {
            get { return _setup.TestContext; }
            set { _setup.TestContext = value; }
        }

        [TestMethod]
        public void GivenBeamOn_WhenBeamOnCalled_NothingHappens()
        {
            _setup.SetUp(ColumnState.BeamOn);
            var r = new Request { Target = ColumnState.BeamOn };
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamOff_WhenBeamOnCalled_BeamOnSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.BeamOff);
            var r = new Request { Target = ColumnState.BeamOn };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.BeamOn, ColumnState.BeamBlocked),
                new Item(ColumnCommand.SetAperture, ColumnState.L1Parked),
                new Item(ColumnCommand.UnparkL1, ColumnState.Blanked),
                new Item(ColumnCommand.Unblank, ColumnState.BeamOn)
                );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamBlocked_WhenBeamOnCalled_PartialBeamOnSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.BeamBlocked);
            var r = new Request { Target = ColumnState.BeamOn };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.SetAperture, ColumnState.L1Parked),
                new Item(ColumnCommand.UnparkL1, ColumnState.Blanked),
                new Item(ColumnCommand.Unblank, ColumnState.BeamOn)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenL1Parked_WhenBeamOnCalled_PartialBeamOnSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.L1Parked);
            var r = new Request { Target = ColumnState.BeamOn };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.UnparkL1, ColumnState.Blanked),
                new Item(ColumnCommand.Unblank, ColumnState.BeamOn)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenL1Blanked_WhenBeamOnCalled_UnblankIsExecuted()
        {
            _setup.SetUp(ColumnState.Blanked);
            var r = new Request { Target = ColumnState.BeamOn };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.Unblank, ColumnState.BeamOn)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        private TestSetup _setup = new TestSetup();

    }
}