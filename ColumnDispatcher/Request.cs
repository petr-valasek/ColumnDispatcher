namespace ColumnDispatcher.TrainModel;


public enum ChangeTypeSingle
{
    Ht,
    Booster,
    Aperture,
    Optics,
};

public class AperturePosition
{
    public double X, Y;
};

public class ChangeData
{
    public double Ht;
    public AperturePosition? Aperture;
    /*...*/
};
public class Request
{
    public HashSet<ChangeTypeSingle> Change = new();
    public ColumnState? Target; // null == don't care
    public ChangeData? Data; // reference only to avoid copying?
};