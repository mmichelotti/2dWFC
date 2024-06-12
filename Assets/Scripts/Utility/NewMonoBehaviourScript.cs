using UnityEngine;
public class VectorTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector<D2, int> vector2D = new(1, 2);
        Vector<D4, int> vector4D = new(3, 4, 5, 6);
        Vector<D4, int> vector4D2 = new(3,2);

        Vector<D2, int> vector4D22 = new(3);
        Debug.LogError(vector4D22);
        Debug.LogError("x" + vector4D22.X);
        Debug.LogError("y" + vector4D22.Y);
        Debug.LogError("z" + vector4D22.Z);


        //Debug.LogError(vector2D.Z);
        // Adding vectors of different dimensions
        var result = vector4D2 + vector2D;
        Debug.LogError($"Result of adding 4D {vector4D2} and 2D {vector2D}");
        Debug.LogError($"Result of the sum is {result}");
    }
}
