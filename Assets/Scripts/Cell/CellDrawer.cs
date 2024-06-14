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
        // Get the mouse position in the worl0d
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, grid.transform.position);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 localPosition = grid.transform.InverseTransformPoint(hitPoint);

            int xCoordinate = Mathf.FloorToInt(localPosition.x / grid.Size.x) + grid.Length / 2;
            int yCoordinate = Mathf.FloorToInt(localPosition.y / grid.Size.y) + grid.Length / 2;

            return new Vector2Int(xCoordinate, yCoordinate);
        }

        return Vector2Int.zero; // Return a default value if the raycast does not hit the grid
    }
}
