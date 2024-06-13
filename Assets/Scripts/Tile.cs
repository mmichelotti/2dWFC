using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

[System.Serializable]
public enum TileType
{
    Road,
    Free
}

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tile", order = 1)]
public class Tile : ScriptableObject, IDirectionable
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Directions Directions { get; set; }
    public bool HasDirection(Directions dir) => Directions.HasFlag(dir);
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
        Directions = Directions.Bitshift(Shift.Right);
        Rotation += new Vector3(0, 0, 90);
    }
    public void Debug() => UnityEngine.Debug.Log($"{Name}, {Directions.ToStringCustom()} roads with {Rotation.z} degrees rotation.");
}
