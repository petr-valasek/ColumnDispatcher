using ColumnDispatcher.TrainModel;

namespace ColumnDispatcherUnitTests
{
    using Item = StateMachineMock.FlowItem;

    [TestClass]
    public class BeamOffTests
    {
        public TestContext TestContext
        {
            get { return _setup.TestContext; }
            set { _setup.TestContext = value; }
        }

        [TestMethod]
        public void GivenBeamOff_WhenBeamOffCalled_NothingHappens()
        {
            _setup.SetUp(ColumnState.BeamOff);
            var r = new Request { Target = ColumnState.BeamOff };
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamOn_WhenBeamOffCalled_BeamOffSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.BeamOn);
            var r = new Request { Target = ColumnState.BeamOff };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.Blank, null, ColumnState.Blanked),
                new Item(ColumnCommand.ParkL1, null, ColumnState.L1Parked),
                new Item(ColumnCommand.SetApertureBlocking, null, ColumnState.BeamBlocked),
                new Item(ColumnCommand.BeamOff, null, ColumnState.BeamOff)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBlanked_WhenBeamOffCalled_PartialBeamOffSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.Blanked);
            var r = new Request { Target = ColumnState.BeamOff };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.ParkL1, ColumnState.L1Parked),
                new Item(ColumnCommand.SetApertureBlocking, ColumnState.BeamBlocked),
                new Item(ColumnCommand.BeamOff, ColumnState.BeamOff)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }
        [TestMethod]
        public void GivenL1Parked_WhenBeamOffCalled_PartialBeamOffSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.L1Parked);
            var r = new Request { Target = ColumnState.BeamOff };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.SetApertureBlocking, ColumnState.BeamBlocked),
                new Item(ColumnCommand.BeamOff, ColumnState.BeamOff)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }
        [TestMethod]
        public void GivenBeamBlocked_WhenBeamOffCalled_BeamOffIsExecuted()
        {
            _setup.SetUp(ColumnState.BeamBlocked);
            var r = new Request { Target = ColumnState.BeamOff };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.BeamOff, ColumnState.BeamOff)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamOnExecuting_WhenBeamOffCalled_BeamOnAbortedAndBeamOffExecuted()
        {
            _setup.SetUp(ColumnState.BeamOff);
            var beamOnRequest = new Request { Target = ColumnState.BeamOn };
            _setup.Controller.SetExpectedFlow(
                // forth (beam on, never moves from transitional state)
                new Item(ColumnCommand.BeamOn, ColumnState.BeamBlocked),
                new Item(ColumnCommand.SetAperture, ColumnState.ApertureMovingNoL1),
                // abort to get to blocked
                new Item(ColumnCommand.AbortAperture, ColumnState.BeamBlocked),
                // and go back (beam off)
                new Item(ColumnCommand.BeamOff, ColumnState.BeamOff)
            );
            bool cancelled = false;
            var beamOnTask = new Task(() =>
            {
                try
                {
                    _setup.ColumnDispatcher.Execute(beamOnRequest);
                }
                catch (System.AggregateException e)
                {
                    if (e.Flatten().InnerExceptions.Any(e => e is OperationCanceledException))
                    {
                        cancelled = true;
                    }
                    else
                    {
                        throw e;
                    }
                }
            });
            beamOnTask.Start();
            Thread.Sleep(200);
            var beamOffRequest = new Request { Target = ColumnState.BeamOff };
            _setup.ColumnDispatcher.Execute(beamOffRequest);
            _setup.Controller.CheckExpectedFlowIsExhausted();
            beamOnTask.Wait();
            Assert.IsTrue(cancelled);
        }


        private TestSetup _setup = new TestSetup();

    }
}