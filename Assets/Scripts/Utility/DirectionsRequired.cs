public struct DirectionsRequired
{
    public Directions Required { get; private set; }
    public Directions Excluded { get; private set; }

    public DirectionsRequired(Directions onlyPositive) => (Required, Excluded) = (onlyPositive, default);
    public DirectionsRequired(Directions required, Directions excluded) => (Required, Excluded) = (required, excluded);

    //is it correct to assign data to immutable objects
    //since it is a struct and not a class?ù
    //i feel like this is wrong, this is supposed to be a Class if i want to handle the data like that
    public void Flip() => (Required, Excluded) = (Excluded, Required);

}