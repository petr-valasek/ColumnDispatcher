using ColumnDispatcher.TrainModel;

namespace ColumnDispatcherUnitTests
{
    using Item = StateMachineMock.FlowItem;

    [TestClass]
    public class HtTests
    {
        public TestContext TestContext
        {
            get { return _setup.TestContext; }
            set { _setup.TestContext = value; }
        }

        [TestMethod]
        public void GivenBeamOff_WhenHtChange_NoStateChange_HtCached()
        {
            _setup.SetUp(ColumnState.BeamOff);
            var r = new Request { };
            r.Change.Add(ChangeTypeSingle.Ht);
            r.Data = new ChangeData { Ht = 30000 };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.ChangeHt, 30000d , ColumnState.BeamOff)
            );
            _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamOn_WhenHtChange_HtChangeSequenceIsExecuted()
        {
            _setup.SetUp(ColumnState.BeamOn);
            var r = new Request { };
            r.Change.Add(ChangeTypeSingle.Ht);
            r.Data = new ChangeData { Ht = 15000 };
            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.Blank, ColumnState.Blanked),
                new Item(ColumnCommand.ParkL1, ColumnState.L1Parked),
                new Item(ColumnCommand.SetApertureBlocking, ColumnState.BeamBlocked),
                new Item(ColumnCommand.ChangeHt, 15000d, ColumnState.BeamBlocked),
                new Item(ColumnCommand.SetAperture, ColumnState.L1Parked),
                new Item(ColumnCommand.UnparkL1, ColumnState.Blanked),
                new Item(ColumnCommand.Unblank, ColumnState.BeamOn)
            );
        _setup.ColumnDispatcher.Execute(r);
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenRampingHt_WhenHtChange_NoStateChange_HtTargetUpdated()
        {
            _setup.SetUp(ColumnState.BeamBlocked);
            var originalRequest = new Request {};
            originalRequest.Change.Add(ChangeTypeSingle.Ht);
            originalRequest.Data = new ChangeData { Ht = 20000 };
            _setup.Controller.SetExpectedFlow(
                // first use case - original request, will get stuck there
                new Item(ColumnCommand.ChangeHt, 20000d, ColumnState.RampingHt),
                // second use case - amending request, will move both use cases
                new Item(ColumnCommand.ChangeHt, 30000d, ColumnState.BeamBlocked)
                );
            var originalTask = new Task(() =>
            {
                _setup.ColumnDispatcher.Execute(originalRequest);
            });
            originalTask.Start();
            Thread.Sleep(200);
            var amendingRequest = new Request { };
            amendingRequest.Change.Add(ChangeTypeSingle.Ht);
            amendingRequest.Data = new ChangeData { Ht = 30000 };
            _setup.ColumnDispatcher.Execute(amendingRequest);
            originalTask.Wait();
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        [TestMethod]
        public void GivenBeamOffExecuting_WhenHtChange_HtCached()
        {
            _setup.SetUp(ColumnState.BeamOn);
            var beamOffRequest = new Request { Target = ColumnState.BeamOff };
            var htChangeRequest = new Request { };
            htChangeRequest.Change.Add(ChangeTypeSingle.Ht);
            htChangeRequest.Data = new ChangeData { Ht = 15000 };

            _setup.Controller.SetExpectedFlow(
                new Item(ColumnCommand.Blank, null, ColumnState.Blanked),
                new Item(ColumnCommand.ParkL1, null, ColumnState.L1Parked),
                new Item(ColumnCommand.SetApertureBlocking, null, ColumnState.ApertureMovingNoL1),
                // here, the use case gets stuck, it will be prodded manually
                // but first, it will expect a change ht request
                new Item(ColumnCommand.ChangeHt, 15000d, ColumnState.ApertureMovingNoL1),
                // then, let's finish the beam off sequence
                new Item(ColumnCommand.BeamOff, null, ColumnState.BeamOff)
            );
            // Execute the beam off request on another thread
            var beamOffTask = new Task(() =>
            {
                _setup.ColumnDispatcher.Execute(beamOffRequest);
            });
            beamOffTask.Start();
            // wait for the Controller state change to TurningOff with timeout as TimeSpan
            _setup.Controller.WaitForStateChange(ColumnState.ApertureMovingNoL1, TimeSpan.FromSeconds(5));
            _setup.ColumnDispatcher.Execute(htChangeRequest);
            _setup.Controller.State = ColumnState.BeamBlocked;
            // wait for beam off task to finish
            beamOffTask.Wait();
            _setup.Controller.CheckExpectedFlowIsExhausted();
        }

        private readonly TestSetup _setup = new();

    }
}