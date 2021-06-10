// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace ML.MazeGame{
    public class MazeCell : MonoBehaviour
    {
    public Vector2Int position;

    private MazeCellEdge[] edges = new MazeCellEdge[MazeDirections.Count];

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
        return edges[(int)direction];
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
        edges[(int)direction] = edge;
    }
    }
}
