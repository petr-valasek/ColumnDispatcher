namespace ColumnDispatcher.TrainModel;

public enum ColumnState
{
    EmissionOff,
    StartingEmission,
    StoppingEmission,
    BeamOff,
    TurningOn,
    TurningOff,
    BeamBlocked,
    RampingHt,
    ApertureMovingNoL1,
    L1Parked,
    UnparkingL1,
    ParkingL1,
    Blanked,
    ApertureMoving,
    BeamOn
}

public enum ColumnCommand
{
    StartEmission,
    StopEmission,
    BeamOn,
    BeamOff,
    ChangeHt,
    SetAperture,
    SetApertureBlocking,
    UnparkL1,
    ParkL1,
    Blank,
    Unblank,
    AbortAperture
}

public interface IStateMachine
{
    void SendCommand(ColumnCommand command, object? data = null);
    ColumnState State { get; }
    event EventHandler<ColumnState>? StateChanged;
}

public static class ColumnStatePrecedence
{
    public static bool Precedes(ColumnState a, ColumnState b)
    {
        // simplistic implementation
        return a < b;
    }
    public static bool PrecedesOrEquals(ColumnState a, ColumnState b)
    {
        // simplistic implementation
        return a <= b;
    }
}

