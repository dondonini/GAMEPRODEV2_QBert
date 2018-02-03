public class WListValue
{
    public float d { get; set; }
    public Waypoint w { get; set; }

    public WListValue(float dist, Waypoint way)
    {
        d = dist;
        w = way;
    }
}
