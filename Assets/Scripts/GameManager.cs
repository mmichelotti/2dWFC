using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InputManager InputManager { get; private set; }
    public GridManager GridManager { get; private set; }

    private void Awake()
    {
        Instance = this.MakeSingleton();
        InputManager = this.GetManager<InputManager>();
        GridManager = this.GetManager<GridManager>();
    }
}