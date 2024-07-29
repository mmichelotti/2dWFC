public interface IDirectionable
{
    public Directions2D Directions { get; set; }
    public bool HasDirection(Directions2D dir);
}