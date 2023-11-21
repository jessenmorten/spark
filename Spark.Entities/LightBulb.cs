namespace Spark.Entities;

public class LightBulb : ILightBulb
{
    public string Id { get; }

    public bool On { get; private set; }

    public LightBulb(string id, bool on)
    {
        Id = id;
        On = on;
    }
}
