using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static DirectionUtility;


/// <summary>
/// Class responsible for instnacinng cells withing a grid
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private Cell prefab;

    /// <summary>
    /// Bind a cell at a given coordinate
    /// </summary>
    private readonly Dictionary<Vector2Int, Cell> cellAtPosition = new();
    [field: SerializeField] public MazeGrid Grid { get; private set; }

    /// <summary>
    /// Returns the Cell coordinate that has the least entropy
    /// </summary>
    private Vector2Int LowestEntropy
    {
        get
        {
            (Vector2Int Position, float Entropy) lowestEntropy = (new(), float.PositiveInfinity);

            foreach (var (pos, cell) in cellAtPosition)
            {
                if (cell.State.IsEntangled) continue;
                if (cell.State.Entropy < lowestEntropy.Entropy) lowestEntropy = (pos, cell.State.Entropy);

            }
            return lowestEntropy.Position;
        }
    }


    /// <summary>
    /// Initialize cells and starts setting them
    /// </summary>
    private void Start()
    {
        InitializeCells();
        StartCoroutine(SetCellsCoroutine(.01f));
    }

    private IEnumerator SetCellsCoroutine(float time)
    {
        // Start with a random coordinate for the first iteration since they all have the same entropy
        Vector2Int nextPos = Grid.RandomCoordinate;

        //Using co routine only to help me visually seeing what's happening, so it doesnt happen everything in one frame
        while (true)
        {
            if (cellAtPosition[nextPos].State.IsEntangled) yield break;
            //Recursive function SetCell until all cells are entangled
            SetCell(nextPos);
            yield return new WaitForSeconds(time);
            nextPos = LowestEntropy;
        }
    }

    private void SetCell(Vector2Int pos)
    {
        // Saving the current cell
        Cell currentCell = cellAtPosition[pos];
        //save direciton information
        //for this specific cell, im saving the directions it MUST have and MUST NOT have
        Directions requiredDirections = Directions.None;
        Directions excludedDirections = Directions.None;

        
        //for each direction (up,right,down,left)
        foreach (var (dir, off) in OrientationOf)
        {
            //Try access the current cell neighbour or adjacent
            if (cellAtPosition.TryGetValue(pos + off, out Cell adjacent))
            {
                //if my neighbour is already entangled
                if (adjacent.State.IsEntangled)
                {
                    //if it is connecting with me i save it on the required direciton otherwise i save it on the excluded direciton
                    if (adjacent.CurrentTile.Directions.HasFlag(dir.GetOpposite())) requiredDirections |= dir;
                    else excludedDirections |= dir;
                }
                else
                {
                    /*
                    THIS IS WHERE ALL THE PROBLEM LIES.
                    BY COMMENTING THAT LINE OF CODE MY PROGRAM ACT LIKE THIS:
                    GO AT A RANDOM COORDINATE, GIVE A RANDOM TILE, AND ENTANGLES ITSELF.
                    BUT NOW, IT GOES FROM THE BOTTOM LEFT OF THE COORDINATE AND START ITERATING LIKE A NESTED FOR LOOP
                    IT WORKS JUST FINE BUT IT IS NOT FOLLOWING THE RULE OF ENTANGLING THE CELL WITH THE LEAST AMOUNT OF ENTROPY

                    THE LINE I COMMENTED BELOW, WAS ME TRYING TO UPDATE MY ADJACENT CELLS ENTROPY, SO THAT THE ALGORITHM WOULD GO AND START DEFINING THE CELLS WITH LEAST ENTROPY
                    THE PROBLEM IS THAT ALL THOSE DIRECTIONS ARE FUCKED UP, AND IT OFTEN RUNS IN OUT OF RANGE EXEPTION ERRORS.
                    */
                    //adjacent.UpdateState(dir.GetOpposite(), Directions.None);
                    //Debug.LogError($"Neighbour {adjacent.Coordinate} has {adjacent.State.Entropy} entropy, he wants {adjacent.St}");
                }
            }
            else
            {
                //If  there is no adjacent cell, means it is outside of the grid, hence it should exclude that direction
                excludedDirections |= dir;
            }
        }

        //useful debugging to understand where the prpoblems are
        Debug.Log($"Setting cell at {pos} with required directions: {requiredDirections} and excluded directions: {excludedDirections}");


        //Update the state of the current cell
        //After i updated, i entangled it and set it (to set means set the texture, transform etc)
        //Then i dfinally debug the cell state
        currentCell.UpdateState(requiredDirections, excludedDirections);
        currentCell.Set(currentCell.State.Entangle());
        currentCell.DebugStatus();
    }


    /// <summary>
    /// Instantiating a cell, bind it to the dictionary and give it the proper world space transform
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="parent"></param>
    private void InitializeCell(Vector2Int pos, Transform parent)
    {
        Cell cell = Instantiate(prefab, parent);
        cell.transform.position = Grid.CoordinateToPosition(pos);
        cell.transform.localScale = (Vector2)Grid.Size;
        cell.Coordinate = pos;
        cellAtPosition.Add(pos, cell);
    }


    /// <summary>
    /// Draw a line of the grid
    /// </summary>
    /// <param name="pos"></param>
    private void DrawLine(Vector2Int pos)
    {
        Vector3 wsPos = Grid.CoordinateToPosition(pos);
        Gizmos.DrawWireCube(wsPos, new Vector3(Grid.Size.x, Grid.Size.y, 0));
    }

    /// <summary>
    /// Instantiate all cells
    /// </summary>
    private void InitializeCells()
    {
        GameObject group = new("Cells");
        Action<Vector2Int> action = pos => InitializeCell(pos, group.transform);
        action.MatrixLoop(Grid.Length);
    }

    /// <summary>
    /// Draw all grid
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Action<Vector2Int> action = pos => DrawLine(pos);
        action.MatrixLoop(Grid.Length);
    }



}