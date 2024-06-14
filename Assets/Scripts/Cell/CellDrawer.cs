using System;
using UnityEngine;

public class CellDrawer : MonoBehaviour
{
    [SerializeField] private Grid grid;

    void Update()
    {
        Vector2Int gridCoordinate = GetMouseGridCoordinate();
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
            return grid.ToGridCoordinate(localPosition);
        }
        return -Vector2Int.one;
    }
}
