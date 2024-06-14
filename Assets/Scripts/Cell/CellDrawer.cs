using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CellDrawer : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private SpriteRenderer spriteRenderer;
    private Color canColor = new(0.62f, 1f, 0f,.5f);
    private Color cantColor = new(1.0f, 0.334f, 0f, .5f);
    private Color noneColor = Color.clear;
    private CellManager CellManager => GameManager.Instance.CellManager;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        Vector2Int gridCoordinate = GetMouseGridCoordinate();
        if (CellManager.cellAtPosition.TryGetValue(gridCoordinate, out Cell cell))
        {

            spriteRenderer.color = canColor;
            transform.position = grid.CoordinateToPosition(gridCoordinate);
        }
        else
        {
            spriteRenderer.color = noneColor;
        }

        Debug.Log($"Mouse is over grid coordinate: {gridCoordinate}");
    }

    private Vector2Int GetMouseGridCoordinate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new(Vector3.forward, grid.transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 localPosition = grid.transform.InverseTransformPoint(hitPoint);
            Vector2Int pos = grid.ToGridCoordinate(localPosition);
            return pos;
        }
        return -Vector2Int.one;
    }
}
