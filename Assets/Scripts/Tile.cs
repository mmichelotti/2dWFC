using UnityEngine;


[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field:SerializeField] public Directions Directions { get; set; }
    public Vector3 Rotation { get; set; } = new(0, 0, 0);
    public Tile(Tile to) => (Sprite, Directions, Rotation) = (to.Sprite, to.Directions, to.Rotation);
    public void DirectionShift() => Directions = Directions.Bitshift(); 
}
