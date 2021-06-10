// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace ML.MazeGame
{
  public enum MazeDirection { North, East, South, West }

  public static class MazeDirections
  {
    private static MazeDirection[] opposites = {
      MazeDirection.South,
      MazeDirection.West,
      MazeDirection.North,
      MazeDirection.East
    };

    public static MazeDirection GetOpposite(MazeDirection direction)
    {
      return opposites[(int)direction];
    }
    public static Vector2Int RandomMazeDirection
    {
      get
      {
        MazeDirection randomDir = (MazeDirection)Random.Range(0, 4);
        return MazeDirectionToVector2Int(randomDir);
      }
    }
    ///<summary>
    ///Convert MazeDirection Enum into Vector2Int value
    ///</summary>
    public static Vector2Int MazeDirectionToVector2Int(MazeDirection dir)
    {
      switch (dir)
      {
        case MazeDirection.North:
          return Vector2Int.up;
        case MazeDirection.South:
          return Vector2Int.down;
        case MazeDirection.East:
          return Vector2Int.right;
        case MazeDirection.West:
          return Vector2Int.left;
        default:
          return Vector2Int.one;
      }
    }
    private static Quaternion[] rotations = {
      Quaternion.identity,
      Quaternion.Euler(0f, 0f, -90f),
      Quaternion.Euler(0f, 0f, -180f),
      Quaternion.Euler(0f, 0f, -270f)
    };

    public static Quaternion ToRotation(this MazeDirection direction)
    {
      return rotations[(int)direction];
    }
  }
}
