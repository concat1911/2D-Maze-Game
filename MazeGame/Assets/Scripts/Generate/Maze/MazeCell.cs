// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MazeGame
{
  public class MazeCell : MonoBehaviour
  {
    public Vector2Int position;
    private int initializedEdgeCount;

    [SerializeField] MazeCellEdge[] edges = new MazeCellEdge[4];

    public MazeCellEdge GetEdge(MazeDirection direction)
    {
      return edges[(int)direction];
    }

    public List<MazePassage> AllPassage(){
      List<MazePassage> allPassages = new List<MazePassage>();
      for (int i = 0; i < edges.Length; i++)
      {
        if(edges[i] is MazePassage) allPassages.Add((MazePassage)edges[i]);
      }
      return allPassages;
    }

    public void SetEdge(MazeDirection direction, MazeCellEdge edge)
    {
      edges[(int)direction] = edge;
      initializedEdgeCount += 1;
    }
    
    public bool IsFullyInitialized
    {
      get
      {
        return initializedEdgeCount == 4;
      }
    }

    public MazeDirection RandomUninitializedDirection
    {
      get
      {
        int skips = Random.Range(0, 4 - initializedEdgeCount);
        for (int i = 0; i < 4; i++)
        {
          if (edges[i] == null)
          {
            if (skips == 0)
            {
              return (MazeDirection)i;
            }
            skips -= 1;
          }
        }
        throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
      }
    }
  }
}
