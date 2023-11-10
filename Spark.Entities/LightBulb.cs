namespace Spark.Entities;

public class LightBulb : ILightBulb
{
    public Guid Id { get; }

    public bool On { get; private set; }

    public LightBulb(Guid id, bool on)
    {
        Id = id;
        On = on;
    }
}
