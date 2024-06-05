using UnityEngine;


[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileObject : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField] public Directions Directions { get; set; }
    public TileObject(TileObject to) => (Sprite, Directions) = (to.Sprite, to.Directions);
    public void DirectionShift() => Directions = Directions.Bitshift(); 
}
