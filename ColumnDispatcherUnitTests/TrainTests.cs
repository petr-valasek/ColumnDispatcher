using ColumnDispatcher.TrainModel;

namespace ColumnDispatcherUnitTests
{
    [TestClass]
    public class TrainTests
    {
        [TestMethod]
        public void GivenOneUseCaseExecuted_WhenOtherIsPlaced_ItSucceeds()
        {
            var ctrl = new StateMachineDummy();
            var train = new Train(ctrl);
            var useCaseLibs = new List<IUseCaseLibrary>
            {
                new NothingUseCaseLibrary(),
            };

            var mngr = new ColumnDispatcher.TrainModel.ColumnDispatcher(useCaseLibs, train);
            mngr.Execute(new Request { });
            mngr.Execute(new Request { });
        }


        [TestMethod]
        public void GivenOneUseCaseExecuting_WhenOtherIsPlaced_ItFails()
        {
            var ctrl = new StateMachineDummy();
            var train = new Train(ctrl);
            var useCaseLibs = new List<IUseCaseLibrary>
            {
                new NothingUseCaseLibrary(),
            };

            var mngr = new ColumnDispatcher.TrainModel.ColumnDispatcher(useCaseLibs, train);
            mngr.Start(new Request { Target = ColumnState.BeamOn });
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                mngr.Execute(new Request { });
            });
            var hanger = train.GetRunningUseCases().FirstOrDefault();
            Assert.IsNotNull(hanger);
            hanger.GetCancellationTokenSource().Cancel();
            Thread.Sleep(200);
            Assert.IsFalse(train.GetRunningUseCases().Any());
        }
    }
}

public class StateMachineDummy : IStateMachine
    {
        public void SendCommand(ColumnCommand command, object? data = null)
        {
            throw new NotImplementedException();
        }

        public ColumnState State { get; }
        public event EventHandler<ColumnState>? StateChanged;
    }

    public class NothingUseCaseLibrary : IUseCaseLibrary
    {
        public IUseCase? GetUseCase(Request request, ITrain train)
        {
            if (request.Target == null)
                return train.PlaceNewUseCase(new NothingUseCase { });
            else return train.PlaceNewUseCase(new HangUseCase { });
    }
    }

    public class NothingUseCase : IUseCase
    {

        public NothingUseCase()
        {
            _task = new Task(() => { });
        }
        public Task GetWaitableTask()
        {
            return _task;
        }

        public CancellationTokenSource GetCancellationTokenSource()
        {
            return new CancellationTokenSource();
        }

        public string Name => "Nothing";
        private readonly Task _task;
    }

public class HangUseCase : IUseCase
{

    public HangUseCase()
    {
        _task = new Task(() =>
        {
            _cancellationTokenSource.Token.WaitHandle.WaitOne();
        });
    }
    public Task GetWaitableTask()
    {
        return _task;
    }

    public CancellationTokenSource GetCancellationTokenSource()
    {
        return _cancellationTokenSource;
    }

    public string Name => "Nothing";
    private readonly Task _task;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
}


