using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Direction Directions { get; set; }
    public Vector3 Rotation { get; set; }
    public Tile(Tile tile) => (Name, Sprite, Directions, Rotation) = (tile.Name, tile.Sprite, tile.Directions, tile.Rotation);
    public List<Tile> AllConfigurations
    {
        get
        {
            List<Tile> allDirections = new();
            Tile toRotate = new(this);
            for (int i = 0; i < 4; i++)
            {
                allDirections.Add(new(toRotate));
                toRotate.Rotate();
            }
            return allDirections;
        }
    }
    private void Rotate()
    {
        Directions = Directions.Bitshift();
        Rotation += new Vector3(0, 0, 90);
    }
    public void DebugStatus() => Debug.Log($"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.");
}
