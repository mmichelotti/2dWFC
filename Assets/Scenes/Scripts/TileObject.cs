using UnityEngine;


[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileObject : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }

    [SerializeField]
    private Directions directions;
    public Directions Directions
    {
        get
        {
            if ((int)directions != -1) return directions; //what is this -1 everything bullshit?
            return Directions.Up | Directions.Down | Directions.Left | Directions.Right;
            
        }
        private set
        {
            directions = value;
        }
    }

    public TileObject(TileObject to) => (Sprite, Directions) = (to.Sprite, to.Directions);
    public void DirectionShift() => Directions = Directions.Bitshift(); 
}
